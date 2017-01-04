using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GameResources
{
    public class Connection
    {
        public Connection()
        {
            
        }

        public Connection(int partTemplate1Index, int joint1Index, int partTemplate2Index, int joint2Index)
        {
            PartTemplate1Index = partTemplate1Index;
            Joint1Index = joint1Index;
            PartTemplate2Index = partTemplate2Index;
            Joint2Index = joint2Index;
        }

        public int PartTemplate1Index { get; set; }
        public int Joint1Index { get; set; }
        public int PartTemplate2Index { get; set; }
        public int Joint2Index { get; set; }
    }
}
