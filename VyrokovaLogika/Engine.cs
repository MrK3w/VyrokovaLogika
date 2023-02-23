using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VyrokovaLogika
{
    public class Engine
    {
        string mPropositionalSentence;
        public Tree tree { get; set; }
        Splitter mSplitter;
        Node mainNode;
        int deepestLevel = 0;
        List<List<string>> treeNodes = new List<List<string>>();
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
                tree = new Tree(mainNode);
                BuildTree(mainNode, tree);
                PrintData(tree,0);                
            }
        }

        void PrintData(Tree p, int indent)
        {
            // Print me
            if (p.childNodeLeft != null)
            {
                PrintData(p.childNodeLeft, indent + 1); // Increase the indent for children
            }

            if (p.childNodeRight != null)
            {
                PrintData(p.childNodeRight, indent + 1); // Increase the indent for children
            }
        }



    

    //private void PrintTree(Tree node, int indent)
    //{
    //    PrintWithIndent(node.Item.mSentence, indent);
    //    List<Tree> list = new List<Tree>(); 
    //    if (!node.childNodeLeft.IsLeaf && !node.childNodeRight.IsLeaf)
    //    {
    //        list = new List<Tree>() { node.childNodeLeft, node.childNodeRight };
    //    }
    //    foreach (var child in list)
    //    {
    //        PrintTree(child, indent + 1); // Increase the indent for children
    //    }
    //}
    //private void PrintWithIndent(string value, int indent)
    //{
    //    Console.WriteLine("{0}{1}", new string(' ', indent * 2), value);
    //}

    private void BuildTree(Node node, Tree tree)
        {

            Node mRightNode = null;
            Node mLeftNode = null;
            if (Validator.CheckParenthesses(node.mSentence))
            {
                mSplitter = new Splitter(node.mSentence);
                //find spot where we should split that sentence
                if (mSplitter.FindSplitPoint())
                {
                    //split string at that point
                    var splitterParts = mSplitter.SplitString();
                    //remove brackets if part has it
                    splitterParts.Item1 = CheckAndPreparePart(splitterParts.Item1);
                    mLeftNode = new Node(splitterParts.Item1, node.level + 1);

                    node.mOperator = Operator.GetOperator(splitterParts.Item2);

                    splitterParts.Item3 = CheckAndPreparePart(splitterParts.Item3);
                    mRightNode = new Node(splitterParts.Item3, node.level + 1);
                }
                else
                {
                    var item = CheckAndPreparePart(node.mSentence);
                    mLeftNode = new Node(item, node.level + 1);
                    if (deepestLevel < mLeftNode.level) deepestLevel = mLeftNode.level;
                    var pokus = tree.AddChild(mLeftNode, "left");

                    BuildTree(mLeftNode, pokus);
                    return;
                }
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
                else if (Validator.ContainsNegation(node.mSentence) && !Validator.ContainsOperator(node.mSentence))
                {
                    node.mOperator = Operator.GetOperator(node.mSentence[0].ToString());
                    //remove first sign
                    string sen = node.mSentence.Substring(1);
                    mLeftNode = new Node(sen, node.level + 1);
                    var temp = tree.AddChild(mLeftNode, "left") ;
                    BuildTree(mLeftNode, temp);
                    return;
                }
                else
                { 
                    if (node.level > deepestLevel) deepestLevel = node.level;
                    return;
                }
            }

            if (deepestLevel < mLeftNode.level) deepestLevel = mLeftNode.level;
            if (deepestLevel < mRightNode.level) deepestLevel = mRightNode.level;

            var first = tree.AddChild(mLeftNode,"left");
            var second = tree.AddChild(mRightNode,"right");
         
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
            part = part.Substring(2, part.Length - 3);
            if (part.Contains('&'))
            {
                var parts = part.Split('&');
                var part1 = parts[0];
                var part2 = parts[1];
                newPart.Append('-' + part1 + "|-" + part2);
            }
            else if(part.Contains('|'))
            {
                var parts = part.Split('|');
                var part1 = parts[0];
                var part2 = parts[1];
                newPart.Append('-' + part1 + "&-" + part2);
            }
            else if(part.Contains('>'))
            {
                var parts = part.Split('>');
                var part1 = parts[0].Substring(1);
                var part2 = parts[1];
                newPart.Append(part1 + "&-" + part2);
            }
            return newPart.ToString();
        }
    }


}
