using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using server.Controllers;

namespace courses_odata.Model
{
    public class Course
    {
        public Course() => this.Lectures = new Collection<Lecture>();

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
        public virtual ICollection<Lecture> Lectures { get; set; }

        [RowLevelSecurity(KeyName = "StudentId")]
        public virtual ICollection<Course_Student> Course_Students { get; set; }
    }

    public class Lecture
    {
        public Lecture() => this.TeachingActivities = new Collection<TeachingActivity>();

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool NotRequired { get; set; }
        public int Position { get; set; }
        public virtual ICollection<TeachingActivity> TeachingActivities { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }

    public enum TeachingActivityKind { Slide = 1, MultipleChoice = 2 }

    public class TeachingActivity
    {
        public TeachingActivity() => this.Answers = new Collection<Answer>();

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public int LectureId { get; set; }
        public Lecture Lecture { get; set; }
        public string Question { get; set; }
        public string Content { get; set; }
        public TeachingActivityKind Discriminator { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }

    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public bool IsCorrect { get; set; }
        public string Content { get; set; }
        public int TeachingActivityId { get; set; }
        public TeachingActivity TeachingActivity { get; set; }
    }

    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Course_Student> Course_Students { get; set; }
    }

    public class Course_Student
    {
        [Key]
        public int Id { get; set; }

        public virtual Course Course { get; set; }
        public int CourseId { get; set; }
        public virtual Student Student { get; set; }

        public int StudentId { get; set; }

    }
}
