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
        public struct MeasuringStation
        {
            private String _location;
            public String Location { get { return _location; } }
            private String _unit;
            private DateTime _lastUpdate;
            public DateTime LastUpdate { get { return _lastUpdate; } }
            public String LastUpdateAgo
            {
                get { return TimeAgo.Since(_lastUpdate); }
            }
            private float _currentTemperature;
            public float CurrentTemperature { get { return _currentTemperature; } }
            public String CurrentTemperatureString { get { return String.Format("{0:F2} {1}", _currentTemperature, _unit); } }
            private float _lastTemperature;
            public float LastTemperature { get { return _lastTemperature; } }
            public String LastTemperatureString { get { return String.Format("{0:F2} {1}", _lastTemperature, _unit); } }

            public MeasuringStation(String location, DateTime lastUpdate, float currentTemperature, float lastTemperature, String unit = TemperatureUnit.CELSIUS)
            {
                this._location = location;
                this._unit = unit;
                this._lastUpdate = lastUpdate;
                this._currentTemperature = currentTemperature;
                this._lastTemperature = lastTemperature;
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
