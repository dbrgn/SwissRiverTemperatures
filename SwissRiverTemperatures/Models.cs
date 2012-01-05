using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            public const String CELSIUS = "°C";
            public const String FAHRENHEIT = "°F";
            public const String KELVIN = "°K";
        }

        /// <summary>
        /// A measuring station that reports temperature levels.
        /// </summary>
        public class MeasuringStation
        {
            // Private fields
            private String _location;
            private String _unit;

            // Readonly properties
            public String Location { get { return _location; } }
            public String CurrentTemperatureString { get { return String.Format("{0:F2} {1}", CurrentTemperature, _unit); } }
            public String LastUpdateAgo { get { return TimeAgo.Since(LastUpdate); } }

            // Writeable properties
            public DateTime LastUpdate { get; set; }
            public float CurrentTemperature { get; set; }
            
            public MeasuringStation(String location, String unit = TemperatureUnit.CELSIUS)
            {
                this._location = location;
                this._unit = unit;
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
