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

namespace SwissRiverTemperatures
{
    public partial class RiverDetail : UserControl
    {
        public RiverDetail()
        {
            InitializeComponent();

            // Get river object
            FrameworkElement root = Application.Current.RootVisual as FrameworkElement;
            Models.River river = (Models.River)root.DataContext;
            
            // Set name
            RiverName.DataContext = river.Name;

            // Set measuring station list
            MeasuringStationList.DataContext = river.MeasuringStations;
        }
    }
}
