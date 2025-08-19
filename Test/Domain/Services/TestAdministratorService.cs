using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.services;
using MinimalApi.Infrastructure.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.Services
{
    [TestClass]
    public class TestAdministratorService
    {
        private ContextDb CreateTestContext()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // ou Directory.GetCurrentDirectory()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return new ContextDb(configuration);
        }

        [TestMethod]
        public void TestSaveAdministrator()
        {
            var context = CreateTestContext();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var admin = new Administrator();
            admin.Id = 1;
            admin.Email = "test@test.com";
            admin.Password = "test123";
            admin.Profile = "Admin";

            var administratorService = new AdministratorService(context);

            administratorService.Add(admin);

            Assert.AreEqual(1, administratorService.GetAll(1).Count());
        }
    }
}
