using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Question
{
    public class QuestionReportContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<QuestionReport>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.HasOne(r => r.Question)
                    .WithMany(r => r.QuestionReports)
                    .HasForeignKey(fk => fk.QuestionId)
                    .IsRequired();

                b.HasOne(r => r.User)
                    .WithMany(r => r.QuestionReports)
                    .HasForeignKey(fk => fk.UserId)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("QuestionReports");
            });
        }
    }
}
