using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class Splitter
    {
        public Node MLeftNode { get; set; } = null;
        public Node MRightNode { get; set; } = null;

        public Node MNode { get; set; }
        private bool isNegation = false;

        public Splitter(Node node)
        {
            MNode = node;     
        }

        public void Split()
        {
            //get formula from node
            string vl = MNode.MSentence;
            //vl is final variable
            if (Validator.IsLiteral(vl))
            {
                
                return;
            }
            //we need to take of negation on start of formula
            if (Validator.ContainsNegationOnFirstPlace(vl))
            {
                SeparateWithNegation(vl);
                vl = Converter.ReduceParenthessis(vl);
                return;
            }
            //we need to take of brackets on start of formula
            if (Validator.ContainsParenthesses(vl) && vl[0] == '(')
            {
                //Validate parenthesses
                if(Validator.ValidateParenthesses(vl))
                {
                    SeparateByBrackets(vl);
                    return;
                }
            
            }
            //if we need to separate by operator firstly
            if(Validator.ContainsOperator(vl))
            {
                SeparateByOperator(vl);
                return;

            }
        }

        private void SeparateWithNegation(string vl)
        {
            //¬x
            if (vl.Length == 2)
            {
                MNode.MOperator = Operator.OperatorEnum.NEGATION;
                MLeftNode = new Node(vl[1].ToString(), MNode.level + 1);
            }
            //¬¬x
            else if (vl.Length == 3 && vl[0] == '¬' && vl[1] == '¬')
            {
                MNode.MOperator = Operator.OperatorEnum.DOUBLENEGATION;
                MLeftNode = new Node(vl[2].ToString(), MNode.level + 1);
            }
            //¬(
            else if (vl[1] == '(')
            {
                MNode.MOperator = Operator.OperatorEnum.NEGATION;
                isNegation = true;
                SeparateByBrackets(vl);
            }
            else
            {
                SeparateByOperator(vl);
            }
        }

        private void SeparateByBrackets(string vl)
        {
            var parts = SplitStringByParenthessis(vl);
            //reduces start and closing brackets of formula
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i] = Converter.ReduceParenthessis(parts[i]);
            }
            //check if new part is not same like upper just without brackets
            if (MNode.MSentence != '(' + parts[0] + ')')
            {
                MLeftNode = new Node(parts[0], MNode.level + 1);
            }
            //otherwise separate by operator
            else
            {
                SeparateByOperator(parts[0]);
            }
            //if we have more than one part
            if (parts.Count > 1)
            {
                if (MNode.MSentence != '(' + parts[2] + ')')
                {
                    MNode.MOperator = Operator.GetOperator(parts[1]);
                    MRightNode = new Node(parts[2], MNode.level + 1);
                }
                else
                {
                    SeparateByOperator(parts[2]);
                }

            }
        }

        private void SeparateByOperator(string vl)
        {
            //separate parts by operator
            var parts = SplitStringByOperator(vl);
            MLeftNode = new Node(parts[0], MNode.level + 1);
            MNode.MOperator = Operator.GetOperator(parts[1]);
            MRightNode = new Node(parts[2], MNode.level + 1);
        }

        private static List<string> SplitStringByOperator(string vl)
        {
            List<string> parts = new();
            //logical operators in formula
            List<char> separators = new() { '∧', '∨', '⇒', '≡' };
            int operatorIndex = -1; 
            foreach (char s in vl)
            {
                //if we found brackets stop searching
                if (s == '(') break;

                //if we found separator in vl
                if (separators.Contains(s))
                {
                    //find his index
                    int separatorIndex = separators.IndexOf(s);
                    //compare index with previous index to check priority
                    if (operatorIndex >= 0 && separatorIndex < operatorIndex)
                    {
                        continue;
                    }
                    else
                    {
                        operatorIndex = separatorIndex;
                    }
                }
  
            }
            int sepIndex = vl.IndexOf(separators[operatorIndex]);
            string firstPart = vl[..sepIndex]; // from index 0 to separatorIndex-1
            string thirdPart = vl[(sepIndex + 1)..]; // from separatorIndex+1 to the end of the string
            //add parts
            parts.Add(firstPart);
            parts.Add(separators[operatorIndex].ToString());
            parts.Add(thirdPart);
            return parts;
        }

        //split string by parenthessis
        private List<string>? SplitStringByParenthessis(string vl)
        {
            List<string> parts = new();
            Stack<char> parenthessesStack = new();
            StringBuilder sb = new();
            bool metParenthesses = false;
            int position = 0;
            foreach (char c in vl)
            {
                sb.Append(c);
                //if this brackets add to stack 
                if (c == '(')
                {
                    parenthessesStack.Push(c);
                    metParenthesses = true;
                }
                //if this brackets pop
                else if (c == ')')
                {
                    //if parentheses stack is zero or there is not another ( 
                    if (parenthessesStack.Count == 0 || parenthessesStack.Pop() != '(')
                    {
                        throw new Exception();
                    }
                }
                //if there is zero parenthesses and we already met some we will split string
                if (metParenthesses && parenthessesStack.Count == 0)
                {
                    string part = sb.ToString();

                    string? op = null;
                    string? secondPart = null;
                    if (position + 1 < vl.Length)
                    {
                        op = vl[position + 1].ToString();
                        secondPart = vl[(position + 2)..];
                    }
                    // if there is negation we will split it away and return string without it
                    if (isNegation)
                    {
                        if (secondPart == null)
                        {
                            part = part[1..];
                        }
                        
                    }
                    parts.Add(part);
                    if (secondPart != null)
                    {
                        parts.Add(op);
                        parts.Add(secondPart);
                    }
                    return parts;
                }
                position++;
            }
            return null;
        }
    }
}
