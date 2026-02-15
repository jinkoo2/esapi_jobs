using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace variandb
{

public class db
{


public
db(string _dbserver, string _dbname, string uid, string pw)
{
    this.dbserver = _dbserver;
    this.dbname = _dbname;
    this.user_id = uid;
    this.user_pw = pw;

    this.deliminator = "|";
}


	public string dbserver;
	public string dbname;
	public	string user_id;
	public string user_pw;

    public string deliminator;

	public string connection_string
	{
		get
		{
			return "Data Source="+this.dbserver+";Initial Catalog="+this.dbname+";Integrated Security=False;User Id="+this.user_id+";Password="+this.user_pw+";MultipleActiveResultSets=True";
		}
	}

	private SqlConnection _con;
	public void open_connection()
	{
		if(this._con==null)
			this._con = new SqlConnection(this.connection_string);
		this._con.Open();
	}
    	public void close_connection()
	{
		if(this._con==null)
			return;
		this._con.Close();
	}

    public SqlConnection get_sqlconnection()
	{
		return this._con;			
	}

    public SqlDataReader exe_sql_cmd(string cmdString)
	{
		SqlCommand cmd = new SqlCommand(cmdString, 
			this.get_sqlconnection());
		return cmd.ExecuteReader();
	}

    public object exe_sql_cmd_scalar(string cmdString)
    {
            SqlCommand cmd = new SqlCommand(cmdString,
                this.get_sqlconnection());
            return cmd.ExecuteScalar();
    }

};



}
