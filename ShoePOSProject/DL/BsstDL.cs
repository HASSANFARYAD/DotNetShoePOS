using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.DL
{
    public class BsstDL
    {
        public List<BSST> GetActiveBSSTsList(DatabaseEntities de)
        {
            return de.BSSTs.Where(x => x.IsActive == 1).ToList();
        }
        public BSST GeteActiveBSSTById(int _Id, DatabaseEntities de)
        {
            return de.BSSTs.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
        }
        public bool AddBSST(BSST _BSST, DatabaseEntities de)
        {
            try
            {
                de.BSSTs.Add(_BSST);
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateBSST(BSST _BSST, DatabaseEntities de)
        {
            try
            {
                de.Entry(_BSST).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteBSST(int _Id, DatabaseEntities de)
        {
            try
            {
                BSST BSST = de.BSSTs.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
                BSST.IsActive = 0;
                de.Entry(BSST).State = System.Data.Entity.EntityState.Modified;
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