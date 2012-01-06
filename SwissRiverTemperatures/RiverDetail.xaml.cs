﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace SwissRiverTemperatures
{
    public partial class RiverDetail : PhoneApplicationPage
    {
        private readonly Models.River _river;

        public RiverDetail()
        {
            InitializeComponent();

            // Get river object
            var root = Application.Current.RootVisual as FrameworkElement;
            _river = root.DataContext as Models.River;

            // Set data bindings
            PivotElement.Title = _river.Name;
            PivotElement.DataContext = _river.MeasuringStations;
        }

        private void PivotElement_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            // Get current station
            var station = e.Item.DataContext as Models.MeasuringStation;

            // If diagram hasn't been loaded yet, start async call to fetch it
            if (station.Diagram == null)
            {
                // Get diagram request url
                const string url = "{0}{1}/datastreams/{2}.png?width={3}&height={4}&colour=F15A24&duration=24hours&detailed_grid=true&show_axis_labels=true&title=&timezone=UTC+1";
                int width = Convert.ToInt32(PivotElement.ActualWidth - PivotElement.Margin.Left - PivotElement.Margin.Right);
                int height = width/4*3;

                // Create web request
                var wc = new WebClient();
                wc.OpenReadCompleted += new OpenReadCompletedEventHandler((a, b) => wc_LoadingDiagramCompleted(a, b, station));
                wc.OpenReadAsync(new Uri(String.Format(url, API.BaseUrl, API.FeedId, station.Id, width, height)));
            }
        }

        private void wc_LoadingDiagramCompleted(object sender, OpenReadCompletedEventArgs e, Models.MeasuringStation station)
        {
            if (e.Error != null)
            {
                Debug.WriteLine(e.Error);
                return; // TODO handle
            }
            else
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(e.Result);
                station.Diagram = bitmap;
            }
        }
    }
}
