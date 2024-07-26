using AuthService.Models;
using AuthService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Startup
{
    public IConfiguration Configuration { get; }
    private readonly JwtSettings _jwtSettings;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        _jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
        services.AddTransient<IUserService, UserService>(); 
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}