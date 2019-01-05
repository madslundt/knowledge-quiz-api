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

                b.HasAlternateKey(k => new { k.QuestionId, k.LocalizationId, k.QuestionTypeReference });

                b.HasOne(k => k.Question)
                    .WithMany(k => k.QuestionLocalizations)
                    .HasForeignKey(fk => fk.QuestionId)
                    .IsRequired();

                b.HasOne(k => k.Localization)
                    .WithMany(k => k.QuestionLocalizations)
                    .HasForeignKey(fk => fk.LocalizationId)
                    .IsRequired();

                b.HasOne(r => r.QuestionTypeReference)
                    .WithMany()
                    .HasForeignKey(fk => fk.QuestionType)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("QuestionLocalizations");
            });
        }
    }
}
