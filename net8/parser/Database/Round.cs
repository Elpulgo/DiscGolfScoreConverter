using System;
using System.Collections.Generic;
using System.Text;

namespace Tester.Database
{
    public class Round
    {
        public Round()
        {
            Start = DateTime.Now;
        }

        public int Id { get; set; }

        public int CourseId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public float Wind { get; set; }

        public float Degrees { get; set; }

        public string RainFall { get; set; }
    }
}
