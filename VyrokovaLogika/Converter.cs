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
            sentence = sentence.Replace("&", "∧").Replace("|", "∨").Replace("-", "¬").Replace(">", "⇒").Replace("=", "≡");
        }

        public static void ConvertParenthessis(ref string sentence)
        {
            sentence = sentence.Replace("[", "(").Replace("]", ")")
                     .Replace("{", "(").Replace("}", ")");
        }

        public static string ReduceParenthessis(string sentence)
        {
            if(sentence.StartsWith("(") && sentence.EndsWith(")"))
            {
                return sentence.Substring(1, sentence.Length - 2);
            }
            return sentence;
        }
    }
}
