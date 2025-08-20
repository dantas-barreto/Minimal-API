using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;

namespace Test.Mocks
{
    public class VehicleServiceMock : IVehicleService
    {
        private static List<Vehicle> vehicles = new List<Vehicle>()
        {
            new Vehicle
            {
                Id = 1,
                Model = "Model X",
                Brand = "Brand A",
                Year = 2020
            },
            new Vehicle
            {
                Id = 2,
                Model = "Model Y",
                Brand = "Brand B",
                Year = 2021
            }
        };

        public void Add(Vehicle vehicle)
        {
            vehicle.Id = vehicles.Count() + 1;
            vehicles.Add(vehicle);
        }

        public void Delete(Vehicle vehicle)
        {
            vehicles.RemoveAll(v => v.Id == vehicle.Id);
        }

        public List<Vehicle> GetAll(int? page = 1, string? model = null, string? brand = null)
        {
            return vehicles;
        }

        public Vehicle? GetById(int id)
        {
            return vehicles.Find(v => v.Id == id);
        }

        public void Update(Vehicle vehicle)
        {
            var existingVehicle = vehicles.Find(v => v.Id == vehicle.Id);
            if (existingVehicle != null)
            {
                existingVehicle.Model = vehicle.Model;
                existingVehicle.Brand = vehicle.Brand;
                existingVehicle.Year = vehicle.Year;
            }
        }
    }
}
