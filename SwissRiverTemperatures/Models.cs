using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SwissRiverTemperatures
{
    public class Models
    {
        public struct TemperatureUnit
        {
            // TODO: Create some kind of string enum
            public const String Celsius = "°C";
            public const String Fahrenheit = "°F";
            public const String Kelvin = "°K";
        }

        /// <summary>
        /// A measuring station that reports temperature levels.
        /// </summary>
        public class MeasuringStation : INotifyPropertyChanged
        {
            // Private fields
            private readonly String _location;
            private readonly String _id;
            private BitmapImage _diagram;

            // Readonly properties
            public String Id { get { return _id;  } }
            public String Location { get { return _location; } }
            public String CurrentTemperatureString { get { return String.Format("{0:F2} {1}", CurrentTemperature, Unit); } }
            public String LastUpdateAgo { get { return TimeAgo.Since(LastUpdate); } }
            public Visibility DiagramVisibility { get { return Diagram == null ? Visibility.Collapsed : Visibility.Visible; }
            }

            // Writeable properties
            public DateTime LastUpdate { get; set; }
            public float CurrentTemperature { get; set; }
            public String Unit { get; set; }
            public BitmapImage Diagram { get { return _diagram; } set { _diagram = value; OnPropertyChanged("Diagram"); } }
            
            // Events
            public event PropertyChangedEventHandler PropertyChanged;

            public MeasuringStation(String id, String location, String unit = TemperatureUnit.Celsius)
            {
                this._id = id;
                this._location = location;
                this.Unit = unit;
                PropertyChanged += new PropertyChangedEventHandler((sender, args) => Debug.WriteLine("Property changed event fired."));
            }

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler @event = PropertyChanged;
                if (@event != null)
                    @event(
                        this,
                        new PropertyChangedEventArgs(propertyName ?? string.Empty)
                        );
            }

        }

        /// <summary>
        /// A river that may have several measuring stations assigned to it.
        /// </summary>
        public class River
        {
            private ObservableCollection<MeasuringStation> _measuringStations = new ObservableCollection<MeasuringStation>();
            public String Name
            {
                get;
                private set;
            }

            public ObservableCollection<MeasuringStation> MeasuringStations
            {
                get { return this._measuringStations; }
            }

            public int MeasuringStationCount
            {
                get { return this._measuringStations.Count; }
            }

            public River(String name)
            {
                this.Name = name;
            }

            public void AddMeasuringStation(MeasuringStation station)
            {
                this._measuringStations.Add(station);
            }

            public override String ToString()
            {
                return this.Name + " (" + this.MeasuringStationCount + ")";
            }
        }
    }
}
