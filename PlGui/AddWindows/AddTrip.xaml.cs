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

using BlApi;
using BlExceptions;

namespace PlGui.AddWindows
{
    /// <summary>
    /// Interaction logic for AddTrip.xaml
    /// </summary>
    public partial class AddTrip : Window
    {
        static IBL BL = BlFactory.GetBL();
        BackgroundWorker worker = new BackgroundWorker();
        string workerResultTitle, workerResultContent;
        int ID;
        public string Result { get; private set; }

        public AddTrip(int Id)
        {
            InitializeComponent();

            ID = Id;
            txtID.Text = Id.ToString();
            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (workerResultTitle == "LineDoesNotExist")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "Line Does Not Exist",
                    "Line error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
            else if (workerResultTitle == "LineTripAlreadyExist")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "This Line Already Has A Trip",
                    "Add trip error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
            else if (workerResultTitle == "AnErrorOccurred")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "Unknown Error",
                    "Unknown error",
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
                    "Trip Added To The Line",
                    "Add trip",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.Vi);
                Result = "Added";
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) { this.IsEnabled = true; this.Close(); }
            }
            workerResultTitle = workerResultContent = "";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var trip = e.Argument as BO.LineTrip;
            try { BL.AddTripLine(ID, trip.StartTime, trip.EndTime, trip.Frequency); }
            catch (LineDoesNotExistException ex) { workerResultTitle = "LineDoesNotExist"; workerResultContent = ex.Message; }
            catch (LineTripAlreadyExistException ex) { workerResultTitle = "LineTripAlreadyExist"; workerResultContent = ex.Message; }
            catch (AnErrorOccurredException ex) { workerResultTitle = "AnErrorOccurred"; workerResultContent = ex.Message; }
            catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BO.LineTrip trip = new BO.LineTrip
                {
                    ID = int.Parse(txtID.Text),
                    StartTime = TimeSpan.Parse($"{cmbHoursS.SelectedItem}:" +
                                        $"{cmbMinS.SelectedItem}:{cmbSecS.SelectedItem}"),
                    EndTime = TimeSpan.Parse($"{cmbHoursE.SelectedItem}:" +
                                        $"{cmbMinE.SelectedItem}:{cmbSecE.SelectedItem}"),
                    Frequency = TimeSpan.Parse($"{cmbHoursF.SelectedItem}:" +
                                        $"{cmbMinF.SelectedItem}:{cmbSecF.SelectedItem}")
                };
                worker.RunWorkerAsync(trip);
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
            e.Handled = !((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                           e.Key == Key.Back);
        }
    }
}
