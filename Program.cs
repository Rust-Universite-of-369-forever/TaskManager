using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Filters;
using TaskManager.Helpers;
using TaskManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromHours(8); });
builder.Services.AddScoped<AuthorizationFilter>();
builder.Services.AddScoped<ExecutionTimeFilter>();
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<PasswordService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseCookiePolicy();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
