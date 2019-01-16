using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataModel.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocaleReferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocaleReferences", x => x.Id);
                    table.UniqueConstraint("AK_LocaleReferences_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "MetadataTypeReferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataTypeReferences", x => x.Id);
                    table.UniqueConstraint("AK_MetadataTypeReferences_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTypeReferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypeReferences", x => x.Id);
                    table.UniqueConstraint("AK_QuestionTypeReferences_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UniqueId = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_UniqueId", x => x.UniqueId);
                });

            migrationBuilder.CreateTable(
                name: "Localizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    Text = table.Column<string>(nullable: false),
                    Locale = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizations", x => x.Id);
                    table.UniqueConstraint("AK_Localizations_Text_Locale", x => new { x.Text, x.Locale });
                    table.ForeignKey(
                        name: "FK_Localizations_LocaleReferences_Locale",
                        column: x => x.Locale,
                        principalTable: "LocaleReferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    QuestionId = table.Column<Guid>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionReports_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MetadataType = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMetadatas", x => x.Id);
                    table.UniqueConstraint("AK_UserMetadatas_UserId_MetadataType_Value", x => new { x.UserId, x.MetadataType, x.Value });
                    table.ForeignKey(
                        name: "FK_UserMetadatas_MetadataTypeReferences_MetadataType",
                        column: x => x.MetadataType,
                        principalTable: "MetadataTypeReferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMetadatas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false),
                    HintUsed = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserQuestions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionLocalizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    QuestionType = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionLocalizations", x => x.Id);
                    table.UniqueConstraint("AK_QuestionLocalizations_QuestionId_LocalizationId_QuestionType", x => new { x.QuestionId, x.LocalizationId, x.QuestionType });
                    table.ForeignKey(
                        name: "FK_QuestionLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionLocalizations_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionLocalizations_QuestionTypeReferences_QuestionType",
                        column: x => x.QuestionType,
                        principalTable: "QuestionTypeReferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerLocalizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AnswerId = table.Column<Guid>(nullable: false),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerLocalizations", x => x.Id);
                    table.UniqueConstraint("AK_AnswerLocalizations_AnswerId_LocalizationId", x => new { x.AnswerId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AnswerLocalizations_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    AnswerId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LocaleReferences",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "en_US", "English" },
                    { 2, "da_DK", "Dansk" }
                });

            migrationBuilder.InsertData(
                table: "MetadataTypeReferences",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 10, "Timezone" },
                    { 9, "SystemVersion" },
                    { 8, "SystemName" },
                    { 7, "Manufacturer" },
                    { 6, "MacAddress" },
                    { 5, "DeviceName" },
                    { 4, "DeviceLocale" },
                    { 3, "DeviceCountry" },
                    { 2, "BuildNumber" },
                    { 1, "Brand" },
                    { 11, "UniqueId" },
                    { 12, "Version" }
                });

            migrationBuilder.InsertData(
                table: "QuestionTypeReferences",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "Final" },
                    { 2, "Hint" },
                    { 1, "Question" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerLocalizations_LocalizationId",
                table: "AnswerLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Localizations_Locale",
                table: "Localizations",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionLocalizations_LocalizationId",
                table: "QuestionLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionLocalizations_QuestionType",
                table: "QuestionLocalizations",
                column: "QuestionType");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReports_QuestionId",
                table: "QuestionReports",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReports_UserId",
                table: "QuestionReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_AnswerId",
                table: "UserAnswers",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_UserId",
                table: "UserAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMetadatas_MetadataType",
                table: "UserMetadatas",
                column: "MetadataType");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestions_QuestionId",
                table: "UserQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuestions_UserId",
                table: "UserQuestions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UniqueId",
                table: "Users",
                column: "UniqueId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerLocalizations");

            migrationBuilder.DropTable(
                name: "QuestionLocalizations");

            migrationBuilder.DropTable(
                name: "QuestionReports");

            migrationBuilder.DropTable(
                name: "UserAnswers");

            migrationBuilder.DropTable(
                name: "UserMetadatas");

            migrationBuilder.DropTable(
                name: "UserQuestions");

            migrationBuilder.DropTable(
                name: "Localizations");

            migrationBuilder.DropTable(
                name: "QuestionTypeReferences");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "MetadataTypeReferences");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "LocaleReferences");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
