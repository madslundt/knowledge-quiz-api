using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Question
{
    public class QuestionContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<Question>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Updated)
                    .ValueGeneratedOnUpdate();

                b.Property(p => p.ImageUrl);
                
                b.HasKey(k => k.Id);
                b.ToTable("Questions");
            });
        }
    }
}
