using System;
using System.Runtime.InteropServices;

namespace Foundation.Architecture
{
    /// <summary>
    /// Fastest serialize possible
    /// </summary>
    public static class StructHelper
    {
        /// <summary>
        /// Reads a struct into a buffer
        /// </summary>
        public static T ReadStruct<T>(byte[] buffer, int startIndex = 0) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));

            if (buffer.Length + startIndex < size)
                throw new ArgumentOutOfRangeException("Struct exceeds buffer size");

            T str = new T();

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, startIndex, ptr, size);
            str = (T)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        /// <summary>
        /// Writes the struct to a byter buffer
        /// </summary>
        public static byte[] WriteStruct<T>(T instance) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(instance, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;

        }

        /// <summary>
        /// Writes the struct to a byter buffer
        /// </summary>
        public static void WriteStruct<T>( T instance, byte[] buffer, int startIndex = 0) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            
            if(buffer.Length + startIndex < size)
                throw new ArgumentOutOfRangeException("Struct exceeds buffer size");
            
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(instance, ptr, true);
            Marshal.Copy(ptr, buffer, startIndex, size);
            Marshal.FreeHGlobal(ptr);

        }
    }
}