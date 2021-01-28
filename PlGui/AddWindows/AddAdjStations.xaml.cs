using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

using BO;
using BlApi;
using BlExceptions;

namespace PlGui.AddWindows
{
    /// <summary>
    /// Interaction logic for AddAdjStations.xaml
    /// </summary>
    public partial class AddAdjStations : Window
    {
        static IBL BL = BlFactory.GetBL();
        BackgroundWorker worker = new BackgroundWorker();
        string workerResultTitle, workerResultContent;
        public string Result { get; private set; }

        public AddAdjStations()
        {
            InitializeComponent();

            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (workerResultTitle == "AdjStationsAlreadyExist")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "This Adjacent Stations Already Exist",
                    "Add adjacent station error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
            else if (workerResultTitle == "StationDoesNotExist")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "Station Does Not Exist",
                    "Find station error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
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
            else
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "Adjacent Stations Added",
                    "Add adjacent station",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.Vi);
                Result = "Added";
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) { this.IsEnabled = true; this.Close(); }
            }
            txtCode1.Clear();
            txtCode2.Clear();
            txtDistance.Clear();
            workerResultTitle = workerResultContent = "";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            AdjStations adjStation = e.Argument as AdjStations;
            try { BL.AddAdjStations(adjStation.StationCode1, adjStation.StationCode2,
                                    adjStation.Distance, adjStation.Time); }
            catch (StationDoesNotExistException ex) { workerResultTitle = "StationDoesNotExist"; workerResultContent = ex.Message; }
            catch (AdjStationsAlreadyExistException ex) { workerResultTitle = "AdjStationsAlreadyExist"; workerResultContent = ex.Message; }
            catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtCode1.Text) || String.IsNullOrEmpty(txtCode2.Text) ||
                String.IsNullOrEmpty(txtDistance.Text))
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    "Some fields are null or empty.",
                    "Values Error",
                    "Fields error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
                return;
            }
            try
            {
                var adjStations = new AdjStations
                {
                    StationCode1 = int.Parse(txtCode1.Text),
                    StationCode2 = int.Parse(txtCode2.Text),
                    Distance = double.Parse(txtDistance.Text),
                    Time = TimeSpan.Parse($"{cmbHours.SelectedItem}:{cmbMin.SelectedItem}:{cmbSec.SelectedItem}")
                };
                worker.RunWorkerAsync(adjStations);
            }
            catch (Exception ex) 
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    ex.Message,
                    "Values Error",
                    "Fields error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
        }

        private void ComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            var cmb = sender as ComboBox;
            for (int i = 0; i <= 59; i++)
                cmb.Items.Add(String.Format("{0:00}", i));
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
