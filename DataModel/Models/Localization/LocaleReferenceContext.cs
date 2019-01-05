using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Localization
{
    public class LocaleReferenceContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<LocaleReference>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.Property(p => p.Code)
                    .IsRequired();

                b.Property(p => p.Name)
                    .IsRequired();

                b.HasAlternateKey(k => k.Code);

                b.HasKey(k => k.Id);
                b.ToTable("LocaleReferences");
            });
        }
    }
}
