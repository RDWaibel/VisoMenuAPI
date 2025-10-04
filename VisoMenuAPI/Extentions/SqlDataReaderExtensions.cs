using System;
using System.Data.SqlClient;

namespace VizoMenuAPI.Extensions
{
    public static class SqlDataReaderExtensions
    {
        // String
        public static string GetSafeString(this SqlDataReader rdr, int ordinal) =>
            rdr.IsDBNull(ordinal) ? string.Empty : rdr.GetString(ordinal);

        // Int
        public static int GetSafeInt(this SqlDataReader rdr, int ordinal) =>
            rdr.IsDBNull(ordinal) ? 0 : rdr.GetInt32(ordinal);

        // Guid
        public static Guid GetSafeGuid(this SqlDataReader rdr, int ordinal) =>
            rdr.IsDBNull(ordinal) ? Guid.Empty : rdr.GetGuid(ordinal);

        // DateTime
        public static DateTime GetSafeDateTime(this SqlDataReader rdr, int ordinal) =>
            rdr.IsDBNull(ordinal) ? DateTime.MinValue : rdr.GetDateTime(ordinal);

        // Bool (default false if null)
        public static bool GetSafeBoolFalse(this SqlDataReader rdr, int ordinal) =>
            rdr.IsDBNull(ordinal) ? false : rdr.GetBoolean(ordinal);

        // Bool (default true if null)
        public static bool GetSafeBoolTrue(this SqlDataReader rdr, int ordinal) =>
            rdr.IsDBNull(ordinal) ? true : rdr.GetBoolean(ordinal);
    }
}
