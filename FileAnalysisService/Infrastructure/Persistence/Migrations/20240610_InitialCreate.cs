using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileAnalysisService.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "Results",
            schema: "public",
            columns: table => new
            {
                FileId = table.Column<Guid>(type: "uuid", nullable: false),
                ParagraphCount = table.Column<int>(type: "integer", nullable: false),
                WordCount = table.Column<int>(type: "integer", nullable: false),
                CharCount = table.Column<int>(type: "integer", nullable: false),
                CloudLocation = table.Column<string>(type: "text", nullable: false),
                AnalyzedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Results", x => x.FileId);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Results",
            schema: "public");
    }
} 