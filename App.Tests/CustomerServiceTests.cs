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

        [Test]
        public void GivenAValidCustomerWhenAddThen()
        {
            Mock<ICompanyRepository> companyRepository = new Mock<ICompanyRepository>();

            companyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(value: new Company()
                {Classification = Classification.Gold, Id = 5, Name = "VeryImportantClient"});

            var customerService = new CustomerService(companyRepository.Object);
            var result = customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);

            Assert.True(result);

        }
    }
}
