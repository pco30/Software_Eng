using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WinChangeMonitor
{
    internal class ServiceInfo
    {
        public Boolean CanPauseAndContinue { get; private set; }
        public Boolean CanShutdown { get; private set; }
        public Boolean CanStop { get; private set; }
        public String DisplayName { get; private set; }
        public List<String> ServiceNamesDependedOn {get; private set; }
        public ServiceType ServiceType { get; private set; }
        public ServiceStartMode StartType;

        private StringBuilder printableNamesDependedOn = new StringBuilder();

        public ServiceInfo(ServiceController serviceController)
        {
            List<String> serviceNamesDependedOn = new List<String>();

            for (Int32 i = 0; i < serviceController.ServicesDependedOn.Length; ++i)
            {
                serviceNamesDependedOn.Add(serviceController.ServicesDependedOn[i].ServiceName);

                if (i > 0)
                {
                    this.printableNamesDependedOn.Append(", ");
                }

                this.printableNamesDependedOn.Append(serviceController.ServicesDependedOn[i].ServiceName);
            }

            this.CanPauseAndContinue = serviceController.CanPauseAndContinue;
            this.CanShutdown = serviceController.CanShutdown;
            this.CanStop = serviceController.CanStop;
            this.DisplayName = serviceController.DisplayName;
            this.ServiceNamesDependedOn = serviceNamesDependedOn;
            this.ServiceType = serviceController.ServiceType;
            this.StartType = serviceController.StartType;
        }

        public override String ToString()
        {
            return $"{this.CanPauseAndContinue}, {this.CanShutdown}, {this.CanStop}, {this.DisplayName}, ({this.printableNamesDependedOn.ToString()}), {this.ServiceType}, {this.StartType}";
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
            return $"({this.Initial.ToString()}) -> ({this.Current.ToString()})";
        }
    }
}
