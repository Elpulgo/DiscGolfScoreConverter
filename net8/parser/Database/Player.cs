using System;
using System.Collections.Generic;
using System.Text;

namespace Tester.Database
{
    public class Player
    {
        public Player()
        {
            Created = DateTime.Now;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public DateTime Created { get; set; }

        public string PhotoFilePath { get; set; }

        public bool IsMain { get; set; }
    }
}
