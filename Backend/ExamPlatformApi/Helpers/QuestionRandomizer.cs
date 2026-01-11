using ExamPlatform.API.Models;

namespace ExamPlatform.API.Helpers;

public static class QuestionRandomizer
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Randomizes the order of questions and options for each question
    /// Returns a tuple with randomized questions and a mapping of question IDs to option mappings
    /// </summary>
    public static (List<Question> RandomizedQuestions, Dictionary<string, Dictionary<int, int>> OptionMappings) RandomizeQuestionsWithMapping(List<Question> questions)
    {
        var optionMappings = new Dictionary<string, Dictionary<int, int>>();
        
        // Create a copy to avoid modifying the original
        var randomizedQuestions = new List<Question>();
        
        foreach (var q in questions)
        {
            var (randomizedOptions, mapping) = RandomizeOptionsWithMapping(q.Options);
            var randomizedQuestion = new Question
            {
                Id = q.Id,
                ExamId = q.ExamId,
                QuestionText = q.QuestionText,
                QuestionType = q.QuestionType,
                Options = randomizedOptions,
                CorrectAnswer = q.CorrectAnswer,
                Marks = q.Marks,
                Order = q.Order
            };
            
            randomizedQuestions.Add(randomizedQuestion);
            if (q.Id != null)
            {
                optionMappings[q.Id] = mapping;
            }
        }

        // Shuffle questions
        for (int i = randomizedQuestions.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (randomizedQuestions[i], randomizedQuestions[j]) = (randomizedQuestions[j], randomizedQuestions[i]);
        }

        // Update order after shuffling
        for (int i = 0; i < randomizedQuestions.Count; i++)
        {
            randomizedQuestions[i].Order = i + 1;
        }

        return (randomizedQuestions, optionMappings);
    }

    /// <summary>
    /// Randomizes the order of options and returns the mapping (original index -> new index)
    /// </summary>
    private static (List<string> RandomizedOptions, Dictionary<int, int> Mapping) RandomizeOptionsWithMapping(List<string> originalOptions)
    {
        if (originalOptions.Count <= 1)
        {
            var mapping = new Dictionary<int, int>();
            for (int i = 0; i < originalOptions.Count; i++)
            {
                mapping[i] = i;
            }
            return (new List<string>(originalOptions), mapping);
        }

        // Create indexed list
        var indexedOptions = originalOptions.Select((opt, idx) => new { Index = idx, Option = opt }).ToList();
        
        // Shuffle
        for (int i = indexedOptions.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (indexedOptions[i], indexedOptions[j]) = (indexedOptions[j], indexedOptions[i]);
        }

        // Create mapping: original index -> new index
        var mappingDict = new Dictionary<int, int>();
        for (int newIdx = 0; newIdx < indexedOptions.Count; newIdx++)
        {
            mappingDict[indexedOptions[newIdx].Index] = newIdx;
        }

        return (indexedOptions.Select(x => x.Option).ToList(), mappingDict);
    }

    /// <summary>
    /// Updates the correct answer after options have been randomized using the mapping
    /// </summary>
    public static string UpdateCorrectAnswerAfterRandomization(string originalCorrectAnswer, List<string> originalOptions, List<string> randomizedOptions, Dictionary<int, int> optionMapping)
    {
        if (string.IsNullOrWhiteSpace(originalCorrectAnswer) || originalOptions.Count == 0 || randomizedOptions.Count == 0)
            return originalCorrectAnswer;

        // Find the index of the correct answer in original options
        int originalIndex = -1;
        
        // Try exact match first
        originalIndex = originalOptions.FindIndex(opt => 
            opt.Equals(originalCorrectAnswer, StringComparison.OrdinalIgnoreCase) ||
            opt.Trim().Equals(originalCorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase));

        if (originalIndex == -1)
        {
            // Try to match by letter (A, B, C, D)
            var letterMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "A", 0 }, { "B", 1 }, { "C", 2 }, { "D", 3 }, { "E", 4 }
            };
            
            if (letterMap.TryGetValue(originalCorrectAnswer.Trim(), out int letterIndex) && letterIndex < originalOptions.Count)
            {
                originalIndex = letterIndex;
            }
        }

        if (originalIndex >= 0 && originalIndex < originalOptions.Count)
        {
            // Use mapping to find new index
            if (optionMapping.TryGetValue(originalIndex, out int newIndex) && newIndex < randomizedOptions.Count)
            {
                return randomizedOptions[newIndex];
            }
        }

        return originalCorrectAnswer;
    }
}

