using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisoMenuAPI.data
{
    public class arfData
    {
        public class arfMessage
        {
            public string? contactName { get; set; }
            public string emailAddress { get; set; }
            public string message { get; set; }

        }
        string cnSQL = System.Environment.GetEnvironmentVariable("VisoMenuData");

        public async Task<bool> Insert_Arf_Message(arfMessage _Message, ILogger logger)
        {
            if (_Message == null)
            {
                return true;
            }

            SqlCommand cmd = new SqlCommand();
            bool resultOfCall = false;
            logger.LogInformation("Inserting message from ARF site");
            using (SqlConnection conn = new SqlConnection(cnSQL))
            {
                string sSQL = $"arf.InsertMessage";
                conn.Open();
                logger.LogInformation("SQL connection open");
                cmd = new SqlCommand(sSQL,conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ContactName", _Message.contactName);
                cmd.Parameters.AddWithValue("@EmailAddress", _Message.emailAddress);
                cmd.Parameters.AddWithValue("@Message", _Message.message);
                try
                {
                    logger.LogInformation($"Sending to SQL {sSQL}");
                    await cmd.ExecuteNonQueryAsync();
                    resultOfCall = true;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Insert_Arf_Message");
                    resultOfCall = false;
                }
                conn.Close();

            }
            return resultOfCall;
        }
    }
}
