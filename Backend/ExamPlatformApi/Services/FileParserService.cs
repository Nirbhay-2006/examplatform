using ExamPlatform.API.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace ExamPlatform.API.Services;

public class FileParserService : IFileParserService
{
    private readonly ILogger<FileParserService> _logger;

    public FileParserService(ILogger<FileParserService> logger)
    {
        _logger = logger;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<List<Question>> ParseQuestionsFromFileAsync(IFormFile file, string examId)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        return extension switch
        {
            ".pdf" => await ParsePdfAsync(file, examId),
            ".xlsx" or ".xls" => await ParseExcelAsync(file, examId),
            _ => throw new NotSupportedException($"File type {extension} is not supported. Please upload PDF or Excel files.")
        };
    }

    private async Task<List<Question>> ParsePdfAsync(IFormFile file, string examId)
    {
        var questions = new List<Question>();
        
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var pdfReader = new PdfReader(stream);
        using var pdfDocument = new PdfDocument(pdfReader);
        
        var text = new System.Text.StringBuilder();
        
        for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
        {
            var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy();
            var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
            text.AppendLine(pageText);
        }

        var fullText = text.ToString();
        questions = ParseQuestionsFromText(fullText, examId);
        
        _logger.LogInformation("Parsed {Count} questions from PDF file", questions.Count);
        return questions;
    }

    private async Task<List<Question>> ParseExcelAsync(IFormFile file, string examId)
    {
        var questions = new List<Question>();
        
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var package = new ExcelPackage(stream);
        
        // Try to find worksheet with data
        var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Dimension != null) 
            ?? package.Workbook.Worksheets[0];
        
        if (worksheet == null || worksheet.Dimension == null)
        {
            throw new InvalidOperationException("Excel file is empty or invalid");
        }

        var rowCount = worksheet.Dimension.Rows;
        var colCount = worksheet.Dimension.Columns;

        // Detect header row by checking first row for common headers
        int startRow = 1;
        var firstRowText = worksheet.Cells[1, 1]?.Value?.ToString()?.ToLowerInvariant() ?? "";
        if (firstRowText.Contains("question") || firstRowText.Contains("q.") || firstRowText.Contains("q1"))
        {
            startRow = 2; // Skip header row
        }

        // Expected formats:
        // Format 1: Question | Option1 | Option2 | Option3 | Option4 | CorrectAnswer | Marks
        // Format 2: Question | Option1 | Option2 | Option3 | Option4 | CorrectAnswer (no marks column)
        // Format 3: Question | A | B | C | D | CorrectAnswer | Marks (with option labels)
        
        for (int row = startRow; row <= rowCount; row++)
        {
            var questionText = worksheet.Cells[row, 1]?.Value?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(questionText) || questionText.Length < 5)
                continue;

            var options = new List<string>();
            int optionStartCol = 2;
            
            // Check if column 2 is a label (A, B, C, D) or actual option
            var col2Value = worksheet.Cells[row, 2]?.Value?.ToString()?.Trim() ?? "";
            if (Regex.IsMatch(col2Value, @"^[A-Ea-e]$"))
            {
                // Format 3: Skip label column, options start from column 3
                optionStartCol = 3;
            }

            // Collect options (up to 5 options)
            for (int col = optionStartCol; col <= Math.Min(optionStartCol + 4, colCount); col++)
            {
                var option = worksheet.Cells[row, col]?.Value?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(option))
                {
                    options.Add(option);
                }
            }

            // Find correct answer column (usually after options)
            int correctAnswerCol = optionStartCol + options.Count;
            if (correctAnswerCol > colCount) correctAnswerCol = colCount;
            
            var correctAnswer = worksheet.Cells[row, correctAnswerCol]?.Value?.ToString()?.Trim() ?? "";
            
            // If correct answer is a letter/number, try to map it to an option
            if (!string.IsNullOrWhiteSpace(correctAnswer) && options.Count > 0)
            {
                var letterMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    { "A", 0 }, { "B", 1 }, { "C", 2 }, { "D", 3 }, { "E", 4 },
                    { "1", 0 }, { "2", 1 }, { "3", 2 }, { "4", 3 }, { "5", 4 }
                };
                
                if (letterMap.TryGetValue(correctAnswer.Trim(), out int index) && index < options.Count)
                {
                    correctAnswer = options[index];
                }
            }

            // Find marks column (usually last column or after correct answer)
            int marksCol = correctAnswerCol + 1;
            if (marksCol > colCount) marksCol = colCount;
            
            var marksStr = worksheet.Cells[row, marksCol]?.Value?.ToString()?.Trim() ?? "1";
            int.TryParse(marksStr, out int marks);
            if (marks <= 0) marks = 1;

            var question = new Question
            {
                ExamId = examId,
                QuestionText = questionText,
                QuestionType = options.Count >= 2 ? "mcq" : "text",
                Options = options,
                CorrectAnswer = correctAnswer,
                Marks = marks,
                Order = questions.Count + 1
            };

            questions.Add(question);
        }

        if (questions.Count == 0)
        {
            throw new InvalidOperationException("No valid questions found in Excel file. Please check the format.");
        }

        _logger.LogInformation("Parsed {Count} questions from Excel file", questions.Count);
        return questions;
    }

    private List<Question> ParseQuestionsFromText(string text, string examId)
    {
        var questions = new List<Question>();
        
        // Normalize text - remove extra whitespace and line breaks
        text = Regex.Replace(text, @"\r\n|\r", "\n");
        text = Regex.Replace(text, @"\n{3,}", "\n\n");
        
        // Multiple patterns to match different question formats
        var questionPatterns = new[]
        {
            // Pattern 1: Q1. or Question 1. or 1.
            @"(?i)(?:^|\n)\s*(?:Q(?:uestion)?\s*)?(\d+)[\.\)]\s*(.+?)(?=\n\s*(?:Q(?:uestion)?\s*)?\d+[\.\)]|\Z)",
            // Pattern 2: 1) Question text
            @"(?i)(?:^|\n)\s*(\d+)\)\s+(.+?)(?=\n\s*\d+\)|\Z)",
            // Pattern 3: Question 1: text
            @"(?i)(?:^|\n)\s*Question\s+(\d+)[\s:]+(.+?)(?=\n\s*Question\s+\d+|\Z)"
        };

        foreach (var pattern in questionPatterns)
        {
            var matches = Regex.Matches(text, pattern, RegexOptions.Multiline | RegexOptions.Singleline);
            
            foreach (Match match in matches)
            {
                var questionText = match.Groups.Count > 2 ? match.Groups[2].Value.Trim() : match.Groups[1].Value.Trim();
                
                // Extract options - multiple formats
                var optionPatterns = new[]
                {
                    // A) Option or a) Option
                    @"(?i)(?:^|\n)\s*([a-e])[\.\)]\s+([^\n]+?)(?=\n\s*[a-e][\.\)]|\n\s*(?:Correct|Answer|Key|Solution)|\n\s*(?:\d+[\.\)]|Q)|\Z)",
                    // 1) Option or 1. Option
                    @"(?i)(?:^|\n)\s*([1-5])[\.\)]\s+([^\n]+?)(?=\n\s*[1-5][\.\)]|\n\s*(?:Correct|Answer|Key|Solution)|\n\s*(?:\d+[\.\)]|Q)|\Z)",
                    // (A) Option or (a) Option
                    @"(?i)(?:^|\n)\s*\(([a-e])\)\s+([^\n]+?)(?=\n\s*\([a-e]\)|\n\s*(?:Correct|Answer|Key|Solution)|\n\s*(?:\d+[\.\)]|Q)|\Z)"
                };
                
                var options = new List<string>();
                foreach (var optPattern in optionPatterns)
                {
                    var optionMatches = Regex.Matches(questionText, optPattern, RegexOptions.Multiline);
                    if (optionMatches.Count >= 2)
                    {
                        foreach (Match optMatch in optionMatches)
                        {
                            if (optMatch.Groups.Count > 2)
                            {
                                var optionText = optMatch.Groups[2].Value.Trim();
                                if (!string.IsNullOrWhiteSpace(optionText) && !options.Contains(optionText))
                                {
                                    options.Add(optionText);
                                }
                            }
                        }
                        break; // Use first pattern that finds options
                    }
                }

                // Extract correct answer - multiple formats
                var correctAnswer = "";
                var answerPatterns = new[]
                {
                    @"(?i)(?:Correct\s*Answer|Answer|Key|Solution)[\s:]+([a-e1-5]|.+?)(?=\n|$)",
                    @"(?i)Answer[\s:]+([a-e1-5]|.+?)(?=\n|$)",
                    @"(?i)Key[\s:]+([a-e1-5]|.+?)(?=\n|$)"
                };
                
                foreach (var ansPattern in answerPatterns)
                {
                    var answerMatch = Regex.Match(questionText, ansPattern);
                    if (answerMatch.Success)
                    {
                        correctAnswer = answerMatch.Groups[1].Value.Trim();
                        break;
                    }
                }

                // If correct answer is a letter/number, try to match it to an option
                if (!string.IsNullOrWhiteSpace(correctAnswer) && options.Count > 0)
                {
                    var letterMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "A", 0 }, { "B", 1 }, { "C", 2 }, { "D", 3 }, { "E", 4 },
                        { "1", 0 }, { "2", 1 }, { "3", 2 }, { "4", 3 }, { "5", 4 }
                    };
                    
                    if (letterMap.TryGetValue(correctAnswer.Trim(), out int index) && index < options.Count)
                    {
                        correctAnswer = options[index];
                    }
                }

                // Clean question text (remove options and answer)
                foreach (var optPattern in optionPatterns)
                {
                    questionText = Regex.Replace(questionText, optPattern, "", RegexOptions.Multiline);
                }
                foreach (var ansPattern in answerPatterns)
                {
                    questionText = Regex.Replace(questionText, ansPattern, "", RegexOptions.Multiline);
                }
                questionText = Regex.Replace(questionText, @"\s+", " ").Trim();

                if (string.IsNullOrWhiteSpace(questionText) || questionText.Length < 10)
                    continue;

                var question = new Question
                {
                    ExamId = examId,
                    QuestionText = questionText,
                    QuestionType = options.Count >= 2 ? "mcq" : "text",
                    Options = options,
                    CorrectAnswer = correctAnswer,
                    Marks = 1,
                    Order = questions.Count + 1
                };

                questions.Add(question);
            }
            
            if (questions.Count > 0)
                break; // Use first pattern that finds questions
        }

        _logger.LogInformation("Parsed {Count} questions from text", questions.Count);
        return questions;
    }
}

