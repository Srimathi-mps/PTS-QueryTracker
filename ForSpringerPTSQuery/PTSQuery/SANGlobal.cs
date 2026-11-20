using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ServerConfiguration;

namespace PTS
{
    public class SANGlobal
    {

        private static clsServerConfiguration _serverConfig;

        public static clsServerConfiguration ServerConfig
        {
            get { return _serverConfig; }
            set { _serverConfig = value; }
        }
    }
}
