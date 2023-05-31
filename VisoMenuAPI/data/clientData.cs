using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisoMenuAPI.data
{
    public class clientData
    {
        public Clients client { get; set; } = new Clients();
        public List<Contacts> contacts { get; set; } = new List<Contacts>();
        public List<Locations> locations { get; set; } = new List<Locations>();


    }
    public class Clients
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public int? primaryLocationID { get; set; }
    }
    public class Contacts
    {
        public int ContactID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int LocationID { get; set; }
        public string? LocationName { get; set; }
        public int? ClientID { get; set; }
    }
    public class Locations
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }

        public int ClientID { get; set; }
        public int ContactID { get; set; }
        public string Address { get; set; }
        public string? Suite { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string ZipCode { get; set; }
        public string Telephone { get; set; }
    }

    public class Contact_Us
    {
        public string ContactName { get; set; }
        public string email { get; set; }
        public string message { get; set; }
        public string LocationID { get; set; }
    }
}
