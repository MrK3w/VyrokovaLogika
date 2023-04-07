using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public static class Converter
    {

        public static void ConvertLogicalOperators(ref string sentence)
        {
            sentence = sentence.Replace("&", "∧").Replace("|", "∨").Replace("-", "¬").Replace(">", "⇒");
        }

        public static void ConvertParenthessis(ref string sentence)
        {
            sentence = sentence.Replace("[", "(").Replace("]", ")")
                     .Replace("{", "(").Replace("}", ")");
        }

        public static void ReduceParenthessis(ref string sentence)
        {
            while(sentence.StartsWith("(") && sentence.EndsWith(")"))
            {
                sentence = sentence.Substring(1, sentence.Length - 2);
            }

        }
    }
}
