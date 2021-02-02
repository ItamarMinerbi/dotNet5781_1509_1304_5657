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
    /// Interaction logic for AddLine.xaml
    /// </summary>
    public partial class AddLine : Window
    {
        static IBL BL = BlFactory.GetBL();
        BackgroundWorker worker = new BackgroundWorker();
        int[] workerValues = { 0,0,0,0 };
        string workerResultTitle, workerResultContent;
        public string Result { get; private set; }

        public AddLine()
        {
            InitializeComponent();

            worker.DoWork += Worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            cmbArea.ItemsSource = Enum.GetValues(typeof(BO.Line.Areas));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(workerResultTitle == "StationDoesNotExist")
            {
                new CustomMessageBox(
                    workerResultContent,
                    "Station Does Not Exist",
                    "Station error",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.ERROR).ShowDialog();
            }
            else if(workerResultTitle == "LineAlreadyExist")
            {
                new CustomMessageBox(
                    workerResultContent,
                    "Line Already Exist",
                    "Add line error",
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
                    "Line Added",
                    "Add line",
                    CustomMessageBox.Buttons.OK,
                    CustomMessageBox.Icons.Vi);
                Result = "Added";
                if (messageBox.ShowDialog() == false) this.Close();
            }
            txtNumber.Clear();
            txtFirstCode.Clear();
            txtLastCode.Clear();
            workerResultTitle = workerResultContent = "";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BO.Line.Areas area = (BO.Line.Areas)workerValues[3];
            try { BL.AddLine(workerValues[0], workerValues[1], workerValues[2], area); }
            catch(StationDoesNotExistException ex) { workerResultTitle = "StationDoesNotExist"; workerResultContent = ex.Message; }
            catch (LineAlreadyExistException ex) { workerResultTitle = "LineAlreadyExist"; workerResultContent = ex.Message; }
            catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtNumber.Text) || String.IsNullOrEmpty(txtFirstCode.Text) ||
                String.IsNullOrEmpty(txtLastCode.Text))
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
                workerValues[0] = int.Parse(txtNumber.Text);
                workerValues[1] = int.Parse(txtFirstCode.Text);
                workerValues[2] = int.Parse(txtLastCode.Text);
                workerValues[3] = cmbArea.SelectedIndex;
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
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
    }
}
