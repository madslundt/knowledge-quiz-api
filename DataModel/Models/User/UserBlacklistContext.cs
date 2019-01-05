using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.User
{
    public class UserBlacklistContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<UserBlacklist>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.HasOne(r => r.User)
                    .WithOne(r => r.UserBlacklist)
                    .HasForeignKey<UserBlacklist>(fk => fk.UserId)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("UserBlacklist");
            });
        }
    }
}
