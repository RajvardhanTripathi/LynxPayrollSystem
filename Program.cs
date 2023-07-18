using Lib.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using System.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();


        builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddScoped<SalaryAddonMasRepository, SalaryAddonMasRepository>();
      
        builder.Services.AddScoped<DesignationMasRepository, DesignationMasRepository>();
        builder.Services.AddScoped<HolidayMasRepository, HolidayMasRepository>();



        builder.Services.AddScoped<EmployeeMasRepository, EmployeeMasRepository>();
        builder.Services.AddScoped<SalaryAssignRepository, SalaryAssignRepository>();
		builder.Services.AddSession();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "YourCookieName";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.HttpOnly = true;
            options.LoginPath = "/Login/Index";
            options.LogoutPath = "/Login/Logout";
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
      

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Index}/{id?}");

        app.MapControllers();
		app.UseSession();
		app.Run();
    }
}