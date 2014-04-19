using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapsControl.Desktop.Presentation.TypeConverters
{
    public class GeoCoordinateCollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value != null)
            {
                var collection = new ObservableCollection<GeoCoordinate>();
                string strGeoCoord = value.ToString();
                var coords = strGeoCoord.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    .ToArray();

                for (int j = 0; j < coords.Length; j+=2)
                {
                    double latitude = Convert.ToDouble(coords[j]);
                    double longitude = Convert.ToDouble(coords[j + 1]);
                    var geoCoordinate = new GeoCoordinate(latitude, longitude);
                    collection.Add(geoCoordinate);
                }

                return collection;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
