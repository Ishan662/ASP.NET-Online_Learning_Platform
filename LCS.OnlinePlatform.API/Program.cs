using LCS.OnlinePlatform.Data;
using LCS.OnlinePlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using LCS.OnlinePlatform.Service;


namespace LCS.OnlinePlatform.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.Services.AddDbContextPool<OnlinePlatformDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DbContext"),
                    providerOptions => providerOptions.EnableRetryOnFailure());

                options.EnableSensitiveDataLogging();
            });



            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
            builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();


            var app = builder.Build();

            // Ensure the database is created
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<OnlinePlatformDbContext>();
                db.Database.EnsureCreated(); // <-- This will create the database if it doesn't exist
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
