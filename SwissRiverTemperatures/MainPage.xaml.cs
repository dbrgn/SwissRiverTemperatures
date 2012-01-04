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