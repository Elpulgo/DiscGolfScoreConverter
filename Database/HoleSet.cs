using System;
using System.Collections.Generic;
using System.Text;

namespace Tester.Database
{
    public class HoleSet
    {        
        public int HoleId { get; set; }

        public int SetId { get; set; }

        public int? Strokes { get; set; }

        public int? StrokeOffset { get; set; }
    }
}
