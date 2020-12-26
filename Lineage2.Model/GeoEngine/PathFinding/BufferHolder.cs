using Lineage2.Model.Configs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lineage2.Model.GeoEngine.PathFinding
{
    /**
  * NodeBuffer container with specified size and count of separate buffers.
  */
    public class BufferHolder
    {
        public int _size;
        public int _count;
        public List<NodeBuffer> _buffer;

        // statistics
        public int _playableUses = 0;
        public int _uses = 0;
        public int _playableOverflows = 0;
        public int _overflows = 0;
        public long _elapsed = 0;

        public BufferHolder(int size, int count, GeodataConfig geodataConfig)
        {
            _size = size;
            _count = count;
            _buffer = new List<NodeBuffer>(count);

            for (int i = 0; i < count; i++)
                _buffer.Add(new NodeBuffer(size, geodataConfig));
        }

        //    public String toString()
        //    {
        //        StringBuilder sb = new StringBuilder(100);

        //        StringUtil.append(sb, "Buffer ", String.valueOf(_size), "x", String.valueOf(_size), ": count=", String.valueOf(_count), " uses=", String.valueOf(_playableUses), "/", String.valueOf(_uses));

        //        if (_uses > 0)
        //            StringUtil.append(sb, " total/avg(ms)=", String.valueOf(_elapsed), "/", String.format("%1.2f", (double)_elapsed / _uses));

        //        StringUtil.append(sb, " ovf=", String.valueOf(_playableOverflows), "/", String.valueOf(_overflows));

        //        return sb.toString();
        //    }
    }
}
