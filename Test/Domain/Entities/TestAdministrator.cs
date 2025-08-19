using MinimalApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            admin.Email = "asdf@asdf.com";
            admin.Password = "123456";
            admin.Profile = "Administrator";

            Assert.AreEqual(1, admin.Id);
            Assert.AreEqual("asdf@asdf.com", admin.Email);
            Assert.AreEqual("123456", admin.Password);
            Assert.AreEqual("Administrator", admin.Profile);
        }
    }
}
