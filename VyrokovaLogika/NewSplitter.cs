using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class NewSplitter
    {
        public Node mLeftNode { get; set; } = null;
        public Node mRightNode { get; set; } = null;

        private Node mNode;

        public NewSplitter(Node node)
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
            if(Validator.ContainsParenthesses(vl))
            {
                if(Validator.ValidateParenthesses(vl))
                {
                    var parts = SplitStringByParenthessis(vl);
                    mLeftNode = new Node(parts[0], mNode.level + 1);
                    mNode.mOperator = Operator.GetOperator(parts[1]);
                    mRightNode = new Node(parts[2], mNode.level + 1);
                    return;
                }
                else
                {
                    Console.WriteLine("error");
                }
            }
            if(Validator.ContainsOperator(vl))
            {
                var parts = SplitStringByOperator(vl);
                mLeftNode = new Node(parts[0], mNode.level + 1);
                mNode.mOperator = Operator.GetOperator(parts[1]);
                mRightNode = new Node(parts[2], mNode.level + 1);
                return;

            }
            if(Validator.ContainsNegation(vl)) 
            {
                mNode.mOperator = Operator.OperatorEnum.NEGATION;
                mLeftNode = new Node(vl[1].ToString(), mNode.level + 1);
            }
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
                    part = part.Substring(1, part.Length - 1);
                    parts.Add(part);
                    parts.Add(vl[position + 1].ToString());
                    var xd = position + 2;
                    var lol = vl.Length;

                    parts.Add(vl.Substring(position+2, vl.Length-(position+2)));
                    return parts;
                }
                position++;
                sb.Append(c);
            }
            return null;
        }
    }
}
