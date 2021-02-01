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

namespace PlGui
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public string Result { get; private set; }
        public enum Buttons { OK, YESNO, IGNORE }
        public enum Icons { Vi, ERROR, WARNING, QUESTION, INFO, USERNAME, MAIL, EDIT, FILE }
        public CustomMessageBox(string Text, string Caption, string Title = "Message Box", Buttons button = Buttons.OK, Icons icon = Icons.INFO)
        {
            InitializeComponent();

            Result = "Hello again!";
            txtText.Text = Text;
            txtCaption.Text = Caption;
            this.Title = Title;
            switch (button)
            {
                case Buttons.OK:
                    btnOk.Visibility = Visibility.Visible;
                    break;
                case Buttons.YESNO:
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    break;
                case Buttons.IGNORE:
                    btnIgnore.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            switch (icon)
            {
                case Icons.Vi:
                    vbxVi.Visibility = Visibility.Visible;
                    break;
                case Icons.ERROR:
                    vbxError.Visibility = Visibility.Visible;
                    break;
                case Icons.WARNING:
                    vbxWarning.Visibility = Visibility.Visible;
                    break;
                case Icons.QUESTION:
                    vbxQuestion.Visibility = Visibility.Visible;
                    break;
                case Icons.INFO:
                    vbxInfo.Visibility = Visibility.Visible;
                    break;
                case Icons.USERNAME:
                    vbxUsername.Visibility = Visibility.Visible;
                    break;
                case Icons.MAIL:
                    vbxMail.Visibility = Visibility.Visible;
                    break;
                case Icons.EDIT:
                    vbxEdit.Visibility = Visibility.Visible;
                    break;
                case Icons.FILE:
                    vbxFile.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Result = (sender as Button).Content.ToString();
            this.Close();
        }
    }
}
