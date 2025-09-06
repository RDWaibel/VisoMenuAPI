using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisoMenuAPI.data
{
    public class SubMenuModels
    {
    }
    public class SubMenus
    {
        public int SubmenuID { get; set; }
        public int MenuID { get; set; }
        public int SortOrder { get; set; }
        public string SubmenuText { get; set; }
        public string? buttonImagePath { get; set; }

        //public List<MenuItems> _menuItems { get; set; } = new List<MenuItems>();
    }
    public class SubMenuDTO
    {
        public int SubmenuID { get; set; }
        public int MenuID { get; set; }
        public int? ParentSubmenuID { get; set; }   // nullable in SQL
        public int SortOrder { get; set; }

        public string SubmenuText { get; set; } = string.Empty;
        public string AdditionalText1 { get; set; } = string.Empty;
        public string AdditionalText2 { get; set; } = string.Empty;

        public string ButtonImagePath { get; set; } = string.Empty; // built in SQL
        public int Tier { get; set; }               // 1 = top under menu
        public string SortPath { get; set; } = "";  // hierarchical sort key, e.g. 0001-0003-0002
    }
}
