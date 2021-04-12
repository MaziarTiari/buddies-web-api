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
using MimeKit;
using System.Threading.Tasks;

namespace buddiesApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to
        // the container.
        public void ConfigureServices(IServiceCollection services) {
            string hostname = Configuration["Hostname"];

            services.Configure<ForwardedHeadersOptions>(options => {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => {
                c.SwaggerDoc(
                    "v1", new OpenApiInfo { Title = hostname + " API", Version = "v1" }
                );
            });

            string JWTKey = Configuration["JWT:Secret"];

            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(JWTKey)
                        ),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/chatHub"))) {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
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

            services.AddSingleton<ChatService>();

            services.AddSignalR(hubOptions => {
                hubOptions.EnableDetailedErrors = true;
            });

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseCamelCasing(true));

            var smtpServer = Configuration
                .GetSection(nameof(SmtServerConfig)).Get<SmtServerConfig>();

            services.AddSingleton<IAuthenticationManager>(
                new AuthenticationManager(
                    new AuthenticationConfig {
                        Key = JWTKey,
                        Hostname = hostname,
                        AdminMailboxAddress = new MailboxAddress(
                            hostname,
                            smtpServer.EmailAddress),
                        SmtpServerConfig = {
                            EmailAddress = smtpServer.EmailAddress,
                            Password = smtpServer.Password,
                            Portnumber = smtpServer.Portnumber,
                            ServerAddress = smtpServer.ServerAddress
                        }
                    })
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
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
