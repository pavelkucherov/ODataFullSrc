using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;

namespace courses_odata.Model
{
  public class CoursesModelBuilder
  {
    public IEdmModel GetEdmModel(IServiceProvider serviceProvider)
    {
      var builder = new ODataConventionModelBuilder(serviceProvider);

      builder.EntitySet<Course>(nameof(Course))
                      .EntityType
                      .HasKey(x => x.Id)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand(10) // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command
                      .ContainsMany(x => x.Lectures)
                      .AutomaticallyExpand(false);

      builder.EntitySet<Lecture>(nameof(Lecture))
                      .EntityType
                      .HasKey(x => x.Id)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand(10) // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command
                      .ContainsMany(x => x.TeachingActivities)
                      ; 

    builder.AddEnumType(typeof(TeachingActivityKind));

    builder.EntitySet<TeachingActivity>(nameof(TeachingActivity))
                      .EntityType
                      .HasKey(x => x.Id)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand(10) // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command
                      .ContainsMany(x => x.Answers)
                      .AutomaticallyExpand(false);
            

    builder.EntitySet<Answer>(nameof(Answer))
                      .EntityType
                      .HasKey(x => x.Id)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand(10) // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command
                      ;

    // builder.EntitySet<MultipleChoice>(nameof(MultipleChoice))
    //                   .EntityType
    //                   .DerivesFrom<TeachingActivity>()
    //                   .Filter() // Allow for the $filter Command
    //                   .Count() // Allow for the $count Command
    //                   .Expand() // Allow for the $expand Command
    //                   .OrderBy() // Allow for the $orderby Command
    //                   .Page() // Allow for the $top and $skip Commands
    //                   .Select() // Allow for the $select Command
    //                   .ContainsMany(x => x.Answers)
    //                   .AutomaticallyExpand(false);

    // builder.EntitySet<Slide>(nameof(Slide))
    //                   .EntityType
    //                   .DerivesFrom<TeachingActivity>()
    //                   .Filter() // Allow for the $filter Command
    //                   .Count() // Allow for the $count Command
    //                   .Expand() // Allow for the $expand Command
    //                   .OrderBy() // Allow for the $orderby Command
    //                   .Page() // Allow for the $top and $skip Commands
    //                   .Select(); // Allow for the $select Command

      return builder.GetEdmModel();
    }
  }
}