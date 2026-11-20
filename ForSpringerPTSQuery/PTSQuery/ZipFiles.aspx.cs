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
using PTSZippingUtility;
using MPS_ManuscriptTracking.PTSBusinessLayer;
using MPS_ManuscriptTracking.PTSDataAccessLayer;

using System.IO;

namespace PTS
{
    public partial class ZipFiles : System.Web.UI.Page
    {

        IPTSQuery ptsQueryService = new PTSQueryService();
        //clsServerConfiguration serverConfig = SANGlobal.ServerConfig;

        string articleType = ConfigurationManager.AppSettings["ArticleType"].ToString().Trim();
        string issueType = ConfigurationManager.AppSettings["IssueType"].ToString().Trim();
        string jobType = string.Empty;

        DataTable fileTable = new DataTable("zipFiles");
        DataTable zipTable = new DataTable();

        string customer, journal, article, stage, ProblemTypeId, username;
        string customerGroupName, journalname, ptsProblemUser = string.Empty;
        string volume, issue;
        int stageID = 0;
        static bool isZipped = false;
        static bool isDel = false;
        // fileSize Limit
        static long fileSize = 0;
        long fileSizeTemp = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            closeButton.Attributes.Add("onclick", "return CloseWindow();");
            try
            {
                if (!Page.IsPostBack)
                {
                    // pks
                    fileSize = 0;

                    fileTable.Columns.Add(new DataColumn("fileName", typeof(string)));
                    fileTable.Columns.Add(new DataColumn("sourcePath", typeof(string)));
                    fileTable.Columns.Add(new DataColumn("targetPath", typeof(string)));
                    fileTable.Columns.Add(new DataColumn("fullPath", typeof(string)));

                    ViewState["zipfileTable"] = fileTable;

                    customer = Request.QueryString["customerCode"];
                    journal = Request.QueryString["journalCode"];
                    article = Request.QueryString["article"].Trim();
                    stage = Request.QueryString["stageName"].Trim();
                    username = Request.QueryString["userName"].Trim();
                    jobType = Request.QueryString["jobType"].Trim();
                    ProblemTypeId = Request.QueryString["ProblemTypeId"].Trim();
                    int.TryParse(Request.QueryString["stageID"].ToString().Trim(), out stageID);

                    FilesPanel.Visible = false;
                    ZipFilesPanel.Visible = false;


                    int customerCode, journalCode;

                    customerCode = journalCode = 0;
                    int.TryParse(customer, out customerCode);
                    int.TryParse(journal, out journalCode);

                    DataTable dtCustomerJournal = new DataTable();
                    dtCustomerJournal = ptsQueryService.GetCustomerGroupNameAndJournalName(customerCode, journalCode);

                    if (dtCustomerJournal.Rows.Count > 0)
                    {
                        customerGroupName = dtCustomerJournal.Rows[0]["cust_grname"].ToString().Trim();
                        journalname = dtCustomerJournal.Rows[0]["jour_sname"].ToString().Trim();
                    }


                    //the below path has to be from DB
                    string zipFilePath = string.Empty;
                    //string zipFilePath = ConfigurationManager.AppSettings["zipFilesPath"].ToString();
                    //string zipFileName = zipFilePath + @"\" + customer + "_" + journal + "_" + article + "_" + stage + "_" + username + ".zip";

                    DataTable dtProblems = new DataTable();
                    if (jobType == issueType)
                    {
                        string[] strarr = new string[2];

                        strarr = article.Split('~');
                        if (strarr.Length > 1)
                        {
                            volume = strarr[0];
                            issue = strarr[1];

                            dtProblems = ptsQueryService.GetPTSProblems(customerCode, journalCode, volume, issue, stageID);
                            if (dtProblems.Rows.Count > 0)
                                ptsProblemUser = dtProblems.Rows[0]["username"].ToString().Trim();
                        }
                        article = volume + issue;
                    }
                    else if (jobType == articleType)
                    {
                        dtProblems = ptsQueryService.GetPTSProblems(customerCode, journalCode, article, stageID);
                        if (dtProblems.Rows.Count > 0)
                            ptsProblemUser = dtProblems.Rows[0]["username"].ToString().Trim();


                    }

                    //if (ptsProblemUser.Length > 0)
                    //    username = ptsProblemUser;

                    zipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString();
                   // zipFilePath = clsServerConfig.GetStringReplacement(zipFilePath, customerCode);

                    zipFilePath = zipFilePath.Replace("[stage]", stage).Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", journalname.Trim()).Replace("[article]", article.Trim());
                    ViewState["zipFilePath"] = zipFilePath;

                    //DataTable dtConfig = new DataTable();
                    //dtConfig = ptsQueryService.GetPTSConfiguration(stageID, customerGroupName);

                    //if (dtConfig.Rows.Count > 0)
                    //{
                    //    zipFilePath = dtConfig.Rows[0]["zipfiledestination"].ToString().Trim();
                    //    ViewState["zipFilePath"] = zipFilePath;
                    //}

                    string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                    //tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", journalname.Trim()).Replace("[article]", article.Trim()).Replace("[stage]", stage).Replace("[username]", username.Trim());
                    tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", journalname.Trim()).Replace("[article]", article.Trim()).Replace("[stage]", stage).Replace("[problemtypeid]", ProblemTypeId).Replace("[username]", username.Trim());
                    tempZipFileName = tempZipFileName.Replace(" ", string.Empty);

                    string zipFileName = zipFilePath + @"\" + tempZipFileName;
                    
                    //this has to be get from db

                    HiddenZipFileName.Value = zipFileName.Trim();
                    Session["zipFile"] = HiddenZipFileName.Value;

                    if (File.Exists(zipFileName))
                    {
                        ZipFilesPanel.Visible = true;
                        zipLabel.Text = string.Empty;
                        zipTable.Columns.Add(new DataColumn("zipfilename", typeof(string)));
                        zipTable.Columns.Add(new DataColumn("zipfilepath", typeof(string)));

                        DataRow drow = zipTable.NewRow();
                        FileInfo fInfo = new FileInfo(zipFileName);
                        drow["zipfilename"] = fInfo.Name;
                        drow["zipfilepath"] = zipFilePath;

                        zipTable.Rows.Add(drow);

                        ShowZipFileGrid.DataSource = zipTable;
                        ShowZipFileGrid.DataBind();

                        deletebutton.Enabled = true;
                    }
                    else
                    {
                        zipLabel.Text = "No zip file is uploaded by " + username;
                        deletebutton.Enabled = false;

                    }

                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void zipButton_Click(object sender, EventArgs e)
        {
            try
            {
                MessageLabel.Text = string.Empty;
                if (ViewState["zipfileTable"] != null)
                    fileTable = (DataTable)ViewState["zipfileTable"];

                if (fileTable.Rows.Count == 0)
                {
                    MessageLabel.Text = "No files found to zip. Please upload the files to zip";
                    return;
                }

                if (HiddenZipFileName.Value.Length <= 0)
                {
                    MessageLabel.Text = "Invalid ZipFileName";
                    return;
                }

                string zipFileName = HiddenZipFileName.Value.Trim();
                string[] files = new string[fileTable.Rows.Count];
                int i = 0;
                foreach (DataRow drow in fileTable.Rows)
                {
                    files[i] = drow["fullPath"].ToString();
                    i++;
                }

                if (File.Exists(zipFileName))
                    File.Delete(zipFileName);


                ZipFileHelper.CreateZipFile(zipFileName, files);
                MessageLabel.Text = "ZipFile Created Successfully";

                //to show the created zip file in the table
                if (File.Exists(zipFileName))
                {
                    ZipFilesPanel.Visible = true;
                    zipLabel.Text = string.Empty;
                    zipTable = new DataTable();
                    zipTable.Columns.Add(new DataColumn("zipfilename", typeof(string)));
                    zipTable.Columns.Add(new DataColumn("zipfilepath", typeof(string)));

                    DataRow drow = zipTable.NewRow();
                    FileInfo fInfo = new FileInfo(zipFileName);
                    drow["zipfilename"] = fInfo.Name;
                    drow["zipfilepath"] = Path.GetDirectoryName(zipFileName);

                    zipTable.Rows.Add(drow);

                    ShowZipFileGrid.DataSource = zipTable;
                    ShowZipFileGrid.DataBind();
                }
                //end showing the created zip file


                //After zip file is created delete the uploaded files
                FileInfo fileInfo;
                foreach (DataRow drow in fileTable.Rows)
                {
                    fileInfo = new FileInfo(drow["fullPath"].ToString());
                    fileInfo.Delete();
                }

                fileTable.Rows.Clear();
                zipFileGrid.DataSource = fileTable;
                zipFileGrid.DataBind();
                FilesPanel.Visible = false;

                isZipped = true;
                isDel = false;
                if (!isDel)
                    deletebutton.Enabled = true;

                fileSize = 0;
                fileSizeTemp = 0;
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }
      
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            MessageLabel.Text = string.Empty;
            zipLabel.Text = string.Empty;
           
            if (fileUpload.PostedFile.FileName == string.Empty)
            {
                MessageLabel.Text = "Select the file to upload";
                return;
            }

            string tempPath = string.Empty;
            if (ViewState["zipFilePath"] != null)
            {
                tempPath = (string)ViewState["zipFilePath"];
            }

            if (tempPath.Length == 0)
            {
                MessageLabel.Text = "zip file path was not configured.";
                return;
            }

            string fileName = Path.GetFileName(fileUpload.PostedFile.FileName);
            //string fileName = "361_3611063_CH003_S100_test.zip";
            //tempPath = @"D:\Anitha";
            try
            {
                MessageLabel.Text = string.Empty;
                DataRow drow;

                if (ViewState["zipfileTable"] != null)
                    fileTable = (DataTable)ViewState["zipfileTable"];

                for (int i = 0; i < fileTable.Rows.Count; i++)
                {
                    if (fileName.Trim().ToLower() == fileTable.Rows[i]["fileName"].ToString().ToLower())
                    {
                        MessageLabel.Text = "Selected file name already added. Choose different one..";
                        return;
                    }
                }

                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                // check fileSize Start
                //=======================================
                FileInfo f = new FileInfo("temp.txt");
                long s = 0;
                fileSizeTemp = 0;
                //==================================
                long maxFileSize = Convert.ToInt64(ConfigurationManager.AppSettings["maxFileSize"].ToString().Trim());

                s = fileUpload.PostedFile.ContentLength;

                if(maxFileSize > 0)
                    fileSizeTemp = s / maxFileSize;
                    //fileSizeTemp = s / 1024;

                if (fileSizeTemp > 0)
                {
                    fileSize += fileSizeTemp;
                }
                //==================================
                if (fileSize <= 1024)
                {
                    fileUpload.PostedFile.SaveAs(tempPath + @"\" + fileName);

                    drow = fileTable.NewRow();
                    drow["fileName"] = fileName;
                    drow["sourcePath"] = fileUpload.PostedFile.FileName;
                    drow["targetPath"] = tempPath;
                    drow["fullPath"] = tempPath + @"\" + fileName;

                    fileTable.Rows.Add(drow);
                    ViewState["zipfileTable"] = fileTable;

                    zipFileGrid.DataSource = fileTable;
                    zipFileGrid.DataBind();

                    FilesPanel.Visible = true;
                    ZipFilesPanel.Visible = false;
                }
                else
                {
                    MessageLabel.Text = "Total file size exceeds the upload limit " + maxFileSize.ToString().Trim() + " KB. ";
                    fileSize -= fileSizeTemp;
                }
                //=======================================
                // check fileSize End
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void zipFileGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string fileName = zipFileGrid.DataKeys[e.RowIndex].Value.ToString();
                FileInfo fileInfo;

                if (ViewState["zipfileTable"] != null)
                    fileTable = (DataTable)ViewState["zipfileTable"];

                for (int i = 0; i < fileTable.Rows.Count; i++)
                {
                    if (fileName.Trim().ToLower() == fileTable.Rows[i]["fileName"].ToString().ToLower())
                    {
                        fileInfo = new FileInfo(fileTable.Rows[i]["fullPath"].ToString());

                        long s = fileInfo.Length;
                        fileSizeTemp = s / 1024;
                        if (fileSizeTemp > 0)
                        {
                            fileSize -= fileSizeTemp;
                        }

                        fileInfo.Delete();
                        fileTable.Rows.RemoveAt(i);
                        break;
                    }
                }
                ViewState["zipfileTable"] = fileTable;
                zipFileGrid.DataSource = fileTable;
                zipFileGrid.DataBind();


                
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void deletebutton_Click(object sender, EventArgs e)
        {
            try
            {
                MessageLabel.Text = string.Empty;
                if (HiddenZipFileName.Value.Length <= 0)
                {
                    MessageLabel.Text = "Invalid ZipFileName";
                    return;
                }

                string zipFileName = HiddenZipFileName.Value;
                if (File.Exists(zipFileName))
                {
                    File.Delete(zipFileName);

                    ShowZipFileGrid.DataSource = null;
                    ShowZipFileGrid.DataBind();
                }
                else
                {
                    MessageLabel.Text = "No zip file found to delete";
                    return;
                }
                string zipFilePath = string.Empty;
                if (ViewState["zipFilePath"] != null)
                {
                    zipFilePath = (string)ViewState["zipFilePath"];
                }
                isZipped = false;
                MessageLabel.Text = "Zip file deleted successfully from " + zipFilePath;
                //HiddenZipFileName.Value = string.Empty;
                isDel = true;
                if (isDel)
                    deletebutton.Enabled = false;
                
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        // code added by prasanta start
        protected void closeButton_Click(object sender, EventArgs e)
        {
            string close = string.Empty;
            try
            {
                if (File.Exists(HiddenZipFileName.Value.Trim()) && !isDel )
                {
                    Session["zipFile"] = HiddenZipFileName.Value;
                    close = @"<script type='text/javascript'> window.returnValue = true; window.close(); </script>";
                }
                else
                {
                    Session["zipFile"] = null;
                    close = @"<script type='text/javascript'> opener.document.getElementById('lblZipFile').innerHTML='';opener.document.getElementById('lbtnZipFile').innerHTML=''; window.returnValue = true; window.close(); </script>";
                }

                //string close = @"<script type='text/javascript'> window.returnValue = true; window.close(); </script>";
                base.Response.Write(close);
                isZipped = false;
                fileSize = 0;
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message.Trim();
            }
        }
        // code added by prasanta end
    }
}
