using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TodoApi.Services;

namespace TodoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. SERVICES : Base de données MongoDB
            builder.Services.AddSingleton<TodoService>();

            // 2. SERVICES : Authentification JWT
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            // Utilisation de UTF8 pour correspondre à AuthController
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "Cette_Cle_Doit_Faire_Au_Moins_32_Caracteres_Pour_Etre_Securisee");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            // 3. SERVICES : Swagger avec support du Token Bearer
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApi", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Entrez votre jeton JWT. Exemple: eyJhbGci..."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

           
            app.UseSwagger();
            app.UseSwaggerUI();
            

            app.UseHttpsRedirection();

            // L'ordre est crucial ici
            app.UseAuthentication(); // Qui es-tu ?
            app.UseAuthorization();  // As-tu le droit ?

            app.MapControllers();

            app.Run();
        }
    }
}