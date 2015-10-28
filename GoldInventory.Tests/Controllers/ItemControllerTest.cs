using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GoldInventory.Controllers;
using GoldInventory.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoldInventory.Tests.Controllers
{
    [TestClass]
    public class ItemControllerTest
    {
        [TestMethod]
        public void Index()
        {
            var controller = new ItemController();
            var result = controller.Index().Result as ViewResult;
            
            Assert.IsNotNull(result);

            var model = result.Model as IEnumerable<Item>;
            Assert.IsTrue(model != null && model.Any());
        }
    }
}
