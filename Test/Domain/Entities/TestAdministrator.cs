using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class TestAdministrator
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            var admin = new Administrator();

            admin.Id = 1;
            admin.Email = "test@test.com";
            admin.Password = "test123";
            admin.Profile = "Administrator";

            Assert.AreEqual(1, admin.Id);
            Assert.AreEqual("test@test.com", admin.Email);
            Assert.AreEqual("test123", admin.Password);
            Assert.AreEqual("Administrator", admin.Profile);
        }
    }
}
