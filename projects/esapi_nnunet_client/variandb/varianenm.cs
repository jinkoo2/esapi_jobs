using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace variandb
{
    public class varianenm : db
    {
        
        public varianenm(string server, string uid, string pw)
            : base(server, "VARIAN", uid, pw)
        {
        }


        public List<string> select_app_user_userid_from_userid()
        {
            String sql = "SELECT [app_user_userid]  FROM [varianenm].[dbo].[userid]  WHERE status='A' and varianenm.dbo.userid.dflt_rx_typ=1 ORDER BY [userid]";
            
            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();

            while (dr.Read())
            {
                string uid = (string)dr[0];
                list.Add(uid);
            }

            return list;
        }

        public List<string> select_app_user_userid_dsp_name_from_userid()
        {
            string sql = "SELECT [varianenm].[dbo].userid.dsp_name, [varianenm].[dbo].userid.app_user_userid "
                + "FROM [varianenm].[dbo].[userid] "
                + "WHERE status='A' and varianenm.dbo.userid.dflt_rx_typ=1 "
                + " ORDER BY dsp_name";

            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();

            while (dr.Read())
            {
                string dsp_name = ((string)dr[0]).Trim();
                string userid = ((string)dr[1]).Trim();
                list.Add(dsp_name + this.deliminator + userid);
            }

            return list;
        }

       
        public List<string> select_document_type_by_inst_id(string inst_id)
        {
            string sql = "select note_typ.note_typ_desc "
                        + "from note_typ, inst_note_typ "
                        + "where inst_note_typ.inst_id='"+inst_id+"' "
                        + "and inst_note_typ.active_ind='Y' "
                        + "and note_typ.note_typ = inst_note_typ.note_typ "
                        + "order by note_typ.note_typ_desc ";

            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();

            while (dr.Read())
            {
                string v = (string)dr[0];
                list.Add(v);
            }

            return list;
        }

        public List<string> select_inst_id_by_inst_name(string inst_name)
        {
            string sql = "select inst_id from inst where inst_name='"+inst_name+"'";

            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();

            while (dr.Read())
            {
                string v = (string)dr[0];
                list.Add(v);
            }

            return list;
        }

        public List<string> select_inst_name()
        {
            string sql = "select inst_name from inst order by inst_name";

            SqlDataReader dr = this.exe_sql_cmd(sql);

            List<string> list = new List<string>();

            while (dr.Read())
            {
                string v = (string)dr[0];
                list.Add(v);
            }

            return list;
        }


        

    }
}
