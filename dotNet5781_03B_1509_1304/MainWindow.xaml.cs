using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Speech.Synthesis;
using dotNet5781_01_1509_1304;

namespace dotNet5781_03B_1509_1304
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<BusExtensions> buses;
        SpeechSynthesizer speech;

        public MainWindow()
        {
            InitializeComponent();

            buses = new List<BusExtensions>();
            StartValues(buses);
            lbBuses.ItemsSource = buses;

            #region Speak  Welcome message and a short tutorial about the system
            speech = new SpeechSynthesizer();
            speech.Rate = 0;
            try { speech.SelectVoiceByHints(VoiceGender.Female); }
            catch (Exception) { speech.SetOutputToDefaultAudioDevice(); }
            string text = "Hello! A little tutorial: You can add a bus in the button in the bottom left corner of the screen <break time='1000ms' /> " +
                "You can refuel, take a ride, or send for a test, with the buttons on the right side of the bus license number <break time='1000ms' /> " +
                "To see the bus details, double-click on the line where the bus is <break time='1000ms' /> " +
                "Have a fun experience!";
            string SsmlText = $@"<speak version=""1.0"" xmlns=""http://www.w3.org/2001/10/synthesis"" xmlns:mstts=""https://www.w3.org/2001/mstts"" xml:lang=""en-US"">
                                    <voice name=""en-US-AriaNeural"">
                                        {text}
                                    </voice>
                                </speak>";
            speech.SpeakSsmlAsync(SsmlText);
            #endregion
        }

        static Random rnd = new Random();
        public static DateTime GetRandomDate(DateTime from, DateTime to)
        {
            TimeSpan range = new TimeSpan(to.Ticks - from.Ticks);
            return from + new TimeSpan((long)(range.Ticks * rnd.NextDouble()));
        }

        /// <summary>
        /// Add buses to the list by random values
        /// </summary>
        /// <param name="Buses"> The list of the buses </param>
        void StartValues(List<BusExtensions> Buses)
        {
            Random rand = new Random();
            DateTime from = new DateTime(year: 2010, month: 1, day: 1, hour: 0, minute: 0, second: 0);
            DateTime to = DateTime.Now;

            List<string> license = new List<string>();
            int count = rand.Next(10, 100);
            for (int i = 0; i < count; i++)
            {
                DateTime randomDate = GetRandomDate(from, to);
                DateTime startDate = randomDate;
                DateTime lastTritmentDate = GetRandomDate(randomDate, randomDate.AddYears(1));
                int Mileage = startDate.Year % 100 * rand.Next(1000, 10000);
                int Km = 0;
                do { Km = rand.Next(5, 19823); } while (Km > Mileage);
                int Gas = rand.Next(0, 400);
                string LicenseNumber = "";
                do {
                    if (randomDate.Year > 2018)
                        LicenseNumber = String.Format("{0:000}-{1:00}-{2:000}",
                            rand.Next(100, 999), rand.Next(0, 99), rand.Next(0, 999));
                    else
                        LicenseNumber = String.Format("{0:00}-{1:000}-{2:00}",
                            rand.Next(10, 99), rand.Next(0, 999), rand.Next(0, 99));
                } while (license.Contains(LicenseNumber));
                license.Add(LicenseNumber);
                Buses.Add(new BusExtensions(new Bus(LicenseNumber, startDate, lastTritmentDate, Km, Mileage, Gas)));
            }
        }

        private void lbBuses_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var bus = lbBuses.SelectedItem as BusExtensions;
            new BusDetails(bus).Show();
        }

        private void btnRefuel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //   Refuel bus
            Image image = sender as Image;
            var bus = image.DataContext as BusExtensions;            //  Get the current bus from the element
            bus.SendToRefuelGas();
        }

        private void btnRepair_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //   Repair bus
            Image image = sender as Image;
            var bus = image.DataContext as BusExtensions;            //  Get the current bus from the element
            bus.SendToTest();
        }

        private void btnTakeDrive_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //   Take a drive
            Image image = sender as Image;
            var bus = image.DataContext as BusExtensions;            //  Get the current bus from the element
            new TakeADrive(bus).Show();
        }

        private void btnAddBus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //   Add bus
            Thread thread = new Thread(() => {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                new AddBus().Show();
                Dispatcher.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void lblListBox_Loaded(object sender, RoutedEventArgs e)
        {
            //   Subscribe the show event label to his BusObserver
            Label lbl = sender as Label;
            var bus = lbl.DataContext as BusExtensions;
            BusObserver observer = new BusObserver(lbl);
            bus.Subscribe(observer);
        }

        private void imgIcon_Loaded(object sender, RoutedEventArgs e)
        {
            //   Subscribe the show event icon to his BusObserver
            Image image = sender as Image;
            BusObserver observer = new BusObserver(image);
            var bus = image.DataContext as BusExtensions;
            bus.Subscribe(observer);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //   if the speech is speaking, cancle
            speech.Dispose();
        }
    }
}