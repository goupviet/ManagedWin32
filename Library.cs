using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ManagedWin32
{
    public enum ResourceType
    {
        Curor = 1,
        Bitmap = 2,
        Icon = 3,
        Menu = 4,
        Dialog = 5,
        String = 6,
        FontDirectory = 7,
        Font = 8,
        Accelerator = 9,
        RCData = 10,
        MessageTable = 11,
        GroupCursor = 12,
        GroupIcon = 14,
        Version = 16,
        DLGInclude = 17,
        PlugPlay = 19,
        VXD = 20,
        AniCursor = 21,
        AniIcon = 22,
        HTML = 23
    }

    [Flags]
    public enum LoadLibraryFlags
    {
        DontResolveDllReferences = 0x00000001,
        LoadAsDataFile = 0x00000002,
        LoadWithAlteredSearchPath = 0x00000008,
        IgnoreCodeAuthZLevel = 0x00000010
    }

    public class Library : IDisposable
    {
        #region Delegates
        delegate int EnumResTypeProc(IntPtr hModule, IntPtr lpszType, IntPtr lParam);
        delegate bool EnumResNameProc(IntPtr hModule, IntPtr pType, IntPtr pName, IntPtr param);
        #endregion

        public IntPtr Handle { get; private set; }

        #region Factory
        Library(IntPtr Handle) { if (Handle == IntPtr.Zero) throw new Exception(); }

        public Library(string Path) : this(LoadLibrary(Path)) { FileName = Path; }

        public Library(string Path, LoadLibraryFlags Flags)
            : this(LoadLibraryEx(Path, IntPtr.Zero, Flags)) { FileName = Path; }

        public void Dispose()
        {
            FreeLibrary(Handle);
            Handle = IntPtr.Zero;
        }
        #endregion

        public T FindFunction<T>(string Name) where T : DelegateConstraint
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate))) throw new ArgumentException(typeof(T).Name);

            var ptr = GetProcAddress(Handle, Name);

            if (ptr == IntPtr.Zero) throw new EntryPointNotFoundException(Name + " was not found in " + Path.GetFileName(FileName));

            return (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
        }

        public bool HasMethod(string Name) { return GetProcAddress(Handle, Name) != IntPtr.Zero; }

        #region Resources
        public LibraryResource FindResource(IntPtr ResourceID, ResourceType RType)
        {
            return new LibraryResource(FindResource(Handle, ResourceID, (IntPtr)RType), Handle, RType);
        }

        bool EnumResourceNames(ResourceType Type, EnumResNameProc Callback, IntPtr Parameter = default(IntPtr))
        {
            return EnumResourceNames(Handle, (IntPtr)Type, Callback, Parameter);
        }

        public IEnumerable<LibraryResource> EnumerateResources(ResourceType RType)
        {
            List<LibraryResource> FoundResources = new List<LibraryResource>();

            EnumResNameProc Callback = (h, t, name, l) =>
            {
                FoundResources.Add(FindResource(name, RType));

                return true;
            };

            EnumResourceNames(Handle, (IntPtr)RType, Callback, IntPtr.Zero);

            return FoundResources.ToArray();
        }

        public bool HasResource(ResourceType RType)
        {
            int Count = 0;

            EnumResNameProc Callback = (h, t, name, l) =>
            {
                ++Count;
                return true;
            };

            EnumResourceNames(RType, Callback);

            return Count != 0;

        }
        #endregion

        public string FileName { get; private set; }

        #region Native
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibraryEx(string path, IntPtr hFile, LoadLibraryFlags flags);

        [DllImport("kernel32.dll")]
        static extern IntPtr FindResource(IntPtr hModule, IntPtr resourceID, IntPtr type);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool EnumResourceNames(IntPtr hModule, IntPtr pType, EnumResNameProc callback, IntPtr param);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool EnumResourceTypes(IntPtr hModule, EnumResTypeProc callback, IntPtr lParam);
        #endregion
    }

    public class LibraryResource
    {
        public IntPtr Handle { get; private set; }

        IntPtr LibraryHandle;

        public ResourceType ResourceType { get; private set; }

        internal LibraryResource(IntPtr Handle, IntPtr LibraryHandle, ResourceType RType)
        {
            this.Handle = Handle;
            this.LibraryHandle = LibraryHandle;
            ResourceType = RType;

            if (Handle == IntPtr.Zero || LibraryHandle == IntPtr.Zero)
                throw new Exception();
        }

        #region Size
        public int Size { get { return (int)SizeofResource(LibraryHandle, Handle); } }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);
        #endregion

        public byte[] Data
        {
            get
            {
                IntPtr hRes = LoadResource(LibraryHandle, Handle);
                IntPtr LockedRes = LockResource(hRes);

                byte[] buffer = new byte[Size];
                Marshal.Copy(LockedRes, buffer, 0, Size);

                return buffer;
            }
        }

        #region Native
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern bool UpdateResource(IntPtr hUpdate, uint lpType, IntPtr pName, ushort wLanguage, byte[] lpData, uint cbData);
        #endregion
    }
}
