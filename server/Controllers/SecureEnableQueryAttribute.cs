using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using courses_odata.Model;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.AspNetCore.Mvc.Filters;
using server.Controllers;
using System.Reflection;

namespace courses_odata.Controllers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SecureEnableQueryAttribute : EnableQueryAttribute
    {
        // will fail on ?$expand=*
        //public List<Type> RestrictedTypes => new List<Type>() { typeof(Answer) };

        /// <summary>
        /// Validates the OData query in the incoming request. By default, the implementation throws an exception if
        /// the query contains unsupported query parameters. Override this method to perform additional validation of
        /// the query.
        /// Overriden method throws InvalidOperationException if query contains restricted types
        /// Query will fail on ?$expand=*
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <param name="queryOptions">
        /// The <see cref="ODataQueryOptions"/> instance constructed based on the incoming request.
        /// </param>
        public override void ValidateQuery(HttpRequest request, ODataQueryOptions queryOptions)
        {
           
            /* Example for type restriction - column level security
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
                throw new InvalidOperationException("The query contains restricted type");
            }
            */
            base.ValidateQuery(request, queryOptions);
        }

        /// <summary>
        /// Gets the EDM model for the given type and request.
        /// Override this method to customize the EDM model used for
        /// querying.
        /// </summary>
        /// <param name = "elementClrType" > The CLR type to retrieve a model for.</param>
        /// <param name = "request" > The request message to retrieve a model for.</param>
        /// <param name = "actionDescriptor" > The action descriptor for the action being queried on.</param>
        /// <returns>The EDM model for the given type and request.</returns>
        public override IEdmModel GetModel(Type elementClrType, HttpRequest request, ActionDescriptor actionDescriptor)
        {
            return base.GetModel(elementClrType, request, actionDescriptor);
        }

        /// <summary>
        /// Applies the query to the given IQueryable based on incoming query from uri and query settings. By default,
        /// the implementation supports $top, $skip, $orderby and $filter. Override this method to perform additional
        /// query composition of the query.
        /// 
        /// Here we can intercept the IQueryable result AFTER the controller has processed the request and created the intial query.
        /// </summary>
        /// <param name="queryable">The original queryable instance from the response message.(controller)</param>
        /// <param name="queryOptions">
        /// The <see cref="ODataQueryOptions"/> instance constructed based on the incoming request.
        /// </param>
        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            //var test_queryable = ((IQueryable<Course>)queryable).Where(x => x.Course_Students.Where(cs => cs.StudentId.Equals(1)).Any());
            // if we want to use session and cookies - they are accessible from HttpRequest queryOptions.Request
            string strUserId = queryOptions.Request.Query["userId"].FirstOrDefault();
            if (strUserId != null && Int32.TryParse(strUserId, out int userId))
            {
                foreach (var prop in queryable.ElementType.GetProperties())
                {
                    CustomAttributeData rlsAttribute = prop.CustomAttributes.Where(x => x.AttributeType == typeof(RowLevelSecurityAttribute)).FirstOrDefault();
                    if (rlsAttribute != null)
                    {
                        string keyName = rlsAttribute.NamedArguments.FirstOrDefault(x => x.MemberName == nameof(RowLevelSecurityAttribute.KeyName)).TypedValue.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(keyName))
                        {
                            queryable = makeRowFilterExpression(queryable, prop, keyName, userId);
                        }
                    }
                }
            }
            return base.ApplyQuery(queryable, queryOptions);
        }

        private IQueryable makeRowFilterExpression(IQueryable queryable, PropertyInfo prop, string keyName, int userId)
        {
            try
            {
                // cs => cs.[KeyName] == userId
                Type elementType = prop.PropertyType.GetGenericArguments().First();
                ParameterExpression internalParameter = Expression.Parameter(elementType, "cs");
                Expression target = Expression.Constant(userId);
                Expression whereKey = Expression.PropertyOrField(internalParameter, keyName);
                Expression compareMethod = Expression.Equal(whereKey, target);
                LambdaExpression internalWhereLambda = Expression.Lambda(compareMethod, internalParameter);

                //x => x.Course_Students.Where([internalWhereLambda]).Any()
                ParameterExpression parameter = Expression.Parameter(queryable.ElementType, "x");
                Expression property = Expression.Property(parameter, prop.Name);

                //public static IEnumerable<TSource> Enumerable.Where<TSource>(this IEnumerable<TSource> source, Expression<Func<TSource, bool>> predicate);
                //{x.Course_Students.Where(cs => cs.StudentId.Equals(1))}
                MethodCallExpression whereCallExpression = Expression.Call(
                    typeof(Enumerable), "Where", new Type[] { elementType }, property, internalWhereLambda);

                //public static bool Enumerable.Any<TSource>(this IEnumerable<TSource> source);
                //{x.Course_Students.Where(cs => cs.StudentId.Equals(1)).Any()}
                MethodCallExpression anyCallExpression = Expression.Call(
                    typeof(Enumerable), "Any", new Type[] { elementType }, whereCallExpression);

                //x => x.Course_Students.Where([internalWhereLambda]).Any()
                LambdaExpression anyLambda = Expression.Lambda(anyCallExpression, parameter);
                //{x => x.Course_Students.Where(cs => cs.StudentId.Equals(1)).Any()}

                //.Where([anyLambda]);
                // static IQueryable<TSource> Queryable.Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
                MethodCallExpression queryableWhereCallExpression = Expression.Call(
                    typeof(Queryable), "Where", new Type[] { queryable.ElementType }, queryable.Expression, anyLambda);
                // {value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[courses_odata.Model.Course]).Where(x => x.Course_Students.Where(cs => cs.StudentId.Equals(1)).Any())}

                queryable = queryable.Provider.CreateQuery(queryableWhereCallExpression);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"makeRowFilterExpression exception {ex.Message}");
            }
            return queryable;
        }

        /// <summary>
        /// Applies the query to the given entity based on incoming query from uri and query settings.
        /// Works if response is a single entity.
        /// </summary>
        /// <param name="entity">The original entity from the response message.</param>
        /// <param name="queryOptions">
        /// The <see cref="ODataQueryOptions"/> instance constructed based on the incoming request.
        /// </param>
        /// <returns>The new entity after the $select and $expand query has been applied to.</returns>
        public override object ApplyQuery(object entity, ODataQueryOptions queryOptions)
        {
            return base.ApplyQuery(entity, queryOptions);
        }

        /// <summary>
        /// Performs the query composition after action is executed. It first tries to retrieve the IQueryable from the
        /// returning response message. It then validates the query from uri based on the validation settings on
        /// <see cref="EnableQueryAttribute"/>. It finally applies the query appropriately, and reset it back on
        /// the response message.
        /// </summary>
        /// <param name="actionExecutedContext">The context related to this action, including the response message,
        /// request message and HttpConfiguration etc.</param>
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
