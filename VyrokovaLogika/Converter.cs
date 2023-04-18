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
            if (sentence == null || sentence[0] != '(' || sentence[sentence.Length - 1] != ')') return sentence;

            var cantrim = false;
            var openparenthesesIndex = new Stack<int>();
            var count = 0;
            foreach (char c in sentence)
            {
                if (c == '(')
                {
                    openparenthesesIndex.Push(count);
                }
                if (c == ')')
                {
                    cantrim = (count == sentence.Length - 1 && openparenthesesIndex.Count == 1 && openparenthesesIndex.Peek() == 0);
                    openparenthesesIndex.Pop();
                }
                count++;
            }

            if (cantrim)
            {
                return sentence.Trim(new[] { '(', ')' });
            }
            return sentence;
        }

        public static void RemoveExcessParentheses(ref string sentence)
        {
            if (string.IsNullOrEmpty(sentence) || sentence[0] != '(')
            {
                return;
            }

            Stack<char> stack = new Stack<char>();
            int position = 0;
            foreach (char c in sentence)
            {

                if (c == '(')
                {
                    stack.Push(c);
                }
                else if (c == ')')
                {
                    if (stack.Count > 0)
                    {
                        stack.Pop();
                    }
                   
                    if(stack.Count == 0 && (sentence.Length - 1) != position )
                    {
                            return;
                    
                    }
                    else if(stack.Count == 0)
                    {
                        sentence = sentence.Substring(1, sentence.Length - 2);
                        RemoveExcessParentheses(ref sentence);
                        return;
                    }
                }
                position++;
            }
        }

    }
}
