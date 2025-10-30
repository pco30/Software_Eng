using Microsoft.Win32;
using System;
using System.Text.Json.Serialization;

namespace WinChangeMonitor
{
    public class RegistryEntryInfo : IJsonOnDeserializing
    {
        public RegistryValueKind Kind { get; private set; }
        public String Value { get; private set; }


        public RegistryEntryInfo(RegistryValueKind kind, String value)
        {
            try
            {
                this.Kind = kind;
                this.Value = value;
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
                return $"{this.Kind}, {this.Value}";
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
                return null;
            }
        }

        public void OnDeserializing()
        {
            try
            {
                WinChangeMonitorForm.SplashScreen.IncrementStatus();
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
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
