using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class DisplayFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string CreationDate { get; set; }
        public string LastModifiedDate { get; set; }
        public string SizeString { get; set; }
        public long SizeBytes { get; set; }
    }
}
