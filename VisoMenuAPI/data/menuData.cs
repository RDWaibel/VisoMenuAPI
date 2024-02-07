using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisoMenuAPI.data
{
    public class locationMenuSubMenu
    {
        public int MenuID { get; set; }
        public string DisplayText { get; set; }

        public string AddtionalText1 { get; set; }
        public string AddtionalText2 { get; set; }
        public int menuSort { get; set; }
        //public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string theme_name { get; set; }
        public string SubmenuText { get; set; }
        public int SubMenuSort { get; set; }
        public string? subText1 { get; set; }
        public string? subText2 { get; set; }
        public string? buttonImagePath { get; set; }    

    }
    public class locationMenus
    {
        public int MenuID { get; set; }
        public int SortOrder { get; set; }
        public string DisplayText { get; set; }
        public string? AddtionalText1 { get; set; }
        public string? AddtionalText2 { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string theme_name { get; set; }
    }

    public class menuData
    {
        public int locationID {  get; set; }
        public List<Menus> menus { get; set; } = new List<Menus>();
        
    }

    public class vw_LocationsMenu
    {
        public string LocationID { get; set; }
        public string? LocationName { get; set; }
        public string? MenuText { get; set; }
        public string? menuLineOne { get; set; }
        public string? menuLineTwo { get; set; }
        public string? subMenu { get; set; }
        public string? submenuLineOne { get; set; }
        public string? submenuLineTwo { get; set; }
        public string? ItemName { get; set; }
        public string? description { get; set; }
        public string? price { get; set; }
        public string? imagePath { get; set; }
        public int menuSort { get; set; }
        public int subSort { get; set; }
        public int itemSort { get; set; }
        public int MenuID { get; set; } 
        public string theme_name { get; set; }
        public int MenuItemID { get; set; }
        public int SubmenuID { get; set; }
        public string buttonImagePath { get; set; } 
    }

    public class Menus
    {
        public int MenuID { get; set; }
        public int SortOrder { get; set; }
        public string DisplayText { get; set; }
        public string? AddtionalText1 { get; set; }
        public string? AddtionalText2 { get; set; }
        public int LocationID { get; set; }
        public List<SubMenus> _subMenus { get; set; } = new List<SubMenus>();
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
    public class MenuItems
    {
        public int MenuItemID { get; set; }
        public int SubmenuID { get; set; }
        public int sortOrder { get; set; }
        public string? displayName { get; set; }
        public string? description { get; set; }
        public string? price { get; set; }
        public string? imagePath { get; set; }
        public string subMenuText { get; set; }
        public string HasImage { get; set; } = "N";
    }
}
