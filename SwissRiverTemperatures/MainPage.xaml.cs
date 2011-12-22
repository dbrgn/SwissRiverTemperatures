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
        }

        #region DataParsing

        private void UpdateData()
        {
            HtmlWeb.LoadAsync(DataURL, (s, args) =>
                {
                    var rows = from row in args.Document.DocumentNode.Descendants("tr")
                               where row.ParentNode.Id == "mainStationList" && row.Attributes["class"].Value.StartsWith("stationsListeBody")
                               select row;

                    foreach(HtmlNode row in rows)
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

                        String riverName = cols[0].Split('-')[0].Trim();
                        Models.River river = Rivers.Where(r => r.Name == riverName).DefaultIfEmpty(new Models.River(riverName)).Single();
                        if (!Rivers.Contains(river))
                        {
                            Rivers.Add(river);
                        }
                    }
                });
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        #endregion
    }
}