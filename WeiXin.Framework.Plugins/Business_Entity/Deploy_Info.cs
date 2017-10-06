using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Web;
using System.IO;

namespace WeiXin.Framework.Plugins.Business_Entity
{
    class Deploy_Info
    {
        static SQLServerDB DB = new SQLServerDB();
        //public static string formatTime(long createTime)
        //{
        //    long time_JAVA_Long = createTime * 1000L;//java长整型日期，毫秒为单位               
        //    DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0);
        //    long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度                        
        //    long time_tricks = tricks_1970 + time_JAVA_Long * 10000;//日志日期刻度                        
        //    DateTime dt = new DateTime(time_tricks).AddHours(8);//转化为DateTime
        //    return dt.ToString("yyyy-MM-dd HH:mm:ss");
        //}
        public static bool SetEngineer(string FromUserName, string EngineerName)
        {
            
            bool rtn_flag = false;
            //bool IsExit = false;
            ////检查是否存在，如果存在update，不存在insert
            //string strSql = string.Empty;
            //strSql = "SELECT   UserID, User_Name, User_Weixin " +
            //         "FROM      M_User " +
            //         "WHERE   (User_Weixin = N'" + FromUserName + "')";

            //SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);            
            //SqlDataReader sqlReader = sqlCmd.ExecuteReader();
            //if (sqlReader.Read() && sqlReader[0].ToString() != "")
            //{ IsExit = true; }
            //else { IsExit = false; }
            //sqlReader.Close();

            string strSql = string.Empty;
            try
            {
                //if (IsExist(FromUserName))
                //{
                //    strSql = "UPDATE  M_User SET   User_Name = N'" + EngineerName + "' WHERE   (User_Weixin = N'" + FromUserName + "')";
                //    SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
                //    SqlDataReader sqlReader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
                //    sqlCmd.Dispose();
                //}
                //else
                //{
                //    strSql = "INSERT INTO M_User (User_Name, User_Weixin) VALUES ( N'" + EngineerName + "', N'" + FromUserName + "')";
                //    SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
                //    sqlCmd.ExecuteNonQuery();
                //    sqlCmd.Dispose();
                //}

                strSql = "IF EXISTS(SELECT * FROM M_User WHERE User_Weixin = N'" + FromUserName + "') " +
                            "BEGIN "+
                            "UPDATE M_User SET User_Name = N'" + EngineerName + "' WHERE User_Weixin = N'" + FromUserName + "' " +
                            "END "+
                            "ELSE "+
                            "BEGIN "+
                            "INSERT INTO M_User (User_Name, User_Weixin) VALUES(N'" + EngineerName + "', N'" + FromUserName + "') " +
                            "END";
                SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();

                rtn_flag = true;
            }
            catch
            {
                rtn_flag = false;
            }
            return rtn_flag;
        }
        public static string GetEngineer(string FromUserName)
        {
            string strSql = string.Empty;
            string User_Name = string.Empty;
            strSql = "SELECT   UserID, User_Name, User_Weixin " +
                     "FROM      M_User " +
                     "WHERE   (User_Weixin = N'" + FromUserName + "')";
            SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
            SqlDataReader sqlReader = sqlCmd.ExecuteReader();
            if(sqlReader.Read() && sqlReader[0].ToString()!="")
            {
                User_Name = sqlReader[1].ToString();
            }
            else 
            {
                User_Name = "-1";
            }
            
            sqlReader.Close();
            return User_Name;
        }
        public static string GetHosp(string HospID, string FromUserName, string Time)
        {
            string strSql = string.Empty;
            string Hosp_INFO = string.Empty;
            string routine = ConfigurationManager.ConnectionStrings["routine"].ConnectionString;            
            if (HospID!="")
            { 
            strSql = "SELECT   HospID, Hosp_Name, Location_X, Location_Y " +
                     "FROM      M_Hosp_Info " +
                     "WHERE   (HospID = N'" + HospID + "')";
            }
            else if (HospID == "")
            {
                strSql = "SELECT   HospID, Hosp_Name, WorkID, User_WeiXin, User_Name, Work_Time " +
                        "FROM      M_Work " +
                        "WHERE   (User_WeiXin = N'" + FromUserName + "') AND (DATEDIFF(minute, Work_Time , CONVERT(DATETIME,  " +
                        "'" + Time + "', 120)) BETWEEN 0 AND " + routine + ") " +
                        "ORDER BY Work_Time DESC ";
            }

            SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
            SqlDataReader sqlReader = sqlCmd.ExecuteReader();
            
            if (sqlReader.Read() && sqlReader[0].ToString() != "")
            {
                Hosp_INFO = sqlReader[0].ToString() + "|" + sqlReader[1].ToString();
            }
            else
            {
                Hosp_INFO = "-1|-1";
            }
            sqlReader.Close();
            return Hosp_INFO;
        }
        public static bool SetWork(string FromUserName, string User_Name, string HospID, string Hosp_Name, string Time)
        {
            bool rtn_flag = false;
            string strSql = string.Empty;
            try
            {
                strSql = "INSERT INTO M_Work " +
                         "(User_WeiXin, User_Name, HospID, Hosp_Name, Work_Time) " +  //
                         "VALUES  (N'" + FromUserName + "', N'" + User_Name + "', N'" + HospID + "', N'" + Hosp_Name + "' , CONVERT(DATETIME, '" + Time + "', 102))";  //
                SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
                rtn_flag = true;
            }
            catch
            {
                rtn_flag = false;
            }
            return rtn_flag;
        }
        public static bool SetLocation(string Type, string ID, string Location_X, string Location_Y, string Location_Lable)
        {
            bool rtn_flag = false;
            string strSql = string.Empty;
            if (Type == "Hosp")
            {
                try
                {
                    strSql = "UPDATE  M_Hosp_Info " +
                             "SET         Location_X = N'" + Location_X + "', Location_Y = N'" + Location_Y + "', Location_Lable = N'" + Location_Lable + "' " +
                             "WHERE   (HospID = N'" + ID + "')";  //
                    SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
                    sqlCmd.ExecuteNonQuery();
                    sqlCmd.Dispose();
                    rtn_flag = true;
                }
                catch
                {
                    rtn_flag = false;
                }
            }
            return rtn_flag;
        }

