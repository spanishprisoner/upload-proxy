#define HTTPS

using System;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UploadProxy.Core.Services;
using UploadProxy.Core.Services.Implementation;
using UploadProxy.Data.Identity;
using UploadProxy.Data.Identity.Models;

namespace UploadProxy.Front
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public IHostingEnvironment Environment { get; }

		public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
		{
			Configuration = configuration;
			Environment = hostingEnvironment;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IHttpClientService, HttpClientService>();
			services.AddSingleton<IAzureKicker, AzureKicker>();
			services.AddTransient<IEmailSender, EmailSender>();
			services.AddTransient<IFileUploader, FtpFileUploader>();

			services.AddDbContext<UsersDbContext>();
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<UsersDbContext>()
				.AddDefaultTokenProviders();

			services.AddIdentityServer()
				.AddSigningCredential(new SigningCredentials(
					new JsonWebKey(Configuration["IdentityJwk"]),
					SecurityAlgorithms.RsaSha256Signature))
				.AddOperationalStore(options =>
					options.ConfigureDbContext = builder =>
						builder.UseMySql(Configuration.GetConnectionString("UsersDbContext")))
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddInMemoryApiResources(Config.GetApiResources())
				.AddInMemoryClients(Config.GetClients(Configuration["ClientSecret"]))
				.AddAspNetIdentity<IdentityUser>();
			services.AddTransient<IProfileService, IdentityClaimsProfileService>();
			services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

			services.AddMvc();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.Authority = Configuration["Authority"];
				options.Audience = "uploadproxyapi";
#if HTTPS
				options.RequireHttpsMetadata = true;
#else
				options.RequireHttpsMetadata = false;
#endif
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateAudience = true,
					ValidateIssuer = true,
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero,
				};
			});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("UploadProxyApiAccess", policy => policy.RequireClaim("upload_proxy_api_access", "true"));
			});

			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});

			services.AddLogging(e =>
			{
				e.AddDebug();
				e.AddAzureWebAppDiagnostics();
			});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env,
			ILoggerFactory loggerFactory, IAzureKicker azureKicker)
		{
			if (env.IsDevelopment())
			{
				//app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

#if HTTPS
			app.UseHttpsRedirection();
#endif
			app.UseStaticFiles();
			app.UseSpaStaticFiles();

			app.UseIdentityServer();
			app.UseAuthentication();
			app.UseMvc(routes =>
			{
				routes.MapRoute("default", "{controller}/{action=Index}/{id?}");
			});

			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseAngularCliServer("start");
				}
			});

			azureKicker.Start();
		}
	}
}
