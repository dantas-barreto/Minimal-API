using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.DB;

namespace MinimalApi.Domain.services;

public class VehicleService : IVehicleService 
{
    private readonly ContextDb _context;
    public VehicleService(ContextDb context)
    {
        _context = context;
    }

    public List<Vehicle> GetAll(int? page = 1, string? model = null, string? brand = null)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(model))
        {
            query = query.Where(v => v.Model.ToLower().Contains(model));
        }

        if (!string.IsNullOrEmpty(brand))
        {
            query = query.Where(v => v.Brand.ToLower().Contains(brand));
        }

        if (page != null)
            return query.Skip(((int)page - 1) * 10).Take(10).ToList();
    
        return query.ToList();
    }

    public Vehicle? GetById(int id)
    {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Add(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    public void  Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }

    public void Delete(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }
}