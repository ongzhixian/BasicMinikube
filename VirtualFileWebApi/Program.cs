using Microsoft.EntityFrameworkCore;

using VirtualFileWebApi.DbContexts;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpLogging(o => { });

// Add services to the container.

ConfigureDbContexts(builder);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void ConfigureDbContexts(WebApplicationBuilder builder)
{
    var eimaDbConnectionString = builder.Configuration.GetConnectionString("EIMA");

    builder.Services.AddDbContext<EimaContext>(dbContextOption =>
    {
        dbContextOption.UseSqlServer(eimaDbConnectionString, sqlServerOption =>
        {
            sqlServerOption.CommandTimeout(60);

        });

    }, ServiceLifetime.Scoped);

    //builder.Services.AddDbContextFactory
    //builder.Services.AddDbContextPool
}