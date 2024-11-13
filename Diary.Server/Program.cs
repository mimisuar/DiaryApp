using Diary.Server.Data;
using Diary.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Cryptography;
using System.Text;

namespace Diary.Server
{
	public class Program
	{
		private static void SetupSwagger(SwaggerGenOptions options)
		{
			OpenApiSecurityScheme securityScheme = new()
			{
				Name = "JWT Authentication",
				Description = "Enter your JWT token in this field",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT"
			};

			options.AddSecurityDefinition("Bearer", securityScheme);

			OpenApiSecurityRequirement securityRequirement = new()
			{
				{
					new OpenApiSecurityScheme()
					{
						Reference = new OpenApiReference()
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},

					new string[] { }
				}
			};

			options.AddSecurityRequirement(securityRequirement);
		}
        public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

            string publicSigningKey = builder.Configuration["Jwt:PublicSigningKey"] ?? throw new InvalidOperationException("Jwt:PublicSigningKey missing.");
			string encryptionKey = builder.Configuration["Jwt:EncryptionKey"] ?? throw new InvalidOperationException("Jwt:EncryptionKey missing.");
			IdentityModelEventSource.ShowPII = true;

            // Add services to the container.

            builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(SetupSwagger);

			string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not set.");
            builder.Services.AddDbContext<ApplicationDbContext>(
				options => options.UseSqlServer(connectionString));
            
            builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<ApplicationDbContext>();

			builder.Services.AddAuthorization();
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				RSA rsa = RSA.Create();
				rsa.ImportFromPem(publicSigningKey);
				RsaSecurityKey signingKey = new(rsa);

				SymmetricSecurityKey securityKey = new(Encoding.ASCII.GetBytes(encryptionKey));

				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new()
				{
					IssuerSigningKey = signingKey,
					ValidateIssuer = false,
					ValidateAudience = false,
					TokenDecryptionKey = securityKey
				};
				
			});

            builder.Services.AddScoped<CryptoService>();
			builder.Services.AddScoped<JwtService>();
			builder.Services.AddScoped<UserService>();
			builder.Services.AddScoped<JournalService>();

            var app = builder.Build();

			app.UseDefaultFiles();
			app.UseStaticFiles();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapGet("publickey", () => publicSigningKey);
			app.MapControllers();

			app.MapFallbackToFile("/index.html");

			app.Run();
		}
	}
}
