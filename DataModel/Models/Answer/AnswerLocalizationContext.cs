using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Answer
{
    public class AnswerLocalizationContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<AnswerLocalization>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.HasAlternateKey(k => new { k.AnswerId, k.LocalizationId });

                b.HasOne(k => k.Answer)
                    .WithMany(k => k.AnswerLocalizations)
                    .HasForeignKey(fk => fk.AnswerId)
                    .IsRequired();

                b.HasOne(k => k.Localization)
                    .WithMany(k => k.AnswerLocalizations)
                    .HasForeignKey(fk => fk.LocalizationId)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("AnswerLocalizations");
            });
        }
    }
}
