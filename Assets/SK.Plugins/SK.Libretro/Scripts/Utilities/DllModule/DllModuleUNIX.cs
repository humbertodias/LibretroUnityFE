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
    public abstract class DllModuleUNIX : DllModule
    {
        [DllImport("libdl", EntryPoint = "dlopen", SetLastError = true)]
        private static extern IntPtr PlatformLoadLibrary(string fileName, int flags);

        [DllImport("libdl", EntryPoint = "dlsym", SetLastError = true)]
        private static extern IntPtr PlatformGetProcAddress(IntPtr handle, string symbol);

        [DllImport("libdl", EntryPoint = "dlclose", SetLastError = true)]
        private static extern int PlatformFreeLibrary(IntPtr handle);

        [DllImport("libdl", EntryPoint = "dlerror")]
        private static extern IntPtr PlatformLibraryError();

        private const int RTLD_NOW = 2;

        protected DllModuleUNIX(string extension)
        : base(extension)
        {
        }

        protected sealed override void LoadLibrary()
        {
            _nativeHandle = PlatformLoadLibrary(Path, RTLD_NOW);
            IntPtr error  = PlatformLibraryError();
            if (error != IntPtr.Zero)
            {
                throw new Exception($"Failed to load library '{Name}' at path '{Path}' (ErrorMessage: {Marshal.PtrToStringAnsi(error)})");
            }
        }

        protected sealed override IntPtr GetProcAddress(string functionName)
        {
            IntPtr procAddress = PlatformGetProcAddress(_nativeHandle, functionName);
            IntPtr error       = PlatformLibraryError();
            if (error != IntPtr.Zero)
            {
                throw new Exception($"Failed to get function '{functionName}' in library '{Name}' at path '{Path}' (ErrorMessage: {Marshal.PtrToStringAnsi(error)})");
            }

            return procAddress;
        }

        protected sealed override void FreeLibrary()
        {
            _ = PlatformFreeLibrary(_nativeHandle);
            IntPtr error = PlatformLibraryError();
            if (error != IntPtr.Zero)
            {
                throw new Exception($"Failed to free library '{Name}' at path '{Path}' (ErrorMessage: {Marshal.PtrToStringAnsi(error)})");
            }
        }
    }
}
