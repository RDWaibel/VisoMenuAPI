using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisoMenuAPI.data
{
    public class AttribModels
    {
        public int AttribID { get; set; }
        public string AttribName { get; set;}
    }
    public class AvailAttrib
    {
        public string AttribName { get; set; }
    }

    public class ItemProfileValues
    {
        public int MenuItemID { get; set; }
        public string DisplayName { get; set; }
        public string AttribName { get; set; }
        public string ProfileText { get; set; }
    }
}
