using Microsoft.VisualStudio.TestTools.UnitTesting;
using buddiesApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace buddiesApiTests {
    [TestClass]
    public class ValidPasswordAttributeTest {

        class WithPassword : IWithValidPassword {
            [ValidPassword]
            public string Password { get; set; }
        }

        private WithPassword model;

        [TestMethod]
        public void TestBannedPassword() {
            model = new WithPassword { Password = "1234567890" };
            Assert.IsFalse(IsValid(model));
        }

        [TestMethod]
        public void TestNotBannedPassword() {
            model = new WithPassword { Password = "1234567891" };
            Assert.IsTrue(IsValid(model));
        }

        [TestMethod]
        public void TestTooShortPassword() {
            model = new WithPassword { Password = "aaaaaa1" };
            Assert.IsFalse(IsValid(model));
        }

        [TestMethod]
        public void TestPasswortWithMinimumLength() {
            model = new WithPassword { Password = "aaaaaaa1" };
            Assert.IsTrue(IsValid(model));
        }

        [TestMethod]
        public void TestPasswortWithLengthGraterThenMinimum() {
            model = new WithPassword { Password = "aaaaaaa11" };
            Assert.IsTrue(IsValid(model));
        }

        private bool IsValid(IWithValidPassword model) {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, true);
        }
    }
}
