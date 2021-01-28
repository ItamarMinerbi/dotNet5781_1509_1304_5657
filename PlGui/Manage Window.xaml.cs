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

namespace PlGui
{
    /// <summary>
    /// Interaction logic for Manage_Window.xaml
    /// </summary>
    public partial class ManageWindow : Window
    {
        public ManageWindow(string Username)
        {
            InitializeComponent();

            lblBuses.Foreground = Brushes.Orange;
            icoBuses.Fill = Brushes.Orange;
            lblName.Text = Username;
        }

        private void busesGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            frmDisplay.Source = new Uri("DisplayPages/BusesDisplayPage.xaml", UriKind.Relative);
            icoBuses.Fill = lblBuses.Foreground = Brushes.Orange;
            icoLines.Fill = lblLines.Foreground = Brushes.White;
            icoStations.Fill = lblStations.Foreground = Brushes.White;
            icoAdjStations.Fill = lblAdjStations.Foreground = Brushes.White;
        }

        private void linesGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            frmDisplay.Source = new Uri("DisplayPages/LinesDisplayPage.xaml", UriKind.Relative);
            icoBuses.Fill = lblBuses.Foreground = Brushes.White;
            icoLines.Fill = lblLines.Foreground = Brushes.Orange;
            icoStations.Fill = lblStations.Foreground = Brushes.White;
            icoAdjStations.Fill = lblAdjStations.Foreground = Brushes.White;
        }

        private void busStationsGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            frmDisplay.Source = new Uri("DisplayPages/BusStationsDisplayPage.xaml", UriKind.Relative);
            icoBuses.Fill = lblBuses.Foreground = Brushes.White;
            icoLines.Fill = lblLines.Foreground = Brushes.White;
            icoStations.Fill = lblStations.Foreground = Brushes.Orange;
            icoAdjStations.Fill = lblAdjStations.Foreground = Brushes.White;
        }

        private void adjacentStationsGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            frmDisplay.Source = new Uri("DisplayPages/AdjacentStationsDisplayPage.xaml", UriKind.Relative);
            icoBuses.Fill = lblBuses.Foreground = Brushes.White;
            icoLines.Fill = lblLines.Foreground = Brushes.White;
            icoStations.Fill = lblStations.Foreground = Brushes.White;
            icoAdjStations.Fill = lblAdjStations.Foreground = Brushes.Orange;
        }

        private void imgLogout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void grdShowMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (grdMenu.Visibility == Visibility.Visible) grdMenu.Visibility = Visibility.Collapsed;
            else grdMenu.Visibility = Visibility.Visible;
        }
    }
}
