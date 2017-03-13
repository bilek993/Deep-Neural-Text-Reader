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
                writer.WriteLine(network.InputsCount);
                writer.WriteLine(Network.OUTPUTS_COUNT);

                writer.WriteLine(networkFileName.Substring(fileName.LastIndexOf('\\') + 1));
            }

            network.SaveNetwork(networkFileName);
        }

        public Network LoadConfig(string fileName)
        {
            string directoryName = fileName.Substring(0, fileName.LastIndexOf('\\'));
            string networkFileName = "";

            Network network = new Network();

            using (StreamReader reader = new StreamReader(fileName))
            {
                iterationsCount = network.iterationCount = Convert.ToInt32(reader.ReadLine());
                reader.ReadLine();  // InputsCount - loaded from binary file
                reader.ReadLine();  // OutputsCount

                networkFileName = reader.ReadLine();
            }

            network.LoadNetwork(directoryName + "\\" + networkFileName);

            return network;
        }
    }
}