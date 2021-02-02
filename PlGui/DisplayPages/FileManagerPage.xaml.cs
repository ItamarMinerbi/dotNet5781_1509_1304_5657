using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using BO;
using BlApi;
using LiveCharts.Wpf;
using LiveCharts;

namespace PlGui.DisplayPages
{
    /// <summary>
    /// Interaction logic for FileManagerPage.xaml
    /// </summary>
    public partial class FileManagerPage : Page
    {
        static IBL BL = BlFactory.GetBL();
        BackgroundWorker worker = new BackgroundWorker();
        ObservableCollection<DisplayFile> files = new ObservableCollection<DisplayFile>();
        DisplayCounts counts = new DisplayCounts();

        public FileManagerPage()
        {
            InitializeComponent();

            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            this.IsEnabled = false;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgrFiles.ItemsSource = files;
            memoryChart.Series = new SeriesCollection();
            foreach (var file in files)
            {
                memoryChart.Series.Add(new PieSeries
                {
                    Title = file.Name,
                    DataLabels = true,
                    Values = new ChartValues<long>(new List<long>() { file.SizeBytes }),
                    LabelPoint = new Func<ChartPoint, string>((chartPoint) => string.Format("{0} ({1})", file.Name, file.SizeString))
                });
            }
            this.IsEnabled = true;
            this.DataContext = counts;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var file in BL.GetFiles()) files.Add(file);
            counts = BL.GetCounts();
        }
    }
}
