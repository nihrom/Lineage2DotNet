using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    public class Node
    {
        private AbstractNodeLoc _loc;
        private int _neighborsIdx;
        private Node[] _neighbors;
        private Node _parent;
        private short _cost;
        private PathFinding pathFinding;

        public Node(AbstractNodeLoc Loc, int Neighbors_idx, PathFinding pathFinding)
        {
            _loc = Loc;
            _neighborsIdx = Neighbors_idx;
            this.pathFinding = pathFinding;
        }

        public void setParent(Node p)
        {
            _parent = p;
        }

        public void setCost(int cost)
        {
            _cost = (short)cost;
        }

        public void attacheNeighbors()
        {
            if (_loc == null) _neighbors = null;
            else _neighbors = pathFinding.readNeighbors(_loc.getNodeX(), _loc.getNodeY(), _neighborsIdx);
        }

        public Node[] getNeighbors()
        {
            return _neighbors;
        }

        public Node getParent()
        {
            return _parent;
        }

        public AbstractNodeLoc getLoc()
        {
            return _loc;
        }

        public short getCost()
        {
            return _cost;
        }

        public bool equals(Object arg0)
        {
            if (!(arg0 is Node))
                return false;
            Node n = (Node)arg0;
            //Check if x,y,z are the same
            return _loc.getX() == n.getLoc().getX() && _loc.getY() == n.getLoc().getY()
            && _loc.getZ() == n.getLoc().getZ();
        }
    }
}
