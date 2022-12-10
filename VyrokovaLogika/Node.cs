using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VyrokovaLogika.Operator;

namespace VyrokovaLogika
{
    public class Node
    {
        string mSentence = string.Empty;
        Splitter mSplitter;
        Node mLeftNode;
        Node mRightNode;
        OperatorEnum mOperator;
        int upperEquals;

        public Node(string sentence)
        {
            mSentence = sentence;
            mSplitter = new Splitter(mSentence);
        }

        public void Process()
        {
            if (Validator.CheckParenthesses(mSentence))
            {
                //find spot where we should split that sentence
                mSplitter.FindSplitPoint();
                //split string at that point
                var splitterParts = mSplitter.SplitString();
                //check if first part had parenthesses if had then strip them
                if (Validator.CheckParenthesses(splitterParts.Item1))
                {
                    splitterParts.Item1 = splitterParts.Item1.Substring(1, splitterParts.Item1.Length - 2);
                }
                //create new first node and proceed it
                mLeftNode = new Node(splitterParts.Item1);
                mLeftNode.Process();
                //get operator between this two parts
                mOperator = GetOperator(splitterParts.Item2);
                splitterParts.Item3 = CheckAndPreparePart(splitterParts.Item3);
                mRightNode = new Node(splitterParts.Item3);
                mRightNode.Process();
            }
        }

        private string CheckAndPreparePart(string part)
        {
            if (Validator.CheckParenthesses(part))
            {
                part = part.Substring(1, part.Length - 2);
            }
            return part;
        }
    }
}
