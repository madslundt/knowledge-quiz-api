using System;
using System.Text.RegularExpressions;
using DataModel.Models;
using DataModel.Models.Answer;
using DataModel.Models.Localization;
using DataModel.Models.Question;
using DataModel.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using Npgsql.NameTranslation;

namespace DataModel
{
    // Add migration
    // dotnet ef migrations add <MIGRATION NAME> -s ../API

    // Update database with latest migration(s)
    // dotnet ef database update -s ../API

    // Remove latest migration
    // dotnet ef migrations remove -s ../API

    // Revert the database to a migration
    // dotnet ef database update <MIGRATION NAME> -s ../API

    // Generate SQL script
    // dotnet ef migrations script -s ../API

    // -s ../API is used to point to appsettings in API application. 
    // Remember to set environment eg. 'setx ASPNETCORE_ENVIRONMENT Development'

    public class DatabaseContext : DbContext
    {
        private static readonly Regex _keysRegex = new Regex("^(PK|FK|IX)_", RegexOptions.Compiled);


        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserMetadata> UserMetadatas { get; set; }
        public DbSet<MetadataTypeReference> MetadataTypeReferences { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<UserQuestion> UserQuestions { get; set; }
        public DbSet<QuestionReport> QuestionReports { get; set; }

        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionLocalization> QuestionLocalizations { get; set; }
        public DbSet<QuestionTypeReference> QuestionTypeReferences { get; set; }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnswerLocalization> AnswerLocalizations { get; set; }

        public DbSet<Localization> Localizations { get; set; }
        public DbSet<LocaleReference> LocaleReferences { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            FixSnakeCaseNames(builder);

            UserContext.Build(builder);
            UserMetadataContext.Build(builder);
            MetadataTypeReferenceContext.Build(builder);
            UserAnswerContext.Build(builder);
            UserQuestionContext.Build(builder);
            QuestionReportContext.Build(builder);

            QuestionContext.Build(builder);
            QuestionLocalizationContext.Build(builder);
            QuestionTypeReferenceContext.Build(builder);

            AnswerContext.Build(builder);
            AnswerLocalizationContext.Build(builder);

            LocalizationContext.Build(builder);
            LocaleReferenceContext.Build(builder);
        }

        private void FixSnakeCaseNames(ModelBuilder modelBuilder)
        {
            var mapper = new NpgsqlSnakeCaseNameTranslator();
            foreach (var table in modelBuilder.Model.GetEntityTypes())
            {
                ConvertToSnake(mapper, table);
                foreach (var property in table.GetProperties())
                {
                    ConvertToSnake(mapper, property);
                }

                foreach (var primaryKey in table.GetKeys())
                {
                    ConvertToSnake(mapper, primaryKey);
                }

                foreach (var foreignKey in table.GetForeignKeys())
                {
                    ConvertToSnake(mapper, foreignKey);
                }

                foreach (var indexKey in table.GetIndexes())
                {
                    ConvertToSnake(mapper, indexKey);
                }
            }
        }

        private void ConvertToSnake(INpgsqlNameTranslator mapper, object entity)
        {
            switch (entity)
            {
                case IMutableEntityType table:
                    var relationalTable = table.Relational();
                    relationalTable.TableName = ConvertGeneralToSnake(mapper, relationalTable.TableName);
                    if (relationalTable.TableName.StartsWith("asp_net_"))
                    {
                        relationalTable.TableName = relationalTable.TableName.Replace("asp_net_", string.Empty);
                        relationalTable.Schema = "identity";
                    }

                    break;
                case IMutableProperty property:
                    var colmnName = property.Relational().ColumnName;
                    property.Relational().ColumnName = ConvertGeneralToSnake(mapper, property.Relational().ColumnName);
                    break;
                case IMutableKey primaryKey:
                    primaryKey.Relational().Name = ConvertKeyToSnake(mapper, primaryKey.Relational().Name);
                    break;
                case IMutableForeignKey foreignKey:
                    foreignKey.Relational().Name = ConvertKeyToSnake(mapper, foreignKey.Relational().Name);
                    break;
                case IMutableIndex indexKey:
                    indexKey.Relational().Name = ConvertKeyToSnake(mapper, indexKey.Relational().Name);
                    break;
                default:
                    throw new NotImplementedException("Unexpected type was provided to snake case converter");
            }
        }

        private string ConvertKeyToSnake(INpgsqlNameTranslator mapper, string keyName) =>
            ConvertGeneralToSnake(mapper, _keysRegex.Replace(keyName, match => match.Value.ToLower()));

        private string ConvertGeneralToSnake(INpgsqlNameTranslator mapper, string entityName) =>
            mapper.TranslateMemberName(ModifyNameBeforeConvertion(mapper, entityName));

        protected virtual string ModifyNameBeforeConvertion(INpgsqlNameTranslator mapper, string entityName) => entityName;
    }
}
