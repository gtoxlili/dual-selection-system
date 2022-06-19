using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Tools
{
    /// <summary>
    ///     数据表转换类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbTableConvertor<T> where T : new()
    {
        /// <summary>
        ///     将DataTable转换为实体列表
        /// </summary>
        /// <param name="dt">待转换的DataTable</param>
        /// <returns></returns>
        public List<T> ConvertToList(DataTable dt)
        {
            // 定义集合  
            List<T> list = new List<T>();

            if (dt == null || 0 == dt.Rows.Count) return list;

            // 获得此模型的可写公共属性  
            IEnumerable<PropertyInfo> propertys = new T().GetType().GetProperties().Where(u => u.CanWrite);
            list = ConvertToEntity(dt, propertys);

            return list;
        }

        /// <summary>
        ///     将DataTable的首行转换为实体
        /// </summary>
        /// <param name="dt">待转换的DataTable</param>
        /// <returns></returns>
        public T ConvertToEntity(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return default;

            DataTable dtTable = dt.Clone();
            dtTable.Rows.Add(dt.Rows[0].ItemArray);
            return ConvertToList(dtTable)[0];
        }


        private List<T> ConvertToEntity(DataTable dt, IEnumerable<PropertyInfo> propertys)
        {
            List<T> list = new List<T>();
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T entity = new T();

                //遍历该对象的所有属性  
                foreach (PropertyInfo p in propertys)
                {
                    //将属性名称赋值给临时变量
                    string tmpName = p.Name;

                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (!dt.Columns.Contains(tmpName)) continue;
                    //取值  
                    object value = dr[tmpName];
                    //如果非空，则赋给对象的属性  
                    if (value == DBNull.Value) continue;
                    if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32") value = Convert.ToInt32(value);
                    if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Single") value = Convert.ToSingle(value);
                    p.SetValue(entity, value, null);
                }

                //对象添加到泛型集合中  
                list.Add(entity);
            }

            return list;
        }
    }

    public class Tools
    {
        public static int IsChineseChar(string str)
        {
            return str.Count(ch => ch >= 0x4e00 && ch <= 0x9fbb);
        }

        public static string StringToMd5(string str, int salt)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            StringBuilder ret = new StringBuilder();
            foreach (byte t in b)
            {
                byte chr = (byte)((t + salt) % 256);
                ret.Append(chr.ToString("x2"));
            }


            return ret.ToString();
        }

        public static string AesEncrypt(string text, byte[] keyBytes)
        {
            Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;

            byte[] plainBytes = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes;
            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            return Convert.ToBase64String(cipherBytes);
        }

        public static string AesDecrypt(string text, byte[] keyBytes)
        {
            Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;

            byte[] cipherBytes = Convert.FromBase64String(text);
            byte[] plainBytes;
            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }

        public static DataTable ExecuteDataTable(string fileName)
        {
            string strConn;
            
            if (!Environment.Is64BitProcess)
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
            }
            else
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            }
            
            
            DataTable dt = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();
                DataTable dtSheetName =
                    conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (dtSheetName == null) return dt;
                string tableName = dtSheetName.Rows[0]["TABLE_NAME"].ToString().Trim();

                OleDbDataAdapter adapter = new OleDbDataAdapter($@"select * from [{tableName}]", strConn);
                adapter.Fill(dt);
            }

            return dt;
        }
    }
}