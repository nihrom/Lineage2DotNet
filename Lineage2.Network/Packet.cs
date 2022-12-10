using Lineage2.Unility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lineage2.Network
{
    /// <summary>
    /// Represents client packet structure.
    /// </summary>
    public struct Packet
    {
        /// <summary>
        /// Overflow value.
        /// </summary>
        private const int DefaultOverflowValue = 128;

        /// <summary>
        /// Buffer.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// Reader/writer offset.
        /// </summary>
        private int _offset;

        /// <summary>
        /// Was received or created to be sent.
        /// </summary>
        private readonly bool _receivedPacket;

        /// <summary>
        /// Initializes new instance of <see cref="Packet"/> (received packet).
        /// </summary>
        /// <param name="headerOffset"> Header offset (for opcodes).</param>
        /// <param name="buffer">Buffer.</param>
        public Packet(int headerOffset, byte[] buffer)
        {
            _receivedPacket = true;
            _buffer = buffer;
            _offset = headerOffset;
        }

        /// <summary>
        /// Initializes new instance of <see cref="Packet"/> (packet to send).
        /// </summary>
        /// <param name="opcodes">Packet opcodes.</param>
        public Packet(params byte[] opcodes)
        {
            _receivedPacket = false;
            _buffer = opcodes;
            _offset = opcodes.Length;
        }

        /// <summary>
        /// Writes byte value into packet buffer.
        /// </summary>
        /// <param name="value"><see cref="byte"/> value to write.</param>
        public unsafe void WriteByte(byte value)
        {
            ValidateBufferSize(sizeof(byte));

            fixed (byte* buf = _buffer)
                *(buf + _offset++) = value;
        }

        /// <summary>
        /// Writes array of <see cref="byte"/> values into packet buffer.
        /// </summary>
        /// <param name="value">Array of <see cref="byte"/> values.</param>
        public void WriteByte(params byte[] value)
        {
            WriteByteArray(value);
        }

        /// <summary>
        /// Writes array of <see cref="byte"/> into packet buffer.
        /// </summary>
        /// <param name="value">Array of <see cref="byte"/> values.</param>
        public void WriteByteArray(byte[] value)
        {
            int length = value.Length;

            ValidateBufferSize(length);

            L2Buffer.Copy(value, 0, _buffer, _offset, length);
            _offset += length;
        }

        /// <summary>
        /// Writes <see cref="short"/> value into packet buffer.
        /// </summary>
        /// <param name="value"><see cref="short"/> value.</param>
        public unsafe void WriteShort(short value)
        {
            ValidateBufferSize(sizeof(short));

            fixed (byte* buf = _buffer)
                *(short*)(buf + _offset) = value;

            _offset += sizeof(short);
        }

        /// <summary>
        /// Writes array of <see cref="short"/> values into packet buffer.
        /// </summary>
        /// <param name="value">Array of <see cref="short"/> values.</param>
        public unsafe void WriteShort(params short[] value)
        {
            int length = value.Length * sizeof(short);

            ValidateBufferSize(length);

            fixed (byte* buf = _buffer)
            {
                fixed (short* w = value)
                    L2Buffer.UnsafeCopy(w, length, buf, ref _offset);
            }
        }

        /// <summary>
        /// Writes <see cref="int"/> value to packet buffer.
        /// </summary>
        /// <param name="value"><see cref="int"/> value.</param>
        public unsafe void WriteInt(int value)
        {
            ValidateBufferSize(sizeof(int));

            fixed (byte* buf = _buffer)
                *(int*)(buf + _offset) = value;

            _offset += sizeof(int);
        }

        /// <summary>
        /// Writes array of <see cref="int"/> values into packet buffer.
        /// </summary>
        /// <param name="value">Array of <see cref="int"/> values.</param>
        public unsafe void WriteInt(params int[] value)
        {
            int length = value.Length * sizeof(int);

            ValidateBufferSize(Length);

            fixed (byte* buf = _buffer)
            {
                fixed (int* w = value)
                    L2Buffer.UnsafeCopy(w, length, buf, ref _offset);
            }
        }

        /// <summary>
        /// Writes array of <see cref="int"/> values into packet buffer.
        /// </summary>
        /// <param name="value">Array of <see cref="int"/> values.</param>
        public unsafe void WriteIntArray(int[] value)
        {
            int length = value.Length * sizeof(int);

            ValidateBufferSize(Length);

            fixed (byte* buf = _buffer)
            {
                fixed (int* w = value)
                    L2Buffer.UnsafeCopy(w, length, buf, ref _offset);
            }
        }

        /// <summary>
        /// Writes <see cref="double"/> value into packet buffer.
        /// </summary>
        /// <param name="value"><see cref="double"/> value.</param>
        public unsafe void WriteDouble(double value)
        {
            ValidateBufferSize(sizeof(double));

            fixed (byte* buf = _buffer)
                *(double*)(buf + _offset) = value;

            _offset += sizeof(double);
        }

        /// <summary>
        /// Writes array of <see cref="double"/> values into packet buffer.
        /// </summary>
        /// <param name="value">Array of <see cref="double"/> values.</param>
        public unsafe void WriteDouble(params double[] value)
        {
            int length = value.Length * sizeof(double);

            ValidateBufferSize(length);

            fixed (byte* buf = _buffer)
            {
                fixed (double* w = value)
                    L2Buffer.UnsafeCopy(w, length, buf, ref _offset);
            }
        }

        /// <summary>
        /// Writes <see cref="long"/> value into packet buffer.
        /// </summary>
        /// <param name="value"><see cref="long"/> value.</param>
        public unsafe void WriteLong(long value)
        {
            ValidateBufferSize(sizeof(long));

            fixed (byte* buf = _buffer)
                *(long*)(buf + _offset) = value;

            _offset += sizeof(long);
        }

        /// <summary>
        /// Writes array of <see cref="long"/> values into packet buffer.
        /// </summary>
        /// <param name="value">Array of <see cref="long"/> values.</param>
        public unsafe void WriteLong(params long[] value)
        {
            int length = value.Length * sizeof(long);

            ValidateBufferSize(length);

            fixed (byte* buf = _buffer)
            {
                fixed (long* w = value)
                    L2Buffer.UnsafeCopy(w, length, buf, ref _offset);
            }
        }

        /// <summary>
        /// Writes <see cref="string"/> object into packet buffer.
        /// </summary>
        /// <param name="s"><see cref="string"/> value.</param>
        public unsafe void WriteString(string s)
        {
            s += '\0';
            int length = s.Length * sizeof(char);

            ValidateBufferSize(length);

            fixed (byte* buf = _buffer)
            {
                fixed (char* w = s)
                    L2Buffer.UnsafeCopy(w, length, buf, ref _offset);
            }
        }

        /// <summary>
        /// Writes array of <see cref="string"/> values to packet buffer.
        /// </summary>
        /// <param name="s">Array of <see cref="string"/> values.</param>
        public unsafe void WriteString(params string[] s)
        {
            string value = string.Join(string.Empty, s.Select(t => t + '\0').ToArray());

            int length = value.Length * sizeof(char);

            ValidateBufferSize(length);

            fixed (byte* buf = _buffer)
            {
                fixed (char* w = value)
                    L2Buffer.UnsafeCopy(w, length, buf, ref _offset);
            }
        }

        /// <summary>
        /// Writes <see cref="bool"/> value to packet buffer. (Inner network only)
        /// </summary>
        /// <param name="value"><see cref="bool"/> value.</param>
        public void InternalWriteBool(bool value)
        {
            WriteByte(value ? (byte)0x01 : (byte)0x00);
        }

        /// <summary>
        /// Writes <see cref="DateTime"/> value to packet buffer. (Inner network only)
        /// </summary>
        /// <param name="value"><see cref="DateTime"/> value.</param>
        public void InternalWriteDateTime(DateTime value)
        {
            WriteLong(value.Ticks);
        }

        /// <summary>
        /// Reads <see cref="byte"/> value from packet buffer.
        /// </summary>
        /// <returns><see cref="byte"/> value.</returns>
        public unsafe byte ReadByte()
        {
            fixed (byte* buf = _buffer)
                return *(buf + _offset++);
        }

        /// <summary>
        /// Reads array of <see cref="byte"/> values from packet buffer.
        /// </summary>
        /// <param name="length">length of array to read.</param>
        /// <returns>Array of <see cref="byte"/> values.</returns>
        public unsafe byte[] ReadBytesArray(int length)
        {
            byte[] dest = new byte[length];

            fixed (byte* buf = _buffer, dst = dest)
                L2Buffer.Copy(buf, length, dst, ref _offset);
            return dest;
        }

        public byte[] ReadByteArrayAlt(int length)
        {
            byte[] result = new byte[length];
            Array.Copy(GetBuffer(), _offset, result, 0, length);
            _offset += length;
            return result;
        }

        /// <summary>
        /// Reads <see cref="short"/> value from packet buffer.
        /// </summary>
        /// <returns><see cref="short"/> value.</returns>
        public unsafe short ReadShort()
        {
            fixed (byte* buf = _buffer)
            {
                short value = *(short*)(buf + _offset);
                _offset += sizeof(short);
                return value;
            }
        }

        /// <summary>
        /// Reads <see cref="int"/> value from packet buffer. readD
        /// </summary>
        /// <returns><see cref="int"/> value.</returns>
        public unsafe int ReadInt()
        {
            fixed (byte* buf = _buffer)
            {
                int value = *(int*)(buf + _offset);
                _offset += sizeof(int);
                return value;
            }
        }

        /// <summary>
        /// Reads <see cref="double"/> value from packet buffer.
        /// </summary>
        /// <returns><see cref="double"/> value.</returns>
        public unsafe double ReadDouble()
        {
            fixed (byte* buf = _buffer)
            {
                double value = *(double*)(buf + _offset);
                _offset += sizeof(double);
                return value;
            }
        }

        /// <summary>
        /// Reads <see cref="long"/> value from packet buffer.
        /// </summary>
        /// <returns><see cref="long"/> value.</returns>
        public unsafe long ReadLong()
        {
            fixed (byte* buf = _buffer)
            {
                long value = *(long*)(buf + _offset);
                _offset += sizeof(long);
                return value;
            }
        }

        /// <summary>
        /// Reads <see cref="string"/> value from packet buffer.
        /// </summary>
        /// <returns><see cref="string"/> value.</returns>
        public unsafe string ReadString()
        {
            fixed (byte* buf = _buffer)
                return L2Buffer.GetTrimmedString(buf, ref _offset, _buffer.Length);
        }

        /// <summary>
        /// Reads <see cref="bool"/> value from packet buffer. (Inner network only)
        /// </summary>
        /// <returns><see cref="bool"/> value.</returns>
        public bool InternalReadBool()
        {
            return ReadByte() == 0x01;
        }

        /// <summary>
        /// Reads <see cref="DateTime"/> value from packet buffer. (Inner network only)
        /// </summary>
        /// <returns><see cref="DateTime"/> value.</returns>
        public DateTime InternalReadDateTime()
        {
            return new DateTime(ReadLong());
        }

        /// <summary>
        /// Validates buffer capacity before writing into it.
        /// </summary>
        /// <param name="nextValueLength">length of next bytes sequence to write into buffer.</param>
        private void ValidateBufferSize(int nextValueLength)
        {
            if ((_offset + nextValueLength) > _buffer.Length)
                L2Buffer.Extend(ref _buffer, nextValueLength + DefaultOverflowValue);
        }

        /// <summary>
        /// Resizes <see cref="Packet"/> buffer to it's actual capacity and appends buffer length to the beginning of <see cref="Packet"/> buffer.
        /// </summary>
        /// <param name="headerSize"><see cref="Packet"/> header (opcodes) capacity.</param>
        public unsafe void Prepare(int headerSize)
        {
            _offset += headerSize;

            L2Buffer.Extend(ref _buffer, headerSize, _offset);

            fixed (byte* buf = _buffer)
            {
                if (headerSize == sizeof(short))
                    *(short*)buf = (short)_offset;
                else
                    *(int*)buf = _offset;
            }
        }

        /// <summary>
        /// Returns packet buffer.
        /// </summary>
        /// <returns>Packet buffer.</returns>
        public byte[] GetBuffer()
        {
            return _buffer;
        }

        /// <summary>
        /// Returns packet buffer.
        /// </summary>
        /// <param name="skipFirstBytesCount">Amount of first bytes to skip.</param>
        /// <returns>Buffer without provided amount of first bytes.</returns>
        public byte[] GetBuffer(int skipFirstBytesCount)
        {
            return L2Buffer.Copy(_buffer, skipFirstBytesCount, new byte[_buffer.Length - skipFirstBytesCount], 0, _buffer.Length - skipFirstBytesCount);
        }

        /// <summary>
        /// Moves <see cref="Packet"/> offset position.
        /// </summary>
        /// <param name="size">Additional offset length.</param>
        public void MoveOffset(int size)
        {
            _offset += size;
        }

        /// <summary>
        /// Gets first packet opcode.
        /// </summary>
        public unsafe byte FirstOpcode
        {
            get
            {
                fixed (byte* buf = _buffer)
                    return *buf;
            }
        }

        /// <summary>
        /// Gets second packet opcode.
        /// </summary>
        public unsafe int SecondOpcode
        {
            get
            {
                fixed (byte* buf = _buffer)
                    return *(buf + 1);
            }
        }

        /// <summary>
        /// Gets packet capacity.
        /// </summary>
        public int Length => _receivedPacket ? _buffer.Length : _offset;

        /// <summary>
        /// Returns string representation of current packet.
        /// </summary>
        /// <returns>String representation of current packet.</returns>
        public override string ToString()
        {
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendLine("Packet dump:");
            //sb.AppendFormat("1s op: {0}{2}2d op: {1}{2}", FirstOpcode, SecondOpcode, Environment.NewLine);
            //sb.Append(L2Buffer.ToString(m_Buffer));
            //return sb.ToString();
            return L2Buffer.ToString(_buffer);
        }
    }
}
