using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static WinChangeMonitor.RetainedSettings.FileSystemSettings;
using static WinChangeMonitor.RetainedSettings.RegistrySettings;

namespace WinChangeMonitor
{
    internal class JsonExportData
    {
        [JsonInclude]
        public List<TrackedFolder> TrackedFolders { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, Boolean> FolderContentsAdded { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, Boolean> FolderContentsModified { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, FileSystemEntryInfo> FolderContentsRemoved { get; private set; }

        [JsonInclude]
        public List<TrackedKey> TrackedKeys { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, RegistryEntryInfo> RegistryContentsAdded { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, RegistryEntryDiff> RegistryContentsModified { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, RegistryEntryInfo> RegistryContentsRemoved { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, ServiceInfo> ServicesAdded { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, ServiceDiff> ServicesModified { get; private set; }

        [JsonInclude]
        public SortedDictionary<String, ServiceInfo> ServicesRemoved { get; private set; }

        public JsonExportData(
            List<TrackedFolder> trackedFolders,
            SortedDictionary<String, Boolean> folderContentsAdded,
            SortedDictionary<String, Boolean> folderContentsModified,
            SortedDictionary<String, FileSystemEntryInfo> folderContentsRemoved,
            List<TrackedKey> trackedKeys,
            SortedDictionary<String, RegistryEntryInfo> registryContentsAdded,
            SortedDictionary<String, RegistryEntryDiff> registryContentsModified,
            SortedDictionary<String, RegistryEntryInfo> registryContentsRemoved,
            SortedDictionary<String, ServiceInfo> servicesAdded,
            SortedDictionary<String, ServiceDiff> servicesModified,
            SortedDictionary<String, ServiceInfo> servicesRemoved)
        {
            try
            {
                this.TrackedFolders = trackedFolders;
                this.FolderContentsAdded = folderContentsAdded;
                this.FolderContentsModified = folderContentsModified;
                this.FolderContentsRemoved = folderContentsRemoved;
                this.TrackedKeys = trackedKeys;
                this.RegistryContentsAdded = registryContentsAdded;
                this.RegistryContentsModified = registryContentsModified;
                this.RegistryContentsRemoved = registryContentsRemoved;
                this.ServicesAdded = servicesAdded;
                this.ServicesModified = servicesModified;
                this.ServicesRemoved = servicesRemoved;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }
    }
}
