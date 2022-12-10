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
                splitterParts.Item1 = CheckAndPreparePart(splitterParts.Item1);
                mLeftNode = new Node(splitterParts.Item1);
                mLeftNode.Process();

                //get operator between this two parts
                mOperator = GetOperator(splitterParts.Item2);

                //check if third part had parenthesses if had then strip them
                splitterParts.Item3 = CheckAndPreparePart(splitterParts.Item3);
                mRightNode = new Node(splitterParts.Item3);
                mRightNode.Process();
            }
            else if(!Validator.CheckParenthesses(mSentence))
            {
                if(Validator.ContainsOperator(mSentence))
                {
                    mLeftNode = new Node(mSentence[0].ToString());
                    mLeftNode.Process();
                    mOperator = GetOperator(mSentence[1].ToString());
                    mRightNode = new Node(mSentence[2].ToString());
                    mRightNode.Process();
                }
                //b&c
                else
                {
                    Console.WriteLine("Narazil jsi na konec!!");
                }
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

        public override string ToString()
        {
            return "Zdar";
        }
    }
}
