using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMS.TPS.Common.Model.API;

namespace esapi
{
    public static class variandb_query
    {
        public static string[] get_PatientIds_with_TxApproved_Plan_of_Year(int year)
        {
            string sql = string.Format(@"select DISTINCT pt.PatientId
                                            from 
	                                            PlanSetup ps, 
	                                            Course cs,
	                                            Patient pt
                                            where ps.CourseSer = cs.CourseSer
	                                            and cs.PatientSer = pt.PatientSer
	                                            and ps.Status = 'TreatApproval'
	                                            and ps.HstryDateTime between '1/1/{0} 00:01' and '12/31/{0} 23:59'", year);
            return exec_sql(sql);
        }

        public static string[] get_PatientIds_with_TxApproved_Plan_of_Year_Month_Day(int year, int month, int day)
        {
            string sql = string.Format(@"select DISTINCT pt.PatientId
                                            from 
	                                            PlanSetup ps, 
	                                            Course cs,
	                                            Patient pt
                                            where ps.CourseSer = cs.CourseSer
	                                            and cs.PatientSer = pt.PatientSer
	                                            and ps.Status = 'TreatApproval'
	                                            and ps.HstryDateTime between '{1}/{2}/{0} 00:01' and '{1}/{2}/{0} 23:59'", year, month, day);
            return exec_sql(sql);
        }


        public static string[] get_PatientIds_Where_StructureIds_Like(string value)
        {
            string sql = string.Format(@"Select DISTINCT pt.PatientId from PlanSetup as ps, StructureSet as sset, Structure as s, Course as cs, Patient as pt where ps.StructureSetSer = sset.StructureSetSer AND sset.StructureSetSer = s.StructureSetSer
                                AND ps.CourseSer = cs.CourseSer
                                AND cs.PatientSer = pt.PatientSer
                                AND LOWER(s.StructureId) LIKE '%{0}%'
                                AND ps.Status = 'TreatApproval'", value);
            return exec_sql(sql);
        }


        public static string[] get_PatientIds_Where_PlanSetupIds_Like(string value)
        {
            string sql = string.Format(@"Select DISTINCT pt.PatientId
from PlanSetup as ps, Course as cs, Patient as pt
where ps.CourseSer = cs.CourseSer
AND cs.PatientSer = pt.PatientSer
AND ps.Status = 'TreatApproval'
AND LOWER(ps.PlanSetupId) LIKE '%{0}%'", value);
            return exec_sql(sql);
        }

        public static string[] get_PatientIds_CourseIds_PlanSetupIds_Where_PlanSetupIds_Like(string value)
        {
            string sql = string.Format(@"Select pt.PatientId, cs.CourseId, ps.PlanSetupId, ps.CreationDate
from PlanSetup as ps, Course as cs, Patient as pt
where ps.CourseSer = cs.CourseSer
and cs.PatientSer = pt.PatientSer
and LOWER(ps.PlanSetupId) LIKE '%{0}%'
order by ps.HstryTimeStamp desc", value);
            return exec_sql(sql);
        }



        public static string[] get_PrimaryOncologist_Of_Pid(string pid)
        {
            string sql = string.Format(@"select d.LastName, d.FirstName, d.DoctorId
from PatientDoctor pd, Patient p, Doctor d
where pd.PatientSer = p.PatientSer and pd.ResourceSer = d.ResourceSer
and pd.OncologistFlag = 1
and pd.PrimaryFlag = 1
and p.PatientId = '{0}'", pid);
            return exec_sql(sql);
        }

        public static string[] exec_sql(string sql)
        {
            try
            {
                List<string> col_headers = new List<string>();
                List<string> lines = new List<string>();

                variandb.varian sysdb = new variandb.varian(global_variables.db_server, global_variables.db_user, global_variables.db_pw);
                sysdb.open_connection();
                {
                    System.Data.SqlClient.SqlDataReader dr = sysdb.exe_sql_cmd(sql);

                    while (dr.Read())
                    {
                        // get the column header                   
                        if (col_headers.Count == 0)
                        {
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                string key = dr.GetName(i);
                                col_headers.Add(key);
                            }

                            lines.Add(string.Join(",", col_headers));
                        }

                        // get the values
                        List<string> values = new List<string>();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string val = string.Format("{0}", dr[i]);
                            values.Add(val);
                        }
                        lines.Add(string.Join(",", values));
                    }
                }
                sysdb.close_connection();

                return lines.ToArray();
            }
            catch (Exception e)
            {
                return null;
            }
        }


    }
}
