using System;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace App.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private Mock<ICompanyRepository> _companyRepository;
        private Mock<ICustomerDataAccessWrapper> _customerDataAccessWrapper;
        private Mock<ICustomerCreditService> _customerCreditService;
        private CustomerService _customerService;


        [SetUp]
        public void SetUp()
        {
            _companyRepository = new Mock<ICompanyRepository>();
            _customerDataAccessWrapper = new Mock<ICustomerDataAccessWrapper>();
            _customerCreditService = new Mock<ICustomerCreditService>();
            _customerService = new CustomerService(_companyRepository.Object, _customerDataAccessWrapper.Object,
                _customerCreditService.Object);
        }

        [Test]
        public void GivenFirstNameIsEmptyReturnsFalse()
        {
            var result = _customerService.AddCustomer(string.Empty, "surname", "email", DateTime.Now, 1);

            Assert.False(result);
        }

        [Test]
        public void GivenAValidCustomerWhenAddThenCustomerIsAddedToDatabase()
        {
            SetUpCompanyRepository("VeryImportantClient");

            var result = _customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);

            _customerDataAccessWrapper.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Once());
        }

        private void SetUpCompanyRepository(string companyName)
        {
            _companyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(value: new Company()
                {Classification = Classification.Gold, Id = 5, Name = companyName});
        }

        [Test]
        public void GivenAValidImportantCustomerWhenAddThenCreditServiceIsChecked()
        {
            SetUpCompanyRepository("ImportantClient");
            
            _customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);

            _customerCreditService.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void GivenAValidUnimportantCustomerWhenAddThenCreditServiceIsChecked()
        {
            SetUpCompanyRepository("Client");

            _customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);

            _customerCreditService.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }
    }
    }
