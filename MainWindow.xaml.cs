using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CreateMultFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CPreset dPreset; 
        public MainWindow()
        {
            InitializeComponent();
            // Adding the version number to the title
            MainWdw.Title = "Create Multiple Files with Subsitution version " + Assembly.GetExecutingAssembly().GetName().Version;
            UpdatePresetList();
            rtbStatus.AppendText("Program Started \r\n");
        }

        private void cbPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rtbReplace.Document.Blocks.Clear();
            dPreset = CCMultSqlite.GetDPreset(cbPresets.SelectedItem.ToString());
            rtbReplace.AppendText(dPreset.Replace+"\r\n"); 
            rtbStatus.AppendText(cbPresets.SelectedItem.ToString() + "\r\n");
        }

 
        private void btExportDB_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQLite|*.sqlite|All Files|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.Copy(GetDbLocation(), saveFileDialog.FileName, true);
                rtbStatus.AppendText($"Exported DB to: {saveFileDialog.FileName}\r\n");
            } else { rtbStatus.AppendText("DB Export Cancelled"); }
        }
        private void btImportDB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQLite|*.sqlite|All Files|*.*";
            if(openFileDialog.ShowDialog()==true)
            {
                File.Copy(openFileDialog.FileName, GetDbLocation(), true);
                rtbStatus.AppendText($"Imported Presets from: {openFileDialog.FileName}\r\n");
            }
            else { rtbStatus.AppendText("DB Import Cancelled"); }
        }
        private void btNewPreset_Click(object sender, RoutedEventArgs e)
        {
            string strTitle = cbPresets.SelectedItem.ToString();
            if (strTitle == "A Welcome Preset") MessageBox.Show("You can't create from 'A Welcome Preset' select another", "Please select another preset");
            else
            {
                WinPreset PopWinC = new WinPreset(strTitle,"Create");
                bool? boolPopWin = PopWinC.ShowDialog();
                if (boolPopWin==true) {
                    rtbStatus.AppendText("Created new preset, second step is to select it and click Update Replace.\r\n");
                    UpdatePresetList();
                }
            }
        }

        private string GetDbLocation()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.DataDirectory + @"\CreateMult.sqlite";
            }
            return @"C:\Users\AndyL\source\repos\CreateMultFiles\CreateMult.sqlite";
        }

        private void btView_Click(object sender, RoutedEventArgs e)
        {
            WinPreset PopWin = new WinPreset(cbPresets.SelectedItem.ToString(),"End View");
            bool? boolPopWin = PopWin.ShowDialog();
        }

        private void UpdatePresetList()
        {
            cbPresets.SelectedIndex = 0;
            List<string> lsPresetTitles = CCMultSqlite.GetPresetTitles();
            cbPresets.ItemsSource = lsPresetTitles;
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            string strTitle = cbPresets.SelectedItem.ToString();
            if (strTitle == "A Welcome Preset") MessageBox.Show("You can't delete from 'A Welcome Preset' select another", "Please select another preset");
            else if (strTitle == "empty") MessageBox.Show("You can't delete from 'empty' select another", "Please select another preset");
            else
            {
                MessageBoxResult mbResult = MessageBox.Show($"Go ahead and delete {strTitle}", "Are you sure", MessageBoxButton.OKCancel);
                if (mbResult == MessageBoxResult.OK) {
                    bool bResult = CCMultSqlite.DeleteDPreset(strTitle);
                    if (bResult) rtbStatus.AppendText($"Deleted Preset: {strTitle}");
                    UpdatePresetList();
                }
            }
        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            string strTitle = cbPresets.SelectedItem.ToString();
            bool bResult = CCMultSqlite.UpdateDPreset(strTitle, ConvertReplaceTB());
            if (bResult) rtbStatus.AppendText($"Updated Preset: {strTitle} with the shown replace data.\r\n");
        }

        private string ConvertReplaceTB()
        {
            TextRange textRange = new TextRange(rtbReplace.Document.ContentStart, rtbReplace.Document.ContentEnd);
            return textRange.Text;
        }

        private void btProcess_Click(object sender, RoutedEventArgs e)
        {
            string strOutput,strFileOut;
            strOutput = strFileOut = string.Empty;
            List<string> lRows = new List<string>();
            
            // adding in top if no blank
            if (!dPreset.Top.StartsWith("#^Blank#")) strOutput += dPreset.Top;;
            
            // Converting rtbReplace in to list of strings dropping comment rows
            using (StringReader reader = new StringReader(ConvertReplaceTB()))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        if (!line.StartsWith("#^comment")) lRows.Add(line);
                    }

                } while (line != null);
            }

            // Skipping if empty
            Int64 iCount = lRows.Count;

            if ((iCount < 1)||(lRows[0].StartsWith("#^blank")))
            {
                rtbStatus.AppendText($"No Action: Program didn't find any action lines in the replace field \r\n");
                return;
            }
  
            rtbStatus.AppendText($"Row Count of Replace {iCount}\r\n");

            bool blIsOutputing = false;
            // looping for output files
            foreach (string row in lRows)
            {
                if (row.StartsWith("#^file#"))
                {
                    if (!blIsOutputing)
                    {
                        string[] rArray = row.Split('|');
                        rtbStatus.AppendText($"Starting Recording {rArray[1]}\r\n");
                        strFileOut = rArray[1];
                        blIsOutputing = true;
                    } else
                    {
                        closeOut(strFileOut, strOutput);
                        if (!dPreset.Top.StartsWith("#^blank#")) strOutput = dPreset.Top;
                        else strOutput = String.Empty;
                        blIsOutputing = false;

                    }
                } else
                {
                    string[] rArray = row.Split('|');
                    string strMiddle = dPreset.Middle;
                    string strField = string.Empty;
                    int iReplace = 1;
                    foreach(string s in rArray)
                    {
                        if (s.Length > 0)
                        {
                            strField = $"#^field{iReplace:D2}#";
                            strMiddle.Replace(strField, s);
                        }
                    }
                    strOutput += strMiddle;
                }
            }
        }
        private void closeOut(string strFileOut,string strOutput)
        {
            if (!dPreset.Bottom.StartsWith("#^blank#")) strOutput += dPreset.Bottom;
            try
            {
                File.WriteAllText(strFileOut, strOutput);
                rtbStatus.AppendText($"Wrote out: {strFileOut}\r\n");
            }
            catch (Exception e)
            {
                rtbStatus.AppendText($"Failed writing: {strFileOut} {e.Message}\r\n");
            }
        }
    }
}
