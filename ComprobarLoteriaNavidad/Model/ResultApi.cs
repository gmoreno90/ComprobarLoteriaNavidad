using System;
using System.Collections.Generic;
using System.Text;

namespace ComprobarLoteriaNavidad.Model
{
    public class ResultApi
    {
        public int numero { get; set; }
        public int premio { get; set; }
        public int timestamp { get; set; }
        public int status { get; set; }
        public int error { get; set; }
    }
}
