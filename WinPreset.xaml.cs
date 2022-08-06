using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CreateMultFiles
{

    /// <summary>
    /// Interaction logic for WinPreset.xaml
    /// </summary>
    public partial class WinPreset : Window
    {
        public string strMode { get; set; } = "OK";
        public WinPreset(string inString, string Mode)
        {
            InitializeComponent();
            strMode = Mode;
            btAccept.Content = strMode;
            CPreset myPreset = CCMultSqlite.GetDPreset(inString);
            tbTitle.Text = myPreset.Title;
            rtbTop.AppendText(myPreset.Top);
            rtbMiddle.AppendText(myPreset.Middle);
            rtbBottom.AppendText(myPreset.Bottom);
            rtbReplace.AppendText(myPreset.Replace);
        }

        private void btAccept_Click(object sender, RoutedEventArgs e)
        {
            bool bResult = true;
            if (strMode == "Create")
            {
                bResult=CCMultSqlite.PutDPreset(Serialize());
            }
            this.DialogResult = bResult;
        }
        private CPreset Serialize()
        {
            CPreset returnPS = new CPreset();
            returnPS.Title = tbTitle.Text;
            returnPS.Top = ConvertRichTextBox(rtbTop);
            returnPS.Middle = ConvertRichTextBox(rtbMiddle);
            returnPS.Bottom = ConvertRichTextBox(rtbBottom);
            returnPS.Replace = ConvertRichTextBox(rtbReplace);
            return returnPS;
        }
        private string ConvertRichTextBox(RichTextBox myTB)
        {
            TextRange textRange = new TextRange(myTB.Document.ContentStart,myTB.Document.ContentEnd);
            return textRange.Text;
        }
    }
}
