using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Configuration;

namespace QueryTracker
{
    public partial class DownloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //* save or download file *//
                string filename = Request.Params["file"].ToString(); //* get full path to download file*//
                DownloadableProduct_Tracking(filename);
            }
        }
        private bool DownloadableProduct_Tracking(string fname)
        {
            System.IO.FileInfo FileName = new System.IO.FileInfo(fname);
            FileStream myFile = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            //Reads file as binary values
            BinaryReader _BinaryReader = new BinaryReader(myFile);

            //Ckeck whether user is eligible to download the file

            //Check whether file exists in specified location
            if (FileName.Exists)
            {
                try
                {
                    long startBytes = 0;
                    string lastUpdateTiemStamp = System.IO.File.GetLastWriteTimeUtc(fname).ToString("r");
                    string str = System.IO.Path.GetFileName(fname);
                    string _EncodedData = HttpUtility.UrlEncode(str, Encoding.UTF8) + lastUpdateTiemStamp;

                    Response.Clear();
                    Response.Buffer = false;
                    Response.AddHeader("Accept-Ranges", "bytes");
                    Response.AppendHeader("ETag", "\"" + _EncodedData + "\"");
                    Response.AppendHeader("Last-Modified", lastUpdateTiemStamp);
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName.Name);
                    Response.AddHeader("Content-Length", (FileName.Length - startBytes).ToString());
                    Response.AddHeader("Connection", "Keep-Alive");
                    Response.ContentEncoding = Encoding.UTF8;

                    //Send data
                    _BinaryReader.BaseStream.Seek(startBytes, SeekOrigin.Begin);

                    //Dividing the data in 1024 bytes package
                    int maxCount = (int)Math.Ceiling((FileName.Length - startBytes + 0.0) / 1024);

                    //Download in block of 1024 bytes
                    int i;
                    for (i = 0; i < maxCount && Response.IsClientConnected; i++)
                    {
                        Response.BinaryWrite(_BinaryReader.ReadBytes(1024));
                        Response.Flush();
                    }

                    // if blocks transfered not equals total number of blocks
                    if (i < maxCount)
                        return false;

                    return true;

                }
                catch
                {
                    return false;
                }
                finally
                {
                    _BinaryReader.Close();
                    myFile.Close();
                    _BinaryReader = null;
                    FileName = null;
                    GC.Collect();

                    //* delete temporary folders *//
                    string FileCopyPath = ConfigurationManager.AppSettings["DownloadPath"]; //* source to download
                    string username = Request.Params["username"];
                    if (string.IsNullOrEmpty(username))
                    {
                        username = "Anything";
                    }

                    if (Directory.Exists(FileCopyPath + username))
                    { Directory.Delete(FileCopyPath + username, true); }

                    //* delete temporary zip file *//
                    if (File.Exists(FileCopyPath + username + ".zip"))
                    { File.Delete(FileCopyPath + username + ".zip"); } //* delete zip file *//

                    Response.End();
                }
            }

            return false;
        }
    }
}