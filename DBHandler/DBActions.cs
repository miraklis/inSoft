using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DBHandler
{
    public class DataProperty : Attribute
    {
    }

    public static class DBActions
    {
        public static bool IsConnected(SqlConnection con)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT 1", con))
            {
                return ((int)cmd.ExecuteScalar() == 1);
            }
        }

        public static string CurrentUser(SqlConnection con)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT SYSTEM_USER", con))
            {
                return (string)cmd.ExecuteScalar();
            }
        }

        public static List<T> ExecReaderListQR<T>(SqlConnection con, string query, Dictionary<string, object> qParams) where T: class, new()
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.Text;
                if (qParams != null)
                {
                    foreach (var p in qParams)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }
                List<T> items = new List<T>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = new T();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (property.GetCustomAttributes(typeof(DataProperty), true).Length > 0)
                        {
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (reader[property.Name] == DBNull.Value) ? null : Convert.ChangeType(reader[property.Name], t);
                            property.SetValue(item, safeValue, null);
                        }
                    }
                    items.Add(item);
                }
                return items;
            }
        }

        public static T ExecReaderQR<T>(SqlConnection con, string query, Dictionary<string, object> qParams) where T : class, new()
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.Text;
                if (qParams != null)
                {
                    foreach (var p in qParams)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var item = new T();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (property.GetCustomAttributes(typeof(DataProperty), true).Length > 0)
                        {
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (reader[property.Name] == DBNull.Value) ? null : Convert.ChangeType(reader[property.Name], t);
                            property.SetValue(item, safeValue, null);
                        }
                    }
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }

        public static List<T> ExecReaderListSP<T>(SqlConnection con, string spName) where T : class, new()
        {
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                List<T> items = new List<T>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = new T();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (property.GetCustomAttributes(typeof(DataProperty), true).Length > 0)
                        {
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (reader[property.Name] == DBNull.Value) ? null : Convert.ChangeType(reader[property.Name], t);
                            property.SetValue(item, safeValue, null);
                        }
                    }
                    items.Add(item);
                }
                return items;
            }
        }

        public static T ExecReaderSP<T>(SqlConnection con, string spName, Dictionary<string, object> spParams) where T : class, new()
        {
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var p in spParams)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                if (reader.Read())
                {
                    var item = new T();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (property.GetCustomAttributes(typeof(DataProperty), true).Length > 0)
                        {
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (reader[property.Name] == DBNull.Value) ? null : Convert.ChangeType(reader[property.Name], t);
                            property.SetValue(item, safeValue, null);
                        }
                    }
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }

        public static int ExecNonReaderQR(SqlConnection con, string cmdName, Dictionary<string, object> cmdParams)
        {
            using (SqlCommand cmd = new SqlCommand(cmdName, con))
            {
                cmd.CommandType = CommandType.Text;
                foreach (var c in cmdParams)
                {
                    object safeValue = c.Value ?? DBNull.Value;
                    cmd.Parameters.AddWithValue(c.Key, safeValue);
                }
                int r = cmd.ExecuteNonQuery();
                return r;
            }
        }

        public static int ExecNonReaderSP(SqlConnection con, string spName, Dictionary<string, object> spParams)
        {
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var p in spParams)
                {
                    object safeValue = p.Value ?? DBNull.Value;
                    cmd.Parameters.AddWithValue(p.Key, safeValue);
                }
                int r = cmd.ExecuteNonQuery();
                return r;
            }
        }

        public static object ExecNonReaderWithReturnSP(SqlConnection con, string spName, Dictionary<string, object> inParams, Dictionary<string, object> outParams)
        {
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var p in inParams)
                {
                    object safeValue = p.Value?? DBNull.Value;
                    cmd.Parameters.AddWithValue(p.Key, safeValue);
                }
                foreach (var p in outParams)
                {
                    object safeValue = p.Value ?? DBNull.Value;
                    cmd.Parameters.AddWithValue(p.Key, safeValue).Direction = ParameterDirection.Output;
                }
                cmd.ExecuteNonQuery();
                return cmd.Parameters[cmd.Parameters.Count - 1].Value;
            }
        }

    }
}
