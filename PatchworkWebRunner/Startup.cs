using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PatchworkWebRunner.Services;
using System;
using System.IO;
using System.Reflection;

namespace PatchworkWebRunner;

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

			//c.DescribeAllEnumsAsStrings();
			c.IncludeXmlComments(commentsFile);
			//c.SwaggerDoc("v1", new Info { Title = "Patchwork API", Version = "v1" });
		});

		services.AddMvc();
	}

	/// <summary>
	/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	/// </summary>
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseSwagger();
		app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}
