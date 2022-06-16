using Model;

namespace BLL
{
    public class Update
    {
        private readonly DAL.Update _up = new DAL.Update();

        public bool UpdateStuInfo(string newData, string field)
        {
            return _up.UpStudata(stu_base_info.s_id, newData, field);
        }

        public bool UpdateTeaInfo(string newData, string field)
        {
            return _up.UpTeadata(tea_base_info.t_id, newData, field);
        }

        public bool CreateStuPerson(stu_info stuInfo)
        {
            return _up.CreateStuPerson(stuInfo.s_name, stuInfo.major_id, stuInfo.grade, stuInfo.sex, stuInfo.role,
                stuInfo.pwd);
        }

        public bool CreateTeaPerson(tea_info teaInfo)
        {
            return _up.CreateTeaPerson(teaInfo.t_name, teaInfo.major_id, teaInfo.grade, teaInfo.sex, teaInfo.pwd,
                teaInfo.direction);
        }

        public bool UpdateGidInfo(int[] sid, string gid)
        {
            if (!_up.CleanUserGid(gid))
                return false;
            for (int i = 0; i < sid.Length; i++)
            {
                if (i != sid.Length - 1)
                    if (!_up.UpStudata(sid[i], "2", "role"))
                        return false;
                if (!_up.UpStudata(sid[i], gid, "g_id"))
                    return false;
            }

            return true;
        }

        public bool CleanOnlyUserGid(string sid)
        {
            return _up.CleanOnlyUserGid(sid);
        }

        public bool CleanUserGid(string gid)
        {
            return _up.CleanUserGid(gid) && _up.CloseGroup(gid);
        }

        public bool CloseTeaUser(string tid)
        {
            return _up.CloseTeaUser(tid);
        }

        public bool CloseStuUser(string sid)
        {
            return _up.CloseStuUser(sid);
        }

        public bool UpdateChooseTeaInfo(int[] tid, string gid)
        {
            return _up.SetGroupChoose(gid, tid);
        }

        public bool UpdateChooseGroupInfo(int[] gidArr, string tid)
        {
            return _up.UpdateChooseGroupInfo(tid, gidArr);
        }

        public bool UpdateGroupInfo(string gid, string name, string info)
        {
            return _up.UpdateGroupInfo(gid, name, info);
        }

        public string CreateGroup()
        {
            int gid = _up.CreateGroup(stu_base_info.major_id, stu_base_info.grade);
            stu_base_info.g_id = gid;
            return gid.ToString();
        }

        public bool UpdateGroupdata(string value, string field)
        {
            return _up.UpdateGroupdata(stu_base_info.g_id, value, field);
        }

        public bool UpdateSettingInfo(string newData, string field)
        {
            return _up.UpSettingInfo(admin_base_config.admID, newData, field);
        }

        public bool AddRecordtoAutoDisTmp(autodispense_tmp value)
        {
            return _up.AddRecordtoAutoDisTmp(value.t_id, value.g_id, value.g_name, value.t_name, value.score);
        }

        public bool DispenseOkAudit(int tid)
        {
            return _up.DispenseOkAudit(tid);
        }
    }
}