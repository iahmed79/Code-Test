using System;

namespace App
{
    public class CustomerService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICustomerDataAccessWrapper _customerDataAccessWrapper;
        private readonly ICustomerCreditService _customerCreditService;

        public CustomerService(ICompanyRepository companyRepository, ICustomerDataAccessWrapper customerDataAccessWrapper, ICustomerCreditService customerCreditService)
        {
            _companyRepository = companyRepository;
            _customerDataAccessWrapper = customerDataAccessWrapper;
            _customerCreditService = customerCreditService;
        }

        public bool AddCustomer(string firstName, string surname, string email, DateTime dateOfBirth, int companyId)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surname))
            {
                return false;
            }

            if (!email.Contains("@") && !email.Contains("."))
            {
                return false;
            }

            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            if (age < 21)
            {
                return false;
            }

            var company = _companyRepository.GetById(companyId);

            var customer = new Customer
                               {
                                   Company = company,
                                   DateOfBirth = dateOfBirth,
                                   EmailAddress = email,
                                   Firstname = firstName,
                                   Surname = surname
                               };

            if (company.Name == "VeryImportantClient")
            {
                // Skip credit check
                customer.HasCreditLimit = false;
            }
            else if (company.Name == "ImportantClient")
            {
                // Do credit check and double credit limit
                customer.HasCreditLimit = true;
                
                var creditLimit = _customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                creditLimit = creditLimit*2;
                customer.CreditLimit = creditLimit;
                
            }
            else
            {
                // Do credit check
                customer.HasCreditLimit = true;
                using (var customerCreditService = new CustomerCreditServiceClient())
                {
                    var creditLimit = customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                    customer.CreditLimit = creditLimit;
                }
            }

            if (customer.HasCreditLimit && customer.CreditLimit < 500)
            {
                return false;
            }

            _customerDataAccessWrapper.AddCustomer(customer);

            return true;
        }
    }
}
