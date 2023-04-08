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

        private Node mNode;
        private bool isNegation = false;

        public Splitter(Node node)
        {
            mNode = node;     
        }

        public void Split()
        {
            string vl = mNode.mSentence;
            if (Validator.IsVariable(vl))
            {
                return;
            }
            if (Validator.ContainsNegationOnFirstPlace(vl))
            {
                if (vl.Length == 2 )
                {
                    mNode.mOperator = Operator.OperatorEnum.NEGATION;
                    mLeftNode = new Node(vl[1].ToString(), mNode.level + 1);
                    return;
                }
                else if(vl.Length == 3 && vl[0] == '¬' && vl[1] == '¬')
                {
                    mNode.mOperator = Operator.OperatorEnum.DOUBLENEGATION;
                    mLeftNode = new Node(vl[2].ToString(), mNode.level + 1);
                    return;
                }
                else if (vl[1] == '(')
                {
                    isNegation = true;
                    SeparateByBrackets(vl);
                    return;

                }
            }
            if (Validator.ContainsParenthesses(vl) && vl[0] == '(')
            {
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
            if(Validator.ContainsOperator(vl))
            {
                SeparateByOperator(vl);
                return;

            }
        }

        private void SeparateByBrackets(string vl)
        {
            var parts = SplitStringByParenthessis(vl);
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i] = Converter.ReduceParenthessis(parts[i]);
            }
            if (mNode.mSentence != '(' + parts[0] + ')')
            {
                mLeftNode = new Node(parts[0], mNode.level + 1);
            }
            else
            {
                SeparateByOperator(parts[0]);
            }
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
                if (separators.Contains(s) && firstPart)
                {
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

        private static void Laws(ref string part, ref string op, ref string secondPart)
        {
            switch (op[0])
            {
                case '⇒':
                    part = part;
                    op = "∧";
                    secondPart = '¬' + secondPart;
                    break;
                case '≡':
                    part = '¬' + part;
                    op = "≡";
                    secondPart = '¬' + secondPart;
                    break;
                case '∨':
                    part = '¬' + part;
                    op = "∧";
                    secondPart = '¬' + secondPart;
                    break;
                case '∧':
                    part = '¬' + part;
                    op = "∨";
                    secondPart = '¬' + secondPart;
                    break;
                default:
                    break;
            }
        }
    }
}
