using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
        string strAudioPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\")) + @"Audio\";

        public ManageWindow(string Username)
        {
            InitializeComponent();

            lblName.Text = Username;

            using (SoundPlayer spAudio = new SoundPlayer($@"{strAudioPath}LoggedIn.wav"))
                spAudio.Play();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "Buses":
                    frmDisplay.Source = new Uri("DisplayPages/BusesDisplayPage.xaml", UriKind.Relative);
                    break;
                case "Lines":
                    frmDisplay.Source = new Uri("DisplayPages/LinesDisplayPage.xaml", UriKind.Relative);
                    break;
                case "Stations":
                    frmDisplay.Source = new Uri("DisplayPages/BusStationsDisplayPage.xaml", UriKind.Relative);
                    break;
                case "AdjStations":
                    frmDisplay.Source = new Uri("DisplayPages/AdjacentStationsDisplayPage.xaml", UriKind.Relative);
                    break;
                case "File":
                    frmDisplay.Source = new Uri("DisplayPages/FileManagerPage.xaml", UriKind.Relative);
                    break;
                case "lviSimulator":
                    new Simulator().ShowDialog();
                    break;
                default:
                    break;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            using (SoundPlayer spAudio = new SoundPlayer($@"{strAudioPath}LoggedOut.wav"))
                spAudio.Play();
        }
    }
}
