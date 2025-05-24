using Data.Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Layer.Contexts.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(patient => patient.Id);

            builder.Property(prop => prop.FirstName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");

            builder.Property(prop => prop.LastName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");

            builder.Property(prop => prop.NationalId)
                .IsRequired()
                .HasMaxLength(14)
                .HasColumnType("char(14)")
                .HasAnnotation("RegularExpression", "^[0-9]{14}$");

            builder.HasIndex(prop => prop.NationalId)
                .IsUnique();

            //builder.Property(prop => prop.Age)
            //    .IsRequired();

            builder.Property(prop => prop.Address)
              
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");

            builder.Property(prop => prop.PhoneNumber)
                .IsRequired()
                .HasMaxLength(14)
                .HasColumnType("char(11)")
                .HasAnnotation("RegularExpression", "^[0-9]{11}$");


            builder.Property(prop => prop.Email)
                .IsRequired()
                .HasAnnotation("RegularExpression", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");


            builder.Property(prop => prop.MaritalStatus)
                .HasConversion<string>()
                .HasColumnType("nvarchar(10)");

            builder.Property(prop => prop.Gender)
                .HasConversion<string>()
                .HasColumnType("nvarchar(5)");

            builder.Property(prop => prop.DateOfBirth)
                .HasColumnType("date");

            builder.Property(prop => prop.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

        }
    }
}
