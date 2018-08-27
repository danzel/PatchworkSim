using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatchworkWebRunner.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace PatchworkWebRunner
{
	/// <summary>
	/// ASPNetCore Startup
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		internal IConfiguration Configuration { get; }

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<PatchworkService>();

			services.AddSwaggerGen(c =>
			{
				var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
				var commentsFile = Path.Combine(baseDirectory, commentsFileName);

				c.DescribeAllEnumsAsStrings();
				c.IncludeXmlComments(commentsFile);
				c.SwaggerDoc("v1", new Info { Title = "Patchwork API", Version = "v1" });
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// </summary>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

			app.UseMvc();
		}
	}
}
