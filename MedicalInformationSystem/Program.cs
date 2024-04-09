using System.Text.Json.Serialization;
using MedicalInformationSystem.Data;
using MedicalInformationSystem.Data.Entities;
using MedicalInformationSystem.Requirements;
using MedicalInformationSystem.Services;
using MedicalInformationSystem.Services.ConsultationService;
using MedicalInformationSystem.Services.DictionaryService;
using MedicalInformationSystem.Services.DoctorService;
using MedicalInformationSystem.Services.HashService;
using MedicalInformationSystem.Services.InspectionService;
using MedicalInformationSystem.Services.Jwt;
using MedicalInformationSystem.Services.PatientService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;
using TokenHandler = MedicalInformationSystem.Requirements.TokenHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
        //npgsqlOptions.MaxBatchSize(100000); // Установите желаемый размер пакета команд
    }));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TokenPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new TokenRequirement());
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //options.RequireHttpsMetadata = false; 
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = JwtConfigurations.Issuer,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = JwtConfigurations.Audience,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
            
        };
    });



//add services for correct display enum on swagger
builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation  
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MIS",
        Description = "Medical Information System"
    });
    
    //to add annotations 
    swagger.EnableAnnotations();
    
    //To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<HashPasswordService>();
builder.Services.AddScoped<IDictionaryService, DictionaryService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IInspectionService, InspectionService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthorizationHandler, TokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 1 option
using (var scope = app.Services.CreateScope())
{
     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
     db.Database.Migrate();

     if (!db.Speciality.Any())
     {
         var specialityJson = File.ReadAllText("specialities.json");
         var specialities = JsonSerializer.Deserialize<List<SpecialityEntity>>(specialityJson);
         foreach (var speciality in specialities)
         {
             speciality.Id = Guid.NewGuid();
             speciality.CreateTime = DateTime.Now.ToUniversalTime();
             db.Speciality.Add(speciality);
         }
         db.SaveChanges();
     }
     
     if (!db.IcdRoots.Any())
     {
         var icdRootsJson = File.ReadAllText("IcdRoots.json");
         var icdRoots = JsonSerializer.Deserialize<List<IcdRootsEntity>>(icdRootsJson);

         foreach (var icdRoot in icdRoots)
         {
             icdRoot.NewId = Guid.NewGuid();
             db.IcdRoots.Add(icdRoot);
         }
         db.SaveChanges();
     }

     if (!db.Icd.Any())
     {
         var icdJson = File.ReadAllText("ICD.json");
         var icd = JsonSerializer.Deserialize<List<IcdEntity>>(icdJson);
         
         foreach (var icdItem in icd)
         {
             icdItem.NewId = Guid.NewGuid();

             icdItem.IcdRoot = icdItem.ID switch
             {
                 < 933 => db.IcdRoots.First(x => x.ID == 1),
                 < 1806 => db.IcdRoots.First(x => x.ID == 933),
                 < 2009 => db.IcdRoots.First(x => x.ID == 1806),
                 < 2428 => db.IcdRoots.First(x => x.ID == 2009),
                 < 2916 => db.IcdRoots.First(x => x.ID == 2428),
                 < 3314 => db.IcdRoots.First(x => x.ID == 2916),
                 < 3633 => db.IcdRoots.First(x => x.ID == 3314),
                 < 3773 => db.IcdRoots.First(x => x.ID == 3633),
                 < 4238 => db.IcdRoots.First(x => x.ID == 3773),
                 < 4525 => db.IcdRoots.First(x => x.ID == 4238),
                 < 5012 => db.IcdRoots.First(x => x.ID == 4525),
                 < 5416 => db.IcdRoots.First(x => x.ID == 5012),
                 < 6058 => db.IcdRoots.First(x => x.ID == 5416),
                 < 6571 => db.IcdRoots.First(x => x.ID == 6058),
                 < 7067 => db.IcdRoots.First(x => x.ID == 6571),
                 < 7464 => db.IcdRoots.First(x => x.ID == 7067),
                 < 8186 => db.IcdRoots.First(x => x.ID == 7464),
                 < 8587 => db.IcdRoots.First(x => x.ID == 8186),
                 < 10344 => db.IcdRoots.First(x => x.ID == 8587),
                 < 14061 => db.IcdRoots.First(x => x.ID == 10344),
                 < 14960 => db.IcdRoots.First(x => x.ID == 14061),
                 _ => db.IcdRoots.First(x => x.ID == 14960)
             };
             db.Icd.Add(icdItem);
         }
         db.SaveChanges();
     }

}

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();
