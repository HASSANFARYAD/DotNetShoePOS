using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.DL
{
    public class InventoryDL
    {
        public List<Inventory> GetActiveInventoriesList(DatabaseEntities de)
        {
            return de.Inventories.Where(x => x.IsActive == 1).ToList();
        }
        public Inventory GeteActiveInventoryById(int _Id, DatabaseEntities de)
        {
            return de.Inventories.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
        }
        public bool AddInventory(Inventory _Inventory, DatabaseEntities de)
        {
            try
            {
                de.Inventories.Add(_Inventory);
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateInventory(Inventory _Inventory, DatabaseEntities de)
        {
            try
            {
                de.Entry(_Inventory).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteInventory(int _Id, DatabaseEntities de)
        {
            try
            {
                Inventory Inventory = de.Inventories.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
                Inventory.IsActive = 0;
                de.Entry(Inventory).State = System.Data.Entity.EntityState.Modified;
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