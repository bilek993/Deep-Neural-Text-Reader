using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deep_Neural_Text_Reader
{
    class ConfigFileManager
    {
        public int iterationsCount { get; set; }

        public void SaveConfig(string fileName, Network network)
        {
            string networkFileName = fileName.Substring(0, fileName.LastIndexOf('.')) + ".net";

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(iterationsCount);
                writer.WriteLine(network.inputsCount);
                writer.WriteLine(Network.OUTPUTS_COUNT);

                writer.WriteLine(networkFileName.Substring(fileName.LastIndexOf('\\') + 1));
            }

            network.SaveNetwork(networkFileName);
        }
    }
}