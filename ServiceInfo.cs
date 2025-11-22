using MessagePack;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace WinChangeMonitor
{
    [MessagePackObject]
    public class ServiceInfo : IMessagePackSerializationCallbackReceiver
    {
        [Key(0)]
        public Boolean CanPauseAndContinue { get; set; }

        [Key(1)]
        public Boolean CanShutdown { get; set; }

        [Key(2)]
        public Boolean CanStop { get; set; }

        [Key(3)]
        public String DisplayName { get; set; }

        [Key(4)]
        public HashSet<String> ServiceNamesDependedOn { get; set; }

        [Key(5)]
        public ServiceType ServiceType { get; set; }

        [Key(6)]
        public ServiceStartMode StartType { get; set; }

        [IgnoreMember]
        private StringBuilder printableNamesDependedOn = null;

        public static ServiceInfo Parse(ServiceController service)
        {
            HashSet<String> serviceNamesDependedOn = new HashSet<String>();
            foreach (ServiceController serviceDependedOn in service.ServicesDependedOn)
            {
                serviceNamesDependedOn.Add(serviceDependedOn.ServiceName);
            }

            return new ServiceInfo
            {
                CanPauseAndContinue = service.CanPauseAndContinue,
                CanShutdown = service.CanShutdown,
                CanStop = service.CanStop,
                DisplayName = service.DisplayName,
                ServiceNamesDependedOn = serviceNamesDependedOn,
                ServiceType = service.ServiceType,
                StartType = service.StartType
            };
        }

        public override String ToString()
        {
            if (this.printableNamesDependedOn == null)
            {
                this.printableNamesDependedOn = new StringBuilder();

                Boolean isFirstElement = true;

                foreach (String serviceNameDependedOn in this.ServiceNamesDependedOn)
                {
                    if (!isFirstElement)
                    {
                        this.printableNamesDependedOn.Append(", ");
                    }

                    this.printableNamesDependedOn.Append(serviceNameDependedOn);
                    isFirstElement = false;
                }
            }

            return $"{this.CanPauseAndContinue}, {this.CanShutdown}, {this.CanStop}, {this.DisplayName}, ({this.printableNamesDependedOn}), {this.ServiceType}, {this.StartType}";
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

    internal class ServiceDiff
    {
        public ServiceInfo Initial { get; private set; }
        public ServiceInfo Current { get; private set; }

        public ServiceDiff(ServiceInfo initial, ServiceInfo current)
        {
            this.Initial = initial;
            this.Current = current;
        }

        public override String ToString()
        {
            // Convert.ToString() returns String.Empty for null input
            return $"({Convert.ToString(this.Initial)}) -> ({Convert.ToString(this.Current)})";
        }
    }
}
