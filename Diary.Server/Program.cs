using Diary.Server.Data;
using Diary.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Diary.Server
{
	public class Program
	{
        public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not set.");
            builder.Services.AddDbContext<ApplicationDbContext>(
				options => options.UseSqlServer(connectionString));
            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<ApplicationDbContext>();
			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
			});

            builder.Services.AddScoped<CryptoService>();

			string publicSigningKey = builder.Configuration["Jwt:PublicSigningKey"] ?? throw new InvalidOperationException("Jwt:PublicSigningKey missing.");

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
