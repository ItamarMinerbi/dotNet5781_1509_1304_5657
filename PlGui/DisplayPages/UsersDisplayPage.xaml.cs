using BlApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlGui
{
    public partial class UsersDisplayPage : Page
    {
        static IBL BL = BlFactory.GetBL();
        ObservableCollection<BO.User> Users = new ObservableCollection<BO.User>();
        BackgroundWorker loadUsersWorker = new BackgroundWorker();

        public UsersDisplayPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            dgrUsers.IsEnabled = grdUpdate.IsEnabled = false;
            pbarLoad.Visibility = Visibility.Visible;
            pbarLoad.Value = 0;
            string workerResultTitle = "", workerResultContent = "";
            
            loadUsersWorker.WorkerReportsProgress = true;
            loadUsersWorker.WorkerSupportsCancellation = true;
            loadUsersWorker.ProgressChanged += (sender, e) => pbarLoad.Value = e.ProgressPercentage;
            loadUsersWorker.DoWork += (sender, e) =>
            {
                Users = new ObservableCollection<BO.User>();
                int i = 0, count = 0;
                try { count = BL.UsersCount(); }
                catch (Exception ex) { workerResultTitle = "XmlError"; workerResultContent = ex.Message; }
                foreach (var user in BL.GetUsers())
                    try { Users.Add(user); loadUsersWorker.ReportProgress(++i * 100 / count); }
                    catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
            };
            loadUsersWorker.RunWorkerCompleted += (sender, e) =>
            {
                if (workerResultTitle == "XmlError")
                {
                    new CustomMessageBox(
                        workerResultContent,
                        "File Error",
                        "Files error",
                        CustomMessageBox.Buttons.OK,
                        CustomMessageBox.Icons.FILE).ShowDialog();
                }
                else if (workerResultTitle == "UnknownError")
                {
                    new CustomMessageBox(
                        workerResultContent,
                        "Unknown Error",
                        "Unknown error",
                        CustomMessageBox.Buttons.IGNORE,
                        CustomMessageBox.Icons.ERROR).ShowDialog();
                }
                dgrUsers.ItemsSource = Users;
                pbarLoad.Visibility = Visibility.Hidden;
                dgrUsers.IsEnabled = grdUpdate.IsEnabled = true;
                workerResultTitle = "";
            };
            loadUsersWorker.RunWorkerAsync();
        }

        private void grdUpdate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadUsers();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!loadUsersWorker.IsBusy)
            {
                var text = (sender as TextBox).Text;
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(Users);
                if (collectionView != null)
                {
                    dgrUsers.ItemsSource = collectionView;
                    collectionView.Filter = (object o) =>
                    {
                        BO.User p = o as BO.User;
                        if (p == null) return false;
                        if (p.Username.Contains(text)) return true;
                        if (p.Email.Contains(text)) return true;
                        return false;
                    };
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
