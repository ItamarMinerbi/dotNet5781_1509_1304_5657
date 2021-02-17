using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlApi;

namespace PlGui
{
    public partial class BusStationsDisplayPage : Page
    {
        static IBL BL = BlFactory.GetBL();
        ObservableCollection<BO.Station> Stations;

        string workerResultTitle, workerResultContent, txtName = "", txtAddress = "";
        BackgroundWorker worker = new BackgroundWorker();
        Action<object, DoWorkEventArgs> workerDoWorkAction;
        Action<object, RunWorkerCompletedEventArgs> workerCompletedAction;
        Action<object, ProgressChangedEventArgs> workerProgressChangedAction;
        FrameworkElement dataGridRowDetails;
        bool IsRowDetailsOpen = true;

        public BusStationsDisplayPage()
        {
            InitializeComponent();

            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            LoadStations();
        }

        private void LoadStations()
        {
            dgrStations.ItemsSource = new ObservableCollection<BO.Station>();
            dgrStations.IsEnabled = grdUpdate.IsEnabled = false;
            pbarLoad.Visibility = Visibility.Visible;
            pbarLoad.Value = 0;
            workerProgressChangedAction = new Action<object, ProgressChangedEventArgs>(
                (object obj, ProgressChangedEventArgs args) =>
                {
                    pbarLoad.Value = args.ProgressPercentage;
                    Stations.Add((BO.Station)args.UserState);
                });
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
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
                    pbarLoad.Visibility = Visibility.Hidden;
                    dgrStations.IsEnabled = grdUpdate.IsEnabled = true;
                });
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object sender, DoWorkEventArgs e) =>
            {
                Stations = new ObservableCollection<BO.Station>();
                int i = 0, count = 0;
                try { count = BL.GetStationsCount(); }
                catch (Exception ex) { workerResultTitle = "XmlError"; workerResultContent = ex.Message; }
                foreach (var station in BL.GetStations())
                    try { worker.ReportProgress(++i * 100 / count, station); }
                    catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
            });
            worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            workerProgressChangedAction?.Invoke(sender, e);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            workerDoWorkAction?.Invoke(sender, e);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            workerCompletedAction?.Invoke(sender, e);
            dgrStations.ItemsSource = Stations;
            dgrStations?.UnselectAll();
            workerResultTitle = workerResultContent = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsRowDetailsOpen = false;
            dataGridRowDetails?.BeginAnimation(HeightProperty,
                                new DoubleAnimation(0, TimeSpan.Parse("0:0:0.5")));
            BackgroundWorker wait = new BackgroundWorker();
            wait.DoWork += (s, args) => Thread.Sleep(500);
            wait.RunWorkerCompleted += (s, args) => dgrStations.UnselectAll();
            wait.RunWorkerAsync();
        }

        private void dgrStations_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            dataGridRowDetails = (Grid)e.DetailsElement;
            if (IsRowDetailsOpen)
                dataGridRowDetails?.BeginAnimation(HeightProperty,
                    new DoubleAnimation(400, TimeSpan.Parse("0:0:0.5")));
            IsRowDetailsOpen = true;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var Station = dgrStations.SelectedItem as BO.Station;
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object obj, DoWorkEventArgs args) =>
            {
                try
                {
                    BL.UpdateStation(Station.StationCode, txtName,Station.Latitude,
                                          Station.Longitude, txtAddress);
                }
                catch (BlExceptions.StationDoesNotExistException ex) { workerResultTitle = "StationDoesNotExist"; workerResultContent = ex.Message; }
                catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
            });
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    CustomMessageBox messageBox;
                    if (workerResultTitle == "UnknownError")
                    {
                        messageBox = new CustomMessageBox(
                            workerResultContent,
                            "Unknown Error",
                            "Unknown error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR);
                        messageBox.ShowDialog();
                    }
                    else if(workerResultTitle == "StationDoesNotExist")
                    {
                        messageBox = new CustomMessageBox(
                            workerResultContent,
                            "Station Does Not Exist",
                            "Station Error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.EDIT);
                        messageBox.ShowDialog();
                    }
                    else
                    {
                        messageBox = new CustomMessageBox(
                            "Station updated successfuly",
                            "Action Executed Successfuly",
                            "Action Completed",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.Vi);
                        if (messageBox.ShowDialog() == false) LoadStations();
                    }
                });
            worker.RunWorkerAsync();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var Station = dgrStations.SelectedItem as BO.Station;
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object obj, DoWorkEventArgs args) =>
            {
                try { BL.RemoveStation(Station.StationCode); }
                catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
            });
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    if (workerResultTitle == "UnknownError")
                    {
                        new CustomMessageBox(
                            workerResultContent,
                            "Unknown Error",
                            "Unknown error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR).ShowDialog();
                    }
                    else
                    {
                        CustomMessageBox messageBox = new CustomMessageBox(
                            "Station removed successfuly",
                            "Action Executed Successfuly",
                            "Action Completed",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.Vi);
                        
                        if (messageBox.ShowDialog() == false) LoadStations();
                    }
                });
            worker.RunWorkerAsync();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addStation = new AddWindows.AddStation();
            
            if (addStation.ShowDialog() == false && addStation.Result == "Added")
                LoadStations();
        }

        private void grdUpdate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadStations();
        }

        private void txtName_Loaded(object sender, RoutedEventArgs e)
        {
            txtName = (dgrStations.SelectedItem as BO.Station)?.Name;
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtName = (sender as TextBox)?.Text;
        }

        private void txtAddress_Loaded(object sender, RoutedEventArgs e)
        {
            txtAddress = (dgrStations.SelectedItem as BO.Station)?.Address;
        }

        private void txtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtAddress = (sender as TextBox)?.Text;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!worker.IsBusy)
            {
                var text = (sender as TextBox).Text;
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(Stations);
                if (collectionView != null)
                {
                    dgrStations.ItemsSource = collectionView;
                    collectionView.Filter = (object o) =>
                    {
                        BO.Station p = o as BO.Station;
                        if (p == null) return false;
                        if (p.StationCode.ToString().Contains(text)) return true;
                        if (p.Name.Contains(text)) return true;
                        if (p.Address.ToString().Contains(text)) return true;
                        if (p.Latitude.ToString().Contains(text)) return true;
                        if (p.Longitude.ToString().Contains(text)) return true;
                        return false;
                    };
                }
            }
        }
    }
}