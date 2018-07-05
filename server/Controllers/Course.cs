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

    [EnableQuery(MaxExpansionDepth = 6, PageSize = 50)]
    public SingleResult<Course> Get([FromODataUri] int key)
    {
        return SingleResult.Create(context.Courses.Where(c => c.Id == key));
    }

    //sample calls: /OData/Course?userid=1
    // /OData/Course?userid=1&$filter=Id+eq+2
    // /OData/Course/$count?userid=1
    [SecureEnableQuery(MaxExpansionDepth = 6, PageSize = 50)]
    public IQueryable<Course> Get() => this.context.Courses.AsQueryable();
  }
}
