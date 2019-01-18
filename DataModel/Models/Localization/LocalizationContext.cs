using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Localization
{
    public class LocalizationContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<Localization>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Updated)
                    .ValueGeneratedOnUpdate();

                b.Property(p => p.Translation)
                    .IsRequired();

                b.HasAlternateKey(k => new { Value = k.Translation, Locale = k.LocaleId });

                b.HasOne(r => r.LocaleReference)
                    .WithMany()
                    .HasForeignKey(fk => fk.LocaleId)
                    .IsRequired();

                b.HasKey(k => k.Id);
            });
        }
    }
}
