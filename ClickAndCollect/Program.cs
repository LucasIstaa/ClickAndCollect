using ClickAndCollect.Models.DAL;
using ClickAndCollect.Models.DALClasses;
using ClickAndCollect.Models.DALInterfaces;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("default");
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IStoreDAL>(storeDAL => new StoreDAL(connectionString));
builder.Services.AddTransient<IProductDAL>(productDAL => new ProductDAL(connectionString));
builder.Services.AddTransient<ICategoryDAL>(categoryDAL => new CategoryDAL(connectionString));
builder.Services.AddTransient<IUserDAL>(userDAL => new UserDAL(connectionString));

builder.Services.AddSession();


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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "other",
    pattern: "{controller=Product}/{action=GetAllProducts}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
