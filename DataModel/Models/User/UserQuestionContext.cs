using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Models.User
{
    public class UserQuestionContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<UserQuestion>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.Created)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.HintUsed)
                    .IsRequired();

                b.HasOne(r => r.Question)
                    .WithMany(r => r.UserQuestions)
                    .HasForeignKey(fk => fk.QuestionId)
                    .IsRequired();

                b.HasOne(r => r.User)
                    .WithMany(r => r.UserQuestions)
                    .HasForeignKey(fk => fk.UserId)
                    .IsRequired();

                b.HasKey(k => k.Id);
                b.ToTable("UserQuestions");
            });
        }
    }
}
