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
        private WebClient wc;

        // The following dontCacheMe parameter is used to prevent local caching. See http://stackoverflow.com/q/5173052/284318
        private const String apiUrl = "http://api.pachube.com/v2/feeds/43397.xml?dontCacheMe={0}";

        public MainPage()
        {
            InitializeComponent();
            RiverList.DataContext = Rivers;

            // Initialize webclient
            wc = new WebClient();
            wc.Headers["X-PachubeApiKey"] = "FrhPnATeSS0dlM1tGooOHOpS1zSGcRFDczGYwuDCWoAy4ZtZ5hP5wa2pXeMHwOZ9ODG6Er5nhyfWm4AFU3E4DW41f3wtcGNMg26QZepSkAFcrEdDbCUQf82ZNTz4wUEs";
            wc.Headers["Accept"] = "text/xml";
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
            UpdateData();
        }

        #region DataParsing

        private void UpdateData()
        {
            // Make an asynchronous REST GET request
            wc.OpenReadAsync(new Uri(String.Format(apiUrl, DateTime.Now)));
        }

        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            XElement resultXml;

            if (e.Error != null)
            {
                Debug.WriteLine(e.Error);
                return; // TODO handle
            }
            else
            {
                try
                {
                    resultXml = XElement.Load(e.Result);
                    XNamespace ns = resultXml.GetDefaultNamespace();
                    foreach (var datastream in resultXml.Descendants(ns + "data"))
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
                        String location = label.Split('-')[1].Trim();
                        Models.MeasuringStation station = new Models.MeasuringStation(location);
                        var currentValueNodes = datastream.Nodes().Where(n => ((XElement)n).Name == ns + "current_value");
                        if (currentValueNodes.Count() > 0)
                        {
                            XElement currentValueNode = (XElement)currentValueNodes.Single();
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

        #endregion

        #region Click events

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
            FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
            root.DataContext = RiverList.SelectedItem;
            NavigationService.Navigate(new Uri("/RiverDetail.xaml", UriKind.RelativeOrAbsolute));
        }

        #endregion
    }
}