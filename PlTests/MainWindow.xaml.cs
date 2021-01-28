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
using BlApi;

namespace PlTests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static IBL BL = BlFactory.GetBL();
        public MainWindow()
        {
            InitializeComponent();

            try { cmbLinesID.ItemsSource = BL.GetLines().ToList(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            try { cmbLinesID.DisplayMemberPath = "ID"; }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void cmbLinesID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var line = cmbLinesID.SelectedItem as BO.Line;
            lstStations.ItemsSource = line.AdjStations.ToList();
        }
    }
}
