using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public interface ICustomerDataAccessWrapper
    {
        void AddCustomer(Customer customer);
    }

    public class CustomerDataAccessWrapper : ICustomerDataAccessWrapper
    {
        public void AddCustomer(Customer customer)
        {
            CustomerDataAccess.AddCustomer(customer);
        }
    }
}
