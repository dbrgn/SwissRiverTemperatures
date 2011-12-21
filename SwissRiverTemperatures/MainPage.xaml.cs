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

namespace SwissRiverTemperatures
{
    

    public partial class MainPage : PhoneApplicationPage
    {
        public ObservableCollection<Models.River> Rivers = new ObservableCollection<Models.River>();

        public MainPage()
        {
            InitializeComponent();
            Rivers.Add(new Models.River("Emme"));
            Rivers.Add(new Models.River("Aare"));
            Rivers.Add(new Models.River("Rhein"));
            Rivers.Add(new Models.River("Limmat"));
            Rivers.Add(new Models.River("Reuss"));
            Rivers.Add(new Models.River("Thur"));
            RiverList.DataContext = Rivers;
        }
    }
}