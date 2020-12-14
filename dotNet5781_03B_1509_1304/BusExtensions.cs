using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Speech.Synthesis;
using dotNet5781_01_1509_1304;

namespace dotNet5781_03B_1509_1304
{
    public class BusExtensions : IObservable<string>
    {
        public BusExtensions(Bus bus)
        {
            TimeToWaitForEvent = 0;
            worker = new BackgroundWorker();
            InitializeBackgroundWorker();
            this.bus = bus;
        }

        #region Variables
        private Bus bus = null;
        public Bus Bus { get => bus; }
        public string LicenseNumber { get => bus.LicenseNumber; }
        public enum Status { ReadyToDrive, Driving, Refueling, InTest }
        public Status status { get; private set; }
        #endregion

        #region BackgroundWorker
        private BackgroundWorker worker;
        private int TimeToWaitForEvent;
        private void InitializeBackgroundWorker()
        {
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.valueChanged(TimeSpan.FromMinutes(e.ProgressPercentage).ToString(@"hh\:mm"));
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            valueChanged("");
            valueChanged("endEvent");
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = TimeToWaitForEvent; i >= 0; i--)
            {
                Thread.Sleep(100); // wait 0.1 sec as 1 min
                worker.ReportProgress(i);
            }
        }
        #endregion

        #region SendTo Functions
        public void SendToTest()
        {
            if (!worker.IsBusy)
            {
                valueChanged("startEvent");
                TimeToWaitForEvent = 1440;  //  144 sec (0.1 = 1 sec so multiply by 10) = 24 hours of real life
                status = Status.InTest;     //  update the status
                worker.RunWorkerAsync();    //  start the time that the bus is in the test
                bus.Test();                 //  update the bus details
            }
            else if(this.status == Status.Driving)
                MessageBox.Show("The bus while traveling");
            else if (this.status == Status.InTest)
                MessageBox.Show("The bus during a test");
            else if (this.status == Status.Refueling)
                MessageBox.Show("The bus is refueling");
        }

        public void SendToRefuelGas()
        {
            if (!worker.IsBusy)
            {
                valueChanged("startEvent");
                TimeToWaitForEvent = 120;   //  12 sec (0.1 = 1 sec so multiply by 10) = 2 hours of real life
                status = Status.Refueling;  //  update the status
                worker.RunWorkerAsync();    //  start the time that the bus refuels the gas
                bus.Reful();                //  update the bus details
            }
            else if (this.status == Status.Driving)
                MessageBox.Show("The bus while traveling");
            else if (this.status == Status.InTest)
                MessageBox.Show("The bus during a test");
            else if (this.status == Status.Refueling)
                MessageBox.Show("The bus is refueling");
        }

        public void SendToDrive(int KM)
        {
            if (!worker.IsBusy)
            {
                string CanDoTheDrive = bus.CanDoTheDrive(KM);
                if(CanDoTheDrive == "can do the drive")
                {
                    valueChanged("startEvent");
                    int speed = new Random().Next(20, 50);
                    TimeToWaitForEvent = (KM / speed) * 10;   //  V * T = S  -> T = S / V
                    status = Status.Driving;           //  update the status
                    worker.RunWorkerAsync();           //  start the time that the bus is in the test
                    bus.DoTheDrive(KM);                //  update the bus details
                }
                else
                    MessageBox.Show(CanDoTheDrive);
            }
            else if (this.status == Status.Driving)
                MessageBox.Show("The bus while traveling");
            else if (this.status == Status.InTest)
                MessageBox.Show("The bus during a test");
            else if (this.status == Status.Refueling)
                MessageBox.Show("The bus is refueling");
        }
        #endregion

        #region Observers Part
        List<IObserver<string>> observers = new List<IObserver<string>>();
        List<IObserver<string>> newObservers = new List<IObserver<string>>();
        public IDisposable Subscribe(IObserver<string> observer)
        {
            observers.Add(observer);
            return new BusUnsubscriber(observers, observer);
        }
        private void valueChanged(string value)
        {
            foreach (var item in observers)
                item.OnNext(value);
        }
        #endregion
    }

    class BusUnsubscriber : IDisposable
    {
        private List<IObserver<string>> _observers;
        private IObserver<string> _observer;

        public BusUnsubscriber(List<IObserver<string>> observers, IObserver<string> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }

    class BusObserver : IObserver<string>
    {
        Label lbl = new Label();
        Image image = new Image();
        public BusObserver(Label Lbl) { lbl = Lbl; }
        public BusObserver(Image img) { image = img; }

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        public void OnNext(string value) 
        {
            if(lbl != null && !value.Contains("startEvent") && !value.Contains("endEvent"))
                lbl.Content = value;
            if (image != null && value.Contains("startEvent"))
            { image.Visibility = Visibility.Visible; }
            if (image != null && value.Contains("endEvent"))
            { image.Visibility = Visibility.Hidden; }
        }
    }
}
