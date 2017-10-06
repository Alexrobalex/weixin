using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace WeiXin.Framework.Plugins.Business_Entity
{
    public class SQLServerDB
    {
        //连接数据库  
        public SqlConnection sqlCon;
        //连接数据库字符串  
        //public String strSql = @"Data Source=1.0.0.84;Initial Catalog=servicedesk;Persist Security Info=True;User ID=sa;Password=bjYB20090116@capInfo";
        static string DBServer = ConfigurationManager.ConnectionStrings["DBServer"].ConnectionString;
        public String strSql = @"Data Source=" + DBServer + ";Initial Catalog=WeiXin;Persist Security Info=True;User ID=sa;Password=sa";
        //默认构造函数  
        public SQLServerDB()
        {
            if (sqlCon == null)
            {
                sqlCon = new SqlConnection();
                sqlCon.ConnectionString = strSql;
                sqlCon.Open();
            }
        }

        public void Close()
        {
            if (sqlCon != null)
            {
                sqlCon.Close();
                //sqlCon.Dispose();  
            }
        }
    }
}
