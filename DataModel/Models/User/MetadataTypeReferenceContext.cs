using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.User
{
    public class MetadataTypeReferenceContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<MetadataTypeReference>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.Property(p => p.Name)
                    .IsRequired();

                b.HasAlternateKey(k => k.Name);

                b.HasKey(k => k.Id);
                b.ToTable("MetadataTypeReferences");
            });
        }
    }
}
