using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// MongoDB
var connectionString = builder.Configuration.GetSection("CinepapoDatabase:ConnectionString").Value;
var databaseName = builder.Configuration.GetSection("CinepapoDatabase:DatabaseName").Value;
var mongoClient = new MongoClient(connectionString);
builder.Services.AddSingleton<IMongoDatabase>(sp => mongoClient.GetDatabase(databaseName));

// Autenticação
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddHttpClient<CinePapo.Services.TmdbService>();

builder.Services.AddControllersWithViews();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Essencial
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();