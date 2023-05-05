using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    public class InventoryImageBL
    {
        public List<InventoryImage> GetActiveInventoriesList(DatabaseEntities de)
        {
            return new InventoryImageDL().GetActiveInventoryImagesList(de);
        }

        public InventoryImage GetActiveInventoryImageById(int _Id, DatabaseEntities de)
        {
            return new InventoryImageDL().GeteActiveInventoryImageById(_Id, de);
        }

        public bool AddInventoryImage(InventoryImage _InventoryImage, DatabaseEntities de)
        {
            if (_InventoryImage.ImageName == null || _InventoryImage.ImagePath == null)
            {
                return false;
            }
            else
            {
                return new InventoryImageDL().AddInventoryImage(_InventoryImage, de);
            }
        }

        public bool UpdateInventoryImage(InventoryImage _InventoryImage, DatabaseEntities de)
        {
            if (_InventoryImage.ImageName == "" || _InventoryImage.ImagePath == "")
            {
                return false;
            }
            else
            {
                return new InventoryImageDL().UpdateInventoryImage(_InventoryImage, de);
            }
        }

        public bool DeleteInventoryImage(int _id, DatabaseEntities de)
        {
            return new InventoryImageDL().DeleteInventoryImage(_id, de);
        }
    }
}