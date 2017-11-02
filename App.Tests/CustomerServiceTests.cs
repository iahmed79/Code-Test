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
            Mock<ICustomerDataAccessWrapper> customerDataAccessWrapper = new Mock<ICustomerDataAccessWrapper>();

            var customerService = new CustomerService(companyRepository.Object, customerDataAccessWrapper.Object);
            var result = customerService.AddCustomer(string.Empty, "surname", "email", DateTime.Now, 1);

            Assert.False(result);
        }

        [Test]
        public void GivenAValidVeryImportantCustomerWhenAddCustomerThenIsSaved()
        {
            var company = new Company();
            company.Name = "VeryImportantClient";
            Mock<ICompanyRepository> companyRepository = new Mock<ICompanyRepository>();
            Mock<ICustomerDataAccessWrapper> customerDataAccessWrapper = new Mock<ICustomerDataAccessWrapper>();
            companyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);

            var customerService = new CustomerService(companyRepository.Object, customerDataAccessWrapper.Object);
            customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);

            var expected = new Customer
            {
                Company = new Company(),
                CreditLimit = 0,
                DateOfBirth = DateTime.Now.AddYears(-25),
                EmailAddress = "email@email.com",
                Firstname = "firstname",
                Surname = "surname",
                HasCreditLimit = false,
                Id = 7
            };

            customerDataAccessWrapper.Verify(x => x.AddCustomer(It.Is<Customer>(a => IsValidCustomer(a, expected))),
                Times.Once());
        }

        private bool IsValidCustomer(Customer actual, Customer expected)
        {
            Assert.That(actual.Surname, Is.EqualTo(expected.Surname));
            Assert.That(actual.Firstname, Is.EqualTo(expected.Firstname));

            return true;
        }
    }
}
