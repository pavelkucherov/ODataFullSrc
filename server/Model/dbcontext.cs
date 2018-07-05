using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace courses_odata.Model
{
    public class CoursesContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<TeachingActivity> TeachingActivities { get; set; }
        // public DbSet<MultipleChoice> MultipleChoices { get; set; }
        // public DbSet<Slide> Slides { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Article> Articles { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course_Student> Course_Student { get; set; }

        public CoursesContext() : base()
        {
        }

        public CoursesContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasMany(p => p.Lectures)
                .WithOne(b => b.Course)
                .HasForeignKey(b => b.CourseId);

            modelBuilder.Entity<Lecture>()
                .HasMany(p => p.TeachingActivities)
                .WithOne(b => b.Lecture)
                .HasForeignKey(b => b.LectureId);

            modelBuilder.Entity<TeachingActivity>()
                .HasMany(p => p.Answers)
                .WithOne(b => b.TeachingActivity)
                .HasForeignKey(b => b.TeachingActivityId);

            // modelBuilder.Entity<Slide>()
            //     .HasBaseType<TeachingActivity>();

            // modelBuilder.Entity<MultipleChoice>()
            //     .HasBaseType<TeachingActivity>()
            //     .HasMany(p => p.Answers)
            //     .WithOne(b => b.MultipleChoice)
            //     .HasForeignKey(b => b.MultipleChoiceId);

            modelBuilder.Entity<Answer>();

            modelBuilder.Entity<Article>();

            modelBuilder.Entity<Student>();
        }
    }
}