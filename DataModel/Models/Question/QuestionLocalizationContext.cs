using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Question
{
    public class QuestionLocalizationContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<QuestionLocalization>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.QuestionType)
                    .IsRequired();

                b.HasAlternateKey(k => new { k.QuestionId, k.QuestionType, k.TranslationId });

                b.HasOne(k => k.Question)
                    .WithMany(k => k.QuestionTranslations)
                    .HasForeignKey(fk => fk.QuestionId)
                    .IsRequired();

                b.HasOne(k => k.Localization)
                    .WithMany(k => k.QuestionLocalizations)
                    .HasForeignKey(fk => fk.TranslationId)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("QuestionLocalizations");
            });
        }
    }
}
