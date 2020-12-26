using Lineage2.Model.Configs;
using Lineage2.Model.GeoEngine.GeoData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    public class NodeBuffer
    {
        private int _size;
        private Node[,] _buffer;

        // center coordinates
        private int _cx = 0;
        private int _cy = 0;

        // target coordinates
        private int _gtx = 0;
        private int _gty = 0;
        private short _gtz = 0;

        // pathfinding statistics
        private long _lastElapsedTime = 0;

        private Node _current = null;

        GeodataConfig geodataConfig;

        /**
		 * Constructor of NodeBuffer.
		 * @param size : one dimension size of buffer
		 */
        public NodeBuffer(int size, GeodataConfig geodataConfig)
        {
            this.geodataConfig = geodataConfig;
            // set size
            _size = size;

            // initialize buffer
            _buffer = new Node[size, size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    _buffer[x, y] = new Node();
        }

        /**
		 * Find path consisting of Nodes. Starts at origin coordinates, ends in target coordinates.
		 * @param gox : origin point x
		 * @param goy : origin point y
		 * @param goz : origin point z
		 * @param gtx : target point x
		 * @param gty : target point y
		 * @param gtz : target point z
		 * @return Node : first node of path
		 */
        public Node findPath(int gox, int goy, short goz, int gtx, int gty, short gtz)
        {
            // set coordinates (middle of the line (gox,goy) - (gtx,gty), will be in the center of the buffer)
            _cx = gox + (gtx - gox - _size) / 2;
            _cy = goy + (gty - goy - _size) / 2;

            _gtx = gtx;
            _gty = gty;
            _gtz = gtz;

            _current = getNode(gox, goy, goz);
            _current.SetCost(getCostH(gox, goy, goz));

            int count = 0;
            do
            {
                // reached target?
                if (_current.GetLoc().GetGeoX() == _gtx && _current.GetLoc().GetGeoY() == _gty && Math.Abs(_current.GetLoc().getZ() - _gtz) < 8)
                    return _current;

                // expand current node
                expand();

                // move pointer
                _current = _current.getChild();
            }
            while (_current != null && ++count < geodataConfig.MaxIterations);

            return null;
        }

        /**
		 * Creates list of Nodes to show debug path.
		 * @return List<Node> : nodes
		 */
        public List<Node> debugPath()
        {
            List<Node> result = new List<Node>();

            for (Node n = _current; n.GetParent() != null; n = n.GetParent())
            {
                result.Add(n);
                n.SetCost(-n.GetCost());
            }

            foreach (var node in _buffer)
            {
                if (node.GetLoc() == null || node.GetCost() <= 0)
                    continue;

                result.Add(node);
            }

            return result;
        }

        public bool isLocked()
        {
            return _lock.tryLock();
        }

        public void free()
        {
            _current = null;

            foreach (var node in _buffer)
            {
                if (node.GetLoc() != null)
                    node.free();
            }

            _lock.unlock();
        }

        public long GetElapsedTime()
        {
            return _lastElapsedTime;
        }

        /**
		 * Check _current Node and add its neighbors to the buffer.
		 */
        private void expand()
        {
            // can't move anywhere, don't expand
            byte nswe = _current.GetLoc().getNSWE();
            if (nswe == 0)
                return;

            // get geo coords of the node to be expanded
            int x = _current.GetLoc().GetGeoX();
            int y = _current.GetLoc().GetGeoY();
            short z = (short)_current.GetLoc().getZ();

            // can move north, expand
            if ((nswe & GeoStructure.CellFlagN) != 0)
                addNode(x, y - 1, z, geodataConfig.BaseWeight);

            // can move south, expand
            if ((nswe & GeoStructure.CellFlagS) != 0)
                addNode(x, y + 1, z, geodataConfig.BaseWeight);

            // can move west, expand
            if ((nswe & GeoStructure.CellFlagW) != 0)
                addNode(x - 1, y, z, geodataConfig.BaseWeight);

            // can move east, expand
            if ((nswe & GeoStructure.CellFlagE) != 0)
                addNode(x + 1, y, z, geodataConfig.BaseWeight);

            // can move north-west, expand
            if ((nswe & GeoStructure.CellFlagNw) != 0)
                addNode(x - 1, y - 1, z, geodataConfig.DiagonalWeight);

            // can move north-east, expand
            if ((nswe & GeoStructure.CellFlagNe) != 0)
                addNode(x + 1, y - 1, z, geodataConfig.DiagonalWeight);

            // can move south-west, expand
            if ((nswe & GeoStructure.CellFlagSw) != 0)
                addNode(x - 1, y + 1, z, geodataConfig.DiagonalWeight);

            // can move south-east, expand
            if ((nswe & GeoStructure.CellFlagSe) != 0)
                addNode(x + 1, y + 1, z, geodataConfig.DiagonalWeight);
        }

        /**
		 * Returns node, if it exists in buffer.
		 * @param x : node X coord
		 * @param y : node Y coord
		 * @param z : node Z coord
		 * @return Node : node, if exits in buffer
		 */
        private Node getNode(int x, int y, short z)
        {
            // check node X out of coordinates
            int ix = x - _cx;
            if (ix < 0 || ix >= _size)
                return null;

            // check node Y out of coordinates
            int iy = y - _cy;
            if (iy < 0 || iy >= _size)
                return null;

            // get node
            Node result = _buffer[ix, iy];

            // check and update
            if (result.GetLoc() == null)
                result.setLoc(x, y, z);

            // return node
            return result;
        }

        /**
		 * Add node given by coordinates to the buffer.
		 * @param x : geo X coord
		 * @param y : geo Y coord
		 * @param z : geo Z coord
		 * @param weight : weight of movement to new node
		 */
        private void addNode(int x, int y, short z, int weight)
        {
            // get node to be expanded
            Node node = getNode(x, y, z);
            if (node == null)
                return;

            // Z distance between nearby cells is higher than cell size, record as geodata bug
            if (node.GetLoc().getZ() > (z + 2 * GeoStructure.CellHeight))
            {
                //TODO: не понятно для чего это тут
                //if (Config.DEBUG_GEO_NODE)
                //    GeoEngine.getInstance().addGeoBug(node.getLoc(), "NodeBufferDiag: Check Z coords.");

                return;
            }

            // node was already expanded, return
            if (node.GetCost() >= 0)
                return;

            node.setParent(_current);
            if (node.GetLoc().getNSWE() != (byte)0xFF)
                node.SetCost(getCostH(x, y, node.GetLoc().getZ()) + weight * geodataConfig.ObstacleMultiplier);
            else
                node.SetCost(getCostH(x, y, node.GetLoc().getZ()) + weight);

            Node current = _current;
            int count = 0;
            while (current.getChild() != null && count < geodataConfig.MaxIterations * 4)
            {
                count++;
                if (current.getChild().GetCost() > node.GetCost())
                {
                    node.setChild(current.getChild());
                    break;
                }
                current = current.getChild();
            }

            //if (count >= geodataConfig.MaxIterations * 4)
            //    System.err.println("Pathfinding: too long loop detected, cost:" + node.getCost());

            current.setChild(node);
        }

        /**
		 * @param x : node X coord
		 * @param y : node Y coord
		 * @param i : node Z coord
		 * @return double : node cost
		 */
        private double getCostH(int x, int y, int i)
        {
            int dX = x - _gtx;
            int dY = y - _gty;
            int dZ = (i - _gtz) / GeoStructure.CellHeight;

            // return (Math.abs(dX) + Math.abs(dY) + Math.abs(dZ)) * Config.HEURISTIC_WEIGHT; // Manhattan distance
            return Math.Sqrt(dX * dX + dY * dY + dZ * dZ) * geodataConfig.HeuristicWeight; // Direct distance
        }
    }
}
