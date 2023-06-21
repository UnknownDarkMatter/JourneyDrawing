using ExtractPixels.MapProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.Coordinates;

/// <summary>
/// Convert back and forth Lattitude/Longitude to UTM coordinates
/// </summary>
public static class UtmToLatLongConverter
{
    private static decimal K0 = 0.9996M;

    private static decimal E = 0.00669438M;
    private static decimal E2 = (decimal)Math.Pow((double)E, 2);
    private static decimal E3 = (decimal)Math.Pow((double)E, 3);
    private static decimal E_P2 = E / (1 - E);

    private static decimal SQRT_E = (decimal)Math.Sqrt(1 - (double)E);
    private static decimal _E = (1 - SQRT_E) / (1 + SQRT_E);
    private static decimal _E2 = (decimal)Math.Pow((double)_E, 2);
    private static decimal _E3 = (decimal)Math.Pow((double)_E, 3);
    private static decimal _E4 = (decimal)Math.Pow((double)_E, 4);
    private static decimal _E5 = (decimal)Math.Pow((double)_E, 5);

    private static decimal M1 = 1 - E / 4 - 3 * E2 / 64 - 5 * E3 / 256;
    private static decimal M2 = 3 * E / 8 + 3 * E2 / 32 + 45 * E3 / 1024;
    private static decimal M3 = 15 * E2 / 256 + 45 * E3 / 1024;
    private static decimal M4 = 35 * E3 / 3072;

    private static decimal P2 = 3 / 2 * _E - 27 / 32 * _E3 + 269 / 512 * _E5;
    private static decimal P3 = 21 / 16 * _E2 - 55 / 32 * _E4;
    private static decimal P4 = 151 / 96 * _E3 - 417 / 128 * _E5;
    private static decimal P5 = 1097 / 512 * _E4;

    private static decimal R = 6378137;

    private static string ZONE_LETTERS = "CDEFGHJKLMNPQRSTUVWXX";
    private static string NORTHEN_ZONELETTERS = "NPQRSTUVWXX";

    /// <summary>
    /// Converts GPS coordinates in WGS84 to UTM coordinates.
    /// UTM coordinates are used with Universal Transverse Mercator coordinate system.
    /// </summary>
    public static UtmCoordinates fromLatLon(decimal latitude, decimal longitude, int? forceZoneNum = null)
    {
        if (latitude > 84 || latitude < -80)
        {
            throw new ArgumentOutOfRangeException("latitude out of range (must be between 80 deg S and 84 deg N)");
        }
        if (longitude > 180 || longitude < -180)
        {
            throw new ArgumentOutOfRangeException("longitude out of range (must be between 180 deg W and 180 deg E)");
        }

        var latRad = toRadians(latitude);
        var latSin = (decimal)Math.Sin((double)latRad);
        var latCos = (decimal)Math.Cos((double)latRad);

        var latTan = (decimal)Math.Tan((double)latRad);
        var latTan2 = (decimal)Math.Pow((double)latTan, 2);
        var latTan4 = (decimal)Math.Pow((double)latTan, 4);

        int zoneNum;

        if (forceZoneNum == null)
        {
            zoneNum = latLonToZoneNumber(latitude, longitude);
        }
        else
        {
            zoneNum = forceZoneNum.Value;
        }

        var zoneLetter = latitudeToZoneLetter(latitude);

        var lonRad = toRadians(longitude);
        var centralLon = zoneNumberToCentralLongitude(zoneNum);
        var centralLonRad = toRadians(centralLon);

        var n = (decimal)((double)R / Math.Sqrt(1 - ((double)E * (double)latSin * (double)latSin)));
        var c = E_P2 * latCos * latCos;

        var a = latCos * (lonRad - centralLonRad);
        var a2 = (decimal)Math.Pow((double)a, 2);
        var a3 = (decimal)Math.Pow((double)a, 3);
        var a4 = (decimal)Math.Pow((double)a, 4);
        var a5 = (decimal)Math.Pow((double)a, 5);
        var a6 = (decimal)Math.Pow((double)a, 6);

        var m = R * (M1 * latRad -
                     M2 * (decimal)Math.Sin(2 * (double)latRad) +
                     M3 * (decimal)Math.Sin(4 * (double)latRad) -
                     M4 * (decimal)Math.Sin(6 * (double)latRad));

        var easting = K0 * n * (a +
                                a3 / 6 * (1 - latTan2 + c) +
                                a5 / 120 * (5 - 18 * latTan2 + latTan4 + 72 * c - 58 * E_P2)) + 500000;
        var northing = K0 * (m + n * latTan * (a2 / 2 +
                                               a4 / 24 * (5 - latTan2 + 9 * c + 4 * c * c) +
                                               a6 / 720 * (61 - 58 * latTan2 + latTan4 + 600 * c - 330 * E_P2)));
        if (latitude < 0) northing += (decimal)1e7;

        return new UtmCoordinates()
        {
            Easting = easting,
            Northing = northing,
            ZoneNum = zoneNum,
            ZoneLetter = zoneLetter
        };
    }


