using System;
using System.Text;
using Digger.Server.Helpers;
using Digger.Server.Services;
using DiStock.DAL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using Digger.Server.Hubs;
using Digger.Server.Authentification;
using Digger.Server.DGraph;

namespace Digger.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string secretKey = Configuration["Diggos:SecretKey"];
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            services.AddMvc();
            services.AddAuthorization(o =>
            {
                o.AddPolicy("admin", p => p.RequireAuthenticatedUser().RequireRole("admin"));
                o.AddPolicy("user", p => p.RequireAuthenticatedUser().RequireRole("user"));
            });
            services.AddAuthentication(o => o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Authentification/Index";
            })
            .AddJwtBearer(JwtBearerAuthentication.AuthenticationType, o =>
             {
                 o.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = signingKey,
                     AuthenticationType = JwtBearerAuthentication.AuthenticationType,
                     ValidateLifetime = true,
                     ValidateAudience = false,
                     ValidateIssuer = false
                 };
             });
            services.AddSignalR();
            services.AddSingleton<DiggosService>();
            services.AddSingleton<GetAccessUser>();
            services.AddSingleton<TokenService>();
            services.AddSingleton<AuthentificationManager>();
            services.AddSingleton<PasswordHasher>();
            services.AddSingleton<DGraphGateway>();
            services.AddSingleton(_ => new RequestGateway(Configuration["DiStock:ConnectionString"]));
            services.AddSingleton(_ => new UserGateway(Configuration["DiStock:ConnectionString"]));
            services.AddSingleton(_ => new SoftwareGateway(Configuration["DiStock:ConnectionString"]));
            services.AddSingleton(_ => new ResearchModuleGateway(Configuration["DiStock:ConnectionString"]));
            services.AddSingleton(_ => new ProjectGateway(Configuration["DiStock:ConnectionString"]));
            services.Configure<DiggosServiceOptions>(o =>
            {
                o.Url = Configuration["Diggos:Url"];
            });
            services.Configure<DGraphGatewayOptions>(o =>
            {
                o.Host = Configuration["DGraph:Host"];
                o.Port = Convert.ToInt32(Configuration["DGraph:Port"]);
                o.CompleteAdress = Configuration["DGraph:CompleteAdress"];
            });
            services.Configure<TokenServiceOptions>(o =>
            {
                o.SigningCredentials = new SigningCredentials( signingKey, SecurityAlgorithms.HmacSha256 );
                o.Expiration = TimeSpan.FromMinutes( 1 );
            });
            services.AddSingleton<DiggosService>();

            ///HUB
            services.AddSingleton<ProjectHub>();
            services.AddSingleton<GraphHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSignalR(route =>
            {
                route.MapHub<ProjectHub>("/ProjectHub");
                route.MapHub<GraphHub>("/GraphHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Authentification}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "spa-fallback",
                    template: "Home/{*anything}",
                    defaults: new { controller = "Home", action = "Index" });
            });

            
        }
    }
}
