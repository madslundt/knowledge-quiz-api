using DataModel.Models;
using DataModel.Models.Answer;
using DataModel.Models.Localization;
using DataModel.Models.Question;
using DataModel.Models.User;
using Microsoft.EntityFrameworkCore;

namespace DataModel
{
    // Add migration
    // dotnet ef migrations add <MIGRATION NAME> -s ../Src

    // Update database with latest migration(s)
    // dotnet ef database update -s ../Src

    // Remove latest migration
    // dotnet ef migrations remove -s ../Src

    // Revert the database to a migration
    // dotnet ef database update <MIGRATION NAME> -s ../Src

    // Generate SQL script
    // dotnet ef migrations script -s ../Src

    // -s ../Src is used to point to appsettings in Src application. 
    // Remember to set environment eg. 'setx ASPNETCORE_ENVIRONMENT Development'

    public class DatabaseContext : DbContext
    {
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
    }
}
