using Lineage2.Model.GeoEngine.GeoData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    public abstract class PathFinding
    {
        private static PathFinding _instance;

        public static PathFinding getInstance()
        {
            //if (_instance == null)
            //{
            //	if (true /*Config.GEODATA_PATHFINDING*/)
            //	{
            //		//Smaler Memory Usage, Higher Cpu Usage (CalculatedOnTheFly)
            //		return GeoPathFinding.getInstance();
            //	}
            //	else // WORLD_PATHFINDING
            //	{
            //		//Higher Memoru Usage, Lower Cpu Usage (PreCalculated)
            //	}
            //}
            return _instance;
        }

        public abstract bool pathNodesExist(short regionoffset);
        public abstract List<AbstractNodeLoc> findPath(int gx, int gy, short z, int gtx, int gtz, short tz);
        public abstract Node[] readNeighbors(short node_x, short node_y, int idx);

        public LinkedList<AbstractNodeLoc> search(Node start, Node end)
        {
            // The simplest grid-based pathfinding. 
            // Drawback is not having higher cost for diagonal movement (means funny routes)
            // Could be optimized e.g. not to calculate backwards as far as forwards. 

            // List of Visited Nodes
            LinkedList<Node> visited = new LinkedList<Node>();

            // List of Nodes to Visit
            LinkedList<Node> to_visit = new LinkedList<Node>();
            to_visit.AddLast(start);

            int i = 0;
            while (i < 800)
            {
                Node node;
                try
                {
                    node = to_visit.First.Value;
                    to_visit.RemoveFirst();
                }
                catch (Exception e)
                {
                    // No Path found
                    return null;
                }

                if (node.equals(end)) //path found!
                {
                    return constructPath(node);
                }
                else
                {
                    i++;
                    visited.AddLast(node);
                    node.attacheNeighbors();
                    Node[] neighbors = node.getNeighbors();
                    if (neighbors == null) continue;
                    foreach (Node n in neighbors)
                    {
                        if (!visited.Contains(n) && !to_visit.Contains(n))
                        {
                            n.setParent(node);
                            to_visit.AddLast(n);
                        }
                    }
                }
            }
            //No Path found
            return null;
        }

        public LinkedList<AbstractNodeLoc> searchByClosest(Node start, Node end)
        {
            // Always continues checking from the closest to target non-blocked 
            // node from to_visit list. There's extra length in path if needed
            // to go backwards/sideways but when moving generally forwards, this is extra fast 
            // and accurate. And can reach insane distances (try it with 800 nodes..).
            // Minimum required node count would be around 300-400.
            // Generally returns a bit (only a bit) more intelligent looking routes than
            // the basic version. Not a true distance image (which would increase CPU
            // load) level of intelligence though. 

            // List of Visited Nodes
            List<Node> visited = new List<Node>(550);

            // List of Nodes to Visit
            LinkedList<Node> to_visit = new LinkedList<Node>();
            to_visit.AddLast(start);
            short targetx = end.getLoc().getNodeX();
            short targety = end.getLoc().getNodeY();
            int dx, dy;
            bool added;
            int i = 0;
            while (i < 550)
            {
                Node node;
                try
                {
                    node = to_visit.First.Value;
                    to_visit.RemoveFirst();
                }
                catch (Exception e)
                {
                    // No Path found
                    return null;
                }

                if (node.equals(end)) //path found!
                {
                    return constructPath(node);
                }
                else
                {
                    i++;
                    visited.Add(node);
                    node.attacheNeighbors();
                    Node[] neighbors = node.getNeighbors();
                    if (neighbors == null) continue;
                    foreach (Node n in neighbors)
                    {
                        if (!visited.Contains(n) && !to_visit.Contains(n))
                        {
                            added = false;
                            n.setParent(node);
                            dx = targetx - n.getLoc().getNodeX();
                            dy = targety - n.getLoc().getNodeY();
                            n.setCost(dx * dx + dy * dy);

                            LinkedListNode<Node> current = to_visit.First;
                            while (current != null)
                            {
                                if (current.Value.getCost() > n.getCost())
                                {
                                    to_visit.AddAfter(current, n);
                                    added = true;
                                    break;
                                }

                                current = current.Next;
                            }

                            if (!added)
                                to_visit.AddLast(n);
                        }
                    }
                }
            }
            //No Path found
            return null;
        }

        public LinkedList<AbstractNodeLoc> searchAStar(Node start, Node end)
        {
            // Not operational yet?
            int start_x = start.getLoc().getX();
            int start_y = start.getLoc().getY();
            int end_x = end.getLoc().getX();
            int end_y = end.getLoc().getY();
            //List of Visited Nodes
            List<Node> visited = new List<Node>(800);

            // List of Nodes to Visit
            BinaryNodeHeap to_visit = new BinaryNodeHeap(800);
            to_visit.add(start);

            int i = 0;
            while (i < 800)
            {
                Node node;
                try
                {
                    node = to_visit.removeFirst();
                }
                catch (Exception e)
                {
                    // No Path found
                    return null;
                }
                if (node.equals(end)) //path found!
                    return constructPath(node);
                else
                {
                    visited.Add(node);
                    node.attacheNeighbors();
                    foreach (Node n in node.getNeighbors())
                    {
                        if (!visited.Contains(n) && !to_visit.contains(n))
                        {
                            i++;
                            n.setParent(node);
                            n.setCost(Math.Abs(start_x - n.getLoc().getNodeX()) + Math.Abs(start_y - n.getLoc().getNodeY())
                                    + Math.Abs(end_x - n.getLoc().getNodeX()) + Math.Abs(end_y - n.getLoc().getNodeY()));
                            to_visit.add(n);
                        }
                    }
                }
            }
            //No Path found
            return null;
        }

        public LinkedList<AbstractNodeLoc> constructPath(Node node)
        {
            LinkedList<AbstractNodeLoc> path = new LinkedList<AbstractNodeLoc>();
            int previousdirectionx = -1000;
            int previousdirectiony = -1000;
            int directionx;
            int directiony;
            while (node.getParent() != null)
            {
                // only add a new route point if moving direction changes
                directionx = node.getLoc().getNodeX() - node.getParent().getLoc().getNodeX();
                directiony = node.getLoc().getNodeY() - node.getParent().getLoc().getNodeY();
                if (directionx != previousdirectionx || directiony != previousdirectiony)
                {
                    previousdirectionx = directionx;
                    previousdirectiony = directiony;
                    path.AddFirst(node.getLoc());
                }
                node = node.getParent();
            }
            return path;
        }

        /**
		 * Convert geodata position to pathnode position
		 * @param geo_pos
		 * @return pathnode position
		 */
        public short getNodePos(int geo_pos)
        {
            return (short)(geo_pos >> 3); //OK?
        }

        /**
		 * Convert node position to pathnode block position
		 * @param geo_pos
		 * @return pathnode block position (0...255)
		 */
        public short getNodeBlock(int node_pos)
        {
            return (short)(node_pos % 256);
        }

        public byte getRegionX(int node_pos)
        {
            return (byte)((node_pos >> 8) + 16);
        }

        public byte getRegionY(int node_pos)
        {
            return (byte)((node_pos >> 8) + 10);
        }

        public short getRegionOffset(byte rx, byte ry)
        {
            return (short)((rx << 5) + ry);
        }

        /**
		 * Convert pathnode x to World x position
		 * @param node_x, rx
		 * @return
		 */
        public int calculateWorldX(short node_x)
        {
            return GeoStructure.WorldXMin + node_x * 128 + 48;
        }

        /**
		 * Convert pathnode y to World y position
		 * @param node_y
		 * @return
		 */
        public int calculateWorldY(short node_y)
        {
            return GeoStructure.WorldYMin + node_y * 128 + 48;
        }
    }
}
