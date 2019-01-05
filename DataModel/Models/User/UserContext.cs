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

                b.HasIndex(i => i.DeviceId)
                    .IsUnique();

                b.HasAlternateKey(k => k.DeviceId);

                b.HasKey(k => k.Id);
                b.ToTable("Users");
            });
        }
    }
}
