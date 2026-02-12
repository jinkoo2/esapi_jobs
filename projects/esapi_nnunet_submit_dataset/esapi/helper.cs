using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esapi
{
    public static class helper
    {
        public static void print(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void error(string msg)
        {
            Console.WriteLine(msg);
        }



        public static string join(string path, string sep)
        {
            return System.IO.Path.Combine(path, sep);
        }

        public static Dictionary<string, object> json2dict(string jsonString)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
        }

        public static Dictionary<string, string> json2strstrdict(string jsonString)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        }

        public static List<object> json2list(string jsonString)
        {
            return JsonConvert.DeserializeObject<List<object>>(jsonString);
        }




    }
}
