using Lineage2.Model.GeoEngine.GeoData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    public class Node
    {
        // node coords and nswe flag
        private GeoLocation _loc;

        // node parent (for reverse path construction)
        private Node _parent;
        // node child (for moving over nodes during iteration)
        private Node _child;

        // node G cost (movement cost = parent movement cost + current movement cost)
        private double _cost = -1000;

        public void setLoc(int x, int y, int z)
        {
            _loc = new GeoLocation(x, y, z);
        }

        public GeoLocation GetLoc()
        {
            return _loc;
        }

        public void setParent(Node parent)
        {
            _parent = parent;
        }

        public Node GetParent()
        {
            return _parent;
        }

        public void setChild(Node child)
        {
            _child = child;
        }

        public Node getChild()
        {
            return _child;
        }

        public void SetCost(double cost)
        {
            _cost = cost;
        }

        public double GetCost()
        {
            return _cost;
        }

        public void free()
        {
            // reset node location
            _loc = null;

            // reset node parent, child and cost
            _parent = null;
            _child = null;
            _cost = -1000;
        }
    }
}
