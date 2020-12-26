using Lineage2.Model.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.GeoData
{
    public class GeoLocation : Location
    {
        private byte _nswe;

        public GeoLocation(int x, int y, int z) : base(x, y, GeoEngine.getInstance().getHeightNearest(x, y, z))
        {
            _nswe = GeoEngine.getInstance().getNsweNearest(x, y, z);
        }

        public void set(int x, int y, short z)
        {
            base.set(x, y, GeoEngine.getInstance().getHeightNearest(x, y, z));
            _nswe = GeoEngine.getInstance().getNsweNearest(x, y, z);
        }

        public int GetGeoX()
        {
            return _x;
        }

        public int GetGeoY()
        {
            return _y;
        }


        public int getX()
        {
            return GeoEngine.getWorldX(_x);
        }

        public int getY()
        {
            return GeoEngine.getWorldY(_y);
        }

        public byte getNSWE()
        {
            return _nswe;
        }
    }
}
