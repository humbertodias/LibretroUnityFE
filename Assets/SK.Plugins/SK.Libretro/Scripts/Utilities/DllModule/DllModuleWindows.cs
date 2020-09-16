﻿/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using System;
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public sealed class DllModuleWindows : DllModule
    {
        [DllImport("kernel32", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr PlatformLoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr PlatformGetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32", EntryPoint = "FreeLibrary", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool PlatformFreeLibrary(IntPtr hModule);

        public DllModuleWindows()
        : base("dll")
        {
        }

        protected override void LoadLibrary()
        {
            _nativeHandle = PlatformLoadLibrary(Path);
            if (_nativeHandle == null)
            {
                throw new Exception($"Failed to load library '{Name}' at path '{Path}' (ErrorCode: {Marshal.GetLastWin32Error()})");
            }
        }

        protected override IntPtr GetProcAddress(string functionName)
        {
            IntPtr procAddress = PlatformGetProcAddress(_nativeHandle, functionName);
            if (procAddress == null)
            {
                throw new Exception($"Failed to get function '{functionName}' in library '{Name}' at path '{Path}' (ErrorCode: {Marshal.GetLastWin32Error()})");
            }

            return procAddress;
        }

        protected override void FreeLibrary()
        {
            if (!PlatformFreeLibrary(_nativeHandle))
            {
                throw new Exception($"Failed to free library '{Name}' at path '{Path}' (ErrorCode: {Marshal.GetLastWin32Error()})");
            }
        }
    }
}
