using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Serilog;
using Serilog.Events;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    // Add this line:
    .WriteTo.File(
       Path.Combine("Logs", "LogFiles", "Application", "diagnostics.txt"),
       rollingInterval: RollingInterval.Day,
       fileSizeLimitBytes: 10 * 1024 * 1024,
       retainedFileCountLimit: 2,
       rollOnFileSizeLimit: true,
       shared: true,
       flushToDiskInterval: TimeSpan.FromSeconds(1))
    .CreateLogger();
try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    //    .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Default Password settings.
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();
    // Force Identity's security stamp to be validated every minute.
    builder.Services.Configure<SecurityStampValidatorOptions>(o =>
                       o.ValidationInterval = TimeSpan.FromMinutes(1));
    builder.Services.AddMvcCore();
    builder.Services.AddControllersWithViews(options =>
    {
        options.RespectBrowserAcceptHeader = true;
    }).AddNewtonsoftJson();
    builder.Services.AddAuthorizationCore(options =>
    {
        options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    });
    builder.Host.ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    });
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    // Add services to the container.
    builder.Services.AddCors(options =>
    {
        var corsClients = builder.Configuration.GetSection("Clients").Get<string[]>();
        options.AddPolicy(name: MyAllowSpecificOrigins,
                          builder =>
                          {
                              builder.WithOrigins(corsClients).AllowAnyHeader().AllowAnyMethod();
                          });
    });
    builder.Services.AddScoped<IEmailNotification, EmailNotification>();
    builder.Services.AddScoped<ICashFreeClient, CashFreeClientService>();
    builder.Services.AddScoped<IGoogleCalendar, GoogleCalendar>();
    builder.Services.AddScoped<ControllerActionFilter>();
    builder.Services.AddHttpClient();
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.Cookie.Name = "UserSession";
                        options.LoginPath = "/Identity/Account/Login";
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                        options.SlidingExpiration = true;
                    });
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    var cookiePolicyOptions = new CookiePolicyOptions
    {
        MinimumSameSitePolicy = SameSiteMode.Lax,
        HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None,
        Secure = CookieSecurePolicy.None,
    };
    app.UseCookiePolicy(cookiePolicyOptions);

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCors(MyAllowSpecificOrigins);
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
          name: "Admin",
          pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
        );
    });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();
    RotativaConfiguration.Setup(app.Environment.WebRootPath);
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await DbContextSeed.SeedRolesAsync(userManager, roleManager);
            await DbContextSeed.SeedSuperAdminAsync(userManager, roleManager);
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}