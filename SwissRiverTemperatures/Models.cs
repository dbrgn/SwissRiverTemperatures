using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

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
            public bool DiagramAvailable { get { return Diagram != null; } // Caveat: Remember firing of OnPropertyChanged()
            }

            // Writeable properties
            public DateTime LastUpdate { get; set; }
            public float CurrentTemperature { get; set; }
            public String Unit { get; set; }
            public BitmapImage Diagram {
                get { return _diagram; }
                set
                {
                    _diagram = value;
                    OnPropertyChanged("Diagram");
                    OnPropertyChanged("DiagramAvailable");
                }
            }
            
            // Events
            public event PropertyChangedEventHandler PropertyChanged;

            public MeasuringStation(String id, String location, String unit = TemperatureUnit.Celsius)
            {
                this._id = id;
                this._location = location;
                this.Unit = unit;
            }

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler propertyChangedEvent = PropertyChanged;
                if (propertyChangedEvent != null)
                    propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
            }

        }

        /// <summary>
        /// A river that may have several measuring stations assigned to it.
        /// </summary>
        public class River
        {
            public String Name { get; private set; }
            private ObservableCollection<MeasuringStation> _measuringStations = new ObservableCollection<MeasuringStation>();
            public ObservableCollection<MeasuringStation> MeasuringStations {
                get { return _measuringStations; }
                set { _measuringStations = value; }
            }
            public int MeasuringStationCount { get { return MeasuringStations.Count; } }

            public River(String name)
            {
                this.Name = name;
            }

            public override String ToString()
            {
                return this.Name + " (" + this.MeasuringStationCount + ")";
            }
        }
    }
}
