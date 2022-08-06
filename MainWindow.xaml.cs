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
        public MainWindow()
        {
            InitializeComponent();
            // Adding the version number to the title
            MainWdw.Title = "Create Multiple Files with Subsitution version " + Assembly.GetExecutingAssembly().GetName().Version;
            List<string> lsPresetTitles = CCMultSqlite.GetPresetTitles();
            cbPresets.ItemsSource = lsPresetTitles;
            cbPresets.SelectedIndex = 0;
            rtbStatus.AppendText("Program Started \r\n");
        }

        private void cbPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rtbReplace.Document.Blocks.Clear();
            rtbReplace.AppendText(CCMultSqlite.GetPresetReplace(cbPresets.SelectedItem.ToString()));
        }

 
        private void btExportDB_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                string strFile = saveFileDialog.FileName;
                if (File.Exists(strFile))
                {
                    MessageBoxResult result = MessageBox.Show($"Overwrite {strFile}", "File Exists", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.Cancel) goto SkipCopy;
                }
                File.Copy(GetDbLocation(), strFile, true);
            }
        SkipCopy:;
        }
        private void btImportDB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog()==true)
            {
                File.Copy(openFileDialog.FileName, GetDbLocation(), true);
            }
        }
        private void btNewPreset_Click(object sender, RoutedEventArgs e)
        {
            WinPreset PopWin = new WinPreset();
            bool? boolPopWin = PopWin.ShowDialog();
        }

        private string GetDbLocation()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.DataDirectory + @"\CreateMult.sqlite";
            }
            return @"C:\Users\AndyL\source\repos\CreateMultFiles\CreateMult.sqlite";
        }



    }
}
