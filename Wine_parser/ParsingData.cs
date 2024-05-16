using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wine_parser
{
    public class ParsingResult
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int OldPrice { get; set; }
        public double Rating { get; set; }
        public string Volume { get; set; }
        public int Articul { get; set; }
        public string Region { get; set; }
        public string Url { get; set; }
        public string[] Pictures { get; set; }

    }

}
