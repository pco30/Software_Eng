using Microsoft.Win32;
using System;

namespace WinChangeMonitor
{
    internal class RegistryEntryInfo
    {
        public RegistryValueKind Kind { get; private set; }
        public String Value { get; private set; }


        public RegistryEntryInfo(RegistryValueKind kind, String value)
        {
            this.Kind = kind;
            this.Value = value;
        }

        public override String ToString()
        {
            return $"{this.Kind}, {this.Value}";
        }
    }

    internal class RegistryEntryDiff
    {
        public RegistryEntryInfo Initial { get; private set; }
        public RegistryEntryInfo Current { get; private set; }

        public RegistryEntryDiff(RegistryEntryInfo initial, RegistryEntryInfo current)
        {
            this.Initial = initial;
            this.Current = current;
        }

        public override String ToString()
        {
            return $"({this.Initial}) -> ({this.Current})";
        }
    }
}
