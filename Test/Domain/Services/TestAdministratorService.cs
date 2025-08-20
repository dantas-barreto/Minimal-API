using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.services;
using MinimalApi.Infrastructure.DB;

namespace Test.Domain.Services
{
    [TestClass]
    public class TestAdministratorService
    {
        private ContextDb CreateInMemoryTestContext()
        {
            var options = new DbContextOptionsBuilder<ContextDb>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            return new ContextDb(options);
        }

        [TestMethod]
        public void TestSaveAdministrator()
        {
            var context = CreateInMemoryTestContext();

            var admin = new Administrator
            {
                Id = 1,
                Email = "test@test.com",
                Password = "test123",
                Profile = "Admin"
            };

            var administratorService = new AdministratorService(context);

            administratorService.Add(admin);

            var admins = context.Administrators.ToList();
            foreach (var a in admins)
            {
                Console.WriteLine($"{a.Id} | {a.Email} | {a.Password} | {a.Profile}");
            }

            Assert.AreEqual(1, administratorService.GetAll(1).Count());
        }
    }
}
