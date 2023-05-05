using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.DL
{
    //public class CustomerSalesDL
    //{
    //    public List<CustomerSale> GetActiveCustomerSalesList(DatabaseEntities de)
    //    {
    //        return de.CustomerSales.Where(x => x.IsActive == 1).ToList();
    //    }
    //    public CustomerSale GeteActiveCustomerSaleById(int _Id, DatabaseEntities de)
    //    {
    //        return de.CustomerSales.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
    //    }
    //    public bool AddCustomerSale(CustomerSale _CustomerSale, DatabaseEntities de)
    //    {
    //        try
    //        {
    //            de.CustomerSales.Add(_CustomerSale);
    //            de.SaveChanges();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //    public bool UpdateCustomerSale(CustomerSale _CustomerSale, DatabaseEntities de)
    //    {
    //        try
    //        {
    //            de.Entry(_CustomerSale).State = System.Data.Entity.EntityState.Modified;
    //            de.SaveChanges();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //    public bool DeleteCustomerSale(int _Id, DatabaseEntities de)
    //    {
    //        try
    //        {
    //            CustomerSale CustomerSale = de.CustomerSales.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
    //            CustomerSale.IsActive = 0;
    //            de.Entry(CustomerSale).State = System.Data.Entity.EntityState.Modified;
    //            de.SaveChanges();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //}
}