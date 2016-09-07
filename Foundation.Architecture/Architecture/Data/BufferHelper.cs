// Nicholas Ventimiglia 2016-09-05

using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace Foundation.Architecture
{
    /// <summary>
    /// Unsafe I/O for byte arrays
    /// </summary>
    /// <remarks>
    /// Here incase we need it. May not, depends on how we pool things.
    /// </remarks>
    public static class BufferHelper
    {
        #region Read

        public static unsafe bool ReadBool(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(bool));

            bool result;
            fixed (byte* ptr = buffer)
            {
                result = *(bool*) (ptr + index);
            }
            return result;
        }

        public static unsafe byte ReadByte(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(byte));

            byte result;
            fixed (byte* ptr = buffer)
            {
                result = *(byte*) (ptr + index);
            }
            return result;
        }

        public static unsafe short ReadInt16(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(short));

            short result;
            fixed (byte* ptr = buffer)
            {
                result = *(short*) (ptr + index);
            }
            return result;
        }

        public static unsafe ushort ReadUInt16(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(ushort));

            ushort result;
            fixed (byte* ptr = buffer)
            {
                result = *(ushort*) (ptr + index);
            }
            return result;
        }

        public static unsafe int ReadInt32(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(int));

            int result;
            fixed (byte* ptr = buffer)
            {
                result = *(int*) (ptr + index);
            }
            return result;
        }

        public static unsafe uint ReadUInt32(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(uint));

            uint result;
            fixed (byte* ptr = buffer)
            {
                result = *(uint*) (ptr + index);
            }
            return result;
        }

        public static unsafe long ReadInt64(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(long));

            long result;
            fixed (byte* ptr = buffer)
            {
                result = *(long*) (ptr + index);
            }
            return result;
        }

        public static unsafe ulong ReadUInt64(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(ulong));

            ulong result;
            fixed (byte* ptr = buffer)
            {
                result = *(ulong*) (ptr + index);
            }
            return result;
        }

        public static unsafe double ReadDouble(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(double));

            double result;
            fixed (byte* ptr = buffer)
            {
                result = *(double*) (ptr + index);
            }
            return result;
        }

        public static unsafe float ReadFloat(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(float));

            float result;
            fixed (byte* ptr = buffer)
            {
                result = *(float*) (ptr + index);
            }
            return result;
        }

        public static unsafe char ReadChar(byte[] buffer, int index)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(char));

            char result;
            fixed (byte* ptr = buffer)
            {
                result = *(char*) (ptr + index);
            }
            return result;
        }

        public static string ReadString(byte[] buffer, int index)
        {
            //http://stackoverflow.com/questions/10773440/conversion-in-net-native-utf-8-managed-string
            var length = ReadUInt16(buffer, index);
            var result = Encoding.UTF8.GetString(buffer, index + sizeof(ushort), length);
            return result;
        }

        #endregion

        #region Write

        public static void Write(byte[] buffer, int index, byte[] payload, int count)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + count);

            Buffer.BlockCopy(payload, 0, buffer, index, count);
        }

        public static void Write(byte[] buffer, int index, byte b)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(byte));

            buffer[index] = b;
        }

        public static unsafe void Write(byte[] buffer, int index, bool value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(bool));

            fixed (byte* ptr = buffer)
            {
                *(bool*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, short value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(short));

            fixed (byte* ptr = buffer)
            {
                *(short*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, ushort value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(ushort));

            fixed (byte* ptr = buffer)
            {
                *(ushort*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, int value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(int));

            fixed (byte* ptr = buffer)
            {
                *(int*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, uint value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(uint));

            fixed (byte* ptr = buffer)
            {
                *(uint*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, double value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(double));

            fixed (byte* ptr = buffer)
            {
                *(double*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, long value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(long));

            fixed (byte* ptr = buffer)
            {
                *(long*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, ulong value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(ulong));

            fixed (byte* ptr = buffer)
            {
                *(ulong*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, float value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(float));

            fixed (byte* ptr = buffer)
            {
                *(float*) (ptr + index) = value;
            }
        }

        public static unsafe void Write(byte[] buffer, int index, char value)
        {
            Contract.Ensures(buffer != null);
            Contract.Ensures(buffer.Length >= index + sizeof(char));

            fixed (byte* ptr = buffer)
            {
                *(char*) (ptr + index) = value;
            }
        }

        public static void Write(byte[] buffer, int index, string value)
        {
            Contract.Ensures(buffer != null);
            var payload = Encoding.UTF8.GetBytes(value);
            Write(buffer, index, (ushort) payload.Length);
            Write(buffer, index + sizeof(ushort), payload, payload.Length);
        }

        #endregion
    }
}