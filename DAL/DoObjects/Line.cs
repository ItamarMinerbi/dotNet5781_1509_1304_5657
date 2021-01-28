using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class Line
    {
        public enum Areas { General, Center, Jerusalem, North }

        public Areas Area { get; set; }

        public int ID { get; set; }

        public int LineNumber { get; set; }

        public int FirstStation { get; set; }

        public int LastStation { get; set; }

        public bool IsActive { get; set; }
    }
}
