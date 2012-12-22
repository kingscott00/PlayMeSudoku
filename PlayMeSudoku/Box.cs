using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudoKu
{
    class Box
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public List<string> PossibleNumbers { get; set; }

        public Box()
        {
            Row = 0;
            Col = 0;
            PossibleNumbers = new List<string>();
        }
    }
}
