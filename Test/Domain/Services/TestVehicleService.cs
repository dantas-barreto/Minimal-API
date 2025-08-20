using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.services;
using MinimalApi.Infrastructure.DB;
using System.Runtime.CompilerServices;

namespace Test.Domain.Services
{
    [TestClass]
    public class TestVehicleService
    {
        private ContextDb CreateInMemoryTestContext()
        {
            var options = new DbContextOptionsBuilder<ContextDb>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            return new ContextDb(options);
        }

        [TestMethod]
        public void TestSaveVehicle()
        {
            var context = CreateInMemoryTestContext();

            var vehicle = new Vehicle
            {
                Id = 1,
                Model = "Test Model",
                Brand = "Test Brand",
                Year = 2023
            };

            var vehicleService = new VehicleService(context);

            vehicleService.Add(vehicle);

            var vehicles = context.Vehicles.ToList();
            foreach (var v in vehicles)
            {
                Console.WriteLine($"{v.Id} | {v.Model} | {v.Brand} | {v.Year}");
            }
            Assert.AreEqual(1, vehicleService.GetAll(1).Count());
        }
    }
}
