using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using MPS_ManuscriptTracking.PTSBusinessLayer;
using MPS_ManuscriptTracking.PTSDataAccessLayer;

namespace QueryTracker
{
    public partial class NewLogin : System.Web.UI.Page
    {
        IPTSQuery ptsQueryService = new PTSQueryService();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
       
        protected void Button1_Click(object sender, EventArgs e)
        {
            string userName = string.Empty;
            string password = string.Empty;

            userName = UserNameTextBox.Text.Trim();
            password = PasswordTextBox.Text.Trim();
            Session["userName"] = userName;

            //DataTable dtUser = new DataTable();
            //dtUser = ptsQueryService.GetUserDetails(userName, password);

            //if (dtUser.Rows.Count > 0)
            //{
            //    Global.UserName = dtUser.Rows[0]["UserName"].ToString().Trim();

            //    if (dtUser.Rows[0]["roleid"] != DBNull.Value)
            //        Global.RoleID = dtUser.Rows[0]["roleid"].ToString().Trim();
            //    else
            //        Global.RoleID = string.Empty;

            //    if (dtUser.Rows[0]["rolename"] != DBNull.Value)
            //        Global.RoleName = dtUser.Rows[0]["rolename"].ToString().Trim();
            //    else
            //        Global.RoleName = string.Empty;

            //    if (dtUser.Rows[0]["usercode"] != DBNull.Value)
            //        Global.userId = dtUser.Rows[0]["usercode"].ToString().Trim();
            //    else
            //        Global.userId = string.Empty;

            //    //Response.Redirect("Reports/SummaryReport.aspx");
            //    Response.Redirect("RaiseProblem.aspx");
               
            //}
            //else
            //{
            //    //lblMsg.Text = "Invalid username/password";
            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Invalid username/password', 'Required', 'top', 'right');", true);

            //}
            Response.Redirect("RaiseProblems.aspx?Username=bWFjMDA2MDgx-3UH2mSnTa3I=");
        }

        //protected void Button3_Click(object sender, EventArgs e)
        //{
        //    Response.Write("<script>window.open('Reports/SummaryReport.aspx','_blank');</script>");
        //}

        
    }
}
