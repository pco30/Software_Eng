using MessagePack;
using Microsoft.Win32;
using System;

namespace WinChangeMonitor
{
    [MessagePackObject]
    public class RegistryEntryInfo : IMessagePackSerializationCallbackReceiver
    {
        [Key(0)]
        public RegistryValueKind Kind { get; set; }

        [Key(1)]
        public String Value { get; set; }

        public override String ToString()
        {
            try
            {
                return $"{this.Kind}, {this.Value}";
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
                return null;
            }
        }

        public void OnBeforeSerialize()
        {
            // not used, only present to satisfy IMessagePackSerializationCallbackReceiver interface member requirement
        }

        public void OnAfterDeserialize()
        {
            WinChangeMonitorForm.SplashScreen.IncrementStatus();
        }
    }

    internal class RegistryEntryDiff
    {
        public RegistryEntryInfo Initial { get; private set; }
        public RegistryEntryInfo Current { get; private set; }

        public RegistryEntryDiff(RegistryEntryInfo initial, RegistryEntryInfo current)
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