    /// <summary>
    /// Converts UTM coordinates to GPS coordinates in WGS84.
    /// UTM coordinates are used with Universal Transverse Mercator coordinate system.
    /// </summary>
    public static GeodesicCoordinates toLatLon(decimal easting, decimal northing, int zoneNum, string? zoneLetter,
        bool? northern = null, bool? strict = null)
    {
        strict = strict != null ? strict : true;

        if (zoneLetter == null && northern == null)
        {
            throw new Exception("either zoneLetter or northern needs to be set");
        }
        else if (zoneLetter != null && northern != null)
        {
            throw new Exception("set either zoneLetter or northern, but not both");
        }

        if (strict.Value)
        {
            if (easting < 100000 || 1000000 <= easting)
            {
                throw new ArgumentOutOfRangeException("easting out of range (must be between 100 000 m and 999 999 m)");
            }
            if (northing < 0 || northing > 10000000)
            {
                throw new ArgumentOutOfRangeException("northing out of range (must be between 0 m and 10 000 000 m)");
            }
        }
        if (zoneNum < 1 || zoneNum > 60)
        {
            throw new ArgumentOutOfRangeException("zone number out of range (must be between 1 and 60)");
        }
        if (zoneLetter != null)
        {
            zoneLetter = zoneLetter.ToUpper();
            if (zoneLetter.Length != 1 || (ZONE_LETTERS.IndexOf(zoneLetter) < 0))
            {
                throw new ArgumentOutOfRangeException("zone letter out of range (must be between C and X)");
            }
            northern = NORTHEN_ZONELETTERS.IndexOf(zoneLetter) >= 0;
        }

        var x = easting - 500000;
        var y = northing;

        if (!northern.Value)
        {
            y -= (decimal)1e7;
        }

        var m = y / K0;
        var mu = m / (R * M1);

        var pRad = mu +
                   P2 * (decimal)Math.Sin(2 * (double)mu) +
                   P3 * (decimal)Math.Sin(4 * (double)mu) +
                   P4 * (decimal)Math.Sin(6 * (double)mu) +
                   P5 * (decimal)Math.Sin(8 * (double)mu);

        var pSin = (decimal)Math.Sin((double)pRad);
        var pSin2 = (decimal)Math.Pow((double)pSin, 2);

        var pCos = (decimal)Math.Cos((double)pRad);

        var pTan = (decimal)Math.Tan((double)pRad);
        var pTan2 = (decimal)Math.Pow((double)pTan, 2);
        var pTan4 = (decimal)Math.Pow((double)pTan, 4);

        var epSin = 1 - E * pSin2;
        var epSinSqrt = (decimal)Math.Sqrt((double)epSin);

        var n = R / epSinSqrt;
        var r = (1 - E) / epSin;

        var c = _E * pCos * pCos;
        var c2 = c * c;

        var d = x / (n * K0);
        var d2 = (decimal)Math.Pow((double)d, 2);
        var d3 = (decimal)Math.Pow((double)d, 3);
        var d4 = (decimal)Math.Pow((double)d, 4);
        var d5 = (decimal)Math.Pow((double)d, 5);
        var d6 = (decimal)Math.Pow((double)d, 6);

        var latitude = pRad - (pTan / r) *
                       (d2 / 2 -
                        d4 / 24 * (5 + 3 * pTan2 + 10 * c - 4 * c2 - 9 * E_P2)) +
                        d6 / 720 * (61 + 90 * pTan2 + 298 * c + 45 * pTan4 - 252 * E_P2 - 3 * c2);
        var longitude = (d -
                         d3 / 6 * (1 + 2 * pTan2 + c) +
                         d5 / 120 * (5 - 2 * c + 28 * pTan2 - 3 * c2 + 8 * E_P2 + 24 * pTan4)) / pCos;

        return new GeodesicCoordinates()
        {
            Latitude = toDegrees(latitude),
            Longitude = toDegrees(longitude) + zoneNumberToCentralLongitude(zoneNum)
        };
    }


    private static string latitudeToZoneLetter(decimal latitude)
    {
        if (-80 <= latitude && latitude <= 84)
        {
            return ZONE_LETTERS.ToCharArray()[(int)Math.Floor((latitude + 80) / 8)].ToString();
        }
        else
        {
            return null;
        }
    }

    private static int latLonToZoneNumber(decimal latitude, decimal longitude)
    {
        if (56 <= latitude && latitude < 64 && 3 <= longitude && longitude < 12) return 32;

        if (72 <= latitude && latitude <= 84 && longitude >= 0)
        {
            if (longitude < 9) return 31;
            if (longitude < 21) return 33;
            if (longitude < 33) return 35;
            if (longitude < 42) return 37;
        }

        return (int)Math.Floor((longitude + 180) / 6) + 1;
    }

    private static int zoneNumberToCentralLongitude(int zoneNum)
    {
        return (zoneNum - 1) * 6 - 180 + 3;
    }

    private static decimal toDegrees(decimal rad)
    {
        return rad / ((decimal)Math.PI) * 180;
    }

    private static decimal toRadians(decimal deg)
    {
        return deg * ((decimal)Math.PI) / 180;
    }
}
