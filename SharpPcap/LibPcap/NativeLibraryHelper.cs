// Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
// SPDX-License-Identifier: MIT

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    class NativeLibraryHelper
    {
        public delegate IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath);

        private static readonly Type NativeLibraryType = typeof(DllImportSearchPath).Assembly
                .GetType("System.Runtime.InteropServices.NativeLibrary");

        public static void SetDllImportResolver(Assembly assembly, DllImportResolver resolver)
        {
            if (NativeLibraryType == null)
            {
                return;
            }

#if NET6_0_OR_GREATER
            NativeLibrary.SetDllImportResolver(assembly, (lib, asm, path) => resolver(lib, asm, path));
#else
            var dllImportResolverType = typeof(DllImportSearchPath).Assembly
                .GetType("System.Runtime.InteropServices.DllImportResolver");

            var setDllImportResolverMethod = NativeLibraryType
                .GetMethod(
                    "SetDllImportResolver",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(Assembly), dllImportResolverType },
                    null
                );

            setDllImportResolverMethod.Invoke(null, new object[] {
                assembly,
                Delegate.CreateDelegate(dllImportResolverType, resolver, "Invoke")
            });
#endif
        }

        public static bool TryLoad(string libraryPath, out IntPtr handle)
        {
            var tryLoadMethod = NativeLibraryType
                ?.GetMethod(
                    "TryLoad",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(string), typeof(IntPtr).MakeByRefType() },
                    null
                );
            if (tryLoadMethod == null)
            {
                handle = IntPtr.Zero;
                return false;
            }
            var args = new object[] { libraryPath, IntPtr.Zero };
            var res = (bool)tryLoadMethod.Invoke(null, args);
            handle = (IntPtr)args[1];
            return res;
        }
    }
}
