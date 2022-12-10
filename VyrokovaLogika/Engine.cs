using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    class Engine
    {
        bool FirstTime = true;
        string mPropositionalSentence;
        Tree<Node> tree;
        Splitter mSplitter;
        Node mainNode;
        public Engine(string propositionalSentence)
        {
            mPropositionalSentence = propositionalSentence;
        }

        public void ProcessSentence()
        {
            //Replace white spaces to better organize this sentence
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty).ToLowerInvariant();

            //check if sentence is valid
            if (Validator.Check(mPropositionalSentence))
            {
                mainNode = new Node(mPropositionalSentence);
                tree = new Tree<Node>(mainNode);
                BuildTree(mainNode, tree);
                PrintTree();
            }
        }

        private void PrintTree()
        {
          
        }

        private void BuildTree(Node node, Tree<Node> tree)
        {
            Node mRightNode = null;
            Node mLeftNode = null;

            if (Validator.CheckParenthesses(node.mSentence))
            {
                mSplitter = new Splitter(node.mSentence);
                //find spot where we should split that sentence
                mSplitter.FindSplitPoint();
                //split string at that point
                var splitterParts = mSplitter.SplitString();
                //find spot where we should split that sentence
                splitterParts.Item1 = CheckAndPreparePart(splitterParts.Item1);
                mLeftNode = new Node(splitterParts.Item1);
                node.mOperator = Operator.GetOperator(splitterParts.Item2);
                splitterParts.Item3 = CheckAndPreparePart(splitterParts.Item3);
                mRightNode = new Node(splitterParts.Item3);

            }

            else if (!Validator.CheckParenthesses(node.mSentence))
            {
                if (Validator.ContainsOperator(node.mSentence))
                {
                    mLeftNode = new Node(node.mSentence[0].ToString());
                    node.mOperator = Operator.GetOperator(node.mSentence[1].ToString());
                    mRightNode = new Node(node.mSentence[2].ToString());
                }
                //b&c
                else
                {
                    return;
                }
            }
            var first = tree.AddChild(mLeftNode);
            var second = tree.AddChild(mRightNode);
            BuildTree(mLeftNode, first);
            BuildTree(mRightNode, second);
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
