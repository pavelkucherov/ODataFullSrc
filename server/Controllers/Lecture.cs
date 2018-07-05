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

    [EnableQuery(MaxExpansionDepth = 6, PageSize = 50)]
    public SingleResult<Lecture> Get([FromODataUri] int key)
    {
        return SingleResult.Create(context.Lectures.Where(c => c.Id == key));
    }

    [SecureEnableQuery(MaxExpansionDepth = 6, PageSize = 50)]
    public IQueryable<Lecture> Get() => context.Lectures.AsQueryable();
  }
}
