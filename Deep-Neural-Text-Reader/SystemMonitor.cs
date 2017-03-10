using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deep_Neural_Text_Reader
{
    class SystemMonitor
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;

        public SystemMonitor()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public int getCpuUsage()
        {
            return (int)Math.Round(cpuCounter.NextValue());
        }

        public int getRamUsage()
        {
            return (int)Math.Round(ramCounter.NextValue());
        }

        public int getTotalRam()
        {
            return (int) ((new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory) / 1024 / 1024);
        }
    }
}
