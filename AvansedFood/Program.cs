using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using AvansedFood.Web.Data;
using Domain.Repositories;
using Infrastructure.Repositories;
using Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AvansedFood API",
        Version = "v1",
        Description = "REST API for AvansedFood meal package reservations",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Avans Hogeschool",
            Email = "info@avans.nl"
        }
    });
});

builder.Services.AddDbContext<AvansedFood.Web.Data.IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AvansedFood.Web.Data.IdentityDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DomainConnection")));

// Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<ICanteenRepository, CanteenRepository>();

// Services
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICanteenService, CanteenService>();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<AvansedFood.Web.GraphQL.Queries.PackageQueries>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();     

var app = builder.Build();

// Seed Identity users
await SeedData(app);

// Seed Domain data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.SeedDatabase(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Enable Swagger in Development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AvansedFood API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL("/graphql");

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

async Task SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Student"))
    {
        await roleManager.CreateAsync(new IdentityRole("Student"));
    }

    if (!await roleManager.RoleExistsAsync("CanteenEmployee"))
    {
        await roleManager.CreateAsync(new IdentityRole("CanteenEmployee"));
    }

    var studentEmail = "student@avans.nl";
    if (await userManager.FindByEmailAsync(studentEmail) == null)
    {
        var student = new IdentityUser
        {
            UserName = studentEmail,
            Email = studentEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(student, "Avans123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(student, "Student");
        }
    }

    var markEmail = "mark.bakker@avans.nl";
    if (await userManager.FindByEmailAsync(markEmail) == null)
    {
        var mark = new IdentityUser
        {
            UserName = markEmail,
            Email = markEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(mark, "Avans123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(mark, "Student");
        }
    }

    var employeeEmail = "employee@avans.nl";
    if (await userManager.FindByEmailAsync(employeeEmail) == null)
    {
        var employee = new IdentityUser
        {
            UserName = employeeEmail,
            Email = employeeEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(employee, "Avans123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(employee, "CanteenEmployee");
        }
    }
}