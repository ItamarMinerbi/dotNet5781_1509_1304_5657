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

using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Tasks.Geocoding;

namespace PlGui
{
    public partial class ChooseLocation : Window
    {
        public double ResultX { get; private set; }
        public double ResultY { get; private set; }
        public string ResultAddress { get; private set; }

        private readonly Dictionary<string,Basemap> basemapOptions = new Dictionary<string, Basemap>()
        {
            {"Streets (Raster)", Basemap.CreateStreets()},
            {"Streets (Vector)", Basemap.CreateStreetsVector()},
            {"Streets - Night (Vector)", Basemap.CreateStreetsNightVector()},
            {"Imagery (Raster)", Basemap.CreateImagery()},
            {"Imagery with Labels (Raster)", Basemap.CreateImageryWithLabels()},
            {"Imagery with Labels (Vector)", Basemap.CreateImageryWithLabelsVector()},
            {"Dark Gray Canvas (Vector)", Basemap.CreateDarkGrayCanvasVector()},
            {"Light Gray Canvas (Raster)", Basemap.CreateLightGrayCanvas()},
            {"Light Gray Canvas (Vector)", Basemap.CreateLightGrayCanvasVector()},
            {"Navigation (Vector)", Basemap.CreateNavigationVector()},
            {"OpenStreetMap (Raster)", Basemap.CreateOpenStreetMap()}
        };
        private LocatorTask geocoder;
        private Uri serviceUri = new Uri("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer");

        public ChooseLocation()
        {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            ResultX = ResultY = 200;
            MapView.Map = new Map(Basemap.CreateOpenStreetMap()) { MinScale = 100000000, MaxScale = 2000 };
            cmbMaps.ItemsSource = basemapOptions.Keys;
            MapView.SetViewpoint(new Viewpoint(31.76904, 35.21633, MapView.Map.MinScale / 10)); // Jerusalem Coordinates
            lbl.Text = String.Format("{0}, {1}", 31.76904, 35.21633);
            MapView.GeoViewTapped += (sender, e) => ShowLabel(e.Location);
            MapView.MouseRightButtonDown += (sender, e) => MapView.DismissCallout();
            geocoder = await LocatorTask.CreateAsync(serviceUri);
        }

        private async void UpdateSearch()
        {
            string enteredText = txtSearch.Text;
            MapView.GraphicsOverlays.Clear();

            if (String.IsNullOrWhiteSpace(enteredText) || geocoder == null) return;

            try
            {
                IReadOnlyList<SuggestResult> suggestions = await geocoder.SuggestAsync(enteredText);

                if (suggestions.Count < 1) return;
                SuggestResult firstSuggestion = suggestions.First();
                IReadOnlyList<GeocodeResult> addresses = await geocoder.GeocodeAsync(firstSuggestion.Label);

                if (addresses.Count < 1) return;

                lstResults.ItemsSource = addresses;

                CreateGraphicOverly(addresses.First());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
            }
        }

        private void CreateGraphicOverly(GeocodeResult geocode)
        {
            if (geocode == null) return;
            GraphicsOverlay resultOverlay = new GraphicsOverlay();
            resultOverlay.Graphics.Add(new Graphic(geocode.DisplayLocation));
            MapView.GraphicsOverlays.Add(resultOverlay);
            MapView.SetViewpoint(new Viewpoint(geocode.Extent));
            ShowLabel(geocode.DisplayLocation);
        }

        private void ShowLabel(MapPoint point)
        {
            MapPoint mapLocation = point;
            Esri.ArcGISRuntime.Geometry.Geometry myGeometry = GeometryEngine.Project(mapLocation, SpatialReferences.Wgs84);
            MapPoint projectedLocation = (MapPoint)myGeometry;
            string mapLocationDescription = string.Format("Lat: {0:F3} Long:{1:F3}", projectedLocation.Y, projectedLocation.X);
            ResultY = projectedLocation.Y;
            ResultX = projectedLocation.X;
            lbl.Text = String.Format("{0}, {1}", projectedLocation.Y, projectedLocation.X);
            CalloutDefinition myCalloutDefinition = new CalloutDefinition("Location:", mapLocationDescription);
            MapView.ShowCalloutAt(point, myCalloutDefinition);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateSearch();
        }

        private void lstResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateGraphicOverly(lstResults.SelectedItem as GeocodeResult);
        }

        private void cmbMaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedTitle = e.AddedItems[0].ToString();
            MapView.Map.Basemap = basemapOptions[selectedTitle];
        }
    }
}