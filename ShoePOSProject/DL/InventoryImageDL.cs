using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.DL
{
    public class InventoryImageDL
    {
        public List<InventoryImage> GetActiveInventoryImagesList(DatabaseEntities de)
        {
            return de.InventoryImages.Where(x => x.IsActive == 1).ToList();
        }
        public InventoryImage GeteActiveInventoryImageById(int _Id, DatabaseEntities de)
        {
            return de.InventoryImages.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
        }
        public bool AddInventoryImage(InventoryImage _InventoryImage, DatabaseEntities de)
        {
            try
            {
                de.InventoryImages.Add(_InventoryImage);
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateInventoryImage(InventoryImage _InventoryImage, DatabaseEntities de)
        {
            try
            {
                de.Entry(_InventoryImage).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteInventoryImage(int _Id, DatabaseEntities de)
        {
            try
            {
                InventoryImage InventoryImage = de.InventoryImages.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
                InventoryImage.IsActive = 0;
                de.Entry(InventoryImage).State = System.Data.Entity.EntityState.Modified;
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