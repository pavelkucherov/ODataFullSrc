// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using courses_odata.Model;
// using Microsoft.AspNet.OData;
// using Microsoft.AspNetCore.Mvc;

// namespace courses_odata.Controllers
// {
//   [Produces("application/json")]
//   public class SlideController : ODataController
//   {
//     private readonly CoursesContext context;

//     public SlideController(CoursesContext context) => this.context = context;

//     // GET: odata/Slide
//     [EnableQuery(MaxExpansionDepth = 6, PageSize = 50)]
//     public IQueryable<Slide> Get() => context.Slides.AsQueryable();
//   }
// }
