using System.Text.Json.Serialization;
using Claims.Infrastructure.AuditContext;
using Claims.Infrastructure.CosmosDb;
using Claims.Infrastructure.Settings;
using Claims.Services.Claims;
using Claims.Services.Covers;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }
        );

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("LocalhostPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        builder.Services.AddSingleton(GetCosmosClient(builder.Configuration.GetSection("CosmosDb")));
        builder.Services.AddSingleton<CosmosDbSettings>(builder.Configuration.GetSection("CosmosDb").Get<CosmosDbSettings>());

        builder.Services.AddTransient<ICosmosDbService, CosmosDbService>();
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateClaim>());

        builder.Services.AddValidatorsFromAssemblyContaining<CreateClaim.Validator>();
        builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            // https://github.com/swagger-api/swagger-ui/issues/7911
            c.CustomSchemaIds(s => s.FullName.Replace("+", "."));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.UseCors("LocalhostPolicy");

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
            context.Database.Migrate();
        }

        app.Run();

        static CosmosClient GetCosmosClient(IConfigurationSection configurationSection)
        {
            string? databaseName = configurationSection.GetSection("DatabaseName").Value;
            string? containerName = configurationSection.GetSection("ContainerName").Value;
            string? account = configurationSection.GetSection("Account").Value;
            string? key = configurationSection.GetSection("Key").Value;

            return new CosmosClient(account, key);
        }
    }
}

public partial class Program { }