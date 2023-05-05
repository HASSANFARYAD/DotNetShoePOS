using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    public class NewOptionBL
    {
        public List<NewOption> GetActiveNewOptionsList(DatabaseEntities de)
        {
            return new NewOptionDL().GetActiveNewOptionsList(de);
        }

        public NewOption GetActiveNewOptionById(int _Id, DatabaseEntities de)
        {
            return new NewOptionDL().GeteActiveNewOptionById(_Id, de);
        }

        public bool AddNewOption(NewOption _NewOption, DatabaseEntities de)
        {
            if (_NewOption.Name == null || _NewOption.Price == null)
            {
                return false;
            }
            else
            {
                return new NewOptionDL().AddNewOption(_NewOption, de);
            }
        }

        public bool UpdateNewOption(NewOption _NewOption, DatabaseEntities de)
        {
            if (_NewOption.Name == "" || _NewOption.Price == "")
            {
                return false;
            }
            else
            {
                return new NewOptionDL().UpdateNewOption(_NewOption, de);
            }
        }

        public bool DeleteNewOption(int _id, DatabaseEntities de)
        {
            return new NewOptionDL().DeleteNewOption(_id, de);
        }
    }
}