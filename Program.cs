
using BankMore.Infrastructure.Security;
using BankMore.OpenAccount.Api.Interfaces;
using BankMore.OpenAccount.Api.Services;
using BankMore.OpenAccount.Api.Services.EndpointsFactory;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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
			builder.Services.AddTransient<TokenStoreHandler>();

			builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("AccountApiSettings"));

			// HttpClient Config
			builder.Services.AddHttpClient("HttpClientApi", (provider, client) =>
			{

				var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();
				var configuration = provider.GetRequiredService<IConfiguration>();
				client.BaseAddress = new Uri(configuration["AccountApiSettings:BaseUrl"]);
				client.DefaultRequestHeaders.Add("Accept", "application/json");


			}).AddHttpMessageHandler<TokenStoreHandler>();

			builder.Services.AddScoped<IAccountLoginService>(provider =>
			{
				var factory = provider.GetRequiredService<IHttpClientFactory>();
				var httpClient = factory.CreateClient("HttpClientApi");

				var tokenService = provider.GetRequiredService<ITokenService>();
				var logger = provider.GetRequiredService<ILogger<AccountLoginService>>();
				var iOptions = provider.GetRequiredService<IOptions<ApiSettings>>();

				return new AccountLoginService(httpClient, tokenService, logger, iOptions);
			});

			builder.Services.AddScoped<IAccountCurrentService>(provider =>
			{
				var factory = provider.GetRequiredService<IHttpClientFactory>();
				var httpClient = factory.CreateClient("HttpClientApi");

				var tokenService = provider.GetRequiredService<ITokenService>();
				var logger = provider.GetRequiredService<ILogger<AccountCurrentService>>();
				var iOptions = provider.GetRequiredService<IOptions<ApiSettings>>();
				return new AccountCurrentService(httpClient, tokenService, logger, iOptions);
			});

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
		};
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
