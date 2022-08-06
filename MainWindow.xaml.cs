using System;
using System.Collections.Generic;
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

        private void btNewPreset_Click(object sender, RoutedEventArgs e)
        {
            WinPreset PopWin = new WinPreset();
            bool? boolPopWin = PopWin.ShowDialog();
        }
    }
}
