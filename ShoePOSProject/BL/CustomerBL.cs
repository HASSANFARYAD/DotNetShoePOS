﻿using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    public class CustomerBL
    {
        public List<Customer> GetActiveCustomersList(DatabaseEntities de)
        {
            return new CustomerDL().GetActiveCustomersList(de);
        }

        public Customer GetActiveCustomerById(int _Id, DatabaseEntities de)
        {
            return new CustomerDL().GeteActiveCustomerById(_Id, de);
        }

        public bool AddCustomer(Customer _Customer, DatabaseEntities de)
        {
            if (_Customer.IsActive == null)
            {
                return false;
            }
            else
            {
                return new CustomerDL().AddCustomer(_Customer, de);
            }
        }

        public int AddCustomer2(Customer _Customer, DatabaseEntities de)
        {
            if (_Customer.IsActive == null)
            {
                return -1;
            }
            else
            {
                return new CustomerDL().AddCustomer2(_Customer, de);
            }
        }

        public bool UpdateCustomer(Customer _Customer, DatabaseEntities de)
        {
            if (_Customer.IsActive == null)
            {
                return false;
            }
            else
            {
                return new CustomerDL().UpdateCustomer(_Customer, de);
            }
        }

        public bool DeleteCustomer(int _id, DatabaseEntities de)
        {
            return new CustomerDL().DeleteCustomer(_id, de);
        }
    }
}