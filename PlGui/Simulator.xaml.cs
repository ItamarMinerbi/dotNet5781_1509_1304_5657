using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

using BlApi;

namespace PlGui
{
    public partial class Simulator : Window
    {
        private class Args
        {
            public int Rate;
            public TimeSpan StartTime;
        }

        static IBL BL = BlFactory.GetBL();

        BackgroundWorker blStationsWorker = new BackgroundWorker();
        BackgroundWorker clockWorker = new BackgroundWorker();
        Thread clockBackgroundThread = null;
        List<StationPanel> panels = new List<StationPanel>();
        ObservableCollection<BO.Station> stations = new ObservableCollection<BO.Station>();

        public Simulator()
        {
            InitializeComponent();

            blStationsWorker.DoWork += (sender, e) =>
            {
                foreach (var station in BL.GetStations())
                {
                    try { stations.Add(station); }
                    catch (Exception) { }
                }
            };
            blStationsWorker.RunWorkerCompleted += (sender, e) => lstStations.ItemsSource = stations;
            blStationsWorker.RunWorkerAsync();

            clockWorker.WorkerReportsProgress = true;
            clockWorker.WorkerSupportsCancellation = true;
            clockWorker.DoWork += ClockWorker_DoWork;
            clockWorker.ProgressChanged += ClockWorker_ProgressChanged;
        }

        private void ClockWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblTime.Content = ((TimeSpan)e.UserState).ToString(@"hh\:mm\:ss");
        }

        private void ClockWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (Args)e.Argument;
            clockBackgroundThread = Thread.CurrentThread;
            BL.StartSimulator(args.StartTime, args.Rate, (time) => clockWorker.ReportProgress(1, time));
            while (!clockWorker.CancellationPending)
                try { Thread.Sleep(Timeout.Infinite); }
                catch (ThreadInterruptedException) { }
            BL.StopSimulator();
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            int rate = 1;
            if (!startTime.SelectedTime.HasValue || !int.TryParse(txtRate.Text,
                    out rate) || rate <= 0)
                return;
            lstStations.SelectedItem = null;
            if ((string)btnStartStop.Content == "Start")
            {
                clockWorker?.RunWorkerAsync(new Args
                { Rate = rate, StartTime = startTime.SelectedTime.Value.TimeOfDay });
                btnStartStop.Content = "Stop";
                txtRate.IsEnabled = startTime.IsEnabled = false;
            }
            else
            {
                clockWorker?.CancelAsync();
                clockBackgroundThread?.Interrupt();
                btnStartStop.Content = "Start";
                txtRate.IsEnabled = startTime.IsEnabled = true;
                lblTime.Content = "00:00:00";
                ClosePanels();
            }
        }

        private void ClosePanels()
        {
            foreach (var panel in panels)
                if (panel.IsOpened)
                    panel.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            clockWorker?.CancelAsync();
            clockBackgroundThread?.Interrupt();
            ClosePanels();
        }

        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                           e.Key == Key.Back);
        }

        private void lstStations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedStation = lstStations.SelectedItem as BO.Station;
            if (clockWorker.IsBusy)
            {
                if (selectedStation != null)
                {
                    StationPanel stationPanel = new StationPanel(selectedStation);
                    panels.Add(stationPanel);
                    stationPanel.Show();
                }
                else
                    new CustomMessageBox(
                        "Selected item is null.",
                        "Selected error.",
                        "Null Error",
                        CustomMessageBox.Buttons.IGNORE,
                        CustomMessageBox.Icons.ERROR).ShowDialog();
            }
            else
                new CustomMessageBox(
                        "The simulator clock is not working.\n" +
                            "Please turn on the simulator for follow this station.",
                        "Simulator error.",
                        "Simulator Error",
                        CustomMessageBox.Buttons.OK,
                        CustomMessageBox.Icons.SETTINGS).ShowDialog();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!blStationsWorker.IsBusy)
            {
                var text = (sender as TextBox).Text;
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(stations);
                if (collectionView != null)
                {
                    lstStations.ItemsSource = collectionView;
                    collectionView.Filter = (object o) =>
                    {
                        BO.Station p = o as BO.Station;
                        if (p == null) return false;
                        if (p.StationCode.ToString().Contains(text)) return true;
                        if (p.Name.Contains(text)) return true;
                        return false;
                    };
                }
            }
        }
    }
}
