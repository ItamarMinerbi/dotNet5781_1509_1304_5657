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
using System.Windows.Navigation;
using System.Windows.Shapes;

using dotNet5781_02_1509_1304;

namespace dotNet5781_03A_1509_1304
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BusLinesController controller;
        List<BusStationLine> stations;

        public MainWindow()
        {
            InitializeComponent();

            controller = new BusLinesController();
            stations = new List<BusStationLine>();
            StartValues(stations, controller);

            cbBusLines.ItemsSource = controller;
            cbBusLines.DisplayMemberPath = "BusLine";
            cbBusLines.SelectedIndex = 0;
        }

        private dotNet5781_02_1509_1304.Line currentDisplayBusLine;

        private void ShowBusLine(string index)
        {
            try
            {
                currentDisplayBusLine = controller[index];
                UpGrid.DataContext = currentDisplayBusLine;

                lstBusLinesStations.DataContext = currentDisplayBusLine.Stations;
                
            }catch(Exception e) { MessageBox.Show(e.Message); }
        }

        static void StartValues(List<BusStationLine> stations, BusLinesController controller)
        {
            Random rand = new Random();
            for (int i = 0; i < 40; i++)
            {
                int stationCode = 0;
                do { stationCode = rand.Next(100000, 999999); }
                while (stations.Exists(x => x.StationCode == stationCode.ToString()));

                stations.Add(new BusStationLine(
                    StationCode: stationCode.ToString(),
                    Latitude: rand.NextDouble() * (35.5 - 34.3) + 34.3,
                    Longitude: rand.NextDouble() * (33.3 - 31.0) + 31.0,
                    Distance: rand.NextDouble() * 100.0,
                    Time: rand.Next(1, 1440),
                    Address: ""));
            }

            for (int i = 0; i < 10; i++)
            {
                int lineNumber = rand.Next(1, 999);
                do { lineNumber = rand.Next(1, 999); }
                while (controller.BusLines.Exists(x => x.BusLine == lineNumber.ToString()));

                List<BusStationLine> list = new List<BusStationLine>();

                List<int> indexes = new List<int>();
                int count = rand.Next(10, 20);
                for (int j = 0; j < count; j++)
                {
                    int randIndex = rand.Next(0, stations.Count);
                    do { randIndex = rand.Next(0, stations.Count); }
                    while (indexes.Contains(randIndex));
                    list.Add(stations[randIndex]);
                    indexes.Add(randIndex);
                }
                int area = rand.Next(0, 5);
                controller.AddLine(new dotNet5781_02_1509_1304.Line("" + lineNumber, list, (dotNet5781_02_1509_1304.Line.Areas)area));
            }
        }

        private void cbBusLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowBusLine((cbBusLines.SelectedValue as dotNet5781_02_1509_1304.Line).BusLine);
        }
    }
}
