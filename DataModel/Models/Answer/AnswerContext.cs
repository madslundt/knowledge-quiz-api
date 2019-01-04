using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Answer
{
    public class AnswerContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<Answer>(b =>
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

                b.HasOne(r => r.Question)
                    .WithMany(r => r.Answers)
                    .HasForeignKey(fk => fk.QuestionId)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("Answers");
            });
        }
    }
}
