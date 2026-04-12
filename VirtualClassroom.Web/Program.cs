using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Web.Services.Blob;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection();
builder.Services.AddScoped<EmailService>();

//session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//google authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie("Cookies")
//.AddGoogle("Google", options =>
//{
//    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
//    options.ClientId = googleAuth["ClientId"];
//    options.ClientSecret = googleAuth["ClientSecret"];

//    options.CallbackPath = "/signin-google";
//});

.AddGoogle("Google", options =>
{
    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuth["ClientId"];
    options.ClientSecret = googleAuth["ClientSecret"];

    options.CallbackPath = "/signin-google";

    //  HANDLE CANCEL / FAILURE
    options.Events.OnRemoteFailure = context =>
    {
        context.Response.Redirect("/Account/Login?error=Google login cancelled");
        context.HandleResponse(); // VERY IMPORTANT
        return Task.CompletedTask;
    };
});

//blob storage 
builder.Services.AddScoped<BlobService>();

//builder.Services.AddSingleton<BlobSubmissionService>();
builder.Services.AddScoped<BlobSubmissionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

    await next();
});

app.UseSession();                    //  enable session first
app.UseMiddleware<SessionMiddleware>(); //  then custom middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.Run();
