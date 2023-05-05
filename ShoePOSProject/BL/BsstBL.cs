using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    public class BsstBL
    {
        public List<BSST> GetActiveBSSTsList(DatabaseEntities de)
        {
            return new BsstDL().GetActiveBSSTsList(de);
        }

        public BSST GetActiveBSSTById(int _Id, DatabaseEntities de)
        {
            return new BsstDL().GeteActiveBSSTById(_Id, de);
        }

        public bool AddBSST(BSST _BSST, DatabaseEntities de)
        {
            if (_BSST.Name == null || _BSST.BsstCategoryId == null)
            {
                return false;
            }
            else
            {
                return new BsstDL().AddBSST(_BSST, de);
            }
        }

        public bool UpdateBSST(BSST _BSST, DatabaseEntities de)
        {
            if (_BSST.Name == "" || _BSST.BsstCategoryId == null)
            {
                return false;
            }
            else
            {
                return new BsstDL().UpdateBSST(_BSST, de);
            }
        }

        public bool DeleteBSST(int _id, DatabaseEntities de)
        {
            return new BsstDL().DeleteBSST(_id, de);
        }
    }
}