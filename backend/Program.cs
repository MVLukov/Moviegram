using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using moviesApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                {
                    context.Token = context.Request.Cookies["X-Access-Token"];
                    return Task.CompletedTask;
                }
                return Task.CompletedTask;
            }
        };
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsTheStrongestSecret")),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "https://localhost.local",
            ValidAudience = "https://localhost.local"
        };
    });

builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
policy =>
{
    policy.WithOrigins("http://localhost:4173").AllowAnyMethod().AllowAnyHeader();
}));

var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
string connectionStr = "Server=localhost;Database=moviegram;Uid=user;Pwd=User@123;";

builder.Services.AddDbContext<dbContext>(options =>
{
    options.UseMySql(connectionStr, serverVersion);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NgOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
