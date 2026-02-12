using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace esapi
{
    static class global_variables
    {
        public static param app_params { get; set; }
        public static string db_server { get; set; }
        public static string db_user { get; set; }
        public static string db_pw { get; set; }

        public static string data_root { get; set; }

        public static string data_root_secure { get; set; }
        public static string log_path 
        { 
            get 
            {
                DateTime t = DateTime.Now;
                string log_dir = filesystem.join(global_variables.data_root_secure, "_app_logs", true);
                string log_file = $"exporter_log.{app_params.get_value("app_name")}.{t.Year}-{t.Month}-{t.Day}.txt";
                return filesystem.join(log_dir, log_file);
            } 
        }

        public static string exporter_new_obj_log_file { get; set; }

        public static string export_reqs_dir_of_uuids {
            get
            {
                return filesystem.join(data_root, "_export_reqs", false);
            }
        }


        public static string export_reqs_dir_of_pids
        {
            get
            {
                return filesystem.join(data_root_secure, "_export_reqs", false);
            }
        }

        public static string export_reqs_dir_of_planids
        {
            get
            {
                return filesystem.join(data_root_secure, "_export_reqs", false);
            }
        }


        public static string app_id = "exporter1";

        
        public static string get_time_string(DateTime t)
        {
            return string.Format("{0:yyyy.MM.dd_HH.mm.ss}", t);
        }

        public static string make_plan_exporter_new_obj_log_file()
        {
            string exporter_dir = filesystem.join(data_root, "_plan_exporter", true);
            string exporter_log_dir = filesystem.join(exporter_dir, "logs", true);
            string exporter_log_filename = $"log-new_objs-{global_variables.app_params.get_value("app_name")}-{get_time_string(DateTime.Now)}.csv";

            return filesystem.join(exporter_log_dir, exporter_log_filename);
        }

        public static void log(string msg)
        {
            print(msg, true);
        }

        public static void print(string msg, bool write_to_log_file=false)
        {
            Console.WriteLine(msg);
            

            if (write_to_log_file && data_root_secure != null)
            {
                DateTime t = DateTime.Now;

                msg = string.Format("[{0}-{1}-{2} {3}:{4}] {5}", t.Year, t.Month, t.Day, t.Hour, t.Minute, msg);
                System.IO.File.AppendAllLines(log_path, new string[] { msg });
            }
        }

        public static bool make_export_log = false;


    }
}
