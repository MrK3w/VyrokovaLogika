using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class Splitter
    {
        public Node mLeftNode { get; set; } = null;
        public Node mRightNode { get; set; } = null;

        public Node mNode { get; set; }
        private bool isNegation = false;

        public Splitter(Node node)
        {
            mNode = node;     
        }

        public void Split()
        {
            //get formula from node
            string vl = mNode.mSentence;
            //vl is final variable
            if (Validator.isVariable(vl))
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
                else
                {
                    Console.WriteLine("error");
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
                mNode.mOperator = Operator.OperatorEnum.NEGATION;
                mLeftNode = new Node(vl[1].ToString(), mNode.level + 1);
            }
            //¬¬x
            else if (vl.Length == 3 && vl[0] == '¬' && vl[1] == '¬')
            {
                mNode.mOperator = Operator.OperatorEnum.DOUBLENEGATION;
                mLeftNode = new Node(vl[2].ToString(), mNode.level + 1);
            }
            //¬(
            else if (vl[1] == '(')
            {
                mNode.mOperator = Operator.OperatorEnum.NEGATION;
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
            if (mNode.mSentence != '(' + parts[0] + ')')
            {
                mLeftNode = new Node(parts[0], mNode.level + 1);
            }
            //otherwise separate by operator
            else
            {
                SeparateByOperator(parts[0]);
            }
            //if we have more than one part
            if (parts.Count > 1)
            {
                if (mNode.mSentence != '(' + parts[2] + ')')
                {
                    mNode.mOperator = Operator.GetOperator(parts[1]);
                    mRightNode = new Node(parts[2], mNode.level + 1);
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
            mLeftNode = new Node(parts[0], mNode.level + 1);
            mNode.mOperator = Operator.GetOperator(parts[1]);
            mRightNode = new Node(parts[2], mNode.level + 1);
        }

        private List<string> SplitStringByOperator(string vl)
        {
            bool firstPart = true;
            List<string> parts = new List<string>();
            char[] separators = { '∧', '∨', '⇒', '≡' };
            StringBuilder sb = new StringBuilder();

            foreach (char s in vl)
            {
                //if we have just found operator
                if (separators.Contains(s) && firstPart)
                {
                    //first part is part of formula, second part is operator
                    parts.Add(sb.ToString());
                    parts.Add(s.ToString());
                    firstPart = false;
                    sb.Clear();
                    continue;
                }
                sb.Append(s);
            }
            parts.Add(sb.ToString());
            return parts;
        }

        private List<string> SplitStringByParenthessis(string vl)
        {
            List<string> parts = new List<string>();
            Stack<char> parenthessesStack = new Stack<char>();
            StringBuilder sb = new StringBuilder();
            bool metParenthesses = false;
            int position = 0;
            foreach (char c in vl)
            {
                sb.Append(c);
                if (c == '(')
                {
                    parenthessesStack.Push(c);
                    metParenthesses = true;
                }
                else if (c == ')')
                {
                    if (parenthessesStack.Count == 0 || parenthessesStack.Pop() != '(')
                    {
                        throw new Exception();
                    }
                }
                //separating by brackets
                if (metParenthesses && parenthessesStack.Count == 0)
                {
                    string part = sb.ToString();

                    string op = null;
                    string secondPart = null;
                    if (position + 1 < vl.Length)
                    {
                        op = vl[position + 1].ToString();
                        secondPart = vl.Substring(position + 2, vl.Length - (position + 2));
                    }
                    if (isNegation)
                    {
                        if (secondPart == null)
                        {
                            part = part.Substring(1);
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
