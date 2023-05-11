using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.DL
{
    public class CustomerDL
    {
        public List<Customer> GetActiveCustomersList(DatabaseEntities de)
        {
            return de.Customers.Where(x => x.IsActive == 1).ToList();
        }
        public Customer GeteActiveCustomerById(int _Id, DatabaseEntities de)
        {
            return de.Customers.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
        }
        public bool AddCustomer(Customer _Customer, DatabaseEntities de)
        {
            try
            {
                de.Customers.Add(_Customer);
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddCustomer2(Customer _Customer, DatabaseEntities de)
        {
            try
            {
                de.Customers.Add(_Customer);
                de.SaveChanges();
                return _Customer.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateCustomer(Customer _Customer, DatabaseEntities de)
        {
            try
            {
                de.Entry(_Customer).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteCustomer(int _Id, DatabaseEntities de)
        {
            try
            {
                Customer Customer = de.Customers.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
                Customer.IsActive = 0;
                de.Entry(Customer).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}