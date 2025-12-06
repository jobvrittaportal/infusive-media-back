using Infusive_back.EntityData;
using Infusive_back.JwtAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------- CORS CONFIGURATION -----------
var policyName = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
                      builder =>
                      {
                          builder
                            .AllowAnyOrigin()   // allow requests from any domain
                            .AllowAnyMethod()   // allow any HTTP method (GET, POST, etc.)
                            .AllowAnyHeader();  // allow any headers
                      });
});

// ----------- CONTROLLERS & SWAGGER -----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // required for Swagger
builder.Services.AddSwaggerGen(); // enables Swagger UI

// ----------- DATABASE CONFIGURATION (MySQL) -----------
builder.Services.AddDbContext<MyDbContext>(options =>
{
    var connectionString = Infusive_back.Utility_Func.Common_Func.ConnectionString();
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString) // automatically detects MySQL version
    );
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

// ----------- JWT TOKEN SERVICES -----------
builder.Services.AddSingleton<ITokenManager, TokenManager>(); // custom token manager (singleton instance)
builder.Services.AddScoped<CheckPermission>();

// Configure authentication to use JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // default auth = JWT
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    // default challenge = JWT
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // validate signature
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Shubham@#2206@#Macky@kjhsa67576sjb")), // secret key
        ValidateLifetime = true,         // token expiry validation
        ValidateAudience = false,        // skipping audience validation
        ValidateIssuer = false,          // skipping issuer validation
        ClockSkew = TimeSpan.Zero        // no time tolerance for expiry
    };
});

// ----------- HTTP CLIENT SUPPORT (FOR CALLING OTHER APIs) -----------
builder.Services.AddHttpClient();

var app = builder.Build();

// ----------- DEVELOPMENT MODE SWAGGER -----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();  // force HTTPS
app.UseCors(policyName);    // enable CORS policy
app.UseAuthentication();    // enable JWT authentication middleware
app.UseAuthorization();     // enable role/permission checks

app.MapControllers();       // map controller routes

app.Run();                  // run the application