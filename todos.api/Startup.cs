using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using todos.api.Abstractions.Helpers;
using todos.api.Abstractions.Repository;
using todos.api.Config;
using todos.api.Helpers;
using todos.api.Persistency;
using todos.api.Profiles;
using todos.api.Repository;


namespace todos.api
{
    public class Startup
    {
        private readonly string MyCors = "MyCors";
        private readonly ConnectionStringUtility _connectionStringUtility;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _connectionStringUtility = new ConnectionStringUtility(Configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            var connectionString = _connectionStringUtility.GetConnectionString();
            services.AddDbContext<TodosDBContext>(opts => opts.UseNpgsql(connectionString));
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITodoRepository, TodoRepository>();
            services.AddScoped<IEncryptionHelper, EncryptionHelper>();
            services.AddScoped<IBearerTokenHelper, BearerTokenHelper>();
            services.AddScoped<ConnectionStringUtility>();

            if (IsDevelopment)
            {
                services.Configure<EncryptionConfig>(Configuration.GetSection("Encryption"));
                services.Configure<JwtConfig>(Configuration.GetSection("Jwt"));
            }
            else 
            {
                services.Configure<EncryptionConfig>(config => config.Key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY"));
                services.Configure<JwtConfig>(config => config.Key = Environment.GetEnvironmentVariable("JWT_KEY"));
            }

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false
                    };
                });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);


            services.AddAuthorization();


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "todos.api",
                    Version = "v1",
                    Description = "Agilesoft - Prueba tecnica para postular a cargo .net",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors(options =>
            {

                options.AddPolicy(name: MyCors, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });

            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "todos.api v1"));
            }

            app.UseRouting();
            app.UseCors(MyCors);
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
