using System;
using Moq;
using NUnit.Framework;

namespace App.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        [Test]
        public void GivenFirstNameIsEmptyReturnsFalse()
        {
            Mock<ICompanyRepository> companyRepository = new Mock<ICompanyRepository>();

            var customerService = new CustomerService(companyRepository.Object);
            var result = customerService.AddCustomer(string.Empty, "surname", "email", DateTime.Now, 1);

            Assert.False(result);
        }
    }
}
