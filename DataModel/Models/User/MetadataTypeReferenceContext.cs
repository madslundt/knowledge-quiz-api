using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.User
{
    public class MetadataTypeReferenceContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<MetadataTypeReference>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.Property(p => p.Name)
                    .IsRequired();

                b.HasAlternateKey(k => k.Name);

                b.HasKey(k => k.Id);

                b.HasData(GetSeedData());
            });
        }

        private static MetadataTypeReference[] GetSeedData()
        {
            var result = Enum.GetValues(typeof(UserMetadataType)).Cast<UserMetadataType>().Select(userMetadata =>
                new MetadataTypeReference
                {
                    Id = userMetadata,
                    Name = userMetadata.ToString()
                }).ToArray();

            return result;
        }
    }
}
