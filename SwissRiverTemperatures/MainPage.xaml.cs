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
                        int labelPos = label.IndexOf('-');
                        String riverName = label.Substring(0, labelPos).Trim();

                        // Get or create river object
                        Models.River river = Rivers.Where(r => r.Name == riverName).DefaultIfEmpty(new Models.River(riverName)).Single();
                        if (!Rivers.Contains(river))
                        {
                            Rivers.Add(river);
                        }

                        // Retrieve station data
                        String id = datastream.Attribute("id").Value;
                        String location = label.Substring(labelPos+1).Trim();
                        var station = new Models.MeasuringStation(id, location);
                        try
                        {
                            // Parse data
                            var currentValueNode = datastream.Nodes().Single(n => ((XElement) n).Name == ns + "current_value") as XElement;
                            station.CurrentTemperature = float.Parse(currentValueNode.Value);
                            station.LastUpdate = DateTime.Parse(currentValueNode.Attribute("at").Value);

                            // Create and add or update measuring station
                            var measuringStations = river.MeasuringStations.Where(s => s.Id == id);
                            if (measuringStations.Any())
                            {
                                // Replace current station
                                int index = river.MeasuringStations.IndexOf(measuringStations.Single());
                                river.MeasuringStations[index] = station;
                            } else
                            {
                                // Add new station
                                river.MeasuringStations.Add(station);
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            Debug.WriteLine("InvalidOperationException: " + ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        }
                        catch (NullReferenceException ex)
                        {
                            Debug.WriteLine("NullReferenceException: " + ex.Message);
                            Debug.WriteLine(ex.StackTrace);
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