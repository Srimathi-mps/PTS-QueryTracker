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
using System.Data.SqlClient;

/// <summary>
/// Summary description for SqlHelper
/// </summary>
public class SqlHelper
{

    //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconectionstring_Books_New"].ConnectionString);
    SqlConnection con;
    SqlCommand cmd;
    SqlDataAdapter da;
    DataSet ds = new DataSet();

    public string Query = "";

    public SqlHelper()
    {


    }
    private static SqlConnection Get_connection(int Con_Status)
    {
        string Connection_Name = "";
        if (Con_Status == 1) { Connection_Name = "myconectionstring_Books_New"; }
        else if (Con_Status == 2) { Connection_Name = "jetsConstrNew"; }
        SqlConnection con1 = new SqlConnection(ConfigurationManager.ConnectionStrings[Connection_Name].ConnectionString);

        return con1;
    }

    public DataSet ExcuteDataSet(string PassedQuery, int Con_Status)
    {
        try
        {
            con = Get_connection(Con_Status);
            Query = PassedQuery;

            if (con.State == ConnectionState.Closed) { con.Open(); }

            cmd = new SqlCommand(Query, con);
            da = new SqlDataAdapter(cmd);
            da.Fill(ds);
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
                con.Close();

            con = null;
        }
        return ds;
    }


    public DataSet Grid_ds(string Sp_Name, string Field_Name, int Jourcode, string Field_Value, int Con_Status)
    {
        try
        {
            con = Get_connection(Con_Status);
            Query = Sp_Name;

            if (con.State == ConnectionState.Closed) { con.Open(); }

            cmd = new SqlCommand(Query, con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            if (Query == "USP_Insert_CopyPIT_Value")
            {
                cmd.Parameters.AddWithValue("@Type", Field_Name);
                cmd.Parameters.AddWithValue("@Copy_PIT_Value", Field_Value);

            }
            else
            {
                cmd.Parameters.AddWithValue("@Jourcode", Jourcode);
                cmd.Parameters.AddWithValue("@CustomerName", Field_Name);
                cmd.Parameters.AddWithValue("@Article", Field_Value);
            }

            da = new SqlDataAdapter(cmd);
            da.Fill(ds);
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            if (con.State == ConnectionState.Open)
                con.Close();

            con = null;
        }
        return ds;
    }
    public string RemoveSpecialChars(string str)
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
    public void BindData(DropDownList Get_Bindvalue, string query, int Con_Status)
    {
        try
        {
            ds.Reset();
            ds = ExcuteDataSet(query, Con_Status);
            Get_Bindvalue.DataTextField = "Field_Name";
            Get_Bindvalue.DataValueField = "Field_Value";
            Get_Bindvalue.DataSource = ds;
            Get_Bindvalue.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
            // ShowMessage(ex.Message);
        }
    }

    public DataTable GetDataTable(string query, int Con_Status)
    {
        try
        {
            ds.Reset();
            ds = ExcuteDataSet(query, Con_Status);
            return ds.Tables[0];
        }
        catch (Exception ex)
        {
            throw ex;
            //ShowMessage(ex.Message);
        }
    }
    public void BindData_select(HtmlSelect Select1, string query, int Con_Status)
    {
        try
        {
            ds.Reset();
            ds = ExcuteDataSet(query, Con_Status);
            Select1.DataTextField = "Field_Name";
            Select1.DataValueField = "Field_Value";
            Select1.DataSource = ds;
            Select1.DataBind();
        }
        catch (Exception ex)
        {
            //ShowMessage(ex.Message);
            throw ex;
        }
    }
    public void BindData_select(HtmlSelect Select1, DataTable dt, int Con_Status)
    {
        try
        {

            Select1.DataTextField = "FieldId";
            Select1.DataValueField = "FieldId";
            Select1.DataSource = dt;
            Select1.DataBind();
        }
        catch (Exception ex)
        {
            //ShowMessage(ex.Message);
            throw ex;
        }
    }
    public void Bind_GridView(GridView Get_Grid, DataSet Bind_ds)
    {
        try
        {
            Get_Grid.DataSource = Bind_ds;
            Get_Grid.DataBind();
            Get_Grid.Visible = true;
        }
        catch (Exception ex)
        {
            //ShowMessage(ex.Message);
            throw ex;
        }
    }
    public DataSet GetDataSetFromSP(string procedureName, string[] parametervalues, int con_status)
    {
        SqlConnection sqlcon = Get_connection(con_status);

        DataSet resultTable = new DataSet();

        try
        {
            SqlCommand sqlcmd = new SqlCommand(procedureName, sqlcon);
            SqlCommand cmd = new SqlCommand(procedureName, sqlcon);

            sqlcmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandType = CommandType.StoredProcedure;

            if (sqlcon.State == ConnectionState.Closed)
            { sqlcon.Open(); }

            SqlDataAdapter adp = new SqlDataAdapter(sqlcmd);
            SqlCommandBuilder.DeriveParameters(cmd);

            int i = 0;

            foreach (SqlParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.Input)
                {
                    //if (parametervalues[i].Trim() == string.Empty)
                    //    sqlcmd.Parameters.AddWithValue(p.ParameterName, DBNull.Value);
                    //else
                    sqlcmd.Parameters.AddWithValue(p.ParameterName, parametervalues[i]);
                    i++;
                }
            }

            adp.Fill(resultTable);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (sqlcon.State == ConnectionState.Open)
            { sqlcon.Close(); }
            sqlcon = null;
        }

        return resultTable;
    }
    public int GetExecuteFromSP(string procedureName, string[] parametervalues, int con_status)
    {
        SqlConnection sqlcon = Get_connection(con_status);

        int resultTable = 0;

        try
        {
            SqlCommand sqlcmd = new SqlCommand(procedureName, sqlcon);
            SqlCommand cmd = new SqlCommand(procedureName, sqlcon);

            sqlcmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandType = CommandType.StoredProcedure;

            if (sqlcon.State == ConnectionState.Closed)
            { sqlcon.Open(); }

            SqlDataAdapter adp = new SqlDataAdapter(sqlcmd);
            SqlCommandBuilder.DeriveParameters(cmd);

            int i = 0;

            foreach (SqlParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.Input)
                {
                    //if (parametervalues[i].Trim() == string.Empty)
                    //    sqlcmd.Parameters.AddWithValue(p.ParameterName, DBNull.Value);
                    //else
                    sqlcmd.Parameters.AddWithValue(p.ParameterName, parametervalues[i]);
                    i++;
                }
            }

            resultTable = sqlcmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (sqlcon.State == ConnectionState.Open)
            { sqlcon.Close(); }
            sqlcon = null;
        }

        return resultTable;
    }
}
