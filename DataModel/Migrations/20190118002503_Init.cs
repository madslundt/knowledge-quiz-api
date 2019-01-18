using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataModel.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locale_references",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    code = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locale_references", x => x.id);
                    table.UniqueConstraint("AK_locale_references_code", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "metadata_type_references",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_metadata_type_references", x => x.id);
                    table.UniqueConstraint("AK_metadata_type_references_name", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "question_type_references",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_type_references", x => x.id);
                    table.UniqueConstraint("AK_question_type_references_name", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: true),
                    image_url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    unique_id = table.Column<string>(nullable: false),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.UniqueConstraint("AK_users_unique_id", x => x.unique_id);
                });

            migrationBuilder.CreateTable(
                name: "localizations",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: true),
                    translation = table.Column<string>(nullable: false),
                    locale_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_localizations", x => x.id);
                    table.UniqueConstraint("AK_localizations_translation_locale_id", x => new { x.translation, x.locale_id });
                    table.ForeignKey(
                        name: "fk_localizations_locale_references_locale_reference_id",
                        column: x => x.locale_id,
                        principalTable: "locale_references",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "answers",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    updated = table.Column<DateTime>(nullable: true),
                    question_id = table.Column<Guid>(nullable: false),
                    is_correct = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_answers_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_reports",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    question_id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_reports", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_reports_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_reports_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_metadatas",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    metadata_type = table.Column<int>(nullable: false),
                    value = table.Column<string>(nullable: false),
                    created = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_metadatas", x => x.id);
                    table.UniqueConstraint("AK_user_metadatas_user_id_metadata_type_value", x => new { x.user_id, x.metadata_type, x.value });
                    table.ForeignKey(
                        name: "fk_user_metadatas_metadata_type_references_metadata_type_refer~",
                        column: x => x.metadata_type,
                        principalTable: "metadata_type_references",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_metadatas_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_questions",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    question_id = table.Column<Guid>(nullable: false),
                    hint_used = table.Column<bool>(nullable: false),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_questions_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_questions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_localizations",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    question_id = table.Column<Guid>(nullable: false),
                    localization_id = table.Column<Guid>(nullable: false),
                    question_type = table.Column<int>(nullable: false),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_localizations", x => x.id);
                    table.UniqueConstraint("AK_question_localizations_question_id_localization_id_question~", x => new { x.question_id, x.localization_id, x.question_type });
                    table.ForeignKey(
                        name: "fk_question_localizations_localizations_localization_id",
                        column: x => x.localization_id,
                        principalTable: "localizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_localizations_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_localizations_question_type_references_question_type~",
                        column: x => x.question_type,
                        principalTable: "question_type_references",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "answer_localizations",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    answer_id = table.Column<Guid>(nullable: false),
                    localization_id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_answer_localizations", x => x.id);
                    table.UniqueConstraint("AK_answer_localizations_answer_id_localization_id", x => new { x.answer_id, x.localization_id });
                    table.ForeignKey(
                        name: "fk_answer_localizations_answers_answer_id",
                        column: x => x.answer_id,
                        principalTable: "answers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_answer_localizations_localizations_localization_id",
                        column: x => x.localization_id,
                        principalTable: "localizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_answers",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    answer_id = table.Column<Guid>(nullable: false),
                    created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_answers_answers_answer_id",
                        column: x => x.answer_id,
                        principalTable: "answers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_answers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "locale_references",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { 1, "en_US", "English" },
                    { 2, "da_DK", "Dansk" }
                });

            migrationBuilder.InsertData(
                table: "metadata_type_references",
                columns: new[] { "id", "name" },
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
                table: "question_type_references",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 3, "Final" },
                    { 2, "Hint" },
                    { 1, "Question" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_answer_localizations_localization_id",
                table: "answer_localizations",
                column: "localization_id");

            migrationBuilder.CreateIndex(
                name: "ix_answers_question_id",
                table: "answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_localizations_locale_id",
                table: "localizations",
                column: "locale_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_localizations_localization_id",
                table: "question_localizations",
                column: "localization_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_localizations_question_type",
                table: "question_localizations",
                column: "question_type");

            migrationBuilder.CreateIndex(
                name: "ix_question_reports_question_id",
                table: "question_reports",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_reports_user_id",
                table: "question_reports",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_answers_answer_id",
                table: "user_answers",
                column: "answer_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_answers_user_id",
                table: "user_answers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_metadatas_metadata_type",
                table: "user_metadatas",
                column: "metadata_type");

            migrationBuilder.CreateIndex(
                name: "ix_user_questions_question_id",
                table: "user_questions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_questions_user_id",
                table: "user_questions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_unique_id",
                table: "users",
                column: "unique_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answer_localizations");

            migrationBuilder.DropTable(
                name: "question_localizations");

            migrationBuilder.DropTable(
                name: "question_reports");

            migrationBuilder.DropTable(
                name: "user_answers");

            migrationBuilder.DropTable(
                name: "user_metadatas");

            migrationBuilder.DropTable(
                name: "user_questions");

            migrationBuilder.DropTable(
                name: "localizations");

            migrationBuilder.DropTable(
                name: "question_type_references");

            migrationBuilder.DropTable(
                name: "answers");

            migrationBuilder.DropTable(
                name: "metadata_type_references");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "locale_references");

            migrationBuilder.DropTable(
                name: "questions");
        }
    }
}
