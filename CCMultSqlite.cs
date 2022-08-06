using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateMultFiles
{
    class CCMultSqlite
    {
        static public string DBFile()
        {
            // For the next line not to error you need to add System.Deployment reference
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.DataDirectory + @"\CreateMult.sqlite";
            }
            return @"C:\Users\AndyL\source\repos\CreateMultFiles\CreateMult.sqlite";
        }
        static public List<string> GetPresetTitles()
        {
            List<string> mList = new List<string>();
            SQLiteConnection m_dbConnection = new SQLiteConnection();
            string strDBFile = DBFile();
            if (File.Exists(strDBFile))
            {
                m_dbConnection.ConnectionString = "Data Source=" + strDBFile + ";Version=3;";
                m_dbConnection.Open();
                string sql = "select PresetTitle from Preset order by PresetTitle;";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    mList.Add(reader["PresetTitle"].ToString());
                reader.Close();
                m_dbConnection.Close();
            }
            return mList;
        }
        static public CPreset GetDPreset(String strTitle)
        {
            CPreset returnPreset = new CPreset();
            SQLiteConnection m_dbConnection = new SQLiteConnection();
            string strDBFile = DBFile();
            if (File.Exists(strDBFile))
            {
                m_dbConnection.ConnectionString = "Data Source=" + strDBFile + ";Version=3;";
                m_dbConnection.Open();
                string sql = "select PresetTitle,PresetTop,PresetMiddle,PresetBottom,PresetReplace from Preset where PresetTitle ='"
                                + strTitle + "'order by PresetTitle limit 1;";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    returnPreset.Title = reader.GetString(0);
                    returnPreset.Top = reader.GetString(1);
                    returnPreset.Middle = reader.GetString(2);
                    returnPreset.Bottom = reader.GetString(3);
                    returnPreset.Replace = reader.GetString(4);
                }
                    reader.Close();
                m_dbConnection.Close();
            }
            return returnPreset;
        }
        static public bool PutDPreset(CPreset pPreset)
        {
            bool blreturn = false;
            SQLiteConnection m_dbConnection = new SQLiteConnection();
            string strDBFile = DBFile();
            if (File.Exists(strDBFile))
            {
                m_dbConnection.ConnectionString = "Data Source=" + strDBFile + ";Version=3;";
                m_dbConnection.Open();
                string sql = "INSERT INTO Preset(\"PresetTitle\",\"PresetTop\",\"PresetMiddle\",\"PresetBottom\",\"PresetReplace\")"
                    + "VALUES (@Title,@Top,@Middle,@Bottom,@Replace);";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                // protected from single quotes in the passed strings
                command.Parameters.Add(new SQLiteParameter("Title", pPreset.Title));
                command.Parameters.Add(new SQLiteParameter("Top", pPreset.Top));
                command.Parameters.Add(new SQLiteParameter("Middle", pPreset.Top));
                command.Parameters.Add(new SQLiteParameter("Bottom", pPreset.Top));
                command.Parameters.Add(new SQLiteParameter("Replace", pPreset.Top));
                int rows = command.ExecuteNonQuery();
                if (rows == 1) blreturn = true;
            }
            return blreturn;
        }
        static public bool DeleteDPreset(string strTitle)
        {
            bool blreturn = false;
            SQLiteConnection m_dbConnection = new SQLiteConnection();
            string strDBFile = DBFile();
            if (File.Exists(strDBFile))
            {
                m_dbConnection.ConnectionString = "Data Source=" + strDBFile + ";Version=3;";
                m_dbConnection.Open();
                string sql = "DELETE FROM Preset WHERE PresetTitle='" + strTitle + "';";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                int rows = command.ExecuteNonQuery();
                if (rows == 1) blreturn = true;
            }
            return blreturn;
        }
        static public bool UpdateDPreset(string strTitle,string strReplace)
        {
            bool blreturn = false;
            SQLiteConnection m_dbConnection = new SQLiteConnection();
            string strDBFile = DBFile();
            if (File.Exists(strDBFile))
            {
                m_dbConnection.ConnectionString = "Data Source=" + strDBFile + ";Version=3;";
                m_dbConnection.Open();
                string sql = "UPDATE Preset SET PresetReplace=@Replace WHERE PresetTitle=@Title";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.Parameters.Add(new SQLiteParameter("Title", strTitle));
                command.Parameters.Add(new SQLiteParameter("Replace", strReplace));
                int rows = command.ExecuteNonQuery();
                if (rows == 1) blreturn = true;
            }
            return blreturn;
        }
    }
}
