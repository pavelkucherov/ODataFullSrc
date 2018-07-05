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
//   public class MultipleChoiceController : ODataController
//   {
//     private readonly CoursesContext context;

//     public MultipleChoiceController(CoursesContext context) => this.context = context;

//     // GET: odata/MultipleChoice
//     [EnableQuery(MaxExpansionDepth = 6, PageSize = 50)]
//     public IQueryable<MultipleChoice> Get() => context.MultipleChoices.AsQueryable();
//   }
// }
