using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    class Splitter
    {
        int position = 0;
        string vl = String.Empty;

        public Splitter(string sentence)
        {
            vl = sentence;
        }
        public void FindSplitPoint()
        {
            int parenthesses = 0;
            foreach (var letter in vl)
            {
                if (letter == '(') parenthesses++;
                else if (letter == ')') parenthesses--;
                position++;
                if (parenthesses == 0) break;
            }
        }

        public (string, string,string) SplitString() // tuple return type
        {
            string firstPart = vl.Substring(0, position);
            string conditional = vl.Substring(position,1);
            string secondPart = vl.Substring(position+1);
            return (firstPart, conditional, secondPart); // tuple literal
        }
    }
}
