using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using MPS_ManuscriptTracking.PTSBusinessLayer;
using MPS_ManuscriptTracking.PTSDataAccessLayer;
using System.Collections.Generic;

namespace PTS
{
    /// <summary>
    /// Summary description for AutoCompleteUserName
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AutoCompleteUserName : System.Web.Services.WebService
    {

        public AutoCompleteUserName()
        { }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string[] GetCompletionList(string prefixText, int count)
        {
            if (count == 0)
            {
                count = 10;
            }

            IPTSQuery ptsQueryService = new PTSQueryService();
             List<String> CustList = new List<string>(count);

            //string connString = ConfigurationManager.AppSettings["constr"].ToString();
            //string selectString = "SELECT DISTINCT TOP " + count.ToString() + " isbn from jobsmaster WHERE isbn LIKE '" + prefixText + "%'";

           
            //using (SqlConnection sqlConn = new SqlConnection(connString))
            //{
            //    sqlConn.Open();

            //    using (SqlCommand sqlCmd = new SqlCommand(selectString, sqlConn))
            //    {
            //        SqlDataReader reader = sqlCmd.ExecuteReader();
            //        while (reader.Read())
            //            CustList.Add(reader["isbn"].ToString());
            //    }
            //}

            DataTable dt = new DataTable();
            dt = ptsQueryService.GetUserNameForAutoComplete(prefixText, count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CustList.Add(dt.Rows[i]["UserName"].ToString().Trim());
            }
            
            return (CustList.ToArray());

            
        }
        
    }
}
