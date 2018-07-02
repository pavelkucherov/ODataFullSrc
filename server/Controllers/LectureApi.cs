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
  public class LectureController : ODataController
  {
    private readonly CoursesContext context;

    public LectureController(CoursesContext context) => this.context = context;

    // GET: odata/lecture
    [EnableQuery]
    public IQueryable<Lecture> Get() => context.Lectures.AsQueryable();
  }
}
