using Data.Layer.Contexts;
using Data.Layer.Entities;
using Data.Layer.Helper;
using Data.Layer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Layer;
using Service.Layer.Configuration;
using System.Text;
using System.Text.Json.Serialization;
using UserManagementService.Error;
using UserManagementService.Interfaces;
using UserManagementService.Middleware;

namespace UserManagementService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(o =>
               {
                   o.RequireHttpsMetadata = false;
                   o.SaveToken = false;
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateAudience = true,
                       ValidateIssuer = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                       ValidAudience = builder.Configuration["JWT:ValidAudience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"])),
                       ClockSkew = TimeSpan.Zero
                   };
               });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers().AddJsonOptions(options =>

            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())

            ); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
            builder.Services.AddScoped(typeof(IEmailSend), typeof(EmailSender));
            builder.Services.AddScoped(typeof(IUserService), typeof(UserService));
            builder.Services.AddHostedService<KafkaUserConsumerService>();

             builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddIdentity<User, IdentityRole>(option =>
            {
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;
                option.Password.RequireDigit = true;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequiredLength = 8;
                option.Tokens.PasswordResetTokenProvider = "Default";

            }).AddEntityFrameworkStores<UserDbContext>().AddDefaultTokenProviders();


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (contextAtion) =>
                {
                    var error = contextAtion.ModelState.Where(m => m.Value.Errors.Count > 0)
                                                       .SelectMany(m => m.Value.Errors)
                                                       .Select(m => m.ErrorMessage)
                                                       .ToList();
                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = error
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection("Kafka"));
            builder.Services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<KafkaConfig>>().Value
            );


            builder.Services.AddHostedService<KafkaUserConsumerService>();

            var app = builder.Build();
            using var scope = app.Services.CreateScope(); /// instead of using try finally to dispose the scope
            var services = scope.ServiceProvider;
            var _dbcontext = services.GetRequiredService<UserDbContext>();
            var _userManager = services.GetRequiredService<UserManager<User>>();
            var _roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _dbcontext.Database.MigrateAsync();
                await DataSeeding.SeedingDataAsync(_userManager, _roleManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<ILoggerFactory>();
                logger.LogError(ex, "There is Error in Migration");
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAllOrigins");

            app.UseExceptionMiddleWare();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.MapControllers();

            app.UseAuthentication();

            app.UseAuthorization();

            app.Run();
        }
    }
}