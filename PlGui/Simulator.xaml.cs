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
    /// <summary>
    /// Interaction logic for Simulator.xaml
    /// </summary>
    public partial class Simulator : Window
    {
        private class Args
        {
            public int Rate;
            public TimeSpan StartTime;
        }

        static IBL BL = BlFactory.GetBL();

        ObservableCollection<BO.LineTiming> lineTimings = new ObservableCollection<BO.LineTiming>();
        BackgroundWorker blStationsWorker = new BackgroundWorker();
        BackgroundWorker clockWorker = new BackgroundWorker();
        Thread clockBackgroundThread = null;
        BackgroundWorker travelOperatorWorker = new BackgroundWorker();
        Thread travelOperatorBackgroundThread = null;

        public Simulator()
        {
            InitializeComponent();

            ObservableCollection<BO.Station> stations = new ObservableCollection<BO.Station>(); 
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

            travelOperatorWorker.WorkerReportsProgress = true;
            travelOperatorWorker.WorkerSupportsCancellation = true;
            travelOperatorWorker.DoWork += TravelOperatorWorker_DoWork;
            travelOperatorWorker.ProgressChanged += TravelOperatorWorker_ProgressChanged;
        }

        private void TravelOperatorWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var line = (BO.LineTiming)e.UserState;
            int index = lineTimings.ToList().FindIndex(i => i.ID == line.ID);
            if (index != -1)
            {
                if (line.ArrivalTime == TimeSpan.Zero) lineTimings.RemoveAt(index);
                else lineTimings[index] = line;
            }
            else lineTimings.Add(line);
            lstLineTimings.ItemsSource = lineTimings;
        }

        private void TravelOperatorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int stationCode = 63111;
            travelOperatorBackgroundThread = Thread.CurrentThread;
            BL.SetStationPanel(stationCode, (line) => travelOperatorWorker.ReportProgress(1, line));
            while (!travelOperatorWorker.CancellationPending)
                try { Thread.Sleep(Timeout.Infinite); }
                catch (ThreadInterruptedException) { }
            BL.SetStationPanel(-1, (line) => { });
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
                clockWorker.RunWorkerAsync(new Args
                { Rate = rate, StartTime = startTime.SelectedTime.Value.TimeOfDay });
                travelOperatorWorker.RunWorkerAsync();
                btnStartStop.Content = "Stop";
                txtRate.IsEnabled = startTime.IsEnabled = false;
                yellowSign.Visibility = grdRow1.Visibility = Visibility.Visible;
            }
            else
            {
                travelOperatorWorker.CancelAsync();
                clockWorker.CancelAsync();
                travelOperatorBackgroundThread?.Interrupt();
                clockBackgroundThread?.Interrupt();
                btnStartStop.Content = "Start";
                txtRate.IsEnabled = startTime.IsEnabled = true;
                yellowSign.Visibility = grdRow1.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            clockWorker.CancelAsync();
            clockBackgroundThread?.Interrupt();
        }

        private void lstStations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            yellowSign.DataContext = lstStations?.SelectedItem as BO.Station;
        }

        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                           e.Key == Key.Back);
        }
    }
}
