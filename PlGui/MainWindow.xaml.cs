using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

using BlApi;
using BlExceptions;
using BO;
using PlGuiSecure;

namespace PlGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static IBL BL = BlFactory.GetBL();
        BackgroundWorker worker = new BackgroundWorker();
        UserDisplay userReply;
        string workerResultTitle;
        string workerResultContent;
        public MainWindow()
        {
            InitializeComponent();

            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (userReply != null && userReply.IsAdmin)
            {
                new ManageWindow(userReply.Username).Show();
                this.Close();
            }
            else if(userReply != null && !userReply.IsAdmin)
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    "You can not enter to this part of the system because you are not an admin!",
                    "You are not an administrator",
                    "Login error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.INFO);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
            else if (workerResultTitle == "UserDoesNotExist")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                        workerResultContent,
                        "Values error",
                        "Login error",
                        CustomMessageBox.Buttons.OK,
                        CustomMessageBox.Icons.USERNAME);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
            else
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                        workerResultContent,
                        "Unknown error occured",
                        "Login error",
                        CustomMessageBox.Buttons.IGNORE,
                        CustomMessageBox.Icons.ERROR);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
            txtPassword.Clear();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            User user = e.Argument as User;
            try { userReply = BL.TryLogin(user.Name, user.Password); }
            catch (UserDoesNotExistException ex) { workerResultTitle = "UserDoesNotExist"; workerResultContent = ex.Message; }
            catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            User user = new User() { 
                Name = txtUsername.Text,
                Password = txtPassword.Password.CreateMD5()
            };
            worker.RunWorkerAsync(user);
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPass.Content = (sender as PasswordBox).Password;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new Register().Show();
            this.Close();
        }
    }
}
