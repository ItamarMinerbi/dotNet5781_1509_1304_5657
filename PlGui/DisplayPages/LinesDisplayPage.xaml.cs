﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BlApi;

namespace PlGui
{
    /// <summary>
    /// Interaction logic for LinesDisplayPage.xaml
    /// </summary>
    public partial class LinesDisplayPage : Page
    {
        static IBL BL = BlFactory.GetBL();
        ObservableCollection<BO.Line> Lines;

        string workerResultTitle, workerResultContent, txtLineNumber = "";
        BO.Line.Areas selectedArea;
        BackgroundWorker worker = new BackgroundWorker();
        Action<object, DoWorkEventArgs> workerDoWorkAction;
        Action<object, RunWorkerCompletedEventArgs> workerCompletedAction;
        Action<object, ProgressChangedEventArgs> workerProgressChangedAction;
        FrameworkElement dataGridRowDetails;
        bool IsRowDetailsOpen = true;

        public LinesDisplayPage()
        {
            InitializeComponent();

            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            LoadLines();
        }

        private void LoadLines()
        {
            dgrLines.ItemsSource = new ObservableCollection<BO.Line>();
            dgrLines.IsEnabled = grdUpdate.IsEnabled = false;
            pbarLoad.Visibility = Visibility.Visible;
            pbarLoad.Value = 0;
            workerProgressChangedAction = new Action<object, ProgressChangedEventArgs>(
                (object obj, ProgressChangedEventArgs args) =>
                {
                    pbarLoad.Value = args.ProgressPercentage;
                    Lines.Add((BO.Line)args.UserState);
                });
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    if (workerResultTitle == "XmlError")
                    {
                        new CustomMessageBox(
                            workerResultContent,
                            "File Error",
                            "Files error",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.FILE).ShowDialog();
                    }
                    else if (workerResultTitle == "UnknownError")
                    {
                        new CustomMessageBox(
                            workerResultContent,
                            "Unknown Error",
                            "Unknown error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR).ShowDialog(); 
                    }
                    pbarLoad.Visibility = Visibility.Hidden;
                    dgrLines.IsEnabled = grdUpdate.IsEnabled = true;
                    workerResultTitle = "";
                });
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object sender, DoWorkEventArgs e) =>
            {
                Lines = new ObservableCollection<BO.Line>();
                int i = 0, count = 0;
                try { count = BL.GetLinesCount(); }
                catch (Exception ex) { workerResultTitle = "XmlError"; workerResultContent = ex.Message; }
                foreach (var line in BL.GetLines())
                    try { worker.ReportProgress(++i * 100 / count, line); }
                    catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
            });
            worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            workerProgressChangedAction?.Invoke(sender, e);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            workerDoWorkAction?.Invoke(sender, e);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            workerCompletedAction?.Invoke(sender, e);
            dgrLines.ItemsSource = Lines;
            dgrLines.UnselectAll();
            workerResultTitle = workerResultContent = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsRowDetailsOpen = false;
            dataGridRowDetails?.BeginAnimation(HeightProperty,
                                new DoubleAnimation(0, TimeSpan.Parse("0:0:0.5")));
            BackgroundWorker wait = new BackgroundWorker();
            wait.DoWork += (s, args) => Thread.Sleep(500);
            wait.RunWorkerCompleted += (s, args) => dgrLines.UnselectAll();
            wait.RunWorkerAsync();
        }

        private void dgrLines_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            dataGridRowDetails = (Grid)e.DetailsElement;
            if(IsRowDetailsOpen)
                dataGridRowDetails?.BeginAnimation(HeightProperty,
                    new DoubleAnimation(400, TimeSpan.Parse("0:0:0.5")));
            IsRowDetailsOpen = true;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int lineNumber = 0;
            if (!int.TryParse(txtLineNumber, out lineNumber))
            {
                new CustomMessageBox("Some fields are illegal.",
                      "Invalid Values",
                      "Fields Error",
                      CustomMessageBox.Buttons.IGNORE,
                      CustomMessageBox.Icons.EDIT).ShowDialog();
                return;
            }

            var Line = dgrLines.SelectedItem as BO.Line;
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object obj, DoWorkEventArgs args) =>
            {
                try { BL.UpdateLine(Line.ID, lineNumber, selectedArea); }
                catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
            });
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    CustomMessageBox messageBox;
                    if (workerResultTitle == "UnknownError")
                    {
                        messageBox = new CustomMessageBox(
                            workerResultContent,
                            "Unknown Error",
                            "Unknown error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR);
                        messageBox.ShowDialog();
                    }
                    else
                    {
                        messageBox = new CustomMessageBox(
                            "Adjacent station updated successfuly",
                            "Action Executed Successfuly",
                            "Action Completed",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.Vi);
                        if (messageBox.ShowDialog() == false) LoadLines();
                    }
                });
            worker.RunWorkerAsync();
            dgrLines.UnselectAll();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addLine = new AddWindows.AddLine();
            
            if (addLine.ShowDialog() == false && addLine.Result == "Added")
                LoadLines();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var Line = dgrLines.SelectedItem as BO.Line;
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object obj, DoWorkEventArgs args) =>
            {
                try { BL.RemoveLine(Line.ID); }
                catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
            });
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    if (workerResultTitle == "UnknownError")
                    {
                        new CustomMessageBox(
                            workerResultContent,
                            "Unknown Error",
                            "Unknown error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR).ShowDialog();
                    }
                    else
                    {
                        CustomMessageBox messageBox = new CustomMessageBox(
                            "Line removed successfuly",
                            "Action Executed Successfuly",
                            "Action Completed",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.Vi);
                        if (messageBox.ShowDialog() == false) LoadLines();
                    }
                });
            worker.RunWorkerAsync();
        }

        private void btnAddTrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var addTripLine = new AddWindows.AddTrip((dgrLines.SelectedItem as BO.Line).ID);
            
            if (addTripLine.ShowDialog() == false && addTripLine.Result == "Added")
                LoadLines();
        }

        private void btnRemoveTrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var lineTrip = (sender as Grid).DataContext as BO.LineTrip;
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object obj, DoWorkEventArgs args) =>
            {
                try { BL.RemoveLineTrip(lineTrip.ID, lineTrip.StartTime); }
                catch (Exception ex) { workerResultTitle = "UnknownError"; workerResultContent = ex.Message; }
            });
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    if (workerResultTitle == "UnknownError")
                    {
                        new CustomMessageBox(
                            workerResultContent,
                            "Unknown Error",
                            "Unknown error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR).ShowDialog();
                    }
                    else
                    {
                        CustomMessageBox messageBox = new CustomMessageBox(
                            "Trip removed successfuly from the line",
                            "Action Executed Successfuly",
                            "Action Completed",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.Vi);
                        if (messageBox.ShowDialog() == false) LoadLines();
                    }
                });
            worker.RunWorkerAsync();
        }

        private void addStationAfter_Click(object sender, RoutedEventArgs e)
        {
            var Index = ((int)(sender as MenuItem).CommandParameter) + 1;
            int Id = (dgrLines.SelectedItem as BO.Line).ID;
            var addStationLine = new AddWindows.AddStationLine(Id, Index);
            
            if (addStationLine.ShowDialog() == false && addStationLine.Result == "Added")
                LoadLines();
        }

        private void addStationBefore_Click(object sender, RoutedEventArgs e)
        {
            var Index = (int)(sender as MenuItem).CommandParameter;
            int Id = (dgrLines.SelectedItem as BO.Line).ID;
            var addStationLine = new AddWindows.AddStationLine(Id, Index);
            if (addStationLine.ShowDialog() == false && addStationLine.Result=="Added") 
                LoadLines(); 
        }

        private void removeStation_Click(object sender, RoutedEventArgs e)
        {
            var Line = dgrLines.SelectedItem as BO.Line;
            var Item = (BO.DisplayStationLine)(sender as MenuItem).CommandParameter;
            workerCompletedAction = new Action<object, RunWorkerCompletedEventArgs>(
                (object obj, RunWorkerCompletedEventArgs args) =>
                {
                    if (workerResultTitle == "RemoveError")
                    {
                        new CustomMessageBox(
                            workerResultContent,
                            "Remove Error",
                            "Remove error",
                            CustomMessageBox.Buttons.IGNORE,
                            CustomMessageBox.Icons.ERROR).ShowDialog();
                    }
                    else
                    {
                        CustomMessageBox messageBox = new CustomMessageBox(
                            "Station removed successfuly",
                            "Action Executed Successfuly",
                            "Action Completed",
                            CustomMessageBox.Buttons.OK,
                            CustomMessageBox.Icons.Vi);
                        
                        if (messageBox.ShowDialog() == false) LoadLines();
                    }
                });
            workerDoWorkAction = new Action<object, DoWorkEventArgs>((object obj, DoWorkEventArgs args) =>
            {
                try { BL.RemoveStationLine(Line.ID, Item.StationCode); }
                catch (Exception ex) { workerResultTitle = "RemoveError"; workerResultContent = ex.Message; }
            });
            worker.RunWorkerAsync();
        }

        private void grdUpdate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadLines();
        }

        private void cmbStations_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).ItemsSource = Enum.GetValues(typeof(BO.Line.Areas));
            if ((dgrLines.SelectedItem as BO.Line) != null)
                selectedArea = (dgrLines.SelectedItem as BO.Line).Area;
            else selectedArea = BO.Line.Areas.General;
        }

        private void cmbStations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Enum.TryParse((sender as ComboBox).SelectedValue.ToString(), out selectedArea);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtLineNumber = (sender as TextBox)?.Text;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if ((dgrLines.SelectedItem as BO.Line) != null)
                txtLineNumber = (dgrLines.SelectedItem as BO.Line).LineNumber.ToString();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!worker.IsBusy)
            {
                var text = (sender as TextBox).Text;
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(Lines);
                if (collectionView != null)
                {
                    dgrLines.ItemsSource = collectionView;
                    collectionView.Filter = (object o) =>
                    {
                        BO.Line p = o as BO.Line;
                        if (p == null) return false;
                        if (p.ID.ToString().Contains(text)) return true;
                        if (p.LineNumber.ToString().Contains(text)) return true;
                        if (p.Area.ToString().Contains(text)) return true;
                        if (p.TotalTime.ToString(@"hh\:mm\:ss").Contains(text)) return true;
                        return false;
                    };
                }
            }
        }

        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                          (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                           e.Key == Key.Back);
        }
    }
}