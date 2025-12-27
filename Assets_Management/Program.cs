using AssetManagement_DataAccess;
using AssetManagement_DataAccess.Reports;
using Assets_Management.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Register configuration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//-------------------- Register SQL_DB -----------------------------
builder.Services.AddScoped<SQL_DB>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new SQL_DB(configuration.GetConnectionString("DefaultConnection"));
});

//-------------------- Utility Classes -----------------------------
builder.Services.AddScoped<ExcelHelper>();
builder.Services.AddScoped<FilePathProvider>();

//-------------------- Data Access Classes -------------------------
builder.Services.AddScoped<DashboardReports>();
builder.Services.AddScoped<CallLogs>();
builder.Services.AddScoped<AssetDetailsAndReports>();
builder.Services.AddScoped<ProductionMachine>();
builder.Services.AddScoped<Login>();
builder.Services.AddScoped<MachineReport>();

// Asset Manager related
//builder.Services.AddScoped<AssetQueryBuilder>();
//builder.Services.AddScoped<AssetManager>();            
builder.Services.AddScoped<AssetManagerFactory>();

//-------------------- API Connection -----------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApiConnect>();

//-------------------- Controllers -----------------------------
builder.Services.AddControllersWithViews();

var app = builder.Build();

//-------------------- Middleware -----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

//-------------------- Routes -----------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
