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
        public void SetUp()
        {
            _companyRepository = new Mock<ICompanyRepository>();
            _customerDataAccessWrapper = new Mock<ICustomerDataAccessWrapper>();
            _customerCreditService = new Mock<ICustomerCreditService>();
        }

        [Test]
        public void GivenFirstNameIsEmptyReturnsFalse()
        {
            var customerService = CallCustomerService();
            var result = customerService.AddCustomer(string.Empty, "surname", "email", DateTime.Now, 1);

            Assert.False(result);
        }

        [Test]
        public void GivenAValidCustomerWhenAddThenCustomerIsAddedToDatabase()
        {
            _companyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(value: new Company()
                {Classification = Classification.Gold, Id = 5, Name = "VeryImportantClient"});

            var customerService = CallCustomerService();
            var result = customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);

            _customerDataAccessWrapper.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Once());
        }

        [Test]
        public void GivenAValidImportantCustomerWhenAddThenCreditServiceIsChecked()
        {
            _companyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(value: new Company()
                { Classification = Classification.Gold, Id = 5, Name = "ImportantClient" });

            var customerService = CallCustomerService();
            customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);

            _customerCreditService.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        private CustomerService CallCustomerService()
        {
            var customerService = new CustomerService(_companyRepository.Object, _customerDataAccessWrapper.Object,
                _customerCreditService.Object);
            return customerService;
        }
    }
    }
