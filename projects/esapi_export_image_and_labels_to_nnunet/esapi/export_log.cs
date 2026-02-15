using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esapi
{
    public static class export_log
    {
        private static void append_line(string line)
        {
            string fn = $"export_log.append_line(line={line})";
            helper.print(fn);

            DateTime t = DateTime.Now;
            string line2 = global_variables.get_time_string(t) + "," + line.Replace(global_variables.data_root, "{data_root}");
            filesystem.appendline( global_variables.exporter_new_obj_log_file, line2);
            //Console.WriteLine("++++++++++++++++");
            //Console.WriteLine(line2);
            //Console.WriteLine("++++++++++++++++");

            //string[] elms = line2.Split(',');
            //ExporterLog log = new ExporterLog();
            //log.Status = elms[1];
            //log.Type = elms[2];
            //log.Content = elms[3];
            //log.Time = t;
            //log.Comment = line2;
            //log.UpdatedBy = $"{global_variables.app_id}@{System.Environment.MachineName}";

            //string ret = webclient_exporterlog.post_one(log);
            //if (ret=="")
            //    helper.error($"Failed posting to server in append_line({line})");
    }

        public static void new_uuid_created(string uuid)
        {
            append_line($"new,uuid,{uuid}");
        }

        public static void new_pt_dir_created(string dir)
        {
            append_line($"new,pt_dir,{dir}");
        }

        public static void new_cs_dir_created(string dir)
        {
            append_line($"new,cs_dir,{dir}");
        }
        public static void new_img_dir_created(string dir)
        {
            append_line($"new,img_dir,{dir}");
        }

        public static void new_sset_dir_created(string dir)
        {
            append_line($"new,sset_dir,{dir}");
        }

        public static void new_pt_info_file_created(string file)
        {
            append_line($"new,pt_info,{file}");
        }
        public static void new_plansetup_info_file_created(string file)
        {
            append_line($"new,ps_info,{file}");
        }
        public static void new_beam_info_file_created(string file)
        {
            append_line($"new,beam_info,{file}"); 
        }

        public static void new_img_info_file_created(string file)
        {
            append_line($"new,img_info,{file}");
        }
        public static void new_structure_info_file_created(string file)
        {
            append_line($"new,s_info,{file}");
        }
        public static void new_sset_info_file_created(string file)
        {
            append_line($"new,sset_info,{file}");
        }

        public static void new_img_mhd_file_created(string file)
        {
            append_line($"new,img_mhd,{file}");
        }

        public static void new_img_mha_file_created(string file)
        {
            append_line($"new,img_mha,{file}");
        }

        public static void new_structure_mhd_file_created(string file)
        {
            append_line($"new,s_mhd,{file}");
        }

        public static void new_structure_mha_file_created(string file)
        {
            append_line($"new,s_mha,{file}");
        }


    }
}
