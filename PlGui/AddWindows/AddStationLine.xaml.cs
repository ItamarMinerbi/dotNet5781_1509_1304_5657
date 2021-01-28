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
    /// Interaction logic for AddStationLine.xaml
    /// </summary>
    public partial class AddStationLine : Window
    {
        static IBL BL = BlFactory.GetBL();
        BackgroundWorker worker = new BackgroundWorker();
        int[] workerValues = { 0 };
        string workerResultTitle, workerResultContent;
        int ID, Index;
        public string Result { get; private set; }

        public AddStationLine(int Id, int index)
        {
            InitializeComponent();

            ID = Id;
            Index = index;
            txtID.Text = Id.ToString();
            txtIndex.Text = index.ToString();
            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (workerResultTitle == "StationDoesNotExist")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "Station Does Not Exist",
                    "Station error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
            else if (workerResultTitle == "LineDoesNotExist")
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
            else if (workerResultTitle == "StationLineDoesNotExist")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "A Station In The Line Does Not Exist",
                    "Station line error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR);
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) this.IsEnabled = true;
            }
            else if (workerResultTitle == "StationLineAlreadyExist")
            {
                CustomMessageBox messageBox = new CustomMessageBox(
                    workerResultContent,
                    "The Station In The Line Already Exist",
                    "Add station line error",
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
                    "Station Added To The Line",
                    "Add line",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.Vi);
                Result = "Added";
                this.IsEnabled = false;
                if (messageBox.ShowDialog() == false) { this.IsEnabled = true; this.Close(); }
            }
            txtCode.Clear();
            workerResultTitle = "";
            workerResultContent = "";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try { BL.AddStationLine(ID, workerValues[0], Index); }
            catch(StationDoesNotExistException ex) { workerResultTitle = "StationDoesNotExist"; workerResultContent = ex.Message; }
            catch(LineDoesNotExistException ex) { workerResultTitle = "LineDoesNotExist"; workerResultContent = ex.Message; }
            catch(StationLineDoesNotExistException ex) { workerResultTitle = "StationLineDoesNotExist"; workerResultContent = ex.Message; }
            catch(StationLineAlreadyExistException ex) { workerResultTitle = "StationLineAlreadyExist"; workerResultContent = ex.Message; }
            catch(Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtCode.Text))
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
                workerValues[0] = int.Parse(txtCode.Text);
                worker.RunWorkerAsync();
            }
            catch(Exception ex)
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

        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                           e.Key == Key.Back);
        }
    }
}
