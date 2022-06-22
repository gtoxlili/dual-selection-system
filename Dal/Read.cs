using System;
using System.Data;
using System.Data.SQLite;
using System.Text;
using Tools;

namespace Dal
{
    public class Read
    {
        public DataTable CheckAcc(string username, string password, string dbname)
        {
            string idname = dbname == "tea_info" ? "t_id" : "s_id";

            string sql = "Select * from " + dbname + " where " + idname + " = @username and pwd = @password limit 1";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@username", username),
                new SQLiteParameter("@password", password)
            };
            try
            {
                DataTable dt = SqLiteHelper.ExecuteDataTable(sql, CommandType.Text, parameters);
                return dt.Rows.Count > 0 ? dt : null;
            }
            catch
            {
                return null;
            }
        }

        public DataTable GetEntityValue(string extra, string dbname)
        {
            string sql = "Select * from " + dbname;

            if (extra != "")
                sql = sql + " where " + extra;

            sql += " limit 1";

            try
            {
                DataTable dt = SqLiteHelper.ExecuteDataTable(sql);
                return dt.Rows.Count > 0 ? dt : null;
            }
            catch
            {
                return null;
            }
        }

        public DataTable GetApprovedList()
        {
            const string sql = "SELECT t_name,g_name,group_concat(s_name) as nameArr FROM autodispense_tmp inner join stu_info using(g_id) GROUP BY g_id HAVING state = 1";

            try
            {
                DataTable dt = SqLiteHelper.ExecuteDataTable(sql);
                return dt.Rows.Count > 0 ? dt : null;
            }
            catch
            {
                return null;
            }
        }

        public DataSet GetAutoDispenseDt()
        {
            const string strSql1 = "SELECT * FROM group_info WHERE t_id IS NULL AND ( c1 != 0 OR c2 != 0 OR c3 != 0 )";
            DataTable groupChoose = SqLiteHelper.ExecuteDataTable(strSql1);
            const string strSql2 =
                "SELECT * FROM tea_choose INNER JOIN ( SELECT t_name, t_id FROM tea_info ) teainfo ON teainfo.t_id = tea_choose.tid WHERE g1 = - 1 AND g2 = - 1 AND g3 = - 1 AND g4 = - 1 AND g5 = - 1 AND ( c1 != 0 OR c2 != 0 OR c3 != 0 OR c4 != 0 OR c5 != 0 )";
            DataTable teaChoose = SqLiteHelper.ExecuteDataTable(strSql2);

            DataTable dt = new DataTable
            {
                Columns =
                {
                    new DataColumn("t_name"),
                    new DataColumn("t_id", typeof(int)),
                    new DataColumn("g_name"),
                    new DataColumn("g_id", typeof(int)),
                    new DataColumn("g_c1", typeof(int)),
                    new DataColumn("g_c2", typeof(int)),
                    new DataColumn("g_c3", typeof(int)),
                    new DataColumn("t_c1", typeof(int)),
                    new DataColumn("t_c2", typeof(int)),
                    new DataColumn("t_c3", typeof(int)),
                    new DataColumn("t_c4", typeof(int)),
                    new DataColumn("t_c5", typeof(int)),
                    new DataColumn("assigned", typeof(int)),
                    new DataColumn("score", typeof(float))
                }
            };

            foreach (DataRow grow in groupChoose.Rows)
            foreach (DataRow trow in teaChoose.Rows)
            {
                DataRow newRow = dt.NewRow();

                newRow["t_name"] = trow["t_name"];
                newRow["t_id"] = trow["t_id"];
                newRow["g_name"] = grow["g_name"];
                newRow["g_id"] = grow["g_id"];
                newRow["g_c1"] = Convert.ToInt32(grow["c1"]) == Convert.ToInt32(trow["t_id"]) ? 1 : 0;
                newRow["g_c2"] = Convert.ToInt32(grow["c2"]) == Convert.ToInt32(trow["t_id"]) ? 1 : 0;
                newRow["g_c3"] = Convert.ToInt32(grow["c3"]) == Convert.ToInt32(trow["t_id"]) ? 1 : 0;
                newRow["t_c1"] = Convert.ToInt32(trow["c1"]) == Convert.ToInt32(grow["g_id"]) ? 1 : 0;
                newRow["t_c2"] = Convert.ToInt32(trow["c2"]) == Convert.ToInt32(grow["g_id"]) ? 1 : 0;
                newRow["t_c3"] = Convert.ToInt32(trow["c3"]) == Convert.ToInt32(grow["g_id"]) ? 1 : 0;
                newRow["t_c4"] = Convert.ToInt32(trow["c4"]) == Convert.ToInt32(grow["g_id"]) ? 1 : 0;
                newRow["t_c5"] = Convert.ToInt32(trow["c5"]) == Convert.ToInt32(grow["g_id"]) ? 1 : 0;
                newRow["assigned"] = 0;

                int stumark = Convert.ToInt32(newRow["g_c1"] + newRow["g_c2"].ToString() + newRow["g_c3"], 2);
                int teamark =
                    Convert.ToInt32(
                        newRow["t_c1"] + newRow["t_c2"].ToString() + newRow["t_c3"] + newRow["t_c4"] + newRow["t_c5"],
                        2);

                newRow["score"] = stumark + teamark * 1.1;

                dt.Rows.Add(newRow);
            }

            // use score to sort
            dt.DefaultView.RowFilter = "score > 0.0";
            dt.DefaultView.Sort = "score desc";
            dt = dt.DefaultView.ToTable();

            return new DataSet
            {
                Tables =
                {
                    dt, groupChoose, teaChoose
                }
            };
        }

