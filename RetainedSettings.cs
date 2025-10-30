using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WinChangeMonitor
{
    public static class RetainedSettings
    {
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

        public class FileSystemSettings
        {
            public class TrackedFolder
            {
                [JsonInclude]
                public String Folder { get; set; }
                [JsonInclude]
                public Boolean IncludeSubDirectories { get; set; }

                public TrackedFolder(String folder, Boolean includeSubDirectories)
                {
                    this.Folder = folder;
                    this.IncludeSubDirectories = includeSubDirectories;
                }
            }

            [JsonInclude]
            public List<TrackedFolder> FoldersToTrack = new List<TrackedFolder>();
            [JsonInclude]
            public DateTime? PreInstallFinished = null;
            [JsonInclude]
            public SortedDictionary<String, Boolean> Inventory = new SortedDictionary<String, Boolean>(); // key is full path to file/folder, value is whether this is a folder or file (true if folder, false if file)
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

        public static SortedDictionary<String, Boolean> FileSystemInventory
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

        public class RegistrySettings
        {
            public class TrackedKey
            {
                [JsonInclude]
                public String Key { get; set; }
                [JsonInclude]
                public Boolean IncludeSubKeys { get; set; }

                public TrackedKey(String key, Boolean includeSubKeys)
                {
                    this.Key = key;
                    this.IncludeSubKeys = includeSubKeys;
                }
            }

            [JsonInclude]
            public List<TrackedKey> KeysToTrack = new List<TrackedKey>();
            [JsonInclude]
            public DateTime? PreInstallFinished = null;
            [JsonInclude]
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

        private class ServicesSettings
        {
            [JsonInclude]
            public DateTime? PreInstallFinished = null;
            [JsonInclude]
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

        private static String DirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static JsonSerializerOptions SerializerOptions = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        public static void SaveFileSystemSettings(String fileName = "FileSystem.json")
        {
            try
            {
                if (FileSystem != null)
                {
                    
                    String jsonString = JsonSerializer.Serialize(FileSystem, SerializerOptions);
                    String filePath = Path.Combine(DirectoryName, fileName);
                    File.WriteAllText(filePath, jsonString);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void SaveRegistrySettings(String fileName = "Registry.json")
        {
            try
            {
                if (Registry != null)
                {
                    String jsonString = JsonSerializer.Serialize(Registry, SerializerOptions);
                    String filePath = Path.Combine(DirectoryName, fileName);
                    File.WriteAllText(filePath, jsonString);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void SaveServicesSettings(String fileName = "Services.json")
        {
            try
            {
                if (Services != null)
                {
                    String jsonString = JsonSerializer.Serialize(Services, SerializerOptions);
                    String filePath = Path.Combine(DirectoryName, fileName);
                    File.WriteAllText(filePath, jsonString);
                }
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public static void LoadFileSystemSettings(String fileName = "FileSystem.json")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    if (SplashScreen != null)
                    {
                        SplashScreen.InitializeStatus("Loading File System Inventory");
                    }
                    String jsonString = File.ReadAllText(filePath);
                    FileSystem = JsonSerializer.Deserialize<FileSystemSettings>(jsonString, SerializerOptions);
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

        public static void LoadRegistrySettings(String fileName = "Registry.json")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    if (SplashScreen != null)
                    {
                        SplashScreen.InitializeStatus("Loading Registry Inventory");
                    }
                    String jsonString = File.ReadAllText(filePath);
                    Registry = JsonSerializer.Deserialize<RegistrySettings>(jsonString, SerializerOptions);
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

        public static void LoadServicesSettings(String fileName = "Services.json")
        {
            try
            {
                String filePath = Path.Combine(DirectoryName, fileName);
                if (File.Exists(filePath))
                {
                    if (SplashScreen != null)
                    {
                        SplashScreen.InitializeStatus("Loading Services Inventory");
                    }
                    String jsonString = File.ReadAllText(filePath);
                    Services = JsonSerializer.Deserialize<ServicesSettings>(jsonString, SerializerOptions);
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

        public static void DeleteFileSystemSettings(String fileName = "FileSystem.json")
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

        public static void DeleteRegistrySettings(String fileName = "Registry.json")
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

        public static void DeleteServicesSettings(String fileName = "Services.json")
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
