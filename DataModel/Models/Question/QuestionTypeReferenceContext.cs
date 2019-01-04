using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Question
{
    public class QuestionTypeReferenceContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<QuestionTypeReference>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.Property(p => p.Name)
                    .IsRequired();

                b.HasAlternateKey(k => k.Name);

                b.HasKey(k => k.Id);
                b.ToTable("QuestionTypeReferences");
            });
        }
    }
}
