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
                Console.WriteLine("\n");
            }
            //var list = tree.getFinal();
            //foreach (var item in list)
            //{
            //    Console.WriteLine(item);
            //}
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
                    if (!Validator.ContainsNegation(node.mSentence))
                    {
                        mLeftNode = new Node(node.mSentence[0].ToString(), node.level + 1);
                        node.mOperator = Operator.GetOperator(node.mSentence[1].ToString());
                        mRightNode = new Node(node.mSentence[2].ToString(), node.level + 1);
                    }
                    else
                    {
                        mSplitter = new Splitter(node.mSentence);
                        mSplitter.FindSplitPointForNegation();
                        var splitterParts = mSplitter.SplitString();
                        mLeftNode = new Node(splitterParts.Item1, node.level + 1);
                        node.mOperator = Operator.GetOperator(splitterParts.Item2);
                        mRightNode = new Node(splitterParts.Item3, node.level + 1);
                    }
                }
                //b&c
                //else if(Validator.ContainsNegation(node.mSentence) && !Validator.ContainsOperator(node.mSentence))
                //{
                //    node.mOperator = Operator.GetOperator(node.mSentence[0].ToString());
                //    //remove first sign
                //    string sen = node.mSentence.Substring(1);
                //    mLeftNode = new Node(sen, node.level + 1);
                //    var temp = tree.AddChild(mLeftNode);
                //    BuildTree(mLeftNode, temp);
                //    return;
                //}
                else
                { 
                    node.isFinal = true;
                    if (node.level > deepestLevel) deepestLevel = node.level;
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
                if (Validator.ContainsNegation(part[0].ToString()))
                {
                    return RemoveParenthessesWithNegation(part);
                }
                else return part.Substring(1, part.Length - 2);
            }
            return part;
        }

        private string RemoveParenthessesWithNegation(string part)
        {
            StringBuilder newPart = new StringBuilder();
            char negation = '-';
            newPart.Append(negation);
            bool insideParenthesses = false;
            part = part.Substring(2, part.Length - 3);
            foreach (var item in part)
            {
                if (item == '(') insideParenthesses = true;
                if (item == ')') insideParenthesses = false;
                if(insideParenthesses)
                {
                    newPart.Append(item);
                    continue;
                }
                if ((item == '&') || (item == '|') || (item == '>'))
                {
                    newPart.Append(item);
                    newPart.Append(negation);
                }
                else
                {
                    newPart.Append(item);
                }
            }
            return newPart.ToString();
        }
    }


}
