﻿using Microsoft.Win32;
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
            List<string> lsPresetTitles = CCMultSqlite.GetPresetTitles();
            cbPresets.ItemsSource = lsPresetTitles;
            cbPresets.SelectedIndex = 0;
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
                rtbStatus.AppendText($"Exported DB to {saveFileDialog.FileName}\r\n");
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
                WinPreset PopWinC = new WinPreset(strTitle);
                PopWinC.strMode = "Create";
                //PopWin.LoadData(cbPresets.SelectedItem.ToString());
                //PopWin.myPreset = dPreset;
                bool? boolPopWin = PopWinC.ShowDialog();
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
                WinPreset PopWin = new WinPreset(cbPresets.SelectedItem.ToString());
                //PopWin.LoadData(cbPresets.SelectedItem.ToString());
                //PopWin.myPreset = dPreset;
                bool? boolPopWin = PopWin.ShowDialog();
        }
    }
}
