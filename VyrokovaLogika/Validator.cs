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
        public static string? ErrorMessage;
        public static bool ContainsParenthesses(string vl)
        {
            if (vl.Contains('(') || vl.Contains(')')) return true;
            else return false;
        }

        public static bool ValidateSentence(ref string mPropositionalSentence)
        {
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty);
            Converter.ConvertLogicalOperators(ref mPropositionalSentence);
            Converter.ConvertParenthessis(ref mPropositionalSentence);
            Converter.RemoveExcessParentheses(ref mPropositionalSentence);
            //check if sentence include only logical operator 
            if (mPropositionalSentence.Length == 1 && Validator.ContainsOperator(mPropositionalSentence))
            {
                ErrorMessage = "Formule nesmí obsahovat pouze logické operátory!";
                return false;
            }
            //check if sentecne contains literal
            if (!Validator.ContainsLetter(mPropositionalSentence))
            {
                ErrorMessage = "Formule neobsahuje žádný literál!";
                return false;
            }
            //check if literal is on both sides of sentence
            if (!Validator.ValidateSides(mPropositionalSentence))
            {
                ErrorMessage = "Operátor nesmí mít literál pouze na jedné straně!";
                return false;
            }
            //check if brackets are ok
            if (!Validator.ValidateParenthesses(mPropositionalSentence))
            {
                ErrorMessage = "Zkontroluj závorky!";
                return false;
            }
            //check if formula includes only right characters
            if (!Validator.RightCharacters(mPropositionalSentence))
            {
                ErrorMessage = "Ve formuli nejsou validní symboly!";
                return false;
            }
            //check if negation is done correctly e.g cannot be ->
            if (!Validator.ValidateNegation(mPropositionalSentence))
            {
                ErrorMessage = "Nemáš správně danou negaci";
                return false;
            }
            //check if literal is only one
            if (!Validator.CheckIfLiteralIsSingle(mPropositionalSentence))
            {
                ErrorMessage = "Povolené literály mají pouze jedno místo.";
                return false;
            }
            return true;
        }

        private static bool ValidateNegation(string mPropositionalSentence)
        {
            List<char> separators = new List<char> { '∧', '∨', '⇒', '≡' };

            for (int i = 0; i < mPropositionalSentence.Length; i++)
            {
                if (mPropositionalSentence[i] == '¬')
                {
                    if (i + 1 < mPropositionalSentence.Length && separators.Contains(mPropositionalSentence[i + 1]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ValidateSides(string mPropositionalSentence)
        {
            if (Validator.ContainsOperator(mPropositionalSentence[0].ToString())) return false;
            for (int i = 0; i < mPropositionalSentence.Length - 2; i++)
            {
                if (Validator.ContainsOperator(mPropositionalSentence[i].ToString()) && !(isVariable(mPropositionalSentence[i + 1].ToString()) || mPropositionalSentence[i + 1] == '(' || mPropositionalSentence[i+1] == '¬'))
                    return false;
                if (Validator.ContainsOperator(mPropositionalSentence[i].ToString()) && !(isVariable(mPropositionalSentence[i - 1].ToString()) || mPropositionalSentence[i - 1] == ')' || mPropositionalSentence[i -1] == '¬'))
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

        public static bool ContainsLetter(string vl)
        {
            foreach (char c in vl)
            {
                if (char.IsLetter(c))
                {
                    return true;
                }
            }
            return false;
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


