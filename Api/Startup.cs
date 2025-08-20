using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.services;
using MinimalApi.DTO;
using MinimalApi.Infrastructure.DB;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private string key;

    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            
            services.AddAuthorization();

            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<IVehicleService, VehicleService>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT no formato: Bearer {seu_token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

        services.AddDbContext<ContextDb>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Administrators
            string GenerateJwtToken(Administrator administrator)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrator.Email),
                    new Claim("Profile", administrator.Profile),
                    new Claim(ClaimTypes.Role, administrator.Profile),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
            {
                var adm = administratorService.Login(loginDTO);
                if (adm != null)
                {
                    string token = GenerateJwtToken(adm);

                    return Results.Ok(new LoggedAdministrator
                    {
                        Email = adm.Email,
                        Profile = adm.Profile,
                        Token = token
                    });
                }
                else
                {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous().WithTags("Administrators");

            endpoints.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
            {
                var validation = new ValidationError
                {
                    ErrorMessages = []
                };

                if (string.IsNullOrEmpty(administratorDTO.Email))
                    validation.ErrorMessages.Add("O email do administrador é obrigatório.");

                if (string.IsNullOrEmpty(administratorDTO.Password))
                    validation.ErrorMessages.Add("A senha do administrador é obrigatória.");

                if (administratorDTO.Profile == null)
                    validation.ErrorMessages.Add("O perfil do administrador é obrigatório.");


                if (validation.ErrorMessages.Count > 0)
                    return Results.BadRequest(validation);

                var administrator = new Administrator
                {
                    Email = administratorDTO.Email,
                    Password = administratorDTO.Password,
                    Profile = administratorDTO.Profile?.ToString() ?? Profile.User.ToString()
                };

                administratorService.Add(administrator);

                return Results.Created($"/administrators/{administrator.Id}", new AdministratorModelView
                {
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Profile = administrator.Profile
                });
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator" })
            .WithTags("Administrators");

            endpoints.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) =>
            {
                var administratorsList = new List<AdministratorModelView>();
                var administrators = administratorService.GetAll(page);

                foreach (var adm in administrators)
                {
                    administratorsList.Add(new AdministratorModelView
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Profile = adm.Profile
                    });
                }
                return Results.Ok(administratorService.GetAll(page));
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator" })
            .WithTags("Administrators");

            endpoints.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
            {
                var administrator = administratorService.GetById(id);

                if (administrator == null)
                    return Results.NotFound();

                return Results.Ok(new AdministratorModelView
                {
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Profile = administrator.Profile
                });
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator" })
            .WithTags("Administrators");
            #endregion

            #region Vehicles

            ValidationError validateDTO(VehicleDTO vehicleDTO)
            {
                var validation = new ValidationError
                {
                    ErrorMessages = new List<string>()
                };

                if (string.IsNullOrEmpty(vehicleDTO.Model))
                    validation.ErrorMessages.Add("O modelo do veículo é obrigatório.");

                if (string.IsNullOrEmpty(vehicleDTO.Brand))
                    validation.ErrorMessages.Add("A marca do veículo é obrigatória.");

                if (vehicleDTO.Year <= 0)
                    validation.ErrorMessages.Add("O ano do veículo deve ser maior que zero.");

                return validation;
            }

            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
            {
                var validation = validateDTO(vehicleDTO);
                if (validation.ErrorMessages.Count > 0)
                    return Results.BadRequest(validation);

                var vehicle = new Vehicle
                {
                    Model = vehicleDTO.Model,
                    Brand = vehicleDTO.Brand,
                    Year = vehicleDTO.Year
                };
                vehicleService.Add(vehicle);

                return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator, Editor" })
            .WithTags("Vehicles");

            endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
            {
                var vehicles = vehicleService.GetAll(page);

                return Results.Ok(vehicles);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator, Editor" })
            .WithTags("Vehicles");

            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);
                if (vehicle == null)
                    return Results.NotFound();

                return Results.Ok(vehicle);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator, Editor" })
            .WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);

                if (vehicle == null)
                    return Results.NotFound();

                var validation = validateDTO(vehicleDTO);
                if (validation.ErrorMessages.Count > 0)
                    return Results.BadRequest(validation);

                vehicle.Model = vehicleDTO.Model;
                vehicle.Brand = vehicleDTO.Brand;
                vehicle.Year = vehicleDTO.Year;
                vehicleService.Update(vehicle);

                return Results.Ok(vehicle);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator" })
            .WithTags("Vehicles");

            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);

                if (vehicle == null)
                    return Results.NotFound();

                vehicleService.Delete(vehicle);

                return Results.NoContent();
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator" })
            .WithTags("Vehicles");
            #endregion

        });
    }
}
