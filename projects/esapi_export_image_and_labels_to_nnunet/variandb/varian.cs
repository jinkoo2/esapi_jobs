using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace variandb
{
    public class varian : db
    {
        public varian(string server, string uid, string pw)
            : base(server, "VARIAN", uid, pw)
        {
        }

        public string get_PatientSer_by_PatientId(string PatientId)
        {
            String sql = String.Format("SELECT PatientSer FROM Patient WHERE PatientId='{0}'", PatientId);
            SqlDataReader dr = this.exe_sql_cmd(sql);

            if (dr.Read())
            {
                Int64 id = (Int64)dr[0];
                string ret = String.Format("{0}", id);
                dr.Close();
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<string> get_LastName_FirstName_PatientiD()
        {


            String sql = "SELECT LastName, FirstName, PatientId FROM Patient "
                        + "where PatientId is not null "
                        + "and LastName is not null "
                        + "and FirstName is not null "
                        + " ORDER BY LastName";

            SqlDataReader dr = this.exe_sql_cmd(sql);
            List<string> list = new List<string>();
            while (dr.Read())
            {
                string v0 = (string)dr[0];
                string v1 = (string)dr[1];
                string v2 = (string)dr[2];
                list.Add(string.Format("{0}{3}{1}{3}{2}", v0, v1, v2, this.deliminator));
            }
            return list;
        }



        // image id is not unique
        public string get_ImageSer_by_ImageId_SeriesId_PatientSer(string ImageId, string SeriesId, string PatientSer)
        {
            String sql = "SELECT Image.ImageSer FROM Image, Series where (Image.SeriesSer = Series.SeriesSer) AND (Image.PatientSer = @PatientSer) AND (Series.SeriesId = @SeriesId) AND (Image.ImageId = @ImageId)";

            //String sql = String.Format("SELECT ImageSer FROM Image WHERE ImageId=@ImageId AND PatientSer=@PatientSer", ImageId, PatientSer);
            SqlCommand cmd = new SqlCommand(sql, this.get_sqlconnection());
            cmd.Parameters.Add("@ImageId", System.Data.SqlDbType.NVarChar);
            cmd.Parameters["@ImageId"].Value = ImageId;
            cmd.Parameters.Add("@SeriesId", System.Data.SqlDbType.NVarChar);
            cmd.Parameters["@SeriesId"].Value = SeriesId;
            cmd.Parameters.Add("@PatientSer", System.Data.SqlDbType.BigInt);
            cmd.Parameters["@PatientSer"].Value = PatientSer;

            SqlDataReader dr = cmd.ExecuteReader();

            List<string> list = new List<string>();
            while (dr.Read())
            {
                Int64 id = (Int64)dr[0];
                string ret = String.Format("{0}", id);
                list.Add(ret);
            }

            if (list.Count == 0)
                return null;
            else if (list.Count > 1)
                throw new Exception("Return value is not unique!");
            else
                return list[0];

        }

        public string get_CourseSer_by_CourseId_PatientSer(string CourseId, string PatientSer)
        {
            String sql = String.Format("SELECT CourseSer FROM Course WHERE CourseId=@CourseId AND PatientSer=@PatientSer");
            SqlCommand cmd = new SqlCommand(sql, this.get_sqlconnection());
            cmd.Parameters.Add("@CourseId", System.Data.SqlDbType.NVarChar);
            cmd.Parameters["@CourseId"].Value = CourseId;
            cmd.Parameters.Add("@PatientSer", System.Data.SqlDbType.BigInt);
            cmd.Parameters["@PatientSer"].Value = PatientSer;

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                Int64 id = (Int64)dr[0];
                string ret = String.Format("{0}", id);
                dr.Close();
                return ret;
            }
            else
            {
                return null;
            }
        }

        public string get_StructureSer_by_StructureId_StructureSetSer(string StructureId, string StructureSetSer)
        {
            String sql = String.Format("SELECT StructureSer FROM Structure WHERE StructureId=@StructureId AND StructureSetSer=@StructureSetSer");
            SqlCommand cmd = new SqlCommand(sql, this.get_sqlconnection());
            cmd.Parameters.Add("@StructureId", System.Data.SqlDbType.NVarChar);
            cmd.Parameters["@StructureId"].Value = StructureId;
            cmd.Parameters.Add("@StructureSetSer", System.Data.SqlDbType.BigInt);
            cmd.Parameters["@StructureSetSer"].Value = StructureSetSer;

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                Int64 id = (Int64)dr[0];
                string ret = String.Format("{0}", id);
                dr.Close();
                return ret;
            }
            else
            {
                return null;
            }
        }

        public string get_PlanSetupSer_by_PlanSetupId_CourseSer(string PlanSetupId, string CourseSer)
        {
            String sql = String.Format("SELECT PlanSetupSer FROM PlanSetup WHERE PlanSetupId=@PlanSetupId AND CourseSer=@CourseSer", PlanSetupId, CourseSer);
            SqlCommand cmd = new SqlCommand(sql, this.get_sqlconnection());
            cmd.Parameters.Add("@PlanSetupId", System.Data.SqlDbType.NVarChar);
            cmd.Parameters["@PlanSetupId"].Value = PlanSetupId;
            cmd.Parameters.Add("@CourseSer", System.Data.SqlDbType.BigInt);
            cmd.Parameters["@CourseSer"].Value = CourseSer;
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                Int64 id = (Int64)dr[0];
                string ret = String.Format("{0}", id);
                dr.Close();
                return ret;
            }
            else
            {
                return null;
            }
        }

        public string get_StructureSetSer_by_StructureSetUID(string StructureSetUID)
        {
            String sql = String.Format("SELECT StructureSetSer FROM StructureSet WHERE StructureSetUID='{0}'", StructureSetUID);
            SqlDataReader dr = this.exe_sql_cmd(sql);

            if (dr.Read())
            {
                Int64 id = (Int64)dr[0];
                string ret = String.Format("{0}", id);
                dr.Close();
                return ret;
            }
            else
            {
                return null;
            }
        }


        public string get_ImageSer_by_StructureSetUID(string StructureSetUID)
        {
            String sql = String.Format("SELECT ImageSer FROM StructureSet WHERE StructureSetUID='{0}'", StructureSetUID);
            SqlDataReader dr = this.exe_sql_cmd(sql);

            if (dr.Read())
            {
                Int64 id = (Int64)dr[0];
                string ret = String.Format("{0}", id);
                dr.Close();
                return ret;
            }
            else
            {
                return null;
            }
        }


        public List<string> get_UserId()
        {
            String sql = "SELECT [UserId] FROM [VARIAN].[dbo].[AppUser] order by UserId";
            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();

            while (dr.Read())
            {
                string uid = (string)dr[0];
                list.Add(uid);
            }

            return list;
        }

        public List<string> get_AliasName()
        {
            String sql = String.Format("SELECT AliasName FROM Staff  ORDER BY AliasName;");
            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();

            while (dr.Read())
            {
                string uid = (string)dr[0];
                list.Add(uid);
            }

            return list;
        }

        public List<Int64> get_ResourceSer_by_AliasName(string AliasName)
        {
            String sql = String.Format("SELECT AliasName FROM Staff WHERE AliasName={0};", AliasName);
            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<Int64> list = new List<Int64>();

            while (dr.Read())
            {
                Int64 value = (Int64)dr[0];
                list.Add(value);
            }

            return list;
        }

        public List<string> get_Doctor_Lastname_FirstName_MiddleName_by_DoctorId(string doctorId)
        {
            String sql = string.Format("SELECT LastName, FirstName, MiddleName FROM Doctor WHERE DoctorId='{0}'", doctorId);
            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();

            string[] s = new string[3];

            while (dr.Read())
            {

                // last 
                for (int i = 0; i < 3; i++)
                {
                    if (!dr.IsDBNull(i))
                        s[i] = (string)dr[i];
                    else
                        s[i] = "";
                }

                list.Add(
                    s[0] + this.deliminator
                    + s[1] + this.deliminator
                    + s[2]);
            }

            return list;
        }

        public List<string> get_PlanApprovalHistoryList(string PlanSer)
        {
            return get_Status_StatusUserName_DataTime_from_Approval_by_TypeSer(PlanSer);
        }

        // TypeSer is the object primary key
        // if TypeSer is PlanSetupSer, the result should be plan approval history
        public List<string> get_Status_StatusUserName_DataTime_from_Approval_by_TypeSer(string TypeSer)
        {
            String sql = string.Format("SELECT Status, StatusUserName, StatusDate FROM Approval WHERE TypeSer='{0}' ORDER BY StatusDate DESC", TypeSer);
            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();
            string[] s = new string[3];

            while (dr.Read())
            {
                int i = 0;
                s[i] = "";
                if (!dr.IsDBNull(i))
                    s[i] = (string)dr[i];

                i = 1;
                s[i] = "";
                if (!dr.IsDBNull(i))
                    s[i] = (string)dr[i];

                i = 2;
                s[i] = "";
                if (!dr.IsDBNull(i))
                {
                    DateTime d = ((DateTime)dr[i]);
                    //s[i] = d.ToShortDateString() + this.deliminator + d.ToShortTimeString();
                    s[i] = d.ToString();
                }

                list.Add(
                    s[0] + this.deliminator
                    + s[1] + this.deliminator
                    + s[2]);
            }

            return list;
        }

    }
}
