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
            if (sentence == null || sentence.Length < 2 || sentence[0] != '(' || sentence[^1] != ')')
            {
                return sentence;
            }

            var openParenthesesIndex = new Stack<int>();
            for (var i = 0; i < sentence.Length; i++)
            {
                if (sentence[i] == '(')
                {
                    openParenthesesIndex.Push(i);
                }
                else if (sentence[i] == ')')
                {
                    if (openParenthesesIndex.Count == 1 && openParenthesesIndex.Peek() == 0 && i == sentence.Length - 1)
                    {
                        return sentence[1..^1];
                    }
                    openParenthesesIndex.Pop();
                }
            }

            return sentence;
        }

        public static void RemoveExcessParentheses(ref string sentence)
        {
            if (string.IsNullOrEmpty(sentence) || sentence[0] != '(')
            {
                return;
            }

            Stack<char> stack = new();
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
                        sentence = sentence[1..^1];
                        RemoveExcessParentheses(ref sentence);
                        return;
                    }
                }
                position++;
            }
        }

    }
}
