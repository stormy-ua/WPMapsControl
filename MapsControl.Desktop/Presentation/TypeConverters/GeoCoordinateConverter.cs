using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapsControl.Desktop.Presentation.TypeConverters
{
    public class GeoCoordinateConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value != null)
            {
                string strGeoCoord = value.ToString();
                var coords = strGeoCoord.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                double latitude = Convert.ToDouble(coords[0]);
                double longitude = Convert.ToDouble(coords[1]);
                return new GeoCoordinate(latitude, longitude);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
