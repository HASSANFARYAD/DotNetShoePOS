using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.DL
{
    //public class InvoiceDL
    //{
    //    public List<CustomerInvoice> GetActiveCustomerInvoicesList(DatabaseEntities de)
    //    {
    //        return de.CustomerInvoices.Where(x => x.IsActive == 1).ToList();
    //    }
    //    public CustomerInvoice GeteActiveCustomerInvoiceById(int _Id, DatabaseEntities de)
    //    {
    //        return de.CustomerInvoices.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
    //    }
    //    public bool AddCustomerInvoice(CustomerInvoice _CustomerInvoice, DatabaseEntities de)
    //    {
    //        try
    //        {
    //            de.CustomerInvoices.Add(_CustomerInvoice);
    //            de.SaveChanges();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //    public bool UpdateCustomerInvoice(CustomerInvoice _CustomerInvoice, DatabaseEntities de)
    //    {
    //        try
    //        {
    //            de.Entry(_CustomerInvoice).State = System.Data.Entity.EntityState.Modified;
    //            de.SaveChanges();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //    public bool DeleteCustomerInvoice(int _Id, DatabaseEntities de)
    //    {
    //        try
    //        {
    //            CustomerInvoice CustomerInvoice = de.CustomerInvoices.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
    //            CustomerInvoice.IsActive = 0;
    //            de.Entry(CustomerInvoice).State = System.Data.Entity.EntityState.Modified;
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