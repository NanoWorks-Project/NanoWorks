﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Sample.WebApi.Data.Database;

#nullable disable

namespace Sample.WebApi.Data.Database.Migrations;

[DbContext(typeof(BookStoreDatabase))]
[Migration("20240609182430_InitialCreate")]
partial class InitialCreate
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.3")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("Sample.WebApi.Models.Entities.Author", b =>
            {
                b.Property<Guid>("AuthorId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("FirstName")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("LastName")
                    .IsRequired()
                    .HasColumnType("text");

                b.HasKey("AuthorId");

                b.ToTable("Authors");
            });

        modelBuilder.Entity("Sample.WebApi.Models.Entities.Book", b =>
            {
                b.Property<Guid>("BookId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<Guid>("AuthorId")
                    .HasColumnType("uuid");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("text");

                b.HasKey("BookId");

                b.HasIndex("AuthorId");

                b.ToTable("Books");
            });

        modelBuilder.Entity("Sample.WebApi.Models.Entities.Book", b =>
            {
                b.HasOne("Sample.WebApi.Models.Entities.Author", "Author")
                    .WithMany()
                    .HasForeignKey("AuthorId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Author");
            });
#pragma warning restore 612, 618
    }
}
