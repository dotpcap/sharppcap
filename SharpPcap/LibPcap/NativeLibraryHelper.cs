using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    class NativeLibraryHelper
    {

        public delegate IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath);

        private static readonly Type NativeLibraryType;
        static NativeLibraryHelper()
        {
            NativeLibraryType = typeof(DllImportSearchPath).Assembly
                .GetType("System.Runtime.InteropServices.NativeLibrary");
        }

        public static void SetDllImportResolver(Assembly assembly, DllImportResolver resolver)
        {
            if (NativeLibraryType == null)
            {
                return;
            }

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
                resolver
            });
        }

        public static IntPtr Load(string libraryPath)
        {
            var loadMethod = NativeLibraryType
                ?.GetMethod(
                    "Load",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(string) },
                    null
                );
            if (loadMethod == null)
            {
                return IntPtr.Zero;
            }
            return (IntPtr)loadMethod.Invoke(null, new object[] { libraryPath });
        }
    }
}
