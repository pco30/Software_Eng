using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace WinChangeMonitor
{
    public static class RetainedSettings
    {
        [MessagePackObject]
        public class CommonInfo
        {
            [Key(0)]
            public int FileSystemInventoryCount { get; set; }

            [Key(1)]
            public int RegistryInventoryCount { get; set; }

            [Key(2)]
            public int ServicesInventoryCount { get; set; }
        }

        private static int FileSystemInventoryCount = 0, RegistryInventoryCount = 0, ServicesInventoryCount = 0;

        // File System Monitor enabled = whether FileSystem.json exists
        //     Folders to track = dtFoldersToTrack
        //     Pre-Install finised = preInstallFoldersFinished
        //     Inventory = folderContentsPreInstall
        // Registry Monitor enabled = whether Registry.json exists
        //     Keys to track = dtKeysToTrack
        //     Pre-Install finished = preInstallRegistryFinished
        //     Inventory = registryContentsPreInstall
        // Services Monitor enabled = whether Services.json exists
        //     Pre-Install finished = preInstallServicesFinished
        //     Inventory = servicesPreInstall
        private static SplashScreenForm SplashScreen = null;

        [MessagePackObject]
        public class FileSystemSettings
        {
            [MessagePackObject]
            public class TrackedFolder
            {
                [Key(0)]
                public String Folder { get; set; }

                [Key(1)]
                public Boolean IncludeSubFolders { get; set; }
            }

            [MessagePackObject]
            public class FileSystemEntryInfo : IMessagePackSerializationCallbackReceiver
            {
                [Key(0)]
                public Boolean IsFolder { get; set; }

                public void OnBeforeSerialize()
                {
                    // not used, only present to satisfy IMessagePackSerializationCallbackReceiver interface member requirement
                }

                public void OnAfterDeserialize()
                {
                    WinChangeMonitorForm.SplashScreen.IncrementStatus();
                }
            }

            [Key(0)]
            public List<TrackedFolder> FoldersToTrack = new List<TrackedFolder>();

            [Key(1)]
            public DateTime? PreInstallFinished = null;

            [Key(2)]
            public SortedDictionary<String, FileSystemEntryInfo> Inventory = new SortedDictionary<String, FileSystemEntryInfo>(); // key is full path to file/folder, value is whether this is a folder or file (true if folder, false if file)
        }

        private static FileSystemSettings FileSystem = null;

        public static List<FileSystemSettings.TrackedFolder> FoldersToTrack
        {
            get
            {
                Initialize();
                return FileSystem.FoldersToTrack;
            }
            private set
            {
                FileSystem.FoldersToTrack = value;
            }
        }

        public static DateTime? PreInstallFileSystemFinished
        {
            get
            {
                Initialize();
                return FileSystem.PreInstallFinished;
            }
            set
            {
                FileSystem.PreInstallFinished = value;
            }
        }

        public static SortedDictionary<String, FileSystemSettings.FileSystemEntryInfo> FileSystemInventory
        {
            get
            {
                Initialize();
                return FileSystem.Inventory;
            }
            set
            {
                FileSystem.Inventory = value;
            }
        }

        [MessagePackObject]
        public class RegistrySettings
        {
            [MessagePackObject]
            public class TrackedKey
            {
                [Key(0)]
                public String Key { get; set; }

                [Key(1)]
                public Boolean IncludeSubKeys { get; set; }
            }

            [Key(0)]
            public List<TrackedKey> KeysToTrack = new List<TrackedKey>();

            [Key(1)]
            public DateTime? PreInstallFinished = null;

            [Key(2)]
            public SortedDictionary<String, RegistryEntryInfo> Inventory = new SortedDictionary<String, RegistryEntryInfo>(); // key is full path to key/value, value is RegistryEntryInfo for value or null if key
        }

        private static RegistrySettings Registry = null;

        public static List<RegistrySettings.TrackedKey> KeysToTrack
        {
            get
            {
                Initialize();
                return Registry.KeysToTrack;
            }
            set
            {
                Registry.KeysToTrack = value;
            }
        }

        public static DateTime? PreInstallRegistryFinished
        {
            get
            {
                Initialize();
                return Registry.PreInstallFinished;
            }
            set
            {
                Registry.PreInstallFinished = value;
            }
        }

        public static SortedDictionary<String, RegistryEntryInfo> RegistryInventory
        {
            get
            {
                Initialize();
                return Registry.Inventory;
            }
            set
            {
                Registry.Inventory = value;
            }
        }

        [MessagePackObject]
        public class ServicesSettings
        {
            [Key(0)]
            public DateTime? PreInstallFinished = null;

            [Key(1)]
            public SortedDictionary<String, ServiceInfo> Inventory = new SortedDictionary<String, ServiceInfo>(); // key is service name, value is ServiceInfo for service
        }

        private static ServicesSettings Services = null;

        public static DateTime? PreInstallServicesFinished
        {
            get
            {
                Initialize();
                return Services.PreInstallFinished;
            }
            set
            {
                Services.PreInstallFinished = value;
            }
        }

        public static SortedDictionary<String, ServiceInfo> ServicesInventory
        {
            get
            {
                Initialize();
                return Services.Inventory;
            }
            set
            {
                Services.Inventory = value;
            }
        }

        private static Boolean Initialized = false;

        public static void Initialize(SplashScreenForm splashScreen = null)
        {
            try
            {
                if ((SplashScreen == null) && (splashScreen != null))
                {
                    SplashScreen = splashScreen;
                }

                if (!Initialized)
                {
                    LoadCommonInfo();
                    LoadFileSystemSettings();
                    LoadRegistrySettings();
                    LoadServicesSettings();

                    Initialized = true;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        private static String directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static String DirectoryName { get { return directoryName; } }

        public static void SaveCommonInfo(String fileName = "_Common.bin")
        {
            try
            {
                CommonInfo commonInfo = new CommonInfo {
                    FileSystemInventoryCount = FileSystem.Inventory.Count,
                    RegistryInventoryCount = Registry.Inventory.Count,
                    ServicesInventoryCount = Services.Inventory.Count
                };
                Byte[] bytes = MessagePackSerializer.Serialize(commonInfo);
                String filePath = Path.Combine(DirectoryName, fileName);
                File.WriteAllBytes(filePath, bytes);
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void SaveFileSystemSettings(String fileName = "_FileSystem.bin")
        {
            try
            {
                if (FileSystem != null)
                {
                    
                    Byte[] bytes = MessagePackSerializer.Serialize(FileSystem);
                    String filePath = Path.Combine(DirectoryName, fileName);
                    File.WriteAllBytes(filePath, bytes);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void SaveRegistrySettings(String fileName = "_Registry.bin")
        {
            try
            {
                if (Registry != null)
                {
                    Byte[] bytes = MessagePackSerializer.Serialize(Registry);
                    String filePath = Path.Combine(DirectoryName, fileName);
                    File.WriteAllBytes(filePath, bytes);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void SaveServicesSettings(String fileName = "_Services.bin")
        {
            try
            {
                if (Services != null)
                {
                    Byte[] bytes = MessagePackSerializer.Serialize(Services);
                    String filePath = Path.Combine(DirectoryName, fileName);
                    File.WriteAllBytes(filePath, bytes);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void LoadCommonInfo(String fileName = "_Common.bin")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    Byte[] bytes = File.ReadAllBytes(filePath);
                    CommonInfo commonInfo = MessagePackSerializer.Deserialize<CommonInfo>(bytes);
                    FileSystemInventoryCount = commonInfo.FileSystemInventoryCount;
                    RegistryInventoryCount = commonInfo.RegistryInventoryCount;
                    ServicesInventoryCount = commonInfo.ServicesInventoryCount;
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void LoadFileSystemSettings(String fileName = "_FileSystem.bin")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    if (SplashScreen != null)
                    {
                        SplashScreen.InitializeStatus("Loading File System Inventory", FileSystemInventoryCount);
                    }
                    Byte[] bytes = File.ReadAllBytes(filePath);
                    FileSystem = MessagePackSerializer.Deserialize<FileSystemSettings>(bytes);
                }
                else
                {
                    FileSystem = new FileSystemSettings();
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void LoadRegistrySettings(String fileName = "_Registry.bin")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    if (SplashScreen != null)
                    {
                        SplashScreen.InitializeStatus("Loading Registry Inventory", RegistryInventoryCount);
                    }
                    Byte[] bytes = File.ReadAllBytes(filePath);
                    Registry = MessagePackSerializer.Deserialize<RegistrySettings>(bytes);
                }
                else
                {
                    Registry = new RegistrySettings();
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void LoadServicesSettings(String fileName = "_Services.bin")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    if (SplashScreen != null)
                    {
                        SplashScreen.InitializeStatus("Loading Services Inventory", ServicesInventoryCount);
                    }
                    Byte[] bytes = File.ReadAllBytes(filePath);
                    Services = MessagePackSerializer.Deserialize<ServicesSettings>(bytes);
                }
                else
                {
                    Services = new ServicesSettings();
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void DeleteCommonInfo(String fileName = "_Common.bin")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileSystemInventoryCount = RegistryInventoryCount = ServicesInventoryCount = 0;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void DeleteFileSystemSettings(String fileName = "_FileSystem.bin")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileSystem = new FileSystemSettings();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void DeleteRegistrySettings(String fileName = "_Registry.bin")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                Registry = new RegistrySettings();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void DeleteServicesSettings(String fileName = "_Services.bin")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                Services = new ServicesSettings();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }
}
