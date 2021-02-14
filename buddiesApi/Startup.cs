using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using buddiesApi.Models;
using buddiesApi.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using buddiesApi.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using buddiesApi.Middlewares.AuthenticationManager;
using AuthenticationManager = buddiesApi.Middlewares.AuthenticationManager.AuthenticationManager;

namespace buddiesApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to
        // the container.
        public void ConfigureServices(IServiceCollection services) {
            // using System.Net;

            services.Configure<ForwardedHeadersOptions>(options => {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => {
                c.SwaggerDoc(
                    "v1", new OpenApiInfo { Title = "Buddies API", Version = "v1" }
                );
            });

            string JWTKey = Configuration["JWT:Secret"];

            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x => {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(JWTKey)
                        ),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            services.Configure<BuddiesDbContext>(
                Configuration.GetSection(nameof(BuddiesDbContext))
            );
            services.AddSingleton<IBuddiesDbContext>(sp =>
                sp.GetRequiredService<IOptions<BuddiesDbContext>>().Value
            );
            services.AddSingleton<UserService>();
            services.AddSingleton<UserProfileService>();
            services.AddSingleton<CategoryService>();
            services.AddSingleton<PhotoGalleryService>();
            services.AddSingleton<ActivityService>();
            services.AddSignalR(hubOptions => {
                hubOptions.EnableDetailedErrors = true;
            });
            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseCamelCasing(true));
            services.AddSingleton<IAuthenticationManager>(
                new AuthenticationManager(JWTKey)
            );
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            // using Microsoft.AspNetCore.HttpOverrides;

            app.UseCors(builder => {
                builder
                 .AllowAnyOrigin()
                 .AllowAnyHeader()
                 .AllowAnyMethod();
            });

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Buddies API V1");
            });

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<ActivityHub>("/activityHub");
                endpoints.MapHub<UserHub>("/userHub");
            });
        }
    }
}
