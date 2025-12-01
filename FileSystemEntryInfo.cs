using MessagePack;
using System;
using System.Text.Json.Serialization;

namespace WinChangeMonitor
{
    [MessagePackObject]
    public class FileSystemEntryInfo : IMessagePackSerializationCallbackReceiver
    {
        [Key(0)]
        [JsonInclude]
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

    public class FileSystemEntryDiff
    {
        [JsonInclude]
        public Boolean Initial { get; private set; }

        [JsonInclude]
        public Boolean Current { get; private set; }

        public FileSystemEntryDiff(Boolean initial, Boolean current)
        {
            try
            {
                this.Initial = initial;
                this.Current = current;
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
        }

        public override String ToString()
        {
            try
            {
                return $"({this.Initial}) -> ({this.Current})";
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
                return null;
            }
        }
    }
}
