using MinimalApi.Domain.ModelViews;
using MinimalApi.DTO;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class TestAdministratorRequest
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
        public async Task testAdministratorRequest()
        {
            var loginDTO = new LoginDTO
            {
                Email = "adm@teste.com",
                Password = "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            var response = await Setup.client.PostAsync("/administrators/login", content);

            Assert.AreEqual(200, (int)response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var loggedAdm = JsonSerializer.Deserialize<LoggedAdministrator>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(loggedAdm.Email ?? "");
            Assert.IsNotNull(loggedAdm.Profile ?? "");
            Assert.IsNotNull(loggedAdm.Token ?? "");
        }
    }
}
