using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    static class Validator
    {
        public static bool CheckParenthesses(string vl)
        {
            if (vl.Contains('(') || vl.Contains(')')) return true;
            else return false;
        }

        public static bool Check(string vl)
        {
            //TODO kontrola spravnosti cele vety musim si to dodelat zde
            return true;
        }

        public static bool ContainsOperator(string vl)
        {
            if (vl.Contains('&') || vl.Contains('|') || vl.Contains('>')) return true;
            else return false;
        }

        public static bool ContainsNegation(string vl)
        {
            if (vl.Contains('-')) return true;
            else return false;
        }
    }
}
