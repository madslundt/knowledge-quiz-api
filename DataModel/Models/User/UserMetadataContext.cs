using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.User
{
    public class UserMetadataContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<UserMetadata>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Value)
                    .IsRequired();

                b.HasAlternateKey(k => new { k.UserId, k.MetadataType, k.Value });

                b.HasOne(r => r.MetadataTypeReference)
                    .WithMany()
                    .HasForeignKey(fk => fk.MetadataType)
                    .IsRequired();

                b.HasOne(r => r.User)
                    .WithMany(r => r.Metadatas)
                    .HasForeignKey(fk => fk.UserId)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("UserMetadatas");
            });
        }
    }
}
