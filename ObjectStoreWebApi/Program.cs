using Microsoft.EntityFrameworkCore;

using ObjectStoreWebApi.DbContexts;
using ObjectStoreWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

SetupStorageService(builder.Configuration, builder.Services);

// Add services to the container.
builder.Services.AddScoped<IObjectStoreService, LocalDiskObjectStoreService>();

builder.Services.AddControllers();
    //.AddXmlDataContractSerializerFormatters();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddOpenApi("internal"); // Document name is internal

var app = builder.Build();

InitializeStorageService(app.Configuration, app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // http://localhost:5192/openapi/v1.json
}

SetHttpsRedirection(builder.Configuration);

app.UseAuthorization();

app.MapControllers();

app.Run();

return;



void SetupStorageService(ConfigurationManager configuration, IServiceCollection services)
{
    var storageMedium = (configuration["StorageService:StorageMedium"] ?? string.Empty).ToUpperInvariant();

    switch (storageMedium)
    {
        case "MINIO":
        case "S3":
        case "AZURE":
        case "REDIS":
        case "SQLITE_FS":
            // Not supported (YET!)
            break;

        case "LOCAL_DISK":
        default:
            ConfigureLocalDiskStorageService(configuration, services);
            break;
    }

}

void InitializeStorageService(IConfiguration configuration, IServiceProvider services)
{
    var storageMedium = (configuration["StorageService:StorageMedium"] ?? string.Empty).ToUpperInvariant();

    switch (storageMedium)
    {
        case "MINIO":
        case "S3":
        case "AZURE":
        case "REDIS":
        case "SQLITE_FS":
            // Not supported (YET!)
            break;
        case "LOCAL_DISK":
        default:
            {
                using var scope = services.CreateScope();
                using var objectStorageContext = scope.ServiceProvider.GetRequiredService<ObjectStorageContext>();
                objectStorageContext.Database.EnsureCreated();
            }
            break;
    }
}

void ConfigureLocalDiskStorageService(ConfigurationManager configuration, IServiceCollection services)
{
    var storageLocationRootPath = configuration["StorageService:StorageLocationRoot"]
        ?? Path.Combine(AppContext.BaseDirectory, "objectStore");

    if (!Directory.Exists(storageLocationRootPath))
    {
        Console.WriteLine($"Creating {storageLocationRootPath}");
        Directory.CreateDirectory(storageLocationRootPath);
    }

    var indexFileName = configuration["StorageService:StorageIndexFileName"] ?? "object-store.sqlite3";
    
    var storageIndexDbFullPath = Path.Combine(storageLocationRootPath, indexFileName);

    var objectStoreIndexConnectionString = $"Data Source={storageIndexDbFullPath};Cache=Shared;";

    services.AddDbContext<ObjectStorageContext>(options =>
    {
        options.UseSqlite(objectStoreIndexConnectionString, sqliteOptions =>
        {
            sqliteOptions.CommandTimeout(30);
        });
    });
}

void SetHttpsRedirection(ConfigurationManager configuration)
{
    if (bool.TryParse(configuration["FeatureFlags:UseHttpsRedirection"], out bool useHttpsRedirection) && useHttpsRedirection)
    {
        Console.WriteLine("UseHttpsRedirection enabled");
        app.UseHttpsRedirection();
    }
}

// For exposing Program for integration testing
public partial class Program { }