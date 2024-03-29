using Autobarn.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Autobarn.Website; 

public class Startup {
	protected virtual string DatabaseMode => Configuration["DatabaseMode"];

	public Startup(IConfiguration configuration) {
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services) {
		services.AddRouting(options => options.LowercaseUrls = true);
		services.AddControllersWithViews().AddNewtonsoftJson();
		services.AddRazorPages().AddRazorRuntimeCompilation();

		services.AddControllersWithViews().AddNewtonsoftJson();
		services.AddSwaggerGen(options => {
			options.SwaggerDoc("v1", new OpenApiInfo {
				Version = "v1",
				Title = "Autobarn API",
				Description = "The Autobarn vehicle platform API"
			});
		});
		services.AddSwaggerGenNewtonsoftSupport();
		services.AddRazorPages().AddRazorRuntimeCompilation();

		Console.WriteLine(DatabaseMode);
		switch (DatabaseMode) {
			case "sql":
				var sqlConnectionString = Configuration.GetConnectionString("AutobarnSqlConnectionString");
				services.UseAutobarnSqlDatabase(sqlConnectionString);
				break;
			default:
				services.AddSingleton<IAutobarnDatabase, AutobarnCsvFileDatabase>();
				break;
		}
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		if (env.IsDevelopment()) {
			app.UseDeveloperExceptionPage();
		} else {
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}
		app.UseHttpsRedirection();
		app.UseDefaultFiles();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseAuthorization();

		app.UseSwagger();
		app.UseSwaggerUI();

		app.UseEndpoints(endpoints => {
			endpoints.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
		});
	}
}
