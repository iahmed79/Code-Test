﻿using System;
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
        public void GivenAValidCustomerWhenAddCustomerThenCompanyRepositoryIsCalled()
        {
            var company = new Company();
            company.Name = "VeryImportantClient";
            Mock<ICompanyRepository> companyRepository = new Mock<ICompanyRepository>();
            Mock<ICustomerDataAccessWrapper> customerDataAccessWrapper = new Mock<ICustomerDataAccessWrapper>();
            companyRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(company);

            var customerService = new CustomerService(companyRepository.Object, customerDataAccessWrapper.Object);
            customerService.AddCustomer("firstname", "surname", "email@email.com", DateTime.Now.AddYears(-25), 1);

            
        }
    }
}
