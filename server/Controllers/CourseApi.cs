using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using courses_odata.Model;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace courses_odata.Controllers
{
  [Produces("application/json")]
  public class CourseController : ODataController
  {
    private readonly CoursesContext context;

    public CourseController(CoursesContext context) => this.context = context;

    // GET: odata/course
    [EnableQuery]
    public IQueryable<Course> Get() => this.context.Courses.AsQueryable();
  }
}
