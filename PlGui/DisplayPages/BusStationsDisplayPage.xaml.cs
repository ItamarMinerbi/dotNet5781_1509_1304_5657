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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlApi;

namespace PlGui
{
    /// <summary>
    /// Interaction logic for BusStationsDisplayPage.xaml
    /// </summary>
    public partial class BusStationsDisplayPage : Page
    {
        static IBL BL = BlFactory.GetBL();
        ObservableCollection<BO.Station> Stations;

        string workerResultTitle, workerResultContent;
        BackgroundWorker worker = new BackgroundWorker();
        Action<object, DoWorkEventArgs> workerDoWorkAction;
        Action<object, RunWorkerCompletedEventArgs> workerCompletedAction;
        Action<object, ProgressChangedEventArgs> workerProgressChangedAction;

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
            pbarLoad.Visibility = Visibility.Visible;
            pbarLoad.Value = 0;
            workerProgressChangedAction = new Action<object, ProgressChangedEventArgs>(
                (object obj, ProgressChangedEventArgs args) => pbarLoad.Value = args.ProgressPercentage);
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    if (workerResultTitle == "UnknownError")
                    {
                        CustomMessageBox messageBox = new CustomMessageBox(
                            workerResultContent,
                            "Unknown Error",
                            "Unknown error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR);
                        this.IsEnabled = false;
                        if (messageBox.ShowDialog() == false) this.IsEnabled = true;
                    }
                    pbarLoad.Visibility = Visibility.Hidden;
                });
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object sender, DoWorkEventArgs e) =>
            {
                Stations = new ObservableCollection<BO.Station>();
                int i = 0, count = BL.GetStationsCount();
                foreach (var station in BL.GetStations())
                    try { Stations.Add(station); worker.ReportProgress(++i * 100 / count); }
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
            dgrStations.UnselectAll();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var Station = dgrStations.SelectedItem as BO.Station;

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
                    CustomMessageBox messageBox;
                    if (workerResultTitle == "UnknownError")
                    {
                        messageBox = new CustomMessageBox(
                            workerResultContent,
                            "Unknown Error",
                            "Unknown error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR);
                        this.IsEnabled = false;
                        if (messageBox.ShowDialog() == false) this.IsEnabled = true;
                    }
                    else
                    {
                        messageBox = new CustomMessageBox(
                            "Station removed successfuly",
                            "Action Executed Successfuly",
                            "Action Completed",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.Vi);
                        this.IsEnabled = false;
                        if (messageBox.ShowDialog() == false) { this.IsEnabled = true; LoadStations(); }
                    }
                });
            worker.RunWorkerAsync();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addStation = new AddWindows.AddStation();
            this.IsEnabled = false;
            if (addStation.ShowDialog() == false)
            {
                this.IsEnabled = true;
                if (addStation.Result == "Added") LoadStations();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
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
