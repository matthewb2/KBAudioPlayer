using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBAudioPlayer
{


    public class Employee   // public 이어야 함
    {
        public int Seq;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }
        public float Volume { get; set; }
        public int Height { get; set; }
    }

}
