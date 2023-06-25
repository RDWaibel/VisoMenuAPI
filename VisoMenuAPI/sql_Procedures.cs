﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisoMenuAPI.data;

namespace VisoMenuAPI
{
    public class sql_Procedures
    {
        string cnSQL = System.Environment.GetEnvironmentVariable("VisoMenuData");
        public async Task<clientData> GetClientData(int clientID)
        {
            string sql = $"Select * from Clients WHERE ClientID = {clientID}";
            clientData _client = null;
            using (SqlConnection conn = new SqlConnection(cnSQL))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                if (rdr.Read())
                {
                    _client = new clientData();
                    _client.client.ClientID = rdr.GetInt32(0);
                    _client.client.ClientName = rdr.GetString(1);
                    _client.client.ClientContactName = rdr.GetString(2);
                    if (rdr[3] != DBNull.Value)
                    {
                        _client.client.ContactEmail = rdr.GetString(3);
                    }
                    if (rdr[4] != DBNull.Value)
                    {
                        _client.client.ContactPhone = rdr.GetString(4);
                    }
                }
                rdr.Close();
                sql = $"Select * From vw_ContactLocation WHERE ClientID = {_client.client.ClientID}";
                cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                rdr = await cmd.ExecuteReaderAsync();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        Contacts _contact = new Contacts();
                        _contact.ContactID = rdr.GetInt32(0);
                        _contact.Name = rdr.GetString(1);
                        _contact.Position = rdr.GetString(2);
                        _contact.Phone = rdr.GetString(3);
                        _contact.Email = rdr.GetString(4);
                        _contact.LocationID = rdr.GetInt32(5);
                        _contact.ClientID = rdr.GetInt32(6);
                        _contact.LocationName = rdr.GetString(7);

                        _client.contacts.Add(_contact);

                    }
                }
                rdr.Close();
                sql = $"Select * From locations WHERE ClientID = {_client.client.ClientID}";
                cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                rdr = await cmd.ExecuteReaderAsync();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        Locations _locations = new Locations();
                        _locations.LocationID = rdr.GetInt32(0);
                        _locations.ClientID = rdr.GetInt32(1);
                        _locations.ContactID = rdr.GetInt32(2);
                        _locations.Address = rdr.GetString(3);
                        if (rdr[4] != DBNull.Value)
                        {
                            _locations.Suite = rdr.GetString(4);
                        }
                        _locations.CityName = rdr.GetString(5);
                        _locations.StateName = rdr.GetString(6);
                        _locations.ZipCode = rdr.GetString(7);
                        if (rdr[8] != DBNull.Value)
                        {
                            _locations.Telephone = rdr.GetString(8);
                        }
                        if (rdr[9] != DBNull.Value)
                        {
                            _locations.LocationName = rdr.GetString(9);
                        }
                        else
                        {
                            _locations.LocationName = "..Needed..";
                        }
                        _client.locations.Add(_locations);
                    }
                }
                rdr.Close();
                cmd.Dispose();
                conn.Close();
            }
            return _client;
        }

        /// <summary>
        /// Return FULL menu for Location
        /// </summary>
        /// <param name="LocationID"></param>
        /// <returns></returns>
        public async Task<List<vw_LocationsMenu>> GetLocationMenu(int LocationID)
        {
            string sql = $"sp_LocationMenu {LocationID}";
            List<vw_LocationsMenu> theMenu = new List<vw_LocationsMenu>();
            using (SqlConnection conn = new SqlConnection(cnSQL))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        vw_LocationsMenu a = new vw_LocationsMenu();
                        a.LocationID = rdr.GetInt32(0);
                        a.LocationName = rdr.GetString(1);
                        a.MenuText = rdr.GetString(2);
                        a.LineOne = rdr.GetString(3);
                        a.LineTwo = rdr.GetString(4);                        
                        a.InnerMenu = rdr.GetString(5);
                        a.ItemName = rdr.GetString(6);
                        a.description = rdr.GetString(7);
                        a.price = rdr.GetString(8);
                        a.imagePath = rdr.GetString(9);
                        a.menuSort = rdr.GetInt32(10);
                        a.subSort = rdr.GetInt32(11);
                        a.itemSort = rdr.GetInt32(12);
                        a.theme_name = rdr.GetString(13);
                        theMenu.Add(a);
                    }
                }
                rdr.Close();
                cmd.Dispose();
                conn.Close();
            }
            return theMenu;
        }

        /// <summary>
        /// returns the top level menues for the location
        /// </summary>
        /// <param name="_locationID"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<List<locationMenus>> rtn_LocationMenus(int _locationID, ILogger logger)
        {
            logger.LogInformation("Running LocationMenus");
            string SQL = $"rtn_LocationMenu";
            List<locationMenus> locMenus = new List<locationMenus>();
            using (SqlConnection conn = new SqlConnection(cnSQL))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationID", _locationID);
                try
                {
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            locationMenus menus = new locationMenus();
                            menus.LocationID = _locationID;
                            menus.LocationName = rdr.GetString(6);
                            menus.MenuID = rdr.GetInt32(0);
                            menus.DisplayText = rdr.GetString(1);
                            menus.AddtionalText1 = rdr.GetString(2);
                            menus.AddtionalText2 = rdr.GetString(3);
                            menus.SortOrder = rdr.GetInt32(4);
                            menus.theme_name = rdr.GetString(5);
                            locMenus.Add(menus);
                        }
                    }
                    rdr.Close();
                    conn.Close();
                    logger.LogInformation("LocationMenus loaded");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "rtn_LocationMenu");
                }
            }
            return locMenus;

        }
        public async Task<List<SubMenus>> rtn_SubMenus(int _menuID)
        {
            string SQL = $"rtn_LocationSubMenu {_menuID}";
            List<SubMenus> theSubMenus = new List<SubMenus>();
            using (SqlConnection conn = new SqlConnection(cnSQL))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL;
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        SubMenus subMenu = new SubMenus();
                        subMenu.SubmenuID = rdr.GetInt32(0);
                        subMenu.MenuID = rdr.GetInt32(1);
                        subMenu.SortOrder = rdr.GetInt32(2);
                        subMenu.SubmenuText = rdr.GetString(3);
                        theSubMenus.Add(subMenu);
                    }
                }
                rdr.Close();
                conn.Close();
            }
            return theSubMenus;
        }
        /// <summary>
        /// Pulls all of the items based on the submenuID
        /// </summary>
        /// <param name="_subMenuID"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<List<MenuItems>> rtn_MenuItems(int _subMenuID, ILogger logger)
        {
            logger.LogInformation("Running rtn_MenuItems");
            string sql = $"rtn_SubMenuItems";
            List<MenuItems> theItems = new List<MenuItems>();
            using (SqlConnection conn = new SqlConnection(cnSQL))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@subMenuID", _subMenuID);
                try
                {
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            MenuItems theItem = new MenuItems();
                            theItem.MenuItemID = rdr.GetInt32(0);
                            theItem.SubmenuID = rdr.GetInt32(1);
                            theItem.sortOrder = rdr.GetInt32(2);
                            theItem.displayName = rdr.GetString(3);
                            theItem.description = rdr.GetString(4);
                            theItem.price = rdr.GetString(5);
                            theItem.imagePath = rdr.GetString(6);
                            theItems.Add(theItem);
                        }
                    }
                    rdr.Close();
                    conn.Close();
                    logger.LogInformation("Menu Items loaded");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "rtn_MenuItems");
                }
            }
            return theItems;
        }

        public async Task<MenuItems> rtn_MenuItem(int _itemID, ILogger logger)
        {
            SqlCommand cmd = new SqlCommand();
            MenuItems item = new MenuItems();
            logger.LogInformation("Looking for the item information");
            using (SqlConnection conn = new SqlConnection(cnSQL))
            {
                await conn.OpenAsync();
                cmd.Connection = conn;
                cmd.CommandText = "GetSingleItem";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ItemID", _itemID);
                try
                {
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        item.MenuItemID = rdr.GetInt32(0);
                        item.SubmenuID = rdr.GetInt32(1);
                        item.sortOrder = rdr.GetInt32(2);
                        item.displayName = rdr.GetString(3);
                        if(rdr[4] == DBNull.Value)
                        {
                            item.description = "";
                        }
                        else
                        {
                            item.description = rdr.GetString(4);
                        }
                        item.price = rdr.GetString(5);
                        item.imagePath = rdr.GetString(6);
                        rdr.Close();
                    }
                    else
                    {
                        logger.LogError($"Item {_itemID} was not found!");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "save_Contact_Us");
                }
                await conn.CloseAsync();
            }
            return item;
        }

        public async Task<bool> save_Contact_Us(Contact_Us contact, ILogger logger)
        {
            SqlCommand cmd = new SqlCommand();
            bool resultOfCall = false;
            logger.LogInformation("saving the contact us information");
            using(SqlConnection conn = new SqlConnection(cnSQL))
            {
                string sSQL = $"Add_ContactUs '{contact.ContactName}', '{contact.email}', '{contact.message}', {contact.LocationID}";
                int locID = int.Parse(contact.LocationID.ToString());
                conn.Open();
                logger.LogInformation("SQL connection open");
                cmd.CommandText = sSQL;
                cmd.Connection = conn;
                try
                {
                    logger.LogInformation($"Sending to SQL {sSQL}");
                    await cmd.ExecuteNonQueryAsync();
                    resultOfCall = true;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "save_Contact_Us");
                    resultOfCall= false;
                }
                conn.Close ();

            }
            return resultOfCall;    
        }
        /// <summary>
        /// Return a simple list of three recommendations
        /// </summary>
        /// <param name="_locID"></param>
        /// <param name="_menuItemID"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<List<MenuItems>> rtn_Recommendations (int _locID, int _menuItemID, ILogger logger)
        {
            logger.LogInformation("Running sp_LocationRecommendations");
            string sql = $"sp_LocationRecommendations";
            List<MenuItems> theItems = new List<MenuItems>();
            using (SqlConnection conn = new SqlConnection(cnSQL))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationID", _locID);
                cmd.Parameters.AddWithValue("@itemID", _menuItemID);
                try
                {
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            MenuItems theItem = new MenuItems();
                            theItem.MenuItemID = rdr.GetInt32(0);
                            theItem.SubmenuID = rdr.GetInt32(1);
                            theItem.sortOrder = rdr.GetInt32(2);
                            theItem.displayName = rdr.GetString(3);
                            theItem.description = rdr.GetString(4);
                            theItem.price = rdr.GetString(5);
                            theItem.imagePath = rdr.GetString(6);
                            theItems.Add(theItem);
                        }
                    }
                    rdr.Close();
                    conn.Close();
                    logger.LogInformation("Recommendations loaded");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "rtn_Recommendations");
                }
            }
            return theItems;
        }
    }
}
