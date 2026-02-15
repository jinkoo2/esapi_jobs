using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace esapi
{
    public class param
    {
        protected string _file;
        protected string _key_delim;
        protected string _value_delim;

        private string[] split(string line, string delim)
        {
            return line.Trim().Split(new string[] { delim }, StringSplitOptions.None);
        }
        public string file
        {
            get
            {
                return this._file;
            }
        }

        public param(string param_file, string key_delim="=", string value_delim=",")
        {
            this._file = param_file;
            this._key_delim = key_delim;
            this._value_delim = value_delim;
        }

        public string get_value(string key)
        {
            string[] lines = System.IO.File.ReadAllLines(this._file);

            foreach (string line in lines)
            {
                if (line.Trim() == "")
                    continue;

                if (line.Trim().StartsWith("#"))
                    continue;

                string[] elms = this.split(line, this._key_delim); 
                if (elms.Length != 2)
                    continue;

                if (elms[0].Trim().ToLower() == key.Trim().ToLower())
                    return elms[1].Trim();
            }

            throw new Exception("The key not found:" + key);
        }

        public string[] get_keys()
        {
            string[] lines = System.IO.File.ReadAllLines(this._file);

            List<string>           keys = new List<string>();

            foreach (string line in lines)
            {
                if (line.Trim() == "")
                    continue;

                if (line.Trim().StartsWith("#"))
                    continue;

                string[] elms = this.split(line, this._key_delim);
                if (elms.Length != 2)
                    continue;

                string key = elms[0].Trim().ToLower();
                keys.Add(key);
            }

            return keys.ToArray();

        }


        public bool has_key(string key)
        {
            try
            {
                get_value(key);
                return true;
            } 
            catch(Exception exn)
            {
                return false;
            }
        }


        public string[] get_value_as_array(string key)
        {
            List<string> list = new List<string>();
            string values = get_value(key);
            if (values.Trim() != "")
            {
                foreach (string v in this.split(values, this._value_delim))
                    list.Add(v.Trim());
            }

            return list.ToArray();
        }

        public int[] get_value_as_int_array(string key)
        {
            List<int> list = new List<int>();
            string values = get_value(key);
            if (values.Trim() != "")
            {
                foreach (string v in this.split(values, this._value_delim))
                    list.Add(System.Convert.ToInt32(v.Trim()));
            }

            return list.ToArray();
        }

        public double[] get_value_as_double_array(string key)
        {
            List<double> list = new List<double>();
            string values = get_value(key);
            if (values.Trim() != "")
            {
                foreach (string v in this.split(values, this._value_delim))
                    list.Add(System.Convert.ToDouble(v.Trim()));
            }

            return list.ToArray();
        }





    }
}
