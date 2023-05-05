using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    public class InventoryBL
    {
        public List<Inventory> GetActiveInventoriesList(DatabaseEntities de)
        {
            return new InventoryDL().GetActiveInventoriesList(de);
        }

        public Inventory GetActiveInventoryById(int _Id, DatabaseEntities de)
        {
            return new InventoryDL().GeteActiveInventoryById(_Id, de);
        }

        public bool AddInventory(Inventory _Inventory, DatabaseEntities de)
        {
            if (_Inventory.BarcodeNo == null || _Inventory.IsActive == null || _Inventory.CreatedAt== null)
            {
                return false;
            }
            else
            {
                return new InventoryDL().AddInventory(_Inventory, de);
            }
        }

        public bool UpdateInventory(Inventory _Inventory, DatabaseEntities de)
        {
            if (_Inventory.BarcodeNo == "" || _Inventory.IsActive == null || _Inventory.CreatedAt == null)
            {
                return false;
            }
            else
            {
                return new InventoryDL().UpdateInventory(_Inventory, de);
            }
        }

        public bool DeleteInventory(int _id, DatabaseEntities de)
        {
            return new InventoryDL().DeleteInventory(_id, de);
        }
    }
}