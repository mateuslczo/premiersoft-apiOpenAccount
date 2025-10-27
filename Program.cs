
using BankMore.Infrastructure.Security;
using BankMore.OpenAccount.Api.Interfaces;

namespace BankMore.OpenAccount
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

			builder.Services.AddHttpContextAccessor();

			builder.Services.AddSingleton<ITokenService, TokenService>();

			builder.Services.AddHttpClient<IAccountRegistrationService, AccountRegistrationService>((provider, client) =>
			{
				var configuration = provider.GetRequiredService<IConfiguration>();
				client.BaseAddress = new Uri(configuration["AccountApiSettings:BaseUrl"]);
				client.DefaultRequestHeaders.Add("Accept", "application/json");
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
