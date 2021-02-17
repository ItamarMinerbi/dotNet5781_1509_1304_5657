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
    /// Interaction logic for StationPanel.xaml
    /// </summary>
    public partial class StationPanel : Window
    {
        static IBL BL = BlFactory.GetBL();

        public bool IsOpened { get; private set; } = true;
        ObservableCollection<BO.LineTiming> lineTimings = new ObservableCollection<BO.LineTiming>();
        BackgroundWorker stationPanelWorker = new BackgroundWorker();
        Thread stationPanelBackgroundThread = null;
        int StationCode = 1;

        public StationPanel(BO.Station station)
        {
            InitializeComponent();

            yellowSign.DataContext = station;
            StationCode = station.StationCode;
            this.Title += $" - {station.Name}, {StationCode}";

            stationPanelWorker.WorkerReportsProgress = true;
            stationPanelWorker.WorkerSupportsCancellation = true;
            stationPanelWorker.DoWork += StationPanelWorker_DoWork;
            stationPanelWorker.ProgressChanged += StationPanelWorker_ProgressChanged;
            stationPanelWorker.RunWorkerAsync();
        }

        private void StationPanelWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var line = (BO.LineTiming)e.UserState;
            if (line.ID == -1) lblLastLine.Text = "Last line: " + line.LineNumber;
            else if (line.ID == -2) lblLastLine.Text = "";
            else
            {
                int index = lineTimings.ToList().FindIndex(i => i.ID == line.ID &&
                                                       i.StartTime == line.StartTime);
                if (index != -1) lineTimings[index] = line;
                else lineTimings.Add(line);

                for (int i = 0; i < lineTimings.Count; i++)
                    if (lineTimings[i].ArrivalTime <= TimeSpan.Zero)
                        lineTimings.RemoveAt(i);

                lineTimings = new ObservableCollection<BO.LineTiming>(lineTimings.OrderBy(x => x.ArrivalTime));
                ObservableCollection<BO.LineTiming> topFive = new ObservableCollection<BO.LineTiming>();
                for (int i = 0; i < Math.Min(5, lineTimings.Count); i++) topFive.Add(lineTimings[i]);
                lstLineTimings.ItemsSource = topFive;
            }
        }

        private void StationPanelWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            stationPanelBackgroundThread = Thread.CurrentThread;
            BL.SetStationPanel(StationCode, (line) =>
            {
                try
                {
                    stationPanelWorker.ReportProgress(1, line);
                }
                catch (Exception) { }
            });
            while (!stationPanelWorker.CancellationPending)
                try { Thread.Sleep(Timeout.Infinite); }
                catch (ThreadInterruptedException) { }
            BL.SetStationPanel(StationCode, (line) => { }, -1);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            stationPanelWorker?.CancelAsync();
            stationPanelBackgroundThread?.Interrupt();
            IsOpened = false;
        }
    }
}