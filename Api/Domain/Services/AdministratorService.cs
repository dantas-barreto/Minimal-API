using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.DTO;
using MinimalApi.Infrastructure.DB;

namespace MinimalApi.Domain.services;

public class AdministratorService : IAdministratorService
{
    private readonly ContextDb _context;
    public AdministratorService(ContextDb context)
    {
        _context = context;
    }

    public Administrator? Login(LoginDTO loginDTO)
    {
        var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Senha).FirstOrDefault();
        return adm;
    }

    public Administrator Add(Administrator administrator)
    {
        _context.Administrators.Add(administrator);
        _context.SaveChanges();

        return administrator;
    }

    public List<Administrator> GetAll(int? page)
    {
        var query = _context.Administrators.AsQueryable();

        if (page != null)
            return query.Skip(((int)page - 1) * 10).Take(10).ToList();
    
        return query.ToList();
    }

    public Administrator? GetById(int id)
    {
        var adm = _context.Administrators.Where(a => a.Id == id).FirstOrDefault();
        return adm;
    }
}