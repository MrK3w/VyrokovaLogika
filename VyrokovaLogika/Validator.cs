using System.Text.RegularExpressions;

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
            //remove whitespaces
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty);
            //convert logical operators to right format
            Converter.ConvertLogicalOperators(ref mPropositionalSentence);
            //convert parenthessis to right format
            Converter.ConvertParenthessis(ref mPropositionalSentence);
            //remove parenthesses we don't need
            Converter.RemoveExcessParentheses(ref mPropositionalSentence);
            //check if sentence include only logical operator 
            if (mPropositionalSentence.Length == 1 && Validator.ContainsOperator(mPropositionalSentence))
            {
                ErrorMessage = "Formule nesmí obsahovat pouze logické operátory!";
                return false;
            }
            //check if sentecne contains literal
            if (!Validator.ContainsLiteral(mPropositionalSentence))
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
                ErrorMessage = "Nemáš správně umístěnou negaci.";
                return false;
            }
            if(!Validator.ValidateNegationCounts(mPropositionalSentence))
            {
                ErrorMessage = "Zkontroluj počet negací.";
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

        //if there is more than two negation in row then it's invalid
        private static bool ValidateNegationCounts(string mPropositionalSentence)
        {
            string? pattern = @"¬{3,}";

            MatchCollection? matches = Regex.Matches(mPropositionalSentence, pattern);

            return (matches.Count == 0);
            
        }
        //check if there is not negation before operator
        private static bool ValidateNegation(string mPropositionalSentence)
        {
            List<char>? separators = new() { '∧', '∨', '⇒', '≡' };

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

        //check if after operator is ( or ¬ or literal
        private static bool ValidateSides(string mPropositionalSentence)
        {
            if (Validator.ContainsOperator(mPropositionalSentence[0].ToString())) return false;
            for (int i = 0; i < mPropositionalSentence.Length - 2; i++)
            {
                if (Validator.ContainsOperator(mPropositionalSentence[i].ToString()) && !(IsLiteral(mPropositionalSentence[i + 1].ToString()) || mPropositionalSentence[i + 1] == '(' || mPropositionalSentence[i+1] == '¬'))
                    return false;
                if (Validator.ContainsOperator(mPropositionalSentence[i].ToString()) && !(IsLiteral(mPropositionalSentence[i - 1].ToString()) || mPropositionalSentence[i - 1] == ')' || mPropositionalSentence[i -1] == '¬'))
                    return false;
            }
            if (Validator.ContainsOperator(mPropositionalSentence[^1].ToString())) return false;
            return true;
        }
        
        //check if there is only one literal after each one
        private static bool CheckIfLiteralIsSingle(string mPropositionalSentence)
        {
            if (IsLiteral(mPropositionalSentence[0].ToString()) && mPropositionalSentence.Length == 1) return true;
            if (IsLiteral(mPropositionalSentence[0].ToString()) && IsLiteral(mPropositionalSentence[1].ToString()) && mPropositionalSentence.Length == 2)
                return false;
            for (int i = 0; i < mPropositionalSentence.Length - 2; i++)
            {
                if (IsLiteral(mPropositionalSentence[i].ToString()) && IsLiteral(mPropositionalSentence[i + 1].ToString()))
                    return false;
            }

            return true;
        }

        //check if there is negation before literal
        public static bool IsLiteralWithNegation(string vl)
        {

            string? pattern = @"^[a-zA-Z()\u00AC]*$";
            return Regex.IsMatch(vl, pattern);
        }

        //check if it is literal
        public static bool IsLiteral(string vl)
        {
            string? pattern = @"^[a-zA-Z]*$";
            return Regex.IsMatch(vl, pattern);
        }
        
        //check if formula contains literal
        public static bool ContainsLiteral(string vl)
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

        //check if formula contains operator
        public static bool ContainsOperator(string vl)
        {
            if (vl.Contains('∧') || vl.Contains('∨') || vl.Contains('⇒') || vl.Contains('≡')) return true;
            else return false;
        }

        //check if there is negation on first place in formula
        public static bool ContainsNegationOnFirstPlace(string vl)
        {
            if (vl[0] == '¬') return true;
            else return false;
        }

        //method to validate parenthesses
        public static bool ValidateParenthesses(string vl)
        {
            Stack<char>? parenthessesStack = new();

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
            string? allowedRegex = @"^[\s&∧|∨¬>⇒=≡a-zA-Z[\]{}()\-]+$";
            return Regex.IsMatch(mPropositionalSentence, allowedRegex);
        }
    }
}


