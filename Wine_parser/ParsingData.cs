using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wine_parser
{
    public struct ParsingResult
    {
        public string ParsedName { get; set; }
        public int ParsedPrice { get; set; }
        public int ParsedOldPrice { get; set; }
        public double ParsedRating { get; set; }
        public string ParsedVolume { get; set; }
        public int ParsedArticul { get; set; }
        public string ParsedRegion { get; set; }
        public string ParsedUrl { get; set; }
        public string[] ParsedPictures { get; set; }

    }

}
