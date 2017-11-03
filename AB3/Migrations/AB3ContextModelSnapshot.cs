﻿// <auto-generated />
using AB3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace AB3.Migrations
{
    [DbContext(typeof(AB3Context))]
    partial class AB3ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AB3.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryName");

                    b.HasKey("CategoryId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("AB3.Models.Image", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageName");

                    b.Property<bool>("IsCover");

                    b.Property<int?>("ProjectId");

                    b.Property<string>("Src");

                    b.HasKey("ImageId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Image");
                });

            modelBuilder.Entity("AB3.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name");

                    b.Property<double>("Price");

                    b.Property<int>("UnitsInStock");

                    b.Property<int>("ViewCount");

                    b.Property<string>("Year");

                    b.HasKey("ProjectId");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("AB3.Models.ProjectCategory", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("CategoryId");

                    b.HasKey("ProjectId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("ProjectCategory");
                });

            modelBuilder.Entity("AB3.Models.Image", b =>
                {
                    b.HasOne("AB3.Models.Project", "Project")
                        .WithMany("Images")
                        .HasForeignKey("ProjectId");
                });

            modelBuilder.Entity("AB3.Models.ProjectCategory", b =>
                {
                    b.HasOne("AB3.Models.Category", "Category")
                        .WithMany("ProjectCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AB3.Models.Project", "Project")
                        .WithMany("ProjectCategories")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}