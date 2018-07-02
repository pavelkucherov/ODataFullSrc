﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using courses_odata.Model;

namespace server.Migrations
{
    [DbContext(typeof(CoursesContext))]
    partial class CoursesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799");

            modelBuilder.Entity("courses_odata.Model.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<bool>("IsCorrect");

                    b.Property<int>("TeachingActivityId");

                    b.HasKey("Id");

                    b.HasIndex("TeachingActivityId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("courses_odata.Model.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Points");

                    b.Property<int>("Position");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("courses_odata.Model.Lecture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<int>("CourseId");

                    b.Property<bool>("NotRequired");

                    b.Property<int>("Position");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Lectures");
                });

            modelBuilder.Entity("courses_odata.Model.TeachingActivity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("LectureId");

                    b.Property<int>("Position");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("LectureId");

                    b.ToTable("TeachingActivities");

                    b.HasDiscriminator<string>("Discriminator").HasValue("TeachingActivity");
                });

            modelBuilder.Entity("courses_odata.Model.MultipleChoice", b =>
                {
                    b.HasBaseType("courses_odata.Model.TeachingActivity");

                    b.Property<string>("Question");

                    b.ToTable("MultipleChoice");

                    b.HasDiscriminator().HasValue("MultipleChoice");
                });

            modelBuilder.Entity("courses_odata.Model.Slide", b =>
                {
                    b.HasBaseType("courses_odata.Model.TeachingActivity");

                    b.Property<string>("Content");

                    b.ToTable("Slide");

                    b.HasDiscriminator().HasValue("Slide");
                });

            modelBuilder.Entity("courses_odata.Model.Answer", b =>
                {
                    b.HasOne("courses_odata.Model.TeachingActivity", "TeachingActivity")
                        .WithMany("Answers")
                        .HasForeignKey("TeachingActivityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("courses_odata.Model.Lecture", b =>
                {
                    b.HasOne("courses_odata.Model.Course", "Course")
                        .WithMany("Lectures")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("courses_odata.Model.TeachingActivity", b =>
                {
                    b.HasOne("courses_odata.Model.Lecture", "Lecture")
                        .WithMany("TeachingActivities")
                        .HasForeignKey("LectureId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
