using System;
using System.Data;
using System.Data.SQLite;
using Tools;

namespace Dal
{
    public class Update
    {
        public bool UpStudata(int sid, string newData, string field)
        {
            string strSql = "update stu_info set " + field + "=@data where s_id=@sid";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@data", newData),
                new SQLiteParameter("@sid", sid)
            };
            try
            {
                int result = SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool UpTeadata(int tid, string newData, string field)
        {
            string strSql = "update tea_info set " + field + "=@data where t_id=@tid";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@data", newData),
                new SQLiteParameter("@tid", tid)
            };
            try
            {
                int result = SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool UpSettingInfo(string aid, string newData, string field)
        {
            string strSql = "update admin_config set " + field + "=@data where admID = @aid ";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@data", newData),
                new SQLiteParameter("@aid", aid)
            };
            try
            {
                int result = SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateStuPerson(string sName, int majorId, int grade, string sex, int role, string pwd)
        {
            const string strSql =
                "insert into stu_info (s_name,major_id,grade,sex,role,pwd) values(@a, @b, @c, @d, @e,@f)";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@a", sName),
                new SQLiteParameter("@b", majorId),
                new SQLiteParameter("@c", grade),
                new SQLiteParameter("@d", sex),
                new SQLiteParameter("@e", role),
                new SQLiteParameter("@f", pwd)
            };
            try
            {
                int result = SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateTeaPerson(string tName, int majorId, int grade, string sex, string pwd, string direction)
        {
            string strSql =
                "insert into tea_info (t_name,major_id,grade,sex,pwd,direction) values(@a, @b, @c, @d, @e,@f)";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@a", tName),
                new SQLiteParameter("@b", majorId),
                new SQLiteParameter("@c", grade),
                new SQLiteParameter("@d", sex),
                new SQLiteParameter("@e", pwd),
                new SQLiteParameter("@f", direction)
            };
            try
            {
                int result = SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateGroupdata(int gid, string data, string field)
        {
            string strSql = "update group_info set " + field + "=@data where g_id=@gid";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@data", data),
                new SQLiteParameter("@gid", gid)
            };
            try
            {
                int result = SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public int CreateGroup(int majorId, int grade)
        {
            Random rd = new Random();
            int gNo = rd.Next(10000);

            const string strSql =
                "insert or replace into group_info (major_id,grade,g_no,c1,c2,c3) values(@a, @b, @c,0,0,0)";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@a", majorId),
                new SQLiteParameter("@b", grade),
                new SQLiteParameter("@c", gNo)
            };
            try
            {
                int result = SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return result > 0 ? Convert.ToInt32(grade + gNo.ToString("D4") + "03") : default;
            }
            catch
            {
                return default;
            }
        }

        public bool CleanUserGid(string gid)
        {
            const string strSql = "update stu_info set g_id = null where g_id=@gid";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@gid", gid)
            };
            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CloseGroup(string gid)
        {
            const string strSql = "delete from group_info where g_id=@gid";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@gid", gid)
            };
            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CloseStuUser(string sid)
        {
            const string strSql = "delete from stu_info where s_id=@sid";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@sid", sid)
            };
            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CloseTeaUser(string tid)
        {
            const string strSql = "delete from tea_info where t_id=@tid";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@tid", tid)
            };
            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CleanOnlyUserGid(string sid)
        {
            const string strSql = "update stu_info set g_id = null where s_id=@sid";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@sid", sid)
            };
            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetGroupChoose(string gid, int[] tidArr)
        {
            const string strSql = "update group_info set c1 = @c1,c2 = @c2,c3 = @c3 where g_id=@gid";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@c1", tidArr[0]),
                new SQLiteParameter("@c2", tidArr[1]),
                new SQLiteParameter("@c3", tidArr[2]),
                new SQLiteParameter("@gid", gid)
            };

            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateChooseGroupInfo(string tid, int[] gidArr)
        {
            const string strSql =
                "insert or replace into tea_choose(tid,c1,c2,c3,c4,c5) values (@tid,@c1,@c2,@c3,@c4,@c5)";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@c1", gidArr[0]),
                new SQLiteParameter("@c2", gidArr[1]),
                new SQLiteParameter("@c3", gidArr[2]),
                new SQLiteParameter("@c4", gidArr[3]),
                new SQLiteParameter("@c5", gidArr[4]),
                new SQLiteParameter("@tid", tid)
            };

            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateGroupInfo(string gid, string name, string info)
        {
            const string strSql =
                "update group_info set project_name = @project_name,project_info = @project_info where g_id=@gid";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@project_name", name),
                new SQLiteParameter("@project_info", info),
                new SQLiteParameter("@gid", gid)
            };

            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddRecordtoAutoDisTmp(int tid, int gid, string gname, string tname, float score)
        {
            const string strSql =
                "insert or replace into autodispense_tmp(g_id,t_id,t_name,g_name,score, state) values (@gid,@tid,@tname,@gname,@score,0)";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@gid", gid),
                new SQLiteParameter("@tid", tid),
                new SQLiteParameter("@tname", tname),
                new SQLiteParameter("@gname", gname),
                new SQLiteParameter("@score", score)
            };

            try
            {
                SqLiteHelper.ExecuteNonQuery(strSql, CommandType.Text, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DispenseOkAudit(int gid)
        {
            SqLiteHelper.ExecuteNonQuery("UPDATE autodispense_tmp set state = 1 WHERE g_id = " + gid);
            DataRow info = SqLiteHelper.ExecuteDataTable("SELECT * FROM autodispense_tmp WHERE g_id = " + gid).Rows[0];
            int tid = Convert.ToInt32(info["t_id"]);
            int resCount =
                (int)(long)SqLiteHelper.ExecuteScalar("SELECT res_count FROM tea_choose WHERE tid = " + tid);
            SqLiteHelper.ExecuteNonQuery(
                $@"UPDATE tea_choose set res_count = {resCount + 1},g{resCount + 1}={gid} WHERE tid = " + tid);
            SqLiteHelper.ExecuteNonQuery($@"UPDATE group_info set t_id = {tid} WHERE g_id = " + gid);

            return true;
        }

        public bool DispenseDelAudit(int gid)
        {
            try
            {
                SqLiteHelper.ExecuteNonQuery("delete from autodispense_tmp where g_id = " + gid);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}