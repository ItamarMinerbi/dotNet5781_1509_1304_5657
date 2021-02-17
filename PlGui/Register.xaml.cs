using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using BlApi;
using BlExceptions;
using BO;
using PlGuiSecure;

namespace PlGui
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        static IBL BL = BlFactory.GetBL();
        BackgroundWorker worker = new BackgroundWorker();
        string[] workerArguments = { "Name", "Email", "Pass1", "Pass2" };
        string workerResultTitle;
        string workerResultContent;

        public Register()
        {
            InitializeComponent();

            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(workerResultTitle == "InvalidUsername")
            {
                new CustomMessageBox(
                    workerResultContent,
                    "Username Error",
                    "Register error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.USERNAME).ShowDialog();
            }
            else if(workerResultTitle == "InvalidPassword")
            {
                new CustomMessageBox(
                    workerResultContent,
                    "Password Error",
                    "Register error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.USERNAME).ShowDialog();
            }
            else if (workerResultTitle == "InvalidEmail")
            {
                new CustomMessageBox(
                    workerResultContent,
                    "Email Address Error",
                    "Register error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.MAIL).ShowDialog();
            }
            else if (workerResultTitle == "UnknownError")
            {
                new CustomMessageBox(
                        workerResultContent,
                        "Unknown error occured",
                        "Register error",
                        CustomMessageBox.Buttons.IGNORE,
                        CustomMessageBox.Icons.ERROR).ShowDialog();
            }
            else
            {
                new CustomMessageBox(
                        $"Congratulations! Registration succeeded! We sent an email to this" +
                        $" address: {workerArguments[1]}, you can take a look!",
                        "You are currently registered!",
                        "Registration succeeded",
                        CustomMessageBox.Buttons.OK,
                        CustomMessageBox.Icons.Vi).ShowDialog();
                new MainWindow().Show();
                this.Close();
            }
            txtUsername.Clear();
            txtPassword.Clear();
            txtPassword2.Clear();
            txtEmail.Clear();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            { BL.CreateUser(workerArguments[0], workerArguments[1], workerArguments[2], workerArguments[3]); }
            catch (InvalidUsernameException ex) { workerResultTitle = "InvalidUsername"; workerResultContent = ex.Message; }
            catch (InvalidPasswordException ex) { workerResultTitle = "InvalidPassword"; workerResultContent = ex.Message; }
            catch (InvalidEmailException ex) { workerResultTitle = "InvalidEmail"; workerResultContent = ex.Message; }
            catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPass.Content = (sender as PasswordBox).Password;
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            workerArguments[0] = txtUsername.Text;
            workerArguments[1] = txtEmail.Text;
            workerArguments[2] = txtPassword.Password.CreateMD5();
            workerArguments[3] = txtPassword2.Password.CreateMD5();
            worker.RunWorkerAsync();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}
