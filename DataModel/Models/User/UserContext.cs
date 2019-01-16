using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.User
{
    public class UserContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<User>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.HasIndex(i => i.UniqueId)
                    .IsUnique();

                b.HasAlternateKey(k => k.UniqueId);

                b.HasKey(k => k.Id);
            });
        }
    }
}
