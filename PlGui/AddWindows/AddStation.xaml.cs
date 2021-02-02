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
    /// Interaction logic for AddStation.xaml
    /// </summary>
    public partial class AddStation : Window
    {
        static IBL BL = BlFactory.GetBL();
        BackgroundWorker worker = new BackgroundWorker();
        string workerResultTitle, workerResultContent;
        public string Result { get; private set; }

        public AddStation()
        {
            InitializeComponent();

            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(workerResultTitle == "StationAlreadyExist")
            {
                new CustomMessageBox(
                    workerResultContent,
                    "Station Already Exist",
                    "Add station error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR).ShowDialog();
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
            else
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "Station Added",
                    "Add station",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.Vi);
                Result = "Added";
                if (messageBox.ShowDialog() == false) {  this.Close(); }
            }
            txtCode.Clear();
            txtName.Clear();
            workerResultTitle = "";
            workerResultContent = "";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Station station = e.Argument as Station;
            try { BL.AddStation(station.StationCode, station.Name, station.Latitude,
                station.Longitude, station.Address); }
            catch(StationAlreadyExistException ex) { workerResultTitle = "StationAlreadyExist"; workerResultContent = ex.Message; }
            catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(txtCode.Text) || String.IsNullOrEmpty(txtName.Text) ||
                String.IsNullOrEmpty(txtLatitude.Text) || String.IsNullOrEmpty(txtLongitude.Text))
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    "Some fields are null or empty.",
                    "Values Error",
                    "Fields error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
                
                if (messageBox.ShowDialog() == false) 
                return;
            }
            try
            {
                Station station = new Station
                {
                    StationCode = int.Parse(txtCode.Text),
                    Name = txtName.Text,
                    Address = txtAddress.Text,
                    Longitude = double.Parse(txtLongitude.Text),
                    Latitude = double.Parse(txtLatitude.Text)
                };
                worker.RunWorkerAsync(station);
            }
            catch(Exception ex)
            {
                new CustomMessageBox(
                    ex.Message,
                    "Values Error",
                    "Fields error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR).ShowDialog();
            }
        }

        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                           e.Key == Key.Back);
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