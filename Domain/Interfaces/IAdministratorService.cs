using MinimalApi.Domain.Entities;
using MinimalApi.DTO;

namespace MinimalApi.Domain.Interfaces;

public interface IAdministratorService
{
    Administrator? Login(LoginDTO loginDTO);
    List<Administrator> GetAll(int? page);
    Administrator? GetById(int id);
    Administrator Add(Administrator administrator);
}