using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public static class Validator
    {
        public static bool ContainsParenthesses(string vl)
        {
            if (vl.Contains('(') || vl.Contains(')')) return true;
            else return false;
        }

        public static bool ValidateSentence(ref string mPropositionalSentence)
        {
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty).ToLowerInvariant();
            Converter.ConvertLogicalOperators(ref mPropositionalSentence);
            Converter.ConvertParenthessis(ref mPropositionalSentence);
            Converter.RemoveExcessParentheses(ref mPropositionalSentence);
            //check if sentence is valid
            if (mPropositionalSentence.Length == 1 && Validator.ContainsOperator(mPropositionalSentence)) return false;
            if (!Validator.ValidateSides(mPropositionalSentence)) return false;
            if (!Validator.ValidateParenthesses(mPropositionalSentence)) return false;
            if (!Validator.RightCharacters(mPropositionalSentence)) return false;
            if (!Validator.ValidateOperators(mPropositionalSentence)) return false;

            if (!Validator.CheckIfLiteralIsSingle(mPropositionalSentence)) return false;
            return true;
        }

        private static bool ValidateSides(string mPropositionalSentence)
        {
            for (int i = 0; i < mPropositionalSentence.Length - 2; i++)
            {
                if (Validator.ContainsOperator(mPropositionalSentence[i].ToString()) && !(isVariable(mPropositionalSentence[i + 1].ToString()) || mPropositionalSentence[i + 1] == '(' || mPropositionalSentence[i+1] == '¬'))
                    return false;
            }
            if (Validator.ContainsOperator(mPropositionalSentence[mPropositionalSentence.Length - 1].ToString())) return false;
            return true;
        }

        private static bool CheckIfLiteralIsSingle(string mPropositionalSentence)
        {
            if (isVariable(mPropositionalSentence[0].ToString()) && isVariable(mPropositionalSentence[1].ToString()) && mPropositionalSentence.Length == 2)
                return false;
            for (int i = 0; i < mPropositionalSentence.Length - 2; i++)
            {
                if (isVariable(mPropositionalSentence[i].ToString()) && isVariable(mPropositionalSentence[i + 1].ToString()))
                    return false;
            }

            return true;
        }

        //validate that operators have rightly brackets
        private static bool ValidateOperators(string mPropositionalSentence)
        {
            for (int i = 1; i < mPropositionalSentence.Length - 2; i++)
            {
                if (Validator.ContainsOperator(mPropositionalSentence[i - 1].ToString()) && Validator.isVariable(mPropositionalSentence[i].ToString())
                    && Validator.ContainsOperator(mPropositionalSentence[i + 1].ToString()))
                    return false;
            }
            return true;
        }

        public static bool IsValidExpression(string vl)
        {
            string pattern = @"\(((?>[^()]+)|(?<open>\()|(?<-open>\)))*(?(open)(?!))\)";
            MatchCollection matches = Regex.Matches(vl, pattern);

            foreach (Match match in matches)
            {
                if (match.Value.Contains("(") && match.Value.Contains(")"))
                {
                    if (!IsValidExpression(match.Value.Trim('(', ')')))
                        return false;
                }
                else if (match.Value.Contains("(") || match.Value.Contains(")"))
                {
                    return false;
                }
            }

            return true;
        }



        public static bool isVariableWithNegation(string vl)
        {

            string pattern = @"^[a-zA-Z()\u00AC]*$";
            return Regex.IsMatch(vl, pattern);
        }

        public static bool isVariable(string vl)
        {
            string pattern = @"^[a-zA-Z]*$";
            return Regex.IsMatch(vl, pattern);
        }

        public static bool ContainsOperator(string vl)
        {
            if (vl.Contains('∧') || vl.Contains('∨') || vl.Contains('⇒') || vl.Contains('≡')) return true;
            else return false;
        }

        public static bool ContainsNegationOnFirstPlace(string vl)
        {
            if (vl[0] == '¬') return true;
            else return false;
        }

        public static bool ValidateParenthesses(string vl)
        {
            Stack<char> parenthessesStack = new Stack<char>();

            foreach (char c in vl)
            {
                if (c == '(')
                {
                    parenthessesStack.Push(c);
                }
                else if (c == ')')
                {
                    if (parenthessesStack.Count == 0 || parenthessesStack.Pop() != '(')
                    {
                        return false; // closing parenthesis without matching opening parenthesis
                    }
                }
            }

            return parenthessesStack.Count == 0; // check if stack is empty, which means all opening parentheses have matching closing parentheses
        }

        public static bool RightCharacters(string mPropositionalSentence)
        {
            string allowedRegex = @"^[\s&∧|∨¬>⇒=≡a-zA-Z[\]{}()\-]+$";
            return Regex.IsMatch(mPropositionalSentence, allowedRegex);
        }
    }
}


