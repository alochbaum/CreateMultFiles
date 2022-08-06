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
        static public String GetPresetReplace(String strTitle)
        {
            String strReturn = "#^blank# error getting data";
            SQLiteConnection m_dbConnection = new SQLiteConnection();
            string strDBFile = DBFile();
            if (File.Exists(strDBFile))
            {
                m_dbConnection.ConnectionString = "Data Source=" + strDBFile + ";Version=3;";
                m_dbConnection.Open();
                string sql = "Select PresetReplace from Preset where PresetTitle ='"
                                + strTitle + "' order by PresetTitle limit 1;";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                        strReturn = reader.GetString(0);
                }

                reader.Close();
                m_dbConnection.Close();
            }
            return strReturn;
        }
    }
}
