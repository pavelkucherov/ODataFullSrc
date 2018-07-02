using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using courses_odata.Model;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace courses_odata
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<CoursesContext>(options =>
      {
        options.UseSqlite("Data Source=./Courses.db");
      });

      services.AddMvc().AddJsonOptions(opt =>
      {
        opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      });

      services.AddOData();
      services.AddTransient<CoursesModelBuilder>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, CoursesModelBuilder modelBuilder)
    {
      app.UseStaticFiles();

      if (env.IsDevelopment())
      {
        //app.UseDeveloperExceptionPage();

      }

      app.UseMvc(routeBuilder =>
      {
        routeBuilder.MapODataServiceRoute("ODataRoutes", "odata", modelBuilder.GetEdmModel(app.ApplicationServices));
      });
    }
  }
}
