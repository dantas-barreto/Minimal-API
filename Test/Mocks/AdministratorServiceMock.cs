using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.DTO;

namespace Test.Mocks
{
    public class AdministratorServiceMock : IAdministratorService
    {
        private static List<Administrator> administrators = new List<Administrator>()
        {
            new Administrator
            {
                Id = 1,
                Email = "adm@teste.com",
                Password = "123456",
                Profile = "Administrator"
            },
            new Administrator
            {
                Id = 2,
                Email = "editor@teste.com",
                Password = "123456",
                Profile = "Editor"
            }
        };
            
        public Administrator Add(Administrator administrator)
        {
            administrator.Id = administrators.Count() + 1;
            administrators.Add(administrator);
            return administrator;
        }

        public List<Administrator> GetAll(int? page)
        {
            return administrators;
        }

        public Administrator? GetById(int id)
        {
            return administrators.Find(a => a.Id == id);
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            return administrators.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Senha);
        }
    }
}
