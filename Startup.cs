using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using OOP_Fair_Fare.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using OOP_Fair_Fare.Services;

namespace OOP_Fair_Fare
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IEmailService, EmailService>();
            
            services.AddAuthentication("Cookies")
                .AddCookie(options =>
                {
                    options.LoginPath = "/log-in";
                    options.AccessDeniedPath = "/log-in";
                    options.ExpireTimeSpan = TimeSpan.FromHours(24);
                });

            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/admin");
            });
            
            services.AddSession();
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
            
            services.AddScoped<IEmailService, EmailService>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<FareCalculator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();            app.UseRouting();
            app.UseSession();
            
            // Add authentication before authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Seed admin user
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
                // Seed vehicles
                OOP_Fair_Fare.Models.VehicleSeeder.SeedVehicles(db);
                // Seed roles (admin and regular)
                OOP_Fair_Fare.Models.RoleSeeder.SeedRoles(db);
                // Seed admin user
                OOP_Fair_Fare.Models.AdminSeeder.SeedAdmin(db);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}