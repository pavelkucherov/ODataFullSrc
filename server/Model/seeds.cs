using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using courses_odata.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace courses_odata
{
    internal static class CoursesInitializer
    {
        private static List<Course> courses;
        private static List<Lecture> lectures;
        private static List<TeachingActivity> teaching_activities;
        private static List<Answer> answers;
        private static List<Article> articles;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CoursesContext(
                           serviceProvider.GetRequiredService<DbContextOptions<CoursesContext>>()))
            {
                if (context.Courses.Any()) return;

                var r = new Random();

                courses = new List<Course>
                {
                  new Course() {
                    Name = "Dev 1",
                    Points = 4,
                    Position = 1
                  },
                  new Course() {
                    Name = "Dev 2",
                    Points = 4,
                    Position = 2
                  },
                  new Course() {
                    Name = "Dev 3",
                    Points = 4,
                    Position = 1
                  },
                };
                if (!context.Courses.Any())
                {
                    context.Courses.AddRange(courses);
                    context.SaveChanges();
                }

                lectures = new List<Lecture>();
                foreach (var course in courses)
                {
                    for (int i = 0; i < r.Next(3, 6); i++)
                    {
                        lectures.Add(
                          new Lecture()
                          {
                              Title = $"{course.Name}, Lec {i + 1}",
                              Content = "Blahblah",
                              NotRequired = r.NextDouble() > 0.9,
                              Position = i,
                              CourseId = course.Id,
                          }
                        );
                    }
                }

                if (!context.Lectures.Any())
                {
                    context.Lectures.AddRange(lectures);
                    context.SaveChanges();
                }

                teaching_activities = new List<TeachingActivity>();
                foreach (var lecture in lectures)
                {
                    for (int i = 0; i < r.Next(3, 6); i++)
                    {
                        if (r.NextDouble() > 0.5)
                        {
                            teaching_activities.Add(
                              new TeachingActivity()
                              {
                                  Discriminator = TeachingActivityKind.Slide,
                                  Title = $"{lecture.Title}, Slide {i + 1}",
                                  Content = "Blahblah",
                                  Position = i,
                                  LectureId = lecture.Id,
                              }
                            );
                        }
                        else
                        {
                            teaching_activities.Add(
                              new TeachingActivity()
                              {
                                  Discriminator = TeachingActivityKind.MultipleChoice,
                                  Title = $"{lecture.Title}, Question {i + 1}",
                                  Question = "Blahblah",
                                  Position = i,
                                  LectureId = lecture.Id,
                              }
                            );
                        }
                    }
                }

                if (!context.TeachingActivities.Any())
                {
                    context.TeachingActivities.AddRange(teaching_activities);
                    context.SaveChanges();
                }

                answers = new List<Answer>();
                foreach (var mc in teaching_activities.Where(ta => ta.Discriminator == TeachingActivityKind.MultipleChoice))
                {
                    for (int i = 0; i < r.Next(3, 5); i++)
                    {
                        answers.Add(
                          new Answer()
                          {
                              IsCorrect = i == 0,
                              Content = "blah blah answer",
                              TeachingActivityId = mc.Id
                          }
                        );
                    }
                }

                if (!context.Answers.Any())
                {
                    context.Answers.AddRange(answers);
                    context.SaveChanges();
                }

                articles = new List<Article>
                {
                  new Article() {
                    Category = "Student Life",
                    Title = "Finding accommodation",
                    Body = "Nobody said it was easy."
                  },
                  new Article() {
                    Category = "Student Life",
                    Title = "Get organized",
                    Body = "Just do it."
                  },
                  new Article() {
                    Category = "Faculty",
                    Title = "Exam regulations",
                    Body = "Don't be late"
                  },
                };

                if (!context.Articles.Any())
                {
                    context.Articles.AddRange(articles);
                    context.SaveChanges();
                }

                var students = new List<Student>
                {
                    new Student()
                    {
                        FirstName = "First1",
                        LastName = "Last1"
                    },
                    new Student()
                    {
                        FirstName = "First2",
                        LastName = "Last2"
                    }
                };
             
                if (!context.Students.Any())
                {
                    context.Students.AddRange(students);
                    context.SaveChanges();
                }

                var course_students = new List<Course_Student>
                {
                    new Course_Student()
                    {
                        CourseId = courses[0].Id,
                        StudentId = students[0].Id
                    },
                    new Course_Student()
                    {
                        CourseId = courses[1].Id,
                        StudentId = students[0].Id
                    },
                    new Course_Student()
                    {
                        CourseId = courses[2].Id,
                        StudentId = students[1].Id
                    }
                };
                context.Course_Student.AddRange(course_students);
                context.SaveChanges();
            }
        }
    }
}