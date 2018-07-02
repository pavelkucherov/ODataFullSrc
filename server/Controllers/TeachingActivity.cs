using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using courses_odata.Model;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.AspNetCore.Mvc.Filters;

namespace courses_odata.Controllers
{
    [Produces("application/json")]
    public class TeachingActivityController : ODataController
    {
        private readonly CoursesContext context;

        public TeachingActivityController(CoursesContext context) => this.context = context;

        // GET: odata/teachingActivity
        [SecureEnableQuery]
        //[EnableQuery]
        public IQueryable<TeachingActivity> Get()
        {
            //var list = context.TeachingActivities.Include(x => ((MultipleChoice)x).Answers).ToList();

           // var list = context.TeachingActivities.ToList();
            //var qu = context.TeachingActivities.Include(x => ((MultipleChoice)x).Answers).AsQueryable();
            //foreach(var b in qu)
            //{
            //    Console.WriteLine(b.ToString());
            //}

            //return context.TeachingActivities.Include(x => x is MultipleChoice ? ((MultipleChoice)x).Answers : null).AsQueryable();
            return context.TeachingActivities.Include(x => x.Answers).AsQueryable();
            //return context.TeachingActivities.Include(x => ((MultipleChoice)x).Answers).AsQueryable();
        }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SecureEnableQueryAttribute : EnableQueryAttribute
    {
        // will fail on ?$expand=*
        public List<Type> RestrictedTypes => new List<Type>() { typeof(Answer) };

        public override void ValidateQuery(HttpRequest request, ODataQueryOptions queryOptions)
        {
            List<IEdmType> expandedTypes = queryOptions.GetAllExpandedEdmTypes();

            List<string> expandedTypeNames = new List<string>();
            //For single navigation properties
            expandedTypeNames.AddRange(expandedTypes.OfType<EdmEntityType>().Select(entityType => entityType.FullTypeName()));
            //For collection navigation properties
            expandedTypeNames.AddRange(expandedTypes.OfType<EdmCollectionType>().Select(collectionType => collectionType.ElementType.Definition.FullTypeName()));

            //Simply a blanket "If it exists" statement. Feel free to be as granular as you like with how you restrict the types. 
            bool restrictedTypeExists = RestrictedTypes.Select(rt => rt.FullName).Any(rtName => expandedTypeNames.Contains(rtName));

            if (restrictedTypeExists)
            {
                throw new InvalidOperationException();
            }

            base.ValidateQuery(request, queryOptions);
        }
        public override IEdmModel GetModel(Type elementClrType, HttpRequest request, ActionDescriptor actionDescriptor)
        {
            return base.GetModel(elementClrType, request, actionDescriptor);
        }
        /*
        public override IEdmModel GetModel(Type elementClrType, HttpRequest request, ActionDescriptor actionDescriptor)
        {
            var modelRequest = request.GetModel();

            var baseModel = base.GetModel(elementClrType, request, actionDescriptor);

            EdmModel m = baseModel as EdmModel;
            var el = m.EntityContainer.Elements;
            
            var securityModel =  GetSecurityModel(request.HttpContext.RequestServices);

            return securityModel;
            //return baseModel;
        }
        */
        public IEdmModel GetSecurityModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            /*
            builder.EntitySet<Course>(nameof(Course))
                            .EntityType
                            .Filter() // Allow for the $filter Command
                            .Count() // Allow for the $count Command
                            .Expand() // Allow for the $expand Command
                            .OrderBy() // Allow for the $orderby Command
                            .Page() // Allow for the $top and $skip Commands
                            .Select() // Allow for the $select Command
                            .ContainsMany(x => x.Lectures);

            builder.EntitySet<Lecture>(nameof(Lecture))
                            .EntityType
                            .Filter() // Allow for the $filter Command
                            .Count() // Allow for the $count Command
                            .Expand() // Allow for the $expand Command
                            .OrderBy() // Allow for the $orderby Command
                            .Page() // Allow for the $top and $skip Commands
                            .Select() // Allow for the $select Command
                            .ContainsMany(x => x.TeachingActivities);

    */

            builder.EntitySet<Answer>(nameof(Answer))
                      .EntityType
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand() // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select(); // Allow for the $select Command
                                 // .ContainsRequired(x => x.TeachingActivity);


            builder.EntitySet<TeachingActivity>(nameof(TeachingActivity))
                      .EntityType
                      .Abstract()
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand() // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command
                      .ContainsMany(x => x.Answers)
                      .AutomaticallyExpand(false);

            builder.EntitySet<MultipleChoice>(nameof(MultipleChoice))
                      .EntityType
                      .DerivesFrom<TeachingActivity>()
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand() // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select(); // Allow for the $select Command

            builder.EntitySet<Slide>(nameof(Slide))
                              .EntityType
                              .DerivesFrom<TeachingActivity>()
                              .Filter() // Allow for the $filter Command
                              .Count() // Allow for the $count Command
                              .Expand() // Allow for the $expand Command
                              .OrderBy() // Allow for the $orderby Command
                              .Page() // Allow for the $top and $skip Commands
                              .Select(); // Allow for the $select Command


            //   builder.EntitySet<TeachingActivity>(nameof(TeachingActivity))
            //           .EntityType
            //           .Ignore(x => x.Answers);

            return builder.GetEdmModel();
        }
        
        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            return base.ApplyQuery(queryable, queryOptions);
        }

        public override object ApplyQuery(object entity, ODataQueryOptions queryOptions)
        {
            return base.ApplyQuery(entity, queryOptions);
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            return base.OnActionExecutionAsync(context, next);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);
        }

        public override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            return base.OnResultExecutionAsync(context, next);
        }
    }

    public static class ODataQueryOptionsExtensions
    {
        public static List<IEdmType> GetAllExpandedEdmTypes(this ODataQueryOptions self)
        {
            //Fill a list and send it out.
            List<IEdmType> types = new List<IEdmType>();
            fillTypes(self.SelectExpand?.SelectExpandClause, types);
            return types;
        }

        private static void fillTypes(SelectExpandClause selectExpandClause, List<IEdmType> typeList)
        {
            if (selectExpandClause != null)
            {
                foreach (var selectedItem in selectExpandClause.SelectedItems)
                {
                    //We're only looking for the expanded navigation items, as we are restricting authorization based on the entity as a whole, not it's parts. 
                    var expandItem = (selectedItem as ExpandedNavigationSelectItem);
                    if (expandItem != null)
                    {
                        //https://msdn.microsoft.com/en-us/library/microsoft.data.odata.query.semanticast.expandednavigationselectitem.pathtonavigationproperty(v=vs.113).aspx
                        //The documentation states: "Gets the Path for this expand level. This path includes zero or more type segments followed by exactly one Navigation Property."
                        //Assuming the documentation is correct, we can assume there will always be one NavigationPropertySegment at the end that we can use. 
                        typeList.Add(expandItem.PathToNavigationProperty.OfType<NavigationPropertySegment>().Last().EdmType);

                        //Fill child expansions. If it's null, it will be skipped.
                        fillTypes(expandItem.SelectAndExpand, typeList);
                    }
                }
            }
        }
    }
}
