using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args); // Create a new instance of the WebApplication builder

builder.Services.AddControllers(); // Add the Controllers services to the container

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Allow requests from this frontend URL
                   .AllowAnyHeader() // Allow any header (e.g., Authorization)
                   .AllowAnyMethod() // Allow any HTTP method (GET, POST, etc.)
                   .AllowCredentials(); // Allow cookies or other credentials to be included in requests
        });
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // Add the EndpointsApiExplorer services to the container
builder.Services.AddSwaggerGen(); // Add the SwaggerGen services to the container
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); // Configure the DbContext to use SQL Server with the DefaultConnection connection string
});

builder.Services.AddScoped<IStockRepository, StockRepository>(); // Add the StockRepository service to the container with the IStockRepository interface
builder.Services.AddScoped<ICommentsRepository, CommentRepository>(); // Add the CommentRepository service to the container with the ICommentsRepository interface

builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IPortfolioRepository,PortfolioRepository>();

builder.Services.AddControllers().AddNewtonsoftJson(option =>
{
    option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // Ignore reference loops
});


builder.Services.AddIdentity<AppUser,IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 12;
}).AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = 
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme = 
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options=>{
    // TokenValidation
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
        
    };
});

var app = builder.Build(); // Build the WebApplication

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Check if the application is running in the development environment
{
    app.UseSwagger(); // Enable Swagger middleware
    app.UseSwaggerUI(); // Enable Swagger UI middleware
}


app.UseHttpsRedirection(); // Enable HTTPS redirection
// Use CORS
app.UseCors("AllowFrontend"); // Apply the CORS policy to all incoming requests
// Auth
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers(); // Map the controllers

app.Run(); // Run the application

