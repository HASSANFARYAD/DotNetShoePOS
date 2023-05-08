using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    public class CustomerSalesBL
    {
        public List<CustomerSale> GetActiveCustomerSalesList(DatabaseEntities de)
        {
            return new CustomerSalesDL().GetActiveCustomerSalesList(de);
        }

        public CustomerSale GetActiveCustomerSaleById(int _Id, DatabaseEntities de)
        {
            return new CustomerSalesDL().GeteActiveCustomerSaleById(_Id, de);
        }

        public bool AddCustomerSale(CustomerSale _CustomerSale, DatabaseEntities de)
        {
            if (_CustomerSale.CustomerId == null || _CustomerSale.InventoryId == null)
            {
                return false;
            }
            else
            {
                return new CustomerSalesDL().AddCustomerSale(_CustomerSale, de);
            }
        }

        public bool UpdateCustomerSale(CustomerSale _CustomerSale, DatabaseEntities de)
        {
            if (_CustomerSale.CustomerId == null || _CustomerSale.InventoryId == null)
            {
                return false;
            }
            else
            {
                return new CustomerSalesDL().UpdateCustomerSale(_CustomerSale, de);
            }
        }

        public bool DeleteCustomerSale(int _id, DatabaseEntities de)
        {
            return new CustomerSalesDL().DeleteCustomerSale(_id, de);
        }
    }
}