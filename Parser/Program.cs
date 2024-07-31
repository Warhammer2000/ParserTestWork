using Microsoft.EntityFrameworkCore;
using Parser.Db;
using Parser.Services;

namespace Parser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddMemoryCache();

            builder.Services.AddControllersWithViews();


            builder.Services.AddTransient<DataParser>();
            builder.Services.AddScoped<DataService>();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();


            app.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
