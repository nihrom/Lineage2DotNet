using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.Locations
{
    public class Location
    {
        public static Location DUMMY_LOC = new Location(0, 0, 0);

        protected volatile int _x;
        protected volatile int _y;
        protected volatile int _z;

        public Location(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Location(Location loc)
        {
            _x = loc.getX();
            _y = loc.getY();
            _z = loc.getZ();
        }

        public string tostring()
        {
            return _x + ", " + _y + ", " + _z;
        }

        public int getX()
        {
            return _x;
        }

        public int getY()
        {
            return _y;
        }

        public int getZ()
        {
            return _z;
        }

        public void set(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public void set(Location loc)
        {
            _x = loc.getX();
            _y = loc.getY();
            _z = loc.getZ();
        }
        public void clean()
        {
            _x = 0;
            _y = 0;
            _z = 0;
        }
    }
}
