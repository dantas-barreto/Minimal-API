using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class TestVehicle
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            var vehicle = new Vehicle();
            
            vehicle.Id = 1;
            vehicle.Brand = "Toyota";
            vehicle.Model = "Corolla";
            vehicle.Year = 1985;

            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("Toyota", vehicle.Brand);
            Assert.AreEqual("Corolla", vehicle.Model);
            Assert.AreEqual(1985, vehicle.Year);
        }
    }
}
