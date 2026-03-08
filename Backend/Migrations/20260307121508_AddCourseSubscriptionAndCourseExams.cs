using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamNest.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseSubscriptionAndCourseExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_exams",
                columns: table => new
                {
                    course_exam_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    duration_minutes = table.Column<int>(type: "int", nullable: false),
                    is_published = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_exams", x => x.course_exam_id);
                    table.ForeignKey(
                        name: "FK_course_exams_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_subscriptions",
                columns: table => new
                {
                    subscription_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    subscribed_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_subscriptions", x => x.subscription_id);
                    table.ForeignKey(
                        name: "FK_course_subscriptions_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_subscriptions_users_student_id",
                        column: x => x.student_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_exams_course_id",
                table: "course_exams",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_subscriptions_course_id_student_id",
                table: "course_subscriptions",
                columns: new[] { "course_id", "student_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_subscriptions_student_id",
                table: "course_subscriptions",
                column: "student_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_exams");

            migrationBuilder.DropTable(
                name: "course_subscriptions");
        }
    }
}
