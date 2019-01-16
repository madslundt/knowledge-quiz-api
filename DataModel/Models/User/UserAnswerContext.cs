using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.User
{
    public class UserAnswerContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<UserAnswer>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.HasOne(r => r.Answer)
                    .WithMany(r => r.UserAnswers)
                    .HasForeignKey(fk => fk.AnswerId)
                    .IsRequired();

                b.HasOne(r => r.User)
                    .WithMany(r => r.UserAnswers)
                    .HasForeignKey(fk => fk.UserId)
                    .IsRequired();

                b.HasKey(k => k.Id);
            });
        }
    }
}
