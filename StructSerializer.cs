﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ManagedWin32
{
    /// <summary>
    /// Holds a set of utilities.
    /// </summary>
    public static class StructSerializer
    {
        /// <summary>
        /// Reads a structure of type T from the input stream.
        /// </summary>
        /// <typeparam name="T">The structure type to be read.</typeparam>
        /// <param name="inputStream">The input stream to read from.</param>
        /// <returns>A structure of type T that was read from the stream.</returns>
        public static T Read<T>(this Stream instream) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            instream.Read(buffer, 0, size);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, ptr, size);
            object ret = Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);

            return (T)ret;
        }
        /// <summary>
        /// Writes as structure of type T to the output stream.
        /// </summary>
        /// <typeparam name="T">The structure type to be written.</typeparam>
        /// <param name="outputStream">The output stream to write to.</param>
        /// <param name="structure">The structure to be written.</param>
        public static void Write<T>(this T structure, Stream stream) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);
            stream.Write(buffer, 0, size);
        }
    }
}