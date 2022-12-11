using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VyrokovaLogika
{
    class Engine
    {
        string mPropositionalSentence;
        Tree<Node> tree;
        Splitter mSplitter;
        Node mainNode;
        int deepestLevel = 0;
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
            for (int i = 1; i <= deepestLevel; i++)
            {
                var list = tree.ReturnNode(i);
                foreach (var item in list)
                {
                    Console.Write(item + "\t");
                }
                Console.WriteLine("");
            }
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
                //remove brackets if part has it
                splitterParts.Item1 = CheckAndPreparePart(splitterParts.Item1);
                mLeftNode = new Node(splitterParts.Item1,node.level+1);
               
                node.mOperator = Operator.GetOperator(splitterParts.Item2);
                splitterParts.Item3 = CheckAndPreparePart(splitterParts.Item3);
                mRightNode = new Node(splitterParts.Item3, node.level + 1);
               
            }

            else if (!Validator.CheckParenthesses(node.mSentence))
            {
                if (Validator.ContainsOperator(node.mSentence))
                {
                    mLeftNode = new Node(node.mSentence[0].ToString(), node.level + 1);
                    node.mOperator = Operator.GetOperator(node.mSentence[1].ToString());
                    mRightNode = new Node(node.mSentence[2].ToString(), node.level + 1);
                }
                //b&c
                else
                {
                    return;
                }
            }

            if (deepestLevel < mLeftNode.level) deepestLevel = mLeftNode.level;
            if (deepestLevel < mRightNode.level) deepestLevel = mRightNode.level;

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
