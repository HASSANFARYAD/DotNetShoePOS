using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.DL
{
    public class InvoiceDL
    {
        public List<Invoice> GetActiveInvoicesList(DatabaseEntities de)
        {
            return de.Invoices.Where(x => x.IsActive == 1).ToList();
        }
        public Invoice GeteActiveInvoiceById(int _Id, DatabaseEntities de)
        {
            return de.Invoices.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
        }
        public bool AddInvoice(Invoice _Invoice, DatabaseEntities de)
        {
            try
            {
                de.Invoices.Add(_Invoice);
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public int AddInvoice2(Invoice _Invoice, DatabaseEntities de)
        {
            try
            {
                de.Invoices.Add(_Invoice);
                de.SaveChanges();
                return _Invoice.Id;
            }
            catch
            {
                return -1;
            }
        }
        public bool UpdateInvoice(Invoice _Invoice, DatabaseEntities de)
        {
            try
            {
                de.Entry(_Invoice).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteInvoice(int _Id, DatabaseEntities de)
        {
            try
            {
                Invoice Invoice = de.Invoices.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
                Invoice.IsActive = 0;
                de.Entry(Invoice).State = System.Data.Entity.EntityState.Modified;
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