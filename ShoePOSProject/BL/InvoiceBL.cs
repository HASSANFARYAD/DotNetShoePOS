using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    //public class InvoiceBL
    //{
    //    public List<CustomerInvoice> GetActiveInventoriesList(DatabaseEntities de)
    //    {
    //        return new InvoiceDL().GetActiveCustomerInvoicesList(de);
    //    }

    //    public CustomerInvoice GetActiveCustomerInvoiceById(int _Id, DatabaseEntities de)
    //    {
    //        return new InvoiceDL().GeteActiveCustomerInvoiceById(_Id, de);
    //    }

    //    public bool AddCustomerInvoice(CustomerInvoice _CustomerInvoice, DatabaseEntities de)
    //    {
    //        if (_CustomerInvoice.IsActive == null || _CustomerInvoice.CreatedAt == null)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            return new InvoiceDL().AddCustomerInvoice(_CustomerInvoice, de);
    //        }
    //    }

    //    public bool UpdateCustomerInvoice(CustomerInvoice _CustomerInvoice, DatabaseEntities de)
    //    {
    //        if (_CustomerInvoice.IsActive == null || _CustomerInvoice.CreatedAt == null)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            return new InvoiceDL().UpdateCustomerInvoice(_CustomerInvoice, de);
    //        }
    //    }

    //    public bool DeleteCustomerInvoice(int _id, DatabaseEntities de)
    //    {
    //        return new InvoiceDL().DeleteCustomerInvoice(_id, de);
    //    }
    //}
}