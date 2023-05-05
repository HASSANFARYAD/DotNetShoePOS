using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.DL
{
    public class NewOptionDL
    {
        public List<NewOption> GetActiveNewOptionsList(DatabaseEntities de)
        {
            return de.NewOptions.Where(x => x.IsActive == 1).ToList();
        }
        public NewOption GeteActiveNewOptionById(int _Id, DatabaseEntities de)
        {
            return de.NewOptions.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
        }
        public bool AddNewOption(NewOption _NewOption, DatabaseEntities de)
        {
            try
            {
                de.NewOptions.Add(_NewOption);
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateNewOption(NewOption _NewOption, DatabaseEntities de)
        {
            try
            {
                de.Entry(_NewOption).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteNewOption(int _Id, DatabaseEntities de)
        {
            try
            {
                NewOption NewOption = de.NewOptions.Where(x => x.Id == _Id).FirstOrDefault(x => x.IsActive == 1);
                NewOption.IsActive = 0;
                de.Entry(NewOption).State = System.Data.Entity.EntityState.Modified;
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