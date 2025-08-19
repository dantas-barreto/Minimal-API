using MinimalApi.Infrastructure.DB;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Entities;
using MinimalApi.DTO;
using MinimalApi.Domain.Enums;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ContextDb>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administrators
app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
        return Results.Ok("Login efetuado com sucesso!");
    else
        return Results.Unauthorized();
}).WithTags("Administrators");

app.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
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

    return Results.Created($"/administrators/{administrator.Id}", new AdministratorModelView{
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Profile
    });
}).WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) =>
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
}).WithTags("Administrators");

app.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.GetById(id);

    if (administrator == null)
        return Results.NotFound();
    
    return Results.Ok(new AdministratorModelView{
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Profile
    });
}).WithTags("Administrators");
#endregion

#region Vehicles

ValidationError validateDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationError{
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

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAll(page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null)
        return Results.NotFound();

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);

    if (vehicle == null)
        return Results.NotFound();

    vehicleService.Delete(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

