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
using System.Data.SqlClient;
using System.Collections.Generic;

namespace PTS
{
    public partial class QueryReminder : System.Web.UI.Page
    {
        static String conn = System.Configuration.ConfigurationManager.AppSettings["PTSConstr"].ToString();
        DataTable dt = new DataTable();
        int custId = 0, qryStageId = 0;
        string customer = "", qryStage = "", btnFnnction="";

        protected string strUserIDString = Global.Validate();  // Get from PMT... Changed By sathish

        //protected string strUserIDString = "MAC000289";
         

        protected void Page_Load(object sender, EventArgs e)
        {

           
            if (!Page.IsPostBack)
            {
                btn_Save.Attributes.Add("onclick", "return ValidateSave();");

                try
                {
                    populateList("CustomerList");
                    populateList("QueryStatusList");
                }
                catch (Exception ex)
                { }
            }

             lbl_showMsg.Text = "";
          
             
        }

        protected void ReloadSelectedCustomerAndStage()
        {
            try
            {
                customer = ddl_Customer.SelectedItem.ToString().Trim();

                if (ddl_Customer.SelectedItem.ToString().Trim() != "--Select Customer--")
                    custId = Int32.Parse(ddl_Customer.SelectedValue.Trim());

                qryStage = ddl_QueryStatus.SelectedItem.ToString().Trim();

                if (ddl_QueryStatus.SelectedItem.ToString().Trim() != "--Select QueryStatus--")
                    qryStageId = Int32.Parse(ddl_QueryStatus.SelectedValue.Trim());
            }
            catch (Exception ex) { 
                //lbl_showMsg.Text  = "Error in customer selection."; 
            }
        }

        private void populateList(string lstType)
        {

            SqlConnection con = new SqlConnection(conn);
            if (IsPostBack) ReloadSelectedCustomerAndStage();

            try
            {
                SqlCommand sql = new SqlCommand("usp_GetPtsQueryCustomerStatus", con);
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@RequestedList", SqlDbType.VarChar).Value = lstType.Trim();
                sql.Parameters.Add("@Operator", SqlDbType.VarChar).Value = strUserIDString.Trim();
                sql.Parameters.Add("@CustomerCode", SqlDbType.Int).Value = custId;
                sql.Parameters.Add("@QueryStgId", SqlDbType.Int).Value = qryStageId;

                con.Open();
                HttpContext.Current.Server.ScriptTimeout = 300;
                SqlDataReader dbRecList = sql.ExecuteReader();

                switch (lstType)
                {
                    case "CustomerList":
                        ddl_Customer.DataSource = dbRecList;

                        ddl_Customer.DataTextField = "Cust_Name";
                        ddl_Customer.DataValueField = "Cust_Code";
                        ddl_Customer.DataBind();
                        ddl_Customer.Items.Insert(0, new ListItem("--Select Customer--"));
                        break;

                    case "QueryStatusList":

                        ddl_QueryStatus.DataSource = dbRecList;
                        ddl_QueryStatus.DataTextField = "QueryStatus";
                        ddl_QueryStatus.DataValueField = "QueryStatusId";
                        ddl_QueryStatus.DataBind();
                        ddl_QueryStatus.Items.Insert(0, new ListItem("--Select QueryStatus--"));
                        break;

                    case "EmailGridViewList":

                     
                        btn_Cancel.Enabled = false;
                        gv_Reminders.DataSource = null;
                        dt.Load(dbRecList);
                        int rCntRqt = 0;
                        int mailConigCode = 0;
                        int rCntExt = dt.Rows.Count;
                        int recCnt = rCntExt;
                        rCntRqt = rCntExt;

                        if (gv_Reminders.Rows.Count > 0 && btnFnnction == "Add")
                        {
                            dt = ArragneGridviewToTable();
                            rCntExt = dt.Rows.Count;
                            recCnt = rCntExt;
                            rCntRqt = rCntExt;
                        }

                        

                        if (!ddl_QueryStatus.SelectedValue.Equals("--Select QueryStatus--"))
                        {

                            if (dt.Rows.Count > 0)
                            {

                                if ((dt.Columns.Contains("MailConfigId")) && mailConigCode ==0)
                                    mailConigCode = Convert.ToInt32(dt.Rows[0]["MailConfigId"]);

                                if ((dt.Columns.Contains("EMailConfigId")) && mailConigCode ==0)
                                    mailConigCode = Convert.ToInt32(dt.Rows[0]["EMailConfigId"]);

                                if (dt.Rows[0]["EmailDetailsID"].ToString().Trim() == "" && recCnt == 1)
                                { recCnt = 0; dt.Rows[0].Delete(); dt.AcceptChanges(); }

                            }
                            if (txtBx_Reminders.Text.Trim().Length > 0)
                                rCntRqt += Convert.ToInt16((txtBx_Reminders.Text).Trim());

                            for (recCnt++; recCnt <= rCntRqt; recCnt++)
                            {
                                DataRow dr = dt.NewRow();
                                dr["IsActive"] = true;
                                dr["ReminderNo"] = recCnt;
                                dr["EmailDetailsID"] = 0;
                                dr["EmailConfigID"] = mailConigCode;
                                dr["IntervalInHrs"] = 0;
                                dr["EmailFrom"] = ""; dr["EmailTo"] = ""; dr["EmailCC"] = ""; dr["EmailBCC"] = ""; dr["Subject"] = ""; dr["EmailContent"] = "";

                                if (chkBx_Replicate.Checked && recCnt > 1)
                                {
                                    btn_Cancel.Enabled = true;
                                    dr["IntervalInHrs"] = Convert.ToInt32(dt.Rows[rCntExt - 1]["IntervalInHrs"]).ToString();    
                                    dr["EmailFrom"] = dt.Rows[rCntExt - 1]["EmailFrom"].ToString();            
                                    dr["EmailTo"] = dt.Rows[rCntExt - 1]["EmailTo"].ToString();                 
                                    dr["EmailCC"] = dt.Rows[rCntExt - 1]["EmailCC"].ToString();                 
                                    dr["EmailBCC"] = dt.Rows[rCntExt - 1]["EmailBCC"].ToString();    
                                    dr["Subject"] = dt.Rows[rCntExt - 1]["Subject"].ToString();         
                                    dr["EmailContent"] = dt.Rows[rCntExt - 1]["EmailContent"].ToString();        
                                }
                                dt.Rows.Add(dr);
                            }

                            if (rCntRqt == 0)
                                txtBx_Reminders.Text = (--recCnt).ToString().Trim();

                            Session["TaskTable"] = dt;
                            gv_Reminders.DataSource = dt;
                            gv_Reminders.DataBind();


                            btn_Save.Enabled = true;
                            btn_Add.Enabled = true;
                        }
                        else
                        {
                            dt = null;
                            gv_Reminders.DataSource = dt;
                            gv_Reminders.DataBind();
                            txtBx_Reminders.Text = "";
                            lbl_showMsg.Text = "Please select the correct status";
                            btn_Save.Enabled = false;
                            btn_Add.Enabled = false;
                        }
                            break;
                    
                }
            }
            catch (Exception ex)
            {
                //lbl_showMsg.Text = "Error in populating " + lstType.ToString();
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        protected void Customer_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbl_showMsg.Text = "";
            populateList("QueryStatusList");
            
            gv_Reminders.DataSource =null;
            gv_Reminders.DataBind();
            btn_Save.Enabled = false;
            btn_Add.Enabled = false;
        }


        protected void QueryStage_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            txtBx_Reminders.Text = "";
            try
            {
                ReloadSelectedCustomerAndStage();

                if (custId == 0)
                {
                    lbl_showMsg.Text = "Please select the customer..";
                }
                else
                {
                    populateList("EmailGridViewList");
                }
            }
            catch (Exception s)
            {
                HttpContext.Current.Response.Write("Error Occured " + s.Message);
            }
        }

     
        protected void btn_Save_Click(object sender, EventArgs e)
        {
             
            int iRow = 0;
            string strIRow = "";
            try
            {
                DataTable _datatable =  ArragneGridviewToTable();

                bool validMaidlBody = true;
                iRow = 0;
                foreach (DataRow r in _datatable.Rows)
                {
                    iRow++;
                    if (r["EmailContent"].ToString().Trim().Length < 15)
                    {
                        validMaidlBody = false;
                        if (strIRow.Length > 0) strIRow = strIRow + ", ";
                        strIRow = strIRow + iRow.ToString().Trim();
                        lbl_showMsg.Text = "Please add email body content ! ( Row : " + strIRow + ")";
                    }
                }

                if (validMaidlBody && validateRemindersAndIntervalsHrs(_datatable))   
                {

                    foreach(DataRow dr in _datatable.Rows)
                        if(updateEmailConfigDetails(dr, 0))                          //0-Insert/Update   1- Delete
                            lbl_showMsg.Text = "Record(s) successfully updated!";
                    txtBx_Reminders.Text = "";
                    populateList("EmailGridViewList");
                }
               
            }
            catch (Exception ex) { 
                //lbl_showMsg.Text = "Error while saving record(s)."; 
            }
        }

        protected DataTable ArragneGridviewToTable()
        {

            DataTable _datatable = new DataTable();

            DataTable _dt = new DataTable();

            try
            {
                
                _dt = (DataTable)Session["TaskTable"];

                for (int i = 0; i < gv_Reminders.Columns.Count; i++)
                {
                    _datatable.Columns.Add(gv_Reminders.Columns[i].ToString());
                }

                if (!_datatable.Columns.Contains("EmailContent"))
                    _datatable.Columns.Add("EmailContent");

                if (!_datatable.Columns.Contains("mailContent"))
                    _datatable.Columns.Add("mailContent");

                if (!_datatable.Columns.Contains("IsActive"))
                    _datatable.Columns.Add("IsActive");

                int iRow = 0;

                foreach (GridViewRow row in gv_Reminders.Rows)
                {
                    DataRow dr = _datatable.NewRow(); CheckBox ckBx = new CheckBox();

                    dr["ReminderNo"] = ((TextBox)row.FindControl("gv_TxtBx_ReminderNo")).Text.Trim();
                    dr["IntervalInHrs"] = ((TextBox)row.FindControl("gv_TxtBx_IntervalInHrs")).Text.Trim();
                    dr["EmailFrom"] = ((TextBox)row.FindControl("gv_TxtBx_EmailFrom")).Text.Trim();
                    dr["EmailTo"] = ((TextBox)row.FindControl("gv_TxtBx_EmailTo")).Text.Trim();
                    dr["EmailCC"] = ((TextBox)row.FindControl("gv_TxtBx_EmailCC")).Text.Trim();
                    dr["EmailBCC"] = ((TextBox)row.FindControl("gv_TxtBx_EmailBCC")).Text.Trim();
                    dr["Subject"] = ((TextBox)row.FindControl("gv_TxtBx_Subject")).Text.Trim();
                    dr["EmailContent"] = ((TextBox)row.FindControl("gv_TxtBx_EmailBody")).Text.Trim();
                    dr["EmailDetailsID"] = _dt.Rows[iRow]["EmailDetailsID"];
                    dr["EmailConfigID"] = _dt.Rows[iRow]["EmailConfigID"];
                    iRow++;

                    _datatable.Rows.Add(dr);

                }
            }
            catch (Exception ex) { 
                //lbl_showMsg.Text = "Error in arranging records."; 
            }

            return _datatable;
        }


        protected bool validateRemindersAndIntervalsHrs(DataTable vTD)
        {
            bool intervalValidate = true;
            
            Dictionary<int, int> oDt = new Dictionary<int, int>();

            try
            {
                foreach (DataRow r in vTD.Rows)
                    oDt.Add(Convert.ToInt32(r["ReminderNo"]), Convert.ToInt32(r["IntervalInHrs"]));
            }
            catch (Exception ex) { lbl_showMsg.Text = ex.Message.ToString(); ; intervalValidate = false;}

            if (intervalValidate)
            {

                try
                {

                DataTable tdt = new DataTable();
                tdt.Columns.Add("ReminderNo", typeof(int));
                tdt.Columns.Add("IntervalInHrs", typeof(int));
                foreach (KeyValuePair<Int32, Int32> intervals in oDt.OrderBy(key => key.Key))
                {
                    DataRow dr = tdt.NewRow();
                    dr["ReminderNo"] = Convert.ToInt32(intervals.Key.ToString());
                    dr["IntervalInHrs"] = Convert.ToInt32(intervals.Value.ToString());
                    tdt.Rows.Add(dr);
                }

                for (int i = 1; i <= tdt.Rows.Count; i++)
                {

                    if (Convert.ToInt32(tdt.Rows[i - 1]["IntervalInHrs"]) != 0)
                    {

                        if (Convert.ToInt32(tdt.Rows[i - 1]["IntervalInHrs"]) >= Convert.ToInt32(tdt.Rows[i]["IntervalInHrs"]))
                        {
                            lbl_showMsg.Text = "Interval hours are not in sequence.";
                            intervalValidate = false; break;

                        }
                    }
                    if (Convert.ToInt32(tdt.Rows[i - 1]["IntervalInHrs"]) == 0)
                    {
                        lbl_showMsg.Text = "Interval hours should not be zero/blank.";
                        intervalValidate = false; break;
                    }

                }
                }
                catch (Exception ex) { 
                    //lbl_showMsg.Text = "Error in validating Reminders/Hrs.";  
                }
            }
            else
            {
                if (lbl_showMsg.Text == "An item with the same key has already been added.")
                    lbl_showMsg.Text = "Reminder numbers are duplicate.";

                if (lbl_showMsg.Text == "Input string was not in a correct format.")
                    lbl_showMsg.Text = "Check integere value for Reminder Nos / Interval Hrs.";
            }

             return intervalValidate;
        }



        protected bool updateEmailConfigDetails(DataRow dtRow, int DelOrUpdate)
        {
            SqlConnection con = new SqlConnection(conn);
            bool DeleteUpdateSuccess = false;

            try
            {
                SqlCommand sql = new SqlCommand("usp_UpdatePtsQueryEmailConfig", con);
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@Operator", SqlDbType.VarChar).Value = strUserIDString.Trim();
                sql.Parameters.Add("@CustomerCode", SqlDbType.Int).Value = custId;
                sql.Parameters.Add("@QueryStgId", SqlDbType.Int).Value = qryStageId;
                sql.Parameters.Add("@DelOrUpdate", SqlDbType.Int).Value = DelOrUpdate;
 
                sql.Parameters.Add("@ReminderNo", SqlDbType.Int).Value = dtRow["ReminderNo"].ToString();
                sql.Parameters.Add("@IntervalInHrs", SqlDbType.Int).Value = dtRow["IntervalInHrs"].ToString();
                sql.Parameters.Add("@EmailFrom", SqlDbType.VarChar).Value = dtRow["EmailFrom"].ToString();
                sql.Parameters.Add("@EmailTo", SqlDbType.VarChar).Value = dtRow["EmailTo"].ToString();
                sql.Parameters.Add("@EmailCC", SqlDbType.VarChar).Value = dtRow["EmailCC"].ToString();
                sql.Parameters.Add("@EmailBCC", SqlDbType.VarChar).Value = dtRow["EmailBCC"].ToString();
                sql.Parameters.Add("@Subject", SqlDbType.VarChar).Value = dtRow["Subject"].ToString();
                sql.Parameters.Add("@EmailContent", SqlDbType.VarChar).Value = dtRow["EmailContent"].ToString();
                 
                sql.Parameters.Add("@EmailDetailsID", SqlDbType.Int).Value = dtRow["EmailDetailsID"].ToString();
                sql.Parameters.Add("@EmailConfigID", SqlDbType.Int).Value = dtRow["EmailConfigID"].ToString();
                con.Open();
                SqlDataReader dbRecList = sql.ExecuteReader();
                DeleteUpdateSuccess = true;
            }
            catch (Exception ex) { 
                //lbl_showMsg.Text = "Error in record updation"; 
            }
            return DeleteUpdateSuccess;

        }


        protected void DetleRecord_Click(object sender, EventArgs e)
        {

            try
            {
                int rIndex = 0, eMailDetailID = 0, eMailConfigID = 0;
                ImageButton img = (ImageButton)sender;
                GridViewRow row = (GridViewRow)img.Parent.Parent;

                rIndex = row.RowIndex;

                DataTable _dt = new DataTable();
                _dt = (DataTable)Session["TaskTable"];

                eMailDetailID = Convert.ToInt32(_dt.Rows[rIndex]["EmailDetailsID"]);
                eMailConfigID = Convert.ToInt32(_dt.Rows[rIndex]["EmailConfigID"]);

                if (eMailDetailID != 0)
                {
                    DataRow dDr = _dt.Rows[rIndex];
                    if (updateEmailConfigDetails(dDr, 1)) // 1-Delete and  0-Insert/Update
                    {
                        lbl_showMsg.Text = "Selected record deleted successfully!";
                    }

                }

                if (txtBx_Reminders.Text.Trim().Length > 0 && eMailDetailID == 0)
                    txtBx_Reminders.Text = (Convert.ToInt16((txtBx_Reminders.Text).Trim()) - 1).ToString();

                populateList("EmailGridViewList");
            }
            catch (Exception ex) { 
                //lbl_showMsg.Text = "Error in deletion of record.";  
            }
            
        }
  
        protected void btn_Add_Click1(object sender, EventArgs e)
        {
            btnFnnction = "Add";
            customer = ""; qryStage = "";
            custId = 0; qryStageId = 0;
            populateList("EmailGridViewList");
            
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            txtBx_Reminders.Text = ""; populateList("EmailGridViewList"); btnFnnction = "Cancel";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnFnnction= "Save";
                int rIndex = Convert.ToInt32(Session["idx"].ToString().Trim());
                DataTable dt = ArragneGridviewToTable();
                string sTxtBx = RichTextBox.Text;
                dt.Rows[rIndex]["EmailContent"] = sTxtBx;

                gv_Reminders.DataSource = dt;
                gv_Reminders.DataBind();
            }
            catch (Exception ex) { 
                //lbl_showMsg.Text = "Error in email body saving. "; 
            }
  
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            btnFnnction= "Cancel";
        }
  

        protected void gv_Reminders_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gv_Btn_EmailBody_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnUpload = (Button)sender;
                int rowIndex = Convert.ToInt32(btnUpload.CommandArgument);
                string Emailbody = string.Empty;
                GridViewRow grow = gv_Reminders.Rows[rowIndex];
                Session["idx"] = rowIndex;
                Emailbody = (grow.FindControl("gv_TxtBx_EmailBody") as TextBox).Text.ToString();
                RichTextBox.Text = Emailbody;
                ModalPopupExtenderEmailBody.Show();
            }
            catch (Exception ex) { 
            //    lbl_showMsg.Text = "Error in email body receiving. ";  
            }
        } 
    }

    }

