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
            //must be here to not break foreach before I even met first parenthesses
            bool metParenthesses = false;
            foreach (var letter in vl)
            {
                if (letter == '(')
                {
                    parenthesses++;
                    metParenthesses = true;
                }
                else if (letter == ')') parenthesses--;
                position++;
                if (parenthesses == 0 && metParenthesses == true) break;
            }
        }

        public (string, string,string) SplitString() // tuple return type
        {
            string firstPart = vl.Substring(0, position);
            string conditional = vl.Substring(position,1);
            string secondPart = vl.Substring(position+1);
            return (firstPart, conditional, secondPart); // tuple literal
        }

        public void FindSplitPointForNegation()
        {
            while(!Char.IsLetter(vl[position]))
            {
                position++;
            }
            //+ literal position
            position++;
        }
    }
}
