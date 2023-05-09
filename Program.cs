using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VK_TEST.DataBase;
using VK_TEST.VK_T_Objects.Entity;
using VK_TEST.VK_T_Objects.Services;
using WebApi.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VK_TEST.DataBase.AppContext>(option => 
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IDbProvider<User>, UserDbProvider<User>>();
builder.Services.AddTransient<IDbProvider<UserGroup>, DbProvider<UserGroup>>();
builder.Services.AddTransient<IDbProvider<UserState>, DbProvider<UserState>>();
builder.Services.AddTransient<IDbProvider<LimiterCount>, DbProvider<LimiterCount>>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddAuthentication().
            AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
            ("Basic", null);
builder.Services.AddCors();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Description = "Basic auth added to authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "basic",
        Type = SecuritySchemeType.Http
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
                    },
                    new List<string>()
                }
            });
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
