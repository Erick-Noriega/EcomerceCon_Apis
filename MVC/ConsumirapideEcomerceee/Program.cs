using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.Configure<CookiePolicyOptions>(o =>
{
    o.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    o.MinimumSameSitePolicy = SameSiteMode.Strict;
    if (builder.Environment.IsDevelopment())
    {
        o.Secure = CookieSecurePolicy.SameAsRequest;

    }
    else
    {
        o.Secure = CookieSecurePolicy.Always;
    }
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(c =>
{
    c.LoginPath = "/Usuario/Validar";
    c.ExpireTimeSpan = TimeSpan.FromMinutes(15);
});

var apiBaseUrl = builder.Configuration["ApiSettings:baseUrl"];
builder.Services.AddHttpClient("MiApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
