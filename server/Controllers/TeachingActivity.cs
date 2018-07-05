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
  public class TeachingActivityController : ODataController
  {
    private readonly CoursesContext context;

    public TeachingActivityController(CoursesContext context) => this.context = context;


    [EnableQuery(MaxExpansionDepth = 6, PageSize = 50)]
    public SingleResult<TeachingActivity> Get([FromODataUri] int key)
    {
        return SingleResult.Create(context.TeachingActivities.Where(c => c.Id == key));
    }

    [SecureEnableQuery(MaxExpansionDepth = 6, PageSize = 50)]
    public IQueryable<TeachingActivity> Get() => context.TeachingActivities.AsQueryable();
  }
}
