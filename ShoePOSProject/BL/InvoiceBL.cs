using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    public class InvoiceBL
    {
        public List<Invoice> GetActiveInvoicesList(DatabaseEntities de)
        {
            return new InvoiceDL().GetActiveInvoicesList(de);
        }

        public Invoice GetActiveInvoiceById(int _Id, DatabaseEntities de)
        {
            return new InvoiceDL().GeteActiveInvoiceById(_Id, de);
        }

        public bool AddInvoice(Invoice _Invoice, DatabaseEntities de)
        {
            if (_Invoice.IsActive == null || _Invoice.CreatedAt == null)
            {
                return false;
            }
            else
            {
                return new InvoiceDL().AddInvoice(_Invoice, de);
            }
        }

        public int AddInvoice2(Invoice _Invoice, DatabaseEntities de)
        {
            if (_Invoice.IsActive == null || _Invoice.CreatedAt == null)
            {
                return -1;
            }
            else
            {
                return new InvoiceDL().AddInvoice2(_Invoice, de);
            }
        }

        public bool UpdateInvoice(Invoice _Invoice, DatabaseEntities de)
        {
            if (_Invoice.IsActive == null || _Invoice.CreatedAt == null)
            {
                return false;
            }
            else
            {
                return new InvoiceDL().UpdateInvoice(_Invoice, de);
            }
        }

        public bool DeleteInvoice(int _id, DatabaseEntities de)
        {
            return new InvoiceDL().DeleteInvoice(_id, de);
        }
    }
}