using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    public class GeoPathFinding : PathFinding
    {
        private readonly ILogger logger = Log.Logger.ForContext<GeoPathFinding>();
        private static Dictionary<short, BinaryReader> _pathNodes = new Dictionary<short, BinaryReader>();
        private static Dictionary<short, int[]> _pathNodesIndex = new Dictionary<short, int[]>();

        public override bool pathNodesExist(short regionoffset)
        {
            return _pathNodesIndex.ContainsKey(regionoffset);
        }

        public override LinkedList<AbstractNodeLoc> findPath(int gx, int gy, short z, int gtx, int gty, short tz)
        {
            Node start = readNode(gx, gy, z);
            Node end = readNode(gtx, gty, tz);

            if (start == null || end == null)
                return null;

            if (start == end)
                return null;

            return searchByClosest(start, end);
        }

        public override Node[] readNeighbors(short node_x, short node_y, int idx)
        {
            short regoffset = getRegionOffset(getRegionX(node_x), getRegionY(node_y));
            var pn = _pathNodes[regoffset];

            List<Node> Neighbors = new List<Node>(8);
            Node newNode;
            short new_node_x, new_node_y;

            //Region for sure will change, we must read from correct file
            pn.BaseStream.Position = idx;
            byte neighbor = pn.ReadByte();//N
            idx++;
            if (neighbor > 0)
            {
                neighbor--;
                new_node_x = node_x;
                new_node_y = (short)(node_y - 1);
                newNode = readNode(new_node_x, new_node_y, neighbor);
                if (newNode != null) Neighbors.Add(newNode);
            }

            pn.BaseStream.Position = idx;
            neighbor = pn.ReadByte(); //NE
            idx++;

            if (neighbor > 0)
            {
                neighbor--;
                new_node_x = (short)(node_x + 1);
                new_node_y = (short)(node_y - 1);
                newNode = readNode(new_node_x, new_node_y, neighbor);
                if (newNode != null) Neighbors.Add(newNode);
            }

            pn.BaseStream.Position = idx;
            neighbor = pn.ReadByte(); //E
            idx++;

            if (neighbor > 0)
            {
                neighbor--;
                new_node_x = (short)(node_x + 1);
                new_node_y = node_y;
                newNode = readNode(new_node_x, new_node_y, neighbor);
                if (newNode != null) Neighbors.Add(newNode);
            }

            pn.BaseStream.Position = idx;
            neighbor = pn.ReadByte(); //SE
            idx++;

            if (neighbor > 0)
            {
                neighbor--;
                new_node_x = (short)(node_x + 1);
                new_node_y = (short)(node_y + 1);
                newNode = readNode(new_node_x, new_node_y, neighbor);
                if (newNode != null) Neighbors.Add(newNode);
            }

            pn.BaseStream.Position = idx;
            neighbor = pn.ReadByte(); //S
            idx++;

            if (neighbor > 0)
            {
                neighbor--;
                new_node_x = node_x;
                new_node_y = (short)(node_y + 1);
                newNode = readNode(new_node_x, new_node_y, neighbor);
                if (newNode != null) Neighbors.Add(newNode);
            }

            pn.BaseStream.Position = idx;
            neighbor = pn.ReadByte(); //SW
            idx++;

            if (neighbor > 0)
            {
                neighbor--;
                new_node_x = (short)(node_x - 1);
                new_node_y = (short)(node_y + 1);
                newNode = readNode(new_node_x, new_node_y, neighbor);
                if (newNode != null) Neighbors.Add(newNode);
            }

            pn.BaseStream.Position = idx;
            neighbor = pn.ReadByte(); //W
            idx++;

            if (neighbor > 0)
            {
                neighbor--;
                new_node_x = (short)(node_x - 1);
                new_node_y = node_y;
                newNode = readNode(new_node_x, new_node_y, neighbor);
                if (newNode != null) Neighbors.Add(newNode);
            }

            pn.BaseStream.Position = idx;
            neighbor = pn.ReadByte(); //NW
            idx++;

            if (neighbor > 0)
            {
                neighbor--;
                new_node_x = (short)(node_x - 1);
                new_node_y = (short)(node_y - 1);
                newNode = readNode(new_node_x, new_node_y, neighbor);
                if (newNode != null) Neighbors.Add(newNode);
            }

            return Neighbors.ToArray();
        }

        private Node readNode(short node_x, short node_y, byte layer)
        {
            short regoffset = getRegionOffset(getRegionX(node_x), getRegionY(node_y));
            if (!pathNodesExist(regoffset)) return null;
            short nbx = getNodeBlock(node_x);
            short nby = getNodeBlock(node_y);
            int idx = _pathNodesIndex[regoffset][(nby << 8) + nbx];
            var pn = _pathNodes[regoffset];

            //reading
            pn.BaseStream.Position = idx;
            byte nodes = pn.ReadByte();
            idx += layer * 10 + 1;//byte + layer*10byte
            if (nodes < layer)
            {
                logger.Warning("readNode - что то подозрительное");
            }
            pn.BaseStream.Position = idx;
            short node_z = pn.ReadInt16();
            idx += 2;
            return new Node(new GeoNodeLoc(node_x, node_y, node_z), idx, this);
        }

        private Node readNode(int gx, int gy, short z)
        {
            short node_x = getNodePos(gx);
            short node_y = getNodePos(gy);
            short regoffset = getRegionOffset(getRegionX(node_x), getRegionY(node_y));
            if (!pathNodesExist(regoffset)) return null;
            short nbx = getNodeBlock(node_x);
            short nby = getNodeBlock(node_y);
            int idx = _pathNodesIndex[regoffset][(nby << 8) + nbx];
            var pn = _pathNodes[regoffset];

            //reading
            pn.BaseStream.Position = idx;
            byte nodes = pn.ReadByte();
            idx++;//byte
            int idx2 = 0; //create index to nearlest node by z
            short last_z = short.MinValue;
            while (nodes > 0)
            {
                pn.BaseStream.Position = idx;
                short node_z = pn.ReadInt16();
                if (Math.Abs(last_z - z) > Math.Abs(node_z - z))
                {
                    last_z = node_z;
                    idx2 = idx + 2;
                }
                idx += 10; //short + 8 byte
                nodes--;
            }
            return new Node(new GeoNodeLoc(node_x, node_y, last_z), idx2, this);
        }

        public GeoPathFinding()
        {
            string fname = "./data/pathnode/pn_index.txt";
            string path = Path.GetFullPath(fname);

            List<string> names = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        names.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Не удалось загрузить и обработать pn_index.txt");
                throw;
            }

            names.ToArray()
                .AsParallel()
                .ForAll(rawName =>
                {
                    var arrName = rawName.Split('_');
                    byte rx = byte.Parse(arrName[0]);
                    byte ry = byte.Parse(arrName[1]);

                    LoadPathNodeFile(rx, ry);
                });
        }

        private void LoadPathNodeFile(byte rx, byte ry)
        {
            string fname = "./data/pathnode/" + rx + "_" + ry + ".pn";
            string path = Path.GetFullPath(fname);
            short regionoffset = getRegionOffset(rx, ry);
            logger.Information("PathFinding Engine: - Загружается: " + fname + " -> region offset: " + regionoffset + "X: " + rx + " Y: " + ry);

            int node = 0, size, index = 0;
            //TODO:Тут надо бы обернуть в try catch
            //TODO:Надо загружать прямиком в память каким нибудь образом, в зависимости от того, дебаг или релиз 
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));

            int[] intBuffer = new int[65536];
            while (node < 65536)
            {
                byte layer = reader.ReadByte();
                intBuffer[node] = index;
                node++;
                index += layer * 10 + 1;
                reader.BaseStream.Position = index;
            }

            _pathNodesIndex.Add(regionoffset, intBuffer);
            _pathNodes.Add(regionoffset, reader);
        }
    }
}
