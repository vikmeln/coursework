using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coursework
{
    internal class Class1
    {
        public string номер { get; private set; }
        public int виїзд { get; private set; }
        public int прибуття { get; private set; }
        public string пункт { get; private set; }
        public int тривалість { get; private set; }
        public Class1(string номер, int виїзд, int прибуття, string пункт, int тривалість)
        {
            номер = номер;
            виїзд = виїзд;
            прибуття = прибуття;
            пункт = пункт;
            тривалість = тривалість;
        }
        public override string ToString()
        {
            return номер;
        }
    }
}
