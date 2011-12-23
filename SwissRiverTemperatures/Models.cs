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
            public readonly String location;
            public readonly String unit;
            public readonly DateTime lastUpdate;
            public readonly float currentTemperature;
            public readonly float lastTemperature;

            public MeasuringStation(String location, DateTime lastUpdate, float currentTemperature, float lastTemperature, String unit = TemperatureUnit.CELSIUS)
            {
                this.location = location;
                this.unit = unit;
                this.lastUpdate = lastUpdate;
                this.currentTemperature = currentTemperature;
                this.lastTemperature = lastTemperature;
            }
        }

        /// <summary>
        /// A river that may have several measuring stations assigned to it.
        /// </summary>
        public class River
        {
            private ObservableCollection<MeasuringStation> MeasuringStations = new ObservableCollection<MeasuringStation>();
            public String Name
            {
                get;
                private set;
            }
            public int MeasuringStationCount
            {
                get { return this.MeasuringStations.Count; }
            }

            public River(String name)
            {
                this.Name = name;
            }

            public void AddMeasuringStation(MeasuringStation station)
            {
                this.MeasuringStations.Add(station);
            }

            public override String ToString()
            {
                return this.Name + " (" + this.MeasuringStationCount + ")";
            }
        }
    }
}
