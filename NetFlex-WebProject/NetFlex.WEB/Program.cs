using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetFlex.DAL.EF;
using NetFlex.BLL.Interfaces;
using NetFlex.BLL.Services;
using NetFlex.DAL.Interfaces;
using NetFlex.DAL.Repositories;
using NetFlex.DAL.Constants;
using Microsoft.AspNetCore.Mvc;
using GraphQL.AspNet.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(
                    builder.Configuration["ConnectionStrings:DefaultConnection"]));

builder.Services.AddGraphQL();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;    // ���������� email
    options.SignIn.RequireConfirmedAccount = true;

})
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddCors();

builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IRatingService, RatingService>();


builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Contact");
    options.Conventions.AuthorizeFolder("/Private");
    options.Conventions.AllowAnonymousToPage("/Private/PublicPage");
    options.Conventions.AllowAnonymousToFolder("/Private/PublicPages");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/Index";

});

builder.Services.AddAuthentication()
.AddVkontakte(options =>
{
    options.ClientId = builder.Configuration["VKontakte:ClientId"];
    options.ClientSecret = builder.Configuration["VKontakte:ClientSecret"];
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.Policies.RequireAdmin, policy => policy.RequireRole(Constants.Roles.Administrator));
    options.AddPolicy(Constants.Policies.RequireManager, policy => policy.RequireRole(Constants.Roles.Manager));
    //options.AddPolicy("Subscription", policy => policy.AddRequirements());
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseGraphQL();
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Main";
        await next();
    }
});

app.UseCors(builder =>
    builder.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod()
  );

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//using (var serviceScope = app.Services.CreateScope())
//{
//    var services = serviceScope.ServiceProvider;
//    var context = services.GetRequiredService<DatabaseContext>();
//    context.Database.Migrate();
//}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

