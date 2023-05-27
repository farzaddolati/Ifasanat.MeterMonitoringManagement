using Ifasanat.MeterMonitoringManagement.Data;
using Ifasanat.MeterMonitoringManagement.Repositories;
using Ifasanat.MeterMonitoringManagement.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using AutoMapper;

namespace Ifasanat.MeterMonitoringManagement
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyApiDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyDatabaseConnection")));

            //services.AddControllers();

            services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });




            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .WithExposedHeaders("Access-Control-Allow-Origin");
                    });
            });


            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICityRepository, CityRepository>();

            services.AddAutoMapper(typeof(Program).Assembly);

            services.AddControllers().AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<CustomerValidator>();
                options.RegisterValidatorsFromAssemblyContaining<CityValidator>();
            });
            services.AddSwaggerGen(options =>
            {
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAllOrigins");

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