        public static string  GetLocation(string Hosp_ID)
        {
            string  rtn_Info = string .Empty;
            string strSql = string.Empty;            
            try
            {
                strSql = "SELECT   HospID, Hosp_Name, Location_X, Location_Y, Location_Lable "+
                        "FROM      M_Hosp_Info "+
                        "WHERE   (HospID = N'" + Hosp_ID + "')";  //
                SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();

                if (sqlReader.Read() && sqlReader[2].ToString() != "")
                {
                    rtn_Info = sqlReader[2].ToString() + "|" + sqlReader[3].ToString() + "|" + sqlReader[4].ToString();
                }
                else
                {
                    rtn_Info = "-1";
                }
                sqlReader.Close();                
            }
            catch
            {
                rtn_Info = "-1";
            }
            return rtn_Info;
        }

        /// <summary>  
        /// 下载保存多媒体文件,返回多媒体保存路径  
        /// </summary>  
        /// <param name="ACCESS_TOKEN"></param>  
        /// <param name="MEDIA_ID"></param>  
        /// <returns></returns>  
        public static bool SetMultimedia(string ACCESS_TOKEN, string MEDIA_ID, string Hosp_ID)
        {
            string file = string.Empty;
            string content = string.Empty;
            string strpath = string.Empty;
            string savepath = string.Empty;
            string stUrl = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token=" + ACCESS_TOKEN + "&media_id=" + MEDIA_ID;
            bool rtn_flag = false;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(stUrl);

            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();

                strpath = myResponse.ResponseUri.ToString();
                //WriteLog("接收类别://" + myResponse.ContentType);
                WebClient mywebclient = new WebClient();
                savepath = System.Web.HttpContext.Current.Server.MapPath("image") + "\\" + Hosp_ID + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg"; //(new Random()).Next().ToString().Substring(0, 4) 随机数
                //WriteLog("路径://" + savepath);
                try
                {
                    mywebclient.DownloadFile(strpath, savepath);
                    file = savepath;
                    rtn_flag = true;
                }
                catch (Exception ex)
                {
                    savepath = ex.ToString();
                    rtn_flag = false;
                }

            }
            return rtn_flag;
        }

        public static bool IsExist(string FromUserName)
        {
            bool flag = false;
            //检查是否存在，如果存在update，不存在insert
            string strSql = string.Empty;
            strSql = "SELECT   UserID, User_Name, User_Weixin " +
                     "FROM      M_User " +
                     "WHERE   (User_Weixin = N'" + FromUserName + "')";
            SqlCommand sqlCmd = new SqlCommand(strSql, DB.sqlCon);
            SqlDataReader sqlReader = sqlCmd.ExecuteReader();

            if (sqlReader.Read() && sqlReader[0].ToString() != "")
            {
                //存在，update                
                flag = true;
            }
            else
            {
                //不存在，insert                
                flag = false;
            }
            sqlReader.Close();
            sqlCmd.Dispose();
            return flag;
        }

        private static string Fileresult = "";
        
        public static string GetFiles(DirectoryInfo directory, string pattern,string Filter)
        {
            if (directory.Exists || pattern.Trim() != string.Empty)
            {

                foreach (FileInfo info in directory.GetFiles(pattern))
                {
                    string FileName =info.Name.ToString();
                    if (FileName.Contains(Filter))
                    {
                        Fileresult = Fileresult + info.FullName .ToString() + "|"; 
                    }                 
                }

                foreach (DirectoryInfo info in directory.GetDirectories())
                {
                    GetFiles(info, pattern, Filter);
                }
            }
            string returnString = Fileresult;
            return returnString;

        }

        public static FileInfo[] GetMultimedia(string Hosp_ID)
        {
            string File_info = string.Empty;
            string Path = System.Web.HttpContext.Current.Server.MapPath("image");// +@"\" + Hosp_Name;
            Fileresult = "";
            File_info = GetFiles(new DirectoryInfo(Path), "*.jpg", Hosp_ID);
            string [] Fileinfo = File_info.Split ('|');
            FileInfo[] arrFi = new FileInfo[Fileinfo.Length-1];
            for (int i = 0; i < Fileinfo.Length-1;i++ )
            {
                arrFi[i] = new FileInfo(Fileinfo [i]);
            }
            Array.Sort(arrFi, delegate(FileInfo x, FileInfo y) { return y.CreationTime.CompareTo(x.CreationTime); });//按照创建时间倒排序
            //Array.Sort(arrFi, delegate(FileInfo x, FileInfo y) { return x.CreationTime.CompareTo(y.CreationTime); });//按照创建时间正排序 先发生在前
            return arrFi;        }
        
    }
}
