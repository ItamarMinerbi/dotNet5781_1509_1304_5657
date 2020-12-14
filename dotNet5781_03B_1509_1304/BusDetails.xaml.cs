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
using System.Windows.Threading;

namespace dotNet5781_03B_1509_1304
{
    /// <summary>
    /// Interaction logic for BusDetails.xaml
    /// </summary>
    public partial class BusDetails : Window
    {
        BusExtensions currentBus;

        public BusDetails(BusExtensions bus)
        {
            InitializeComponent();

            currentBus = bus;
            Title = $"Bus Details: {bus.LicenseNumber}";
            lblEvent.Content = "No Event";
            BusObserver observer = new BusObserver(lblEvent);
            bus.Subscribe(observer);
            lblLicense.Content = "License Number: " + bus.LicenseNumber;
            lblKm.Content = $"{bus.Bus.km} KM since last test";
            lblMileage.Content = $"{bus.Bus.mileage} KM in total";
            lblGas.Content = $"Gas: {bus.Bus.gas} liters";
            lblStart.Content = "Start Date:\n" + bus.Bus.startActivity.ToString("dd/MM/yyyy HH:mm");
            lblLastTest.Content = "Last Test Date:\n" + bus.Bus.lastTest.ToString("dd/MM/yyyy HH:mm");
        }

        private void btnRepair_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentBus.SendToTest();
        }

        private void btnRefuel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentBus.SendToRefuelGas();
        }
    }
}