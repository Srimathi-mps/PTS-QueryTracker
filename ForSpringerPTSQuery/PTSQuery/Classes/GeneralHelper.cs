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

namespace QueryTrackerSetUp
{
    public class GeneralHelper
    {

        public static string FormatArticlePIINumber(string piiNumber)
        {
            string piiNumberFormat = string.Empty;
            piiNumberFormat = ConfigurationManager.AppSettings["ArticlePIINumberFormat"].ToString().Trim();

            char[] piiNumberArr = piiNumber.ToCharArray();
            char[] piiNumberFormatArr = piiNumberFormat.ToCharArray();

            int j = 0, k = 0;
            for (int i = 0; i < piiNumberFormatArr.Length; i++)
            {
                if (piiNumberFormatArr[i] == '#')
                {
                    if (piiNumberArr.Length > j)
                    {
                        piiNumberFormatArr[i] = piiNumberArr[j];
                        j++;
                    }
                }
                k++;
            }

            string formattedPIINumber = new string(piiNumberFormatArr);

            return formattedPIINumber;
        }

        public static string FormatIssuePIINumber(string piiNumber)
        {
            string piiNumberFormat = string.Empty;
            piiNumberFormat = ConfigurationManager.AppSettings["IssuePIINumberFormat"].ToString().Trim();

            char[] piiNumberArr = piiNumber.ToCharArray();
            char[] piiNumberFormatArr = piiNumberFormat.ToCharArray();

            int j = 0, k = 0;
            for (int i = 0; i < piiNumberFormatArr.Length; i++)
            {
                if (piiNumberFormatArr[i] == '#')
                {
                    if (piiNumberArr.Length > j)
                    {
                        piiNumberFormatArr[i] = piiNumberArr[j];
                        j++;
                    }
                }
                k++;
            }

            string formattedPIINumber = new string(piiNumberFormatArr);

            return formattedPIINumber;
        }

        /// <summary>
        /// Remove Special Char
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialChars(string str)
        {
            // string[] chars = new string[] { "/", "#", "$", "%", "^", "&", "'", "\"", ";", "\r", "\n" };

            string[] chars = new string[] { "\r", "\n" };
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "");
                }
            }

            str = str.Replace(@"\", @"\\");
            str = str.Replace("\"", "\\\"");
            str = str.Replace("\'", "\\\'");

            str = str.Replace(@"\\n", @"\n");
            return str;
        }
    }
}