        public DataTable GetDbContent(string db, int grade, int majorId, string str, string extra, string field)
        {
            string sql = "Select " + field + " from [" + db + "] where 1=2";
            DataColumnCollection codename = SqLiteHelper.ExecuteDataTable(sql).Columns;

            StringBuilder strSql = new StringBuilder();

            strSql.Append("Select * from [" + db + "] where 1=1 ");

            if (grade != 0)
                strSql.Append("and grade=@a ");

            if (majorId != 0)
                strSql.Append("and major_id=@b ");

            if (str != "")
            {
                strSql.Append("and (");
                for (int i = 0; i < codename.Count; i++)
                {
                    if (codename[i].ColumnName == "pwd")
                    {
                        if (i == codename.Count - 1)
                            strSql.Append("1=2");

                        continue;
                    }

                    strSql.Append(codename[i].ColumnName + " like '%" + str + "%' ");

                    if (i != codename.Count - 1)
                        strSql.Append("or ");
                }

                strSql.Append(")");
            }

            if (extra != "")
                strSql.Append("and " + extra);

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@a", grade + 2018),
                new SQLiteParameter("@b", majorId - 1)
            };

            try
            {
                DataTable dt = SqLiteHelper.ExecuteDataTable(strSql.ToString(), CommandType.Text, parameters);
                return dt.Rows.Count > 0 ? dt : null;
            }
            catch
            {
                return null;
            }
        }

        public object GetOnlyContent(string db, string extra, string field)
        {
            string sql = "Select " + field + " from " + db;

            if (extra != "")
                sql = sql + " where " + extra;

            try
            {
                return SqLiteHelper.ExecuteScalar(sql);
            }
            catch
            {
                return default;
            }
        }

        public object GetConditionContent(string db, string extra, string field, int grade, int majorId, string str)
        {
            string sql = "Select * from [" + db + "] where 1=2";
            DataColumnCollection codename = SqLiteHelper.ExecuteDataTable(sql).Columns;

            StringBuilder strSql = new StringBuilder();

            strSql.Append("Select " + field + " from [" + db + "] where 1=1 ");

            if (grade != 0)
                strSql.Append("and grade=@a ");

            if (majorId != 0)
                strSql.Append("and major_id=@b ");

            if (str != "")
            {
                strSql.Append("and (");
                for (int i = 0; i < codename.Count; i++)
                {
                    if (codename[i].ColumnName == "pwd")
                    {
                        if (i == codename.Count - 1)
                            strSql.Append("1=2");

                        continue;
                    }

                    strSql.Append(codename[i].ColumnName + " like '%" + str + "%' ");

                    if (i != codename.Count - 1)
                        strSql.Append("or ");
                }

                strSql.Append(")");
            }

            if (extra != "")
                strSql.Append("and " + extra);

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@a", grade + 2018),
                new SQLiteParameter("@b", majorId - 1)
            };

            try
            {
                return SqLiteHelper.ExecuteScalar(strSql.ToString(), CommandType.Text, parameters);
            }
            catch
            {
                return default;
            }
        }

        public DataTable GetGroupInfo(int grade, int majorId, string str, string extra)
        {
            string[] fieldArr =
            {
                "g_id", "g_no", "g.g_name", "g.grade", "g.major_id", "g.project_name", "g.project_info",
                "group_concat(s_name)"
            };

            StringBuilder strSql = new StringBuilder();

            strSql.Append("SELECT " + string.Join(",", fieldArr) +
                          " as nameArr FROM group_info g INNER JOIN stu_info USING ( g_id ) GROUP BY g_id HAVING 1=1 ");

            if (grade != 0)
                strSql.Append("and g.grade=@a ");

            if (majorId != 0)
                strSql.Append("and g.major_id=@b ");

            if (str != "")
            {
                strSql.Append("and (");
                for (int i = 0; i < fieldArr.Length; i++)
                {
                    strSql.Append(fieldArr[i] + " like '%" + str + "%' ");

                    if (i != fieldArr.Length - 1)
                        strSql.Append("or ");
                }

                strSql.Append(")");
            }

            if (extra != "")
                strSql.Append("and " + extra);

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@a", grade + 2018),
                new SQLiteParameter("@b", majorId - 1)
            };

            try
            {
                DataTable dt = SqLiteHelper.ExecuteDataTable(strSql.ToString(), CommandType.Text, parameters);
                return dt.Rows.Count > 0 ? dt : null;
            }
            catch
            {
                return null;
            }
        }
    }
}