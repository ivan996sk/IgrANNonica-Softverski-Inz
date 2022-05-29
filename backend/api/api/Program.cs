using System.Text;
using api.Data;
using api.Interfaces;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

//Add Cors
builder.Services.AddCors();

// Add services to the container
//dodajemo dep inj

builder.Services.Configure<UserStoreDatabaseSettings>(
    builder.Configuration.GetSection(nameof(UserStoreDatabaseSettings)));

builder.Services.AddSingleton<IUserStoreDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<UserStoreDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("UserStoreDatabaseSettings:ConnectionString")));

//Inject Dependencies
builder.Services.AddScoped<IDatasetService, DatasetService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMlConnectionService, MlConnectionService>();
builder.Services.AddScoped<IModelService, ModelService>();
builder.Services.AddScoped<IPredictorService, PredictorService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IJwtToken, JwtToken>();
builder.Services.AddScoped<IExperimentService, ExperimentService>();
builder.Services.AddHostedService<TempFileService>();
builder.Services.AddHostedService<FillAnEmptyDb>();

//Ml Api Ip Filter
builder.Services.AddScoped<MlApiCheckActionFilter>(container =>
{
    var loggerFactory = container.GetRequiredService<ILoggerFactory>();
    var logger=loggerFactory.CreateLogger<MlApiCheckActionFilter>();
    var MlIp = builder.Configuration.GetValue<string>("AppSettings:MlIp");
    return new MlApiCheckActionFilter(MlIp, logger);
});



//Add Authentication
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings:JwtToken").Value)),
            ValidateIssuer=false,
            ValidateAudience=false
        };
    
    });
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
});

builder.Services.AddSignalR();

builder.Services.AddControllers();
string frontApi = builder.Configuration.GetValue<string>("AppSettings:FrontApi");
string frontApiAlt = builder.Configuration.GetValue<string>("AppSettings:FrontApiAlt");
string mlApi = builder.Configuration.GetValue<string>("AppSettings:MlApi");
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
    .WithOrigins(frontApi, mlApi,frontApiAlt)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
});



var app = builder.Build();


app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
});





//Add Cors
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.

//Add Authentication
app.UseAuthentication();



app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub");
});






app.Run();
