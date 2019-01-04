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
                    .ValueGeneratedOnUpdate()
                    .IsRequired();

                b.Property(p => p.Key)
                    .IsRequired();

                b.Property(p => p.Value)
                    .IsRequired();

                b.HasAlternateKey(k => new { k.Key, k.Locale });

                b.HasOne(r => r.LocaleReference)
                    .WithMany(r => r.Localizations)
                    .HasForeignKey(fk => fk.Locale)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("Localizations");
            });
        }
    }
}
