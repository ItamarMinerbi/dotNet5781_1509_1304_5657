using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

using dotNet5781_01_1509_1304;

namespace dotNet5781_03B_1509_1304
{
    /// <summary>
    /// Interaction logic for AddBus.xaml
    /// </summary>
    public partial class AddBus : Window
    {
        public AddBus()
        {
            InitializeComponent();

            calStartDate.SelectedDate = DateTime.Now;

            txtGas.PreviewTextInput += PreviewTextInput_AllTextBoxes;
            txtKM.PreviewTextInput += PreviewTextInput_AllTextBoxes;
            txtMileage.PreviewTextInput += PreviewTextInput_AllTextBoxes;
            
            txtGas.TextChanged += TextChanged_AllTextBoxes;
            txtKM.TextChanged += TextChanged_AllTextBoxes;
            txtMileage.TextChanged += TextChanged_AllTextBoxes;
        }

        private void TextBoxPasting_AllTextBoxes(object sender, DataObjectPastingEventArgs e)
        {
            string value = e.DataObject.GetData(typeof(string)) as string;
            foreach (var letter in value)
            {
                if (!Char.IsDigit(letter))
                { 
                    e.CancelCommand();
                    break;
                }
            }
        }

        private void TextChanged_AllTextBoxes(object sender, TextChangedEventArgs e)
        {
            if((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = "0";
            }
        }

        private void PreviewTextInput_AllTextBoxes(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]))
                e.Handled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
        }

        private void chbNewBus_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chbNewBus.IsChecked)
            {
                calLastTestDate.Visibility = Visibility.Hidden;
                lblLastTestDate.Visibility = Visibility.Hidden;
                lblKM.Visibility = Visibility.Hidden;
                txtKM.Visibility = Visibility.Hidden;
                lblMileage.Visibility = Visibility.Hidden;
                txtMileage.Visibility = Visibility.Hidden;
                lblGas.Visibility = Visibility.Hidden;
                txtGas.Visibility = Visibility.Hidden;
            }
            else
            {
                calLastTestDate.Visibility = Visibility.Visible;
                lblLastTestDate.Visibility = Visibility.Visible;
                lblKM.Visibility = Visibility.Visible;
                txtKM.Visibility = Visibility.Visible;
                lblMileage.Visibility = Visibility.Visible;
                txtMileage.Visibility = Visibility.Visible;
                lblGas.Visibility = Visibility.Visible;
                txtGas.Visibility = Visibility.Visible;
            }
        }

        private string LicenseNumber(string text) =>
            (calStartDate.DisplayDate.Year > 2018) ? text.Insert(3, "-").Insert(6, "-") : text.Insert(2, "-").Insert(6, "-");

        private Bus CreateBus() =>
            (bool)chbNewBus.IsChecked ? new Bus(LicenseNumber(txtLicenseNumber.Text), calStartDate.DisplayDate) :
                new Bus(LicenseNumber(txtLicenseNumber.Text), calStartDate.DisplayDate, calLastTestDate.DisplayDate,
                                int.Parse(txtKM.Text), int.Parse(txtMileage.Text), int.Parse(txtGas.Text));

        private void btnAddBus_Click(object sender, RoutedEventArgs e)
        {
            Bus bus = CreateBus();
            BusExtensions Bus = new BusExtensions(bus);
            
            //   Leave the current thread and switch to main thread
            Application.Current.Dispatcher.Invoke(new Action(() => 
            {
                List<BusExtensions> buses = (List<BusExtensions>)(((MainWindow)Application.Current.MainWindow).lbBuses.ItemsSource);
                ((MainWindow)Application.Current.MainWindow).lbBuses.ItemsSource = null;
                buses.Add(Bus);
                ((MainWindow)Application.Current.MainWindow).lbBuses.ItemsSource = buses;
            }));
            this.Close();
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtLicenseNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int len = calStartDate.DisplayDate.Year > 2018 ? 8 : 7;
            if (!Char.IsDigit(e.Text[0]))
                e.Handled = true;
            else
                e.Handled = txtLicenseNumber.Text.Length >= len;
        }

        private void txtLicenseNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            int len = calStartDate.DisplayDate.Year > 2018 ? 8 : 7;
            btnAddBus.IsEnabled = txtLicenseNumber.Text.Length == len;
        }

        private void calStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            calLastTestDate.SelectedDate = null;
            calLastTestDate.DisplayDateEnd = calStartDate.SelectedDate.Value.AddDays(-1);
            calLastTestDate.DisplayDateStart = calStartDate.SelectedDate.Value.AddYears(-1);
        }
    }
}