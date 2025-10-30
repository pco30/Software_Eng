using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Text.Json.Serialization;

namespace WinChangeMonitor
{
    public class ServiceInfo : IJsonOnDeserializing
    {
        [JsonInclude]
        public Boolean CanPauseAndContinue { get; set; }
        [JsonInclude]
        public Boolean CanShutdown { get; set; }
        [JsonInclude]
        public Boolean CanStop { get; set; }
        [JsonInclude]
        public String DisplayName { get; set; }
        [JsonInclude]
        public List<String> ServiceNamesDependedOn { get; set; }
        [JsonInclude]
        public ServiceType ServiceType { get; set; }
        [JsonInclude]
        public ServiceStartMode StartType { get; set; }

        private StringBuilder printableNamesDependedOn = null;

        [JsonConstructor]
        public ServiceInfo(Boolean canPauseAndContinue, Boolean canShutdown, Boolean canStop, String displayName, List<String> serviceNamesDependedOn, ServiceType serviceType, ServiceStartMode startType)
        {
            this.CanPauseAndContinue = canPauseAndContinue;
            this.CanShutdown = canShutdown;
            this.CanStop = canStop;
            this.DisplayName = displayName;
            this.ServiceNamesDependedOn = serviceNamesDependedOn;
            this.ServiceType = serviceType;
            this.StartType = startType;
        }

        public ServiceInfo(ServiceController serviceController)
        {
            this.CanPauseAndContinue = serviceController.CanPauseAndContinue;
            this.CanShutdown = serviceController.CanShutdown;
            this.CanStop = serviceController.CanStop;
            this.DisplayName = serviceController.DisplayName;
            this.ServiceNamesDependedOn = new List<String>();

            for (Int32 i = 0; i < serviceController.ServicesDependedOn.Length; ++i)
            {
                this.ServiceNamesDependedOn.Add(serviceController.ServicesDependedOn[i].ServiceName);
            }
            this.ServiceType = serviceController.ServiceType;
            this.StartType = serviceController.StartType;
        }

        public override String ToString()
        {
            if (this.printableNamesDependedOn == null)
            {
                this.printableNamesDependedOn = new StringBuilder();

                for (Int32 i = 0; i < this.ServiceNamesDependedOn.Count; ++i)
                {
                    if (i > 0)
                    {
                        this.printableNamesDependedOn.Append(", ");
                    }

                    this.printableNamesDependedOn.Append(this.ServiceNamesDependedOn[i]);
                }
            }

            return $"{this.CanPauseAndContinue}, {this.CanShutdown}, {this.CanStop}, {this.DisplayName}, ({this.printableNamesDependedOn.ToString()}), {this.ServiceType}, {this.StartType}";
        }

        public void OnDeserializing()
        {
            try
            {
                WinChangeMonitorForm.SplashScreen?.IncrementStatus(); // null-conditional operator ?. calls IncrementStatus() on SplashScreen if SplashScreen is not null. will do nothing if SplashScreen is null
            }
            catch (Exception ex)
            {
                Utilities.HandleException(ex);
            }
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
