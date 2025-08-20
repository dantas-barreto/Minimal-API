using MinimalApi.Domain.Entities;
using System.Text;
using System.Text.Json;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class TestVehicleRequest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Setup.ClassInit(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task TestAddAndGetVehicle()
        {
            // Cria um novo veículo
            var newVehicle = new Vehicle
            {
                Model = "Model Test",
                Brand = "Brand Test",
                Year = 2025
            };

            var content = new StringContent(JsonSerializer.Serialize(newVehicle), Encoding.UTF8, "application/json");

            var postResponse = await Setup.client.PostAsync("/vehicles", content);
            Assert.AreEqual(201, (int)postResponse.StatusCode);

            var createdJson = await postResponse.Content.ReadAsStringAsync();
            var createdVehicle = JsonSerializer.Deserialize<Vehicle>(createdJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(createdVehicle);
            Assert.AreEqual(newVehicle.Model, createdVehicle.Model);
            Assert.AreEqual(newVehicle.Brand, createdVehicle.Brand);
            Assert.AreEqual(newVehicle.Year, createdVehicle.Year);
            Assert.IsTrue(createdVehicle.Id > 0);

            var getResponse = await Setup.client.GetAsync($"/vehicles/{createdVehicle.Id}");
            Assert.AreEqual(200, (int)getResponse.StatusCode);

            var getJson = await getResponse.Content.ReadAsStringAsync();
            var fetchedVehicle = JsonSerializer.Deserialize<Vehicle>(getJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(fetchedVehicle);
            Assert.AreEqual(createdVehicle.Id, fetchedVehicle.Id);
            Assert.AreEqual(createdVehicle.Model, fetchedVehicle.Model);
        }
    }
}

