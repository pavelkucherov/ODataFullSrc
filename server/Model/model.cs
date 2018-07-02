using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

    public abstract class TeachingActivity
    {
        public TeachingActivity() => Answers = new Collection<Answer>();
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public int LectureId { get; set; }
        public Lecture Lecture { get; set; }
        public virtual ICollection<Answer> Answers { get; set; } // workaround to avoid nullref in odata
    }

    public class MultipleChoice : TeachingActivity
    {
        //public MultipleChoice() => this.Answers = new Collection<Answer>();
        public string Question { get; set; }
        //public override ICollection<Answer> Answers { get; set; }
    }

    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public bool IsCorrect { get; set; }
        public string Content { get; set; }
        //public int MultipleChoiceId { get; set; }
        //public MultipleChoice MultipleChoice { get; set; }

        public int TeachingActivityId { get; set; }
        public TeachingActivity TeachingActivity { get; set; }
    }

    public class Slide : TeachingActivity
    {
        public string Content { get; set; }
        public override ICollection<Answer> Answers { get { return null; } set { } }
    }
}