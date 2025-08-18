using System.Data;
using System.Data.SqlClient;
//using SheenlacMISPortal;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AllPaintsEcomAPI;
using AllPaintsEcomAPI.Interfaces;
using AllPaintsEcomAPI.Logging;
using AllPaintsEcomAPI.Middlewares;
using AllPaintsEcomAPI.Repositories;
using AllPaintsEcomAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using EcomShopRepository = AllPaintsEcomAPI.Repositories.EcomShopRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keeps PascalCase
    });


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
var appSettingsSection = builder.Configuration.GetSection("FcmNotification");
//builder.Services.Configure<FcmNotificationSetting>(appSettingsSection);

builder.Services.AddSwaggerGen();

// Add to Services
// ------------ Start ----------------
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<EcomShopRepository, AllPaintsEcomAPI.Services.EcomShopService>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();


// ------------ End ------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                "https://portal.sheenlac.com",
                "https://AllPaintsEcomAPI.sheenlac.com",
                "https://vendor.sheenlac.com",      
                "https://devmisportal.sheenlac.com",
                "https://misportal.sheenlac.com",
                "http://localhost:4200",
                "http://localhost:5000",
                "https://localhost:7257",
                "https://devvendor.sheenlac.com",
                "https://devportal.sheenlac.com"
            )
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});


var app = builder.Build();
app.UseRouting();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapGet("/security/getMessage",
//() => "Hello World!").RequireAuthorization();
app.MapPost("/security/createToken",
[AllowAnonymous] (User user) => {

    var _config = app.Services.GetRequiredService<IConfiguration>();
    var connstrg = _config.GetConnectionString("Database");
    string status = string.Empty;
    using (SqlConnection conn = new SqlConnection(connstrg))
    {
        conn.Open();
        string cmdtext = "sp_validate_AllPaints_login";
        SqlCommand cmd = new SqlCommand(cmdtext, conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@FilterValue1", user.UserName);
        cmd.Parameters.AddWithValue("@FilterValue2", user.Password);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            status = reader["cstatus"].ToString() ?? "";
        }
    }

    if (status == "True")
    {
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes
        (builder.Configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
             }),
            Expires = DateTime.UtcNow.AddMinutes(120),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        var stringToken = tokenHandler.WriteToken(token);
        return Results.Ok(stringToken);
    }
    return Results.Unauthorized();
});

//app.ConfigureExceptionHandler(logger);
app.UseExceptionHandler("/Error");
//app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();


