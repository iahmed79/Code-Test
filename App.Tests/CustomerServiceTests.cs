using System;
using Moq;
using NUnit.Framework;

namespace App.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private Mock<ICompanyRepository> _companyRepository;
        private Mock<ICustomerDataAccessWrapper> _customerDataAccessWrapper;
        private Mock<ICustomerCreditService> _customerCreditService;

        [SetUp]
        public void TestSetUp()
        {
            _companyRepository = new Mock<ICompanyRepository>();
            _customerDataAccessWrapper = new Mock<ICustomerDataAccessWrapper>();
            _customerCreditService = new Mock<ICustomerCreditService>();
        }

        [Test]
        public void GivenFirstNameIsEmptyReturnsFalse()
        {
            var customerService = new CustomerService(_companyRepository.Object, _customerDataAccessWrapper.Object, _customerCreditService.Object);
            var result = customerService.AddCustomer(string.Empty, "surname", "email", DateTime.Now, 1);

            Assert.False(result);
        }

        [Test]
        public void GivenAValidVeryImportantCustomerWhenAddCustomerThenIsSaved()
        {
            var company = new Company();
            company.Name = "VeryImportantClient";
            _companyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);

            var customerService = new CustomerService(_companyRepository.Object, _customerDataAccessWrapper.Object, _customerCreditService.Object);
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

            _customerDataAccessWrapper.Verify(x => x.AddCustomer(It.Is<Customer>(a => IsValidCustomer(a, expected))),
                Times.Once());
        }

        [Test]
        public void GivenAValidImportantCustomerWhenAddCustomerThenIsSaved()
        {
            var company = new Company();
            company.Name = "ImportantClient";
            _customerCreditService = new Mock<ICustomerCreditService>();
            _companyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);
            _customerCreditService
                .Setup(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(10);

            var customerService = new CustomerService(_companyRepository.Object, _customerDataAccessWrapper.Object, _customerCreditService.Object);
            var customerServiceResult = customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);
            
            Assert.That(customerServiceResult, Is.EqualTo(false));
        }

        private bool IsValidCustomer(Customer actual, Customer expected)
        {
            Assert.That(actual.Surname, Is.EqualTo(expected.Surname));
            Assert.That(actual.Firstname, Is.EqualTo(expected.Firstname));

            //ToDo: Add assertions for other fields

            return true;
        }
    }
}
