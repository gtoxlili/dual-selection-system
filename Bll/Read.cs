using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Dal;
using Model;
using Tools;

namespace Bll
{
    public class Read<T> where T : new()
    {
        private readonly DbTableConvertor<T> _convertor = new DbTableConvertor<T>();
        private readonly Read _fi = new Read();

        public List<T> GetExeclContent(string path, int grade, int majorId)
        {
            string etr = "";
            bool haveRole = false;
            bool havePwd = false;
            bool haveDirection = false;
            int seq = 0;

            DataTable dt = Tools.Tools.ExecuteDataTable(path);

            // 获取符合 grade 和 majorId 的行
            string strSql = "";

            if (grade != 0)
                strSql += "grade = " + (grade + 2018);

            if (grade != 0 && majorId != 0)
                strSql += " and ";

            if (majorId != 0)
                strSql += "major_id = " + (majorId - 1);

            DataRow[] row = dt.Select(strSql);
            if (row.Length == 0)
                return default;
            dt = row.CopyToDataTable();

            if (typeof(T) == typeof(stu_info))
            {
                if (dt.Columns["s_name"] == null && dt.Columns["sex"] == null && dt.Columns["grade"] == null &&
                    dt.Columns["major_id"] == null)
                    return default;
                etr = "s";
                seq = Convert.ToInt32(GetOnlyContent("name = 'stu_info'", "seq")) + 1;
            }
            else if (typeof(T) == typeof(tea_info))
            {
                if (dt.Columns["t_name"] == null && dt.Columns["sex"] == null && dt.Columns["grade"] == null &&
                    dt.Columns["major_id"] == null)
                    return default;
                etr = "t";
                seq = Convert.ToInt32(GetOnlyContent("name = 'tea_info'", "seq")) + 1;
            }

            if (dt.Columns[etr + "_no"] == null) dt.Columns.Add(etr + "_no");

            if (dt.Columns["pwd"] == null)
                dt.Columns.Add("pwd");
            else
                havePwd = true;

            if (dt.Columns["role"] == null && typeof(T) == typeof(stu_info))
                dt.Columns.Add("role");
            else
                haveRole = true;

            if (dt.Columns["direction"] == null && typeof(T) == typeof(tea_info))
                dt.Columns.Add("direction");
            else
                haveDirection = true;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][etr + "_no"] = seq + i;
                if (!havePwd)
                    dt.Rows[i]["pwd"] = GetMd5(admin_config.Instance.defaultPwd, seq + i);
                else
                    dt.Rows[i]["pwd"] = GetMd5(dt.Rows[i]["pwd"].ToString(), seq + i);
                if (!haveRole)
                    dt.Rows[i]["role"] = "0";
                if (!haveDirection)
                    dt.Rows[i]["direction"] = "";
            }

            return _convertor.ConvertToList(dt);
        }

        public List<T> GetDbContent(string extra, string field = "*")
        {
            DataTable dt = _fi.GetDbContent(typeof(T).Name.ToLower(), 0, 0, "", extra, field);
            return _convertor.ConvertToList(dt);
        }

        public List<T> GetDbContent(int grade, int majorId, string str, string extra, string field = "*")
        {
            DataTable dt = _fi.GetDbContent(typeof(T).Name.ToLower(), grade, majorId, str, extra, field);
            return _convertor.ConvertToList(dt);
        }

        public List<T> GetGroupInfo(int grade, int majorId, string str, string extra)
        {
            DataTable dt = _fi.GetGroupInfo(grade, majorId, str, extra);
            return _convertor.ConvertToList(dt);
        }

        public List<T> GetGroupInfo(string extra)
        {
            DataTable dt = _fi.GetGroupInfo(0, 0, "", extra);
            return _convertor.ConvertToList(dt);
        }

        public T CheckAcc(string username, string password)
        {
            DataTable dt = _fi.CheckAcc(username, GetMd5(password, Convert.ToInt32(username.Substring(4, 3))),
                typeof(T).Name.ToLower());
            return _convertor.ConvertToEntity(dt);
        }

        public T CheckAdmAcc(string username, string password)
        {
            string extra = $@"admUser = '{username.Replace("'", "''")}' and admPwd = {password.Replace("'", "''")}";
            DataTable dt = _fi.GetEntityValue(extra, typeof(T).Name.ToLower());
            return _convertor.ConvertToEntity(dt);
        }

        public T GetEntityValue(string extra)
        {
            DataTable dt = _fi.GetEntityValue(extra, typeof(T).Name.ToLower());
            return _convertor.ConvertToEntity(dt);
        }

        public object GetOnlyContent(string extra, string field)
        {
            return _fi.GetOnlyContent(typeof(T).Name.ToLower(), extra, field);
        }

        public object GetConditionContent(string extra, string field, int grade, int majorId, string str)
        {
            return _fi.GetConditionContent(typeof(T).Name.ToLower(), extra, field, grade, majorId, str);
        }

        public void EntryInfo(T info)
        {
            PropertyInfo[] propertys = info.GetType().GetProperties();
            T instance = (T)info.GetType().GetProperty("Instance")?.GetValue(info, null);

            foreach (PropertyInfo p in propertys)
            {
                string tmpName = p.Name;
                if (tmpName == "Instance")
                    continue;
                object tmpValue = p.GetValue(info, null);

                p.SetValue(instance, tmpValue, null);
            }
        }


        public string GetMd5(string str)
        {
            return GetMd5(str, stu_info.Instance.s_no);
        }

        public string GetMd5(string str, int salt)
        {
            return Tools.Tools.StringToMd5(str, salt);
        }

        public DataSet GetAutoDispenseDt()
        {
            return _fi.GetAutoDispenseDt();
        }
    }
}