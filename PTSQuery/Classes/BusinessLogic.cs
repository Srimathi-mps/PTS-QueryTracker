using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Configuration;
using System.IO;

using MPS_ManuscriptTracking.PTSBusinessLayer;
using MPS_ManuscriptTracking.PTSDataAccessLayer;
using System.Xml;
//using System.Web.UI;

namespace PTS.Classes
{
    public class BusinessLogic //: System.Web.UI.Page
    {
        IPTSQuery ptsQueryService = new PTSQueryService();
        BizPTSQuery objBizPTSQuery = new BizPTSQuery();

        public string CreateSignalFile(int CustCode, string JourCode, string ArtCode, int StageCode, string Stage, string piiNumber, DataTable dtPTCategory, string problemID, string tempZipFileName, string strDesc, string strZipFilePath, string jobType, string issueType,string userName)
        {
            string outMsg = string.Empty;
            try
            {
                string zipFileName = string.Empty;
                PTSProblem ptsProblem = new PTSProblem();
                DataTable dtConfig = new DataTable();
                dtConfig = ptsQueryService.GetPTSConfiguration(StageCode, CustCode.ToString(), Convert.ToInt64(JourCode), ArtCode);
                //string formattedPIINumber = GeneralHelper.FormatArticlePIINumber(piiNumber);
                string formattedPIINumber = piiNumber;

                string volumeFirst = string.Empty;
                string volumeLast = string.Empty;
                string issueFirst = string.Empty;
                string issueLast = string.Empty;
                string suppl = string.Empty;

                if (dtConfig.Rows.Count > 0)
                {
                    if (dtConfig.Rows[0]["ResponseType"].ToString().Trim() == "0")
                    {
                        string tempSignalFilePath = ConfigurationManager.AppSettings["tempSignalFilePath"].ToString().Trim();
                        tempSignalFilePath = clsServerConfig.GetStringReplacement(tempSignalFilePath, CustCode);

                        string dtdName = ConfigurationManager.AppSettings["DTDName"].ToString().Trim();
                        string signalFileName = dtConfig.Rows[0]["SignalFileName"].ToString().Trim();
                        string signalFileDestination = dtConfig.Rows[0]["SignalFileDestination"].ToString().Trim();
                        signalFileDestination = clsServerConfig.GetStringReplacement(signalFileDestination, CustCode);

                        string signalTemplate = dtConfig.Rows[0]["SignalTemplate"].ToString().Trim();
                        string zipFileDestination = dtConfig.Rows[0]["ZipFileDestination"].ToString().Trim();
                        zipFileDestination = clsServerConfig.GetStringReplacement(zipFileDestination, CustCode);
                        WriteLogText("========================================================================================================");
                        WriteLogText("JourCode|Article :" + JourCode.Trim() + "|" + ArtCode.Trim());
                        WriteLogText("-----------------------------------------------------------");
                        WriteLogText("Starting Signal File Creation");

                        signalTemplate = signalTemplate.Replace("[dtdname]", dtdName);
                        signalTemplate = signalTemplate.Replace("[day]", DateTime.Now.ToString("dd")).Replace("[month]", DateTime.Now.ToString("MM")).Replace("[year]", DateTime.Now.ToString("yyyy")).Replace("[hour]", DateTime.Now.ToString("hh")).Replace("[minutes]", DateTime.Now.ToString("mm")).Replace("[seconds]", DateTime.Now.ToString("ss"));
                        signalTemplate = signalTemplate.Replace("[stage]", Stage); // Stage, stage
                        signalTemplate = signalTemplate.Replace("[piinumber]", formattedPIINumber);
                        string ProblemTypeCodes = "";
                        string ProblemTags = "";
                        string PtsRemarks = "";
                        string PtsLongDesc = "";
                        for (int i = 0; i < dtPTCategory.Rows.Count; i++)
                        {
                            ProblemTypeCodes = dtPTCategory.Rows[i]["ProblemTypeCode"].ToString().Trim();
                            //PtsRemarks = dtPTCategory.Rows[i]["PTCategoryRemark"].ToString().Trim().Replace("&", "&amp;").Replace("/", "&frasl;");
                            PtsRemarks = dtPTCategory.Rows[i]["PTCategoryRemark"].ToString().Trim().Replace("&", "&amp;").Replace(">","&gt;").Replace("<","&lt;");
                            //PtsLongDesc = dtPTCategory.Rows[i]["PTDesc"].ToString().Trim().Replace("&", "&amp;").Replace("/", "&frasl;");
                            PtsLongDesc = dtPTCategory.Rows[i]["PTDesc"].ToString().Trim().Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
                            //ProblemTags += "<problem><problem-type type=\"" + ProblemTypeCodes + "\"/><problem-remark>" + PtsLongDesc + "-" + PtsRemarks + "</problem-remark>";

                            string PTDescPTCategory = dtPTCategory.Rows[i]["PTDescPTCategory"].ToString().Trim().Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
                            ProblemTags += "<problem><problem-type type=\"" + ProblemTypeCodes + "\"/><problem-remark>" + PTDescPTCategory + "-" + PtsRemarks + "</problem-remark>";
                            

                            if (i == dtPTCategory.Rows.Count - 1 && File.Exists(tempZipFileName))
                                ProblemTags += "<file-attached>" + "JQ" + problemID + ".zip" + "</file-attached></problem>";
                            else
                                ProblemTags += "</problem>";
                        }
                        
                        signalTemplate = signalTemplate.Replace("[problems]", ProblemTags);

                        signalTemplate = signalTemplate.Replace("[problemtypecode]", ProblemTypeCodes.TrimEnd(','));
                        //signalTemplate = signalTemplate.Replace("[problemremarks]", PTDescriptionTextBox.Text.Trim());
                        signalTemplate = signalTemplate.Replace("[problemremarks]", strDesc);

                        if (jobType == issueType)
                        {
                            signalTemplate = signalTemplate.Replace("[volumefirst]", volumeFirst);
                            signalTemplate = signalTemplate.Replace("[volumelast]", volumeLast);
                            signalTemplate = signalTemplate.Replace("[issuefirst]", issueFirst);
                            signalTemplate = signalTemplate.Replace("[issuelast]", issueLast);
                            signalTemplate = signalTemplate.Replace("[suppl]", suppl);
                        }

                        signalTemplate = signalTemplate.Replace("[msgid]", "JQ" + problemID);
                        zipFileName = zipFileDestination + problemID + ".zip";

                        if (File.Exists(tempZipFileName))
                            ptsProblem.ZipFileUpload = true;
                        else
                            ptsProblem.ZipFileUpload = false;

                        if (ptsProblem.ZipFileUpload)
                        {
                            WriteLogText("ZIP file is attached");
                            ptsProblem.ZipFilePath = zipFileName;
                           
                        }
                        else
                        {
                            WriteLogText("No ZIP file ");
                            //ptsProblem.ZipFilePath = lblZipFile.Text.Trim();
                            ptsProblem.ZipFilePath = strZipFilePath;
                            signalTemplate = signalTemplate.Replace("[zipfilename]", string.Empty);
                            signalFileDestination = zipFileDestination;
                        }

                        signalFileName = signalFileName.Replace("[msgid]","JQ" + problemID);
                        tempSignalFilePath = tempSignalFilePath.Replace("[stage]", Stage); // Stage, stage

                        if (!Directory.Exists(tempSignalFilePath))
                            Directory.CreateDirectory(tempSignalFilePath);
                        WriteLogText("Creating Signal File");
                        StreamWriter sw = new StreamWriter(tempSignalFilePath + signalFileName);
                        sw.Write(signalTemplate);
                        sw.Close();
                        //File.Copy(tempSignalFilePath + signalFileName, signalFileDestination + signalFileName, true);
                        WriteLogText("Signal File Created in Temp Path :" + tempSignalFilePath + signalFileName);

                        try
                        {
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(tempSignalFilePath + signalFileName);
                            WriteLogText("Parsing Signal File");
                            string ResultMsg = ArtCode + " - " +  ProblemTypeCodes + ":" ;
                            ptsProblem.PtsRemarks = strDesc;
                            ptsProblem.ID = Convert.ToInt32(problemID.Trim()); // Convert.ToInt32(ddlOpenedRequest.SelectedValue);

                            string xmlValue = string.Empty;
                            xmlValue = "<Details>";
                            for (int k = 0; k < dtPTCategory.Rows.Count; k++)
                            {
                                //xmlValue += "<ProblemType><ProblemTypeId>" + dtPTCategory.Rows[k]["ProblemTypeId"].ToString() + "</ProblemTypeId><ProblemTypeCode>" + ProblemTypeCodes + "</ProblemTypeCode><ProblemCategoryId>" + dtPTCategory.Rows[k]["ProblemCategoryId"].ToString() + "</ProblemCategoryId><PTCategoryRemark>" + dtPTCategory.Rows[k]["PTCategoryRemark"].ToString().Trim().Replace("&", "&amp;").Replace("/", "&frasl;") + "</PTCategoryRemark><ProblemCategory>" + dtPTCategory.Rows[k]["ProblemCategory"].ToString().Trim() + "</ProblemCategory><PTDesc>" + dtPTCategory.Rows[k]["PTDesc"].ToString().Trim().Replace("&", "&amp;").Replace("/", "&frasl;") + "</PTDesc></ProblemType>";
                                xmlValue += "<ProblemType><ProblemTypeId>" + dtPTCategory.Rows[k]["ProblemTypeId"].ToString() + "</ProblemTypeId><ProblemTypeCode>" + ProblemTypeCodes + "</ProblemTypeCode><ProblemCategoryId>" + dtPTCategory.Rows[k]["ProblemCategoryId"].ToString() + "</ProblemCategoryId><PTCategoryRemark>" + dtPTCategory.Rows[k]["PTCategoryRemark"].ToString().Trim().Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;") + "</PTCategoryRemark><ProblemCategory>" + dtPTCategory.Rows[k]["ProblemCategory"].ToString().Trim() + "</ProblemCategory><PTDesc>" + dtPTCategory.Rows[k]["PTDesc"].ToString().Trim().Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;") +"</PTDesc></ProblemType>";
                            }
                            xmlValue += "</Details>";



                            ptsProblem.FileContent = xmlValue; // ViewState["xml"].ToString();
                            ptsProblem.UserName = userName;
                            ptsProblem.TaskClosed = true;
                            

                            if (File.Exists(tempZipFileName))
                            {
                                signalTemplate = signalTemplate.Replace("[zipfilename]", problemID + ".zip");
                                //File.Delete(tempZipFileName);
                            }
                            else
                            {
                                signalTemplate = signalTemplate.Replace("[zipfilename]", string.Empty);
                                signalFileDestination = zipFileDestination;
                            }
                            //ResultMsg = ptsQueryService.UpdateStatus(ptsProblem);
                            DataTable dt = ptsQueryService.UpdateStatus(ptsProblem);
                            if (dt.Rows.Count > 0)
                            {
                                ResultMsg += dt.Rows[0][0].ToString();
                                if (Convert.ToInt32(dt.Rows[0][1].ToString().Trim()) >= 0)
                                {
                                    //if zip file exists move the zip file to ftp area and copy the signal file to another path
                                    if (File.Exists(tempZipFileName))
                                    {
                                        File.Copy(tempZipFileName, zipFileName, true);
                                        File.Delete(tempZipFileName);
                                    }
                                    WriteLogText("Copying Signal File To destination Path");
                                    File.Copy(tempSignalFilePath + signalFileName, signalFileDestination + signalFileName, true);
                                    ResultMsg += ". File " + signalFileDestination + signalFileName + " created successfully!";
                                    //WriteLogText("Signal File Created Successfully :" + signalFileDestination + signalFileName);
                                    //WriteLogText("========================================================================================================");
                                }
                               
                               WriteLogText(ResultMsg);
                            }
                            else
                            {
                                ResultMsg += "Error in close/raise query";
                                WriteLogText(ResultMsg);
                            }

                            outMsg = ResultMsg;
                            if (File.Exists(tempSignalFilePath + signalFileName))
                                File.Delete(tempSignalFilePath + signalFileName);
                        }
                        catch
                        {
                            if (File.Exists(tempSignalFilePath + signalFileName))
                                File.Delete(tempSignalFilePath + signalFileName);
                            outMsg = "Parsing failed for " + ArtCode + " - Unable to Close/Raise query";
                            WriteLogText(outMsg);
                        }
                        WriteLogText("========================================================================================================");
                        ////ClearFields();
                    }
                }
                else
                {
                    outMsg = "No configuration is available for this stage";
                    WriteLogText(outMsg);
                    //ShowMessage("No configuration is available for this stage");
                    return outMsg;
                }
            }
            catch { throw; }

            return outMsg;
        }
        public void WriteLogText(string textMessage)
        {
            string logPath = ConfigurationManager.AppSettings["SentEmailLog"].ToString().Trim();
            string fileNameWithPath = "";
            fileNameWithPath = logPath + "SignalLog\\";
            if (!Directory.Exists(fileNameWithPath))
            {
                Directory.CreateDirectory(fileNameWithPath);
            }


            string fileName = fileNameWithPath + "\\PTS_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";
            StreamWriter sw = new StreamWriter(fileName, true);

            sw.WriteLine(textMessage + "-" + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"));

            sw.Flush();
            sw.Close();
        }

        //private void ShowMessage(string Message)
        //{
        //    //ScriptManager.RegisterStartupScript(Page, this.GetType(), "alert", string.Format("alert('{0}');", Message), true);
        //    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), Guid.NewGuid().ToString(), "alert('" + GeneralHelper.RemoveSpecialChars(Message) + "');", true);
        //}
    }
}
