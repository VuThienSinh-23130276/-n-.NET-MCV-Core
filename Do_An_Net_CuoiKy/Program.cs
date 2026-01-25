// TẠM THỜI COMMENT ĐỂ TEST FRONTEND KHÔNG CẦN DATABASE
// using Do_An_Net_CuoiKy.Data;
// using Microsoft.EntityFrameworkCore;

using Do_An_Net_CuoiKy.Data;
using Do_An_Net_CuoiKy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options => { 
options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});
//Service Identity
builder.Services.AddIdentity<AppUserModel, IdentityRole>(/*options => options.SignIn.RequireConfirmedAccount = true*/)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings. Ưu cầu password
    //số
    options.Password.RequireDigit = true;
    // all chữ thường
    options.Password.RequireLowercase = false;
    // các ký tự đặt biệt
    options.Password.RequireNonAlphanumeric = false;
    // Ư cầu chữ số hoa
    options.Password.RequireUppercase = false;
    // chiều dài passwod
    options.Password.RequiredLength = 4;
    // ưu cầu kí tự đặt biệt
    //options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    //// khóa acc sau 5p
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //// có gắng truy cập không quá 5 lần
    //options.Lockout.MaxFailedAccessAttempts = 5;
    //// cho phép tạo user mới
    //options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});
// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

app.UseSession();

app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();




using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserModel>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    var adminEmail = "admin@gmail.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        var newAdmin = new AppUserModel
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Administrator"
        };

        await userManager.CreateAsync(newAdmin, "1234");
        await userManager.AddToRoleAsync(newAdmin, "Admin");
    }
}
app.Run();
