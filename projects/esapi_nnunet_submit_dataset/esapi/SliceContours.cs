using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esapi
{
    public class Contour
    {
        public List<List<double>> Points { get; set; }
        public bool Hole { get; set; }
    }

    public class SliceContours
    {
        public int Slice { get; set; }
        public List<Contour> Contours { get; set; }
    }
}
