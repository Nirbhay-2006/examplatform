using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamNest.Migrations
{
    /// <inheritdoc />
    public partial class AddAntiCheatingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exam_sessions",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    exam_id = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ip_address = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    device_info = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    is_violated = table.Column<bool>(type: "bit", nullable: false),
                    access_locked_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    violation_reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exam_sessions", x => x.session_id);
                });

            migrationBuilder.CreateTable(
                name: "exam_submissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    session_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    mode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    answers_snapshot = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exam_submissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_exam_submissions_exam_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "exam_sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "monitoring_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    session_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    event_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    event_time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    details = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monitoring_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_monitoring_logs_exam_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "exam_sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "violation_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    session_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    violation_type = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    action_taken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_violation_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_violation_logs_exam_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "exam_sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exam_sessions_exam_id_user_id_status",
                table: "exam_sessions",
                columns: new[] { "exam_id", "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_exam_sessions_status",
                table: "exam_sessions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_exam_sessions_user_id_exam_id",
                table: "exam_sessions",
                columns: new[] { "user_id", "exam_id" },
                unique: true,
                filter: "[status] = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_exam_submissions_session_id",
                table: "exam_submissions",
                column: "session_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_monitoring_logs_session_id_event_time",
                table: "monitoring_logs",
                columns: new[] { "session_id", "event_time" });

            migrationBuilder.CreateIndex(
                name: "IX_violation_logs_session_id_timestamp",
                table: "violation_logs",
                columns: new[] { "session_id", "timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exam_submissions");

            migrationBuilder.DropTable(
                name: "monitoring_logs");

            migrationBuilder.DropTable(
                name: "violation_logs");

            migrationBuilder.DropTable(
                name: "exam_sessions");
        }
    }
}
