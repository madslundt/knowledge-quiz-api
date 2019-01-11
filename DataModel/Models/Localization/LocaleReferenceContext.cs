using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

                b.HasData(GetSeedData());
            });
        }

        private static ICollection<LocaleReference> GetSeedData()
        {
            var result = Enum.GetValues(typeof(Locale)).Cast<Locale>().Select(userMetadata =>
            {
                var name = GetDisplayValue(userMetadata);

                return new LocaleReference
                {
                    Id = userMetadata,
                    Code = userMetadata.ToString(),
                    Name = name
                };
            }).ToList();

            return result;
        }

        private static string GetDisplayValue(Locale value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }
}
