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
using HtmlAgilityPack;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Markup;
using System.Xml;

namespace SwissRiverTemperatures
{

    public partial class MainPage : PhoneApplicationPage
    {
        public ObservableCollection<Models.River> Rivers = new ObservableCollection<Models.River>();
        private const String DataURL = "http://www.hydrodaten.admin.ch/lhg/T-Bulletin.html";

        public MainPage()
        {
            InitializeComponent();
            RiverList.DataContext = Rivers;
            UpdateData();
        }

        #region DataParsing

        private void UpdateData()
        {
            EventHandler<HtmlDocumentLoadCompleted> documentLoadCompletedHandler = new EventHandler<HtmlDocumentLoadCompleted>(
            (s, args) =>
            {
                // Fetch rows
                IEnumerable<HtmlNode> rows = System.Linq.Enumerable.Empty<HtmlNode>();
                try
                {
                    // Rows LINQ query
                    rows = from row in args.Document.DocumentNode.Descendants("tr")
                               where row.ParentNode.Id == "mainStationList" && row.Attributes["class"].Value.StartsWith("stationsListeBody")
                               select row;

                    // Restore riverlist visibility if necessary
                    if (ErrorPanel.Visibility == Visibility.Visible)
                    {
                        ErrorPanel.Visibility = Visibility.Collapsed;
                        RiverList.Visibility = Visibility.Visible;
                    }
                }
                // Catch exception if fetching failed
                catch (NullReferenceException)
                {
                    // Hide river list, show error panel
                    RiverList.Visibility = Visibility.Collapsed;
                    ErrorPanel.Visibility = Visibility.Visible;
                }

                // Parse rows
                foreach (HtmlNode row in rows)
                {
                    String[] cols = new String[10];
                    HtmlNode[] fields = row.Descendants("td").ToArray();
                    try
                    {
                        cols[0] = fields[0].Descendants("a").Select(x => x.InnerText).Single();
                    }
                    catch (InvalidOperationException)
                    {
                        continue;
                    }

                    for (int i = 1; i <= 4; i++)
                    {
                        cols[i] = fields[i].InnerText;
                    }

                    // Get or create river
                    String riverName = cols[0].Split('-')[0].Trim();
                    Models.River river = Rivers.Where(r => r.Name == riverName).DefaultIfEmpty(new Models.River(riverName)).Single();
                    if (!Rivers.Contains(river))
                    {
                        Rivers.Add(river);
                    }

                    // Get station location
                    String location = cols[0].Split('-')[1].Trim();

                    // Get temperature unit
                    String temperatureUnit;
                    switch (cols[1])
                    {
                        case "[°C]":
                            temperatureUnit = Models.TemperatureUnit.CELSIUS;
                            break;
                        case "[°F]":
                            temperatureUnit = Models.TemperatureUnit.FAHRENHEIT;
                            break;
                        case "[°K]":
                            temperatureUnit = Models.TemperatureUnit.KELVIN;
                            break;
                        default:
                            temperatureUnit = cols[1];
                            break;
                    }

                    // Get last update datetime
                    IFormatProvider culture = new CultureInfo("de-CH");
                    DateTime lastUpdate;
                    try
                    {
                        lastUpdate = DateTime.ParseExact(cols[2].Replace("&nbsp;", " "), "g", culture);
                    }
                    catch (FormatException)
                    {
                        Debug.WriteLine("Could not parse datetime " + cols[2] + ".");
                        continue;
                    }

                    // Get current and previous temperature
                    float currentTemperature, lastTemperature;
                    currentTemperature = float.Parse(cols[3]);
                    lastTemperature = float.Parse(cols[4]);

                    // Create and add measuring station
                    Models.MeasuringStation station = new Models.MeasuringStation(location, lastUpdate,
                                                        currentTemperature, lastTemperature, temperatureUnit);
                    river.AddMeasuringStation(station);
                }
            });

            try
            {
                HtmlWeb.LoadAsync(DataURL, documentLoadCompletedHandler);
            }
            catch (System.IO.FileNotFoundException)
            {
                Debug.WriteLine("filenotfound");
            }
            catch (System.Net.WebException)
            {
                Debug.WriteLine("webexc");
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