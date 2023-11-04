using App.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var conectionstring = builder.Configuration.GetConnectionString("AppContext") ?? throw new InvalidOperationException("Connection string 'mooreContext' not found.");
builder.Services.AddDbContext<appContext>(options =>
    options.UseMySql(conectionstring, ServerVersion.AutoDetect(conectionstring)));
builder.Services.AddScoped<appContextSeedData>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("LocalLogin")
    .AddCookie("LocalLogin", options =>
    {
        options.LoginPath = "/Pages/Account/login"; // Updated line
    });



builder.Services.AddTransient<CsvFileviewModel>();
builder.Services.AddTransient<appContextSeedData>();
builder.Services.AddSession();
builder.Services.AddRazorPages();


var app = builder.Build();
app.UseSession();
SeedDatabase();
void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
        try
        {
            var scopedContext = scope.ServiceProvider.GetRequiredService<appContext>();
            appContextSeedData.Seed(scopedContext);
        }
        catch
        {
            throw;
        }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();



app.Run();
