using System;
using System.Collections.Generic;
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

namespace dotNet5781_03B_1509_1304
{
    /// <summary>
    /// Interaction logic for TakeADrive.xaml
    /// </summary>
    public partial class TakeADrive : Window
    {
        BusExtensions currentBus = null;

        public TakeADrive(BusExtensions bus)
        {
            InitializeComponent();

            Title = $"Send bus: {bus.LicenseNumber} to a drive";
            currentBus = bus;
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
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

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = "0";
            }
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]))
                e.Handled = true;
        }

        private void txtDistance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            { 
                currentBus.SendToDrive(int.Parse(txtDistance.Text));
                this.Close();
            }
        }
    }
}
