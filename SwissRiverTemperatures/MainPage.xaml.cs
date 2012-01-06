using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Text;
using System.Xml.Linq;

namespace SwissRiverTemperatures
{

    public partial class MainPage : PhoneApplicationPage
    {
        public ObservableCollection<Models.River> Rivers = new ObservableCollection<Models.River>();
        private readonly WebClient _wc;

        public MainPage()
        {
            InitializeComponent();
            RiverList.DataContext = Rivers;

            Rivers.CollectionChanged += (sender, args) => Debug.WriteLine("Collection changed!");

            // Initialize webclient
            _wc = new WebClient();
            _wc.Headers["X-PachubeApiKey"] = API.Key;
            _wc.Headers["Accept"] = "text/xml";
            _wc.OpenReadCompleted += new OpenReadCompletedEventHandler(_wc_OpenReadCompleted);
            UpdateData();
        }

        private void UpdateData()
        {
            // Make an asynchronous REST GET request
            _wc.OpenReadAsync(new Uri(String.Format(API.FeedUrl, DateTime.Now)));
        }

        void _wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine("Unsucessfull request");
                Debug.WriteLine(e.Error);
                return; // TODO handle
            }
            else
            {
                try
                {
                    XElement resultXml = XElement.Load(e.Result);
                    XNamespace ns = resultXml.GetDefaultNamespace();
                    foreach (XElement datastream in resultXml.Descendants(ns + "data"))
                    {
                        // Retrieve river data
                        String label = (from n in datastream.Nodes()
                                          where ((XElement)n).Name == ns + "tag"
                                          select ((XElement)n).Value).Single();
                        String riverName = label.Split('-')[0].Trim();

                        // Get or create river object
                        Models.River river = Rivers.Where(r => r.Name == riverName).DefaultIfEmpty(new Models.River(riverName)).Single();
                        if (!Rivers.Contains(river))
                        {
                            Rivers.Add(river);
                        }

                        // Retrieve station data
                        String id = datastream.Attribute("id").Value;
                        String location = label.Split('-')[1].Trim();
                        var station = new Models.MeasuringStation(id, location);
                        var currentValueNodes = datastream.Nodes().Where(n => ((XElement)n).Name == ns + "current_value");
                        if (currentValueNodes.Any())
                        {
                            var currentValueNode = (XElement)currentValueNodes.Single();
                            station.CurrentTemperature = float.Parse(currentValueNode.Value);
                            Debug.WriteLine(location + ": " + currentValueNode.Attribute("at").Value);
                            station.LastUpdate = DateTime.Parse(currentValueNode.Attribute("at").Value);

                            // Create and add measuring station
                            river.AddMeasuringStation(station);
                        }
                    }

                    return;
                }
                catch (XmlException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return; // TODO handle
                }
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void RiverList_Tap(object sender, GestureEventArgs e)
        {
            var root = Application.Current.RootVisual as FrameworkElement;
            root.DataContext = RiverList.SelectedItem;
            NavigationService.Navigate(new Uri("/RiverDetail.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}