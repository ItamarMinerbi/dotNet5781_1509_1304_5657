using BlApi;
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

namespace PlGui
{
    /// <summary>
    /// Interaction logic for AdjacentStationsDisplayPage.xaml
    /// </summary>
    public partial class AdjacentStationsDisplayPage : Page
    {
        static IBL BL = BlFactory.GetBL();
        ObservableCollection<BO.AdjStations> AdjStations;

        string workerResultTitle, workerResultContent;
        BackgroundWorker worker = new BackgroundWorker();
        Action<object, DoWorkEventArgs> workerDoWorkAction;
        Action<object, RunWorkerCompletedEventArgs> workerCompletedAction;
        Action<object, ProgressChangedEventArgs> workerProgressChangedAction;

        public AdjacentStationsDisplayPage()
        {
            InitializeComponent();

            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            LoadAdjacentStations();
        }

        private void LoadAdjacentStations()
        {
            dgrStations.IsEnabled = grdUpdate.IsEnabled = false;
            pbarLoad.Visibility = Visibility.Visible;
            pbarLoad.Value = 0;
            
            workerProgressChangedAction = new Action<object, ProgressChangedEventArgs>(
                (object obj, ProgressChangedEventArgs args) => pbarLoad.Value = args.ProgressPercentage);
            
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    if (workerResultTitle == "XmlError")
                    {
                        CustomMessageBox messageBox = new CustomMessageBox(
                            workerResultContent,
                            "File Error",
                            "Files error",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.FILE);
                        this.IsEnabled = false;
                        if (messageBox.ShowDialog() == false) this.IsEnabled = true;
                    }
                    else if (workerResultTitle == "UnknownError")
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
                    dgrStations.IsEnabled = grdUpdate.IsEnabled = true;
                });
            
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object sender, DoWorkEventArgs e) =>
            {
                AdjStations = new ObservableCollection<BO.AdjStations>();
                int i = 0, count = 0;
                try { count = BL.GetStationsCount(); }
                catch (Exception ex) { workerResultTitle = "XmlError"; workerResultContent = ex.Message; }
                foreach (var adjStations in BL.GetAdjStations())
                    try { AdjStations.Add(adjStations); worker.ReportProgress(++i * 100 / count); }
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
            dgrStations.ItemsSource = AdjStations;
            dgrStations?.UnselectAll();
            workerResultTitle = workerResultContent = "";
        }

        private void ComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            var cmb = sender as ComboBox;
            for (int i = 0; i <= 59; i++)
                cmb.Items.Add(String.Format("{0:00}", i));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dgrStations.UnselectAll();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var Station = dgrStations.SelectedItem as BO.AdjStations;

            dgrStations.UnselectAll();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var adjStation = dgrStations.SelectedItem as BO.AdjStations;
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object obj, DoWorkEventArgs args) =>
            {
                try { BL.RemoveAdjStations(adjStation.StationCode1, adjStation.StationCode2); }
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
                            "Adjacent station removed successfuly",
                            "Action Executed Successfuly",
                            "Action Completed",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.Vi);
                        this.IsEnabled = false;
                        if (messageBox.ShowDialog() == false) { this.IsEnabled = true; LoadAdjacentStations(); }
                    }
                });
            worker.RunWorkerAsync();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addAdjStations = new AddWindows.AddAdjStations();
            this.IsEnabled = false;
            if (addAdjStations.ShowDialog() == false)
            {
                this.IsEnabled = true;
                if (addAdjStations.Result == "Added") LoadAdjacentStations();
            }
        }

        private void grdUpdate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadAdjacentStations();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!worker.IsBusy)
            {
                var text = (sender as TextBox).Text;
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(AdjStations);
                if (collectionView != null)
                {
                    dgrStations.ItemsSource = collectionView;
                    collectionView.Filter = (object o) =>
                    {
                        BO.AdjStations p = o as BO.AdjStations;
                        if (p == null) return false;
                        if (p.StationCode1.ToString().Contains(text)) return true;
                        if (p.StationCode2.ToString().Contains(text)) return true;
                        if (p.Station1Name.Contains(text)) return true;
                        if (p.Station2Name.Contains(text)) return true;
                        if (p.Distance.ToString().Contains(text)) return true;
                        if (p.Time.ToString(@"hh\:mm\:ss").Contains(text)) return true;
                        return false;
                    };
                }
            }
        }

        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null) cb.IsTextSearchEnabled = false;
            e.Handled = !((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                           e.Key == Key.Back);
            if ((sender as ComboBox)?.Text.Length == 2)
            {
                var str = (sender as ComboBox).Text;
                (sender as ComboBox).Text = "";
                (sender as ComboBox).Text.Insert(0, str.Substring(0, 2));
            }
        }

        private void DoubleTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                           e.Key == Key.Decimal ||
                           e.Key == Key.OemPeriod ||    // ???
                           e.Key == Key.Back);
        }
    }
}
