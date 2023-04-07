﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static VyrokovaLogika.IndexModel;

namespace VyrokovaLogika
{
    public class Engine
    {
        string mPropositionalSentence;
        public Tree tree { get; set; }
        public DAGNode Dag { get; set; }
        public bool Tautology {get; private set; }
        Splitter mSplitter;
        Node mainNode;
        int deepestLevel = 0;
        List<Tuple<int, string>> myFinals = new List<Tuple<int, string>>();
        public List<string> DAGNodes { get; private set; } = new List<string>();
        public List<Tuple<string, string>> TreeConnections { get; private set; } = new List<Tuple<string, string>>();


        int number = 1;

        public Engine(string propositionalSentence)
        {
            mPropositionalSentence = propositionalSentence;
        }

    
        public void ProcessSentence()
        {
            //Replace white spaces to better organize this sentence
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty).ToLowerInvariant();
            Converter.ConvertSentence(ref mPropositionalSentence);
            //check if sentence is valid
            if (Validator.Check(mPropositionalSentence))
            {
                mainNode = new Node(mPropositionalSentence);
                tree = new Tree(mainNode);
                BuildTree(mainNode, tree);
                TreeProof(mainNode, tree);
                Tautology = CheckIfIsItTautology();
                var dagConverter = new ASTtoDAGConverter();
                Dag = dagConverter.Convert(tree);
            }
        }

        private bool CheckIfIsItTautology()
        {
            for (int i = 0; i < myFinals.Count; i++)
            {
                for (int j = 0; j < myFinals.Count; j++)
                {
                    if (myFinals[i].Item2 == myFinals[j].Item2)
                    {
                        if (myFinals[i].Item1 != myFinals[j].Item1)
                            return false;
                    }
                }
            }
            return true;
        }

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
                mainNode.mSentence = item;
                BuildTree(mainNode, tree);
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
            else if (Validator.ContainsNegation(node.mSentence) && !Validator.ContainsOperator(node.mSentence))
            {
                node.mOperator = Operator.GetOperator(node.mSentence[0].ToString());
                //remove first sign
                string sen = node.mSentence.Substring(1);
                mLeftNode = new Node(sen, node.level + 1);
                number++;
                var temp = tree.AddChild(mLeftNode, "left", number) ;
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

            number++;
            var first = tree.AddChild(mLeftNode,"left",number);
            number++;
            var second = tree.AddChild(mRightNode,"right", number);
         
            BuildTree(mLeftNode, first);
            BuildTree(mRightNode, second);
    }

    private void TreeProof(Node node, Tree tree)
    {
            if (tree.IsRoot) tree.Item.valueMustBe = 0;
            var valuesOfNodesList = Rule.GetValuesOfBothSides(tree.Item.valueMustBe, tree.Item.mOperator);
            
            if(!tree.IsLeaf)
            {
                foreach (var valuesOfNodes in valuesOfNodesList)
                {
                    if (tree.childNodeLeft != null)
                    {
                        tree.childNodeLeft.Item.valueMustBe = valuesOfNodes.Item1;
                        TreeProof(tree.childNodeLeft.Item, tree.childNodeLeft);    
                    }

                    if (tree.childNodeRight != null)
                    {
                        tree.childNodeRight.Item.valueMustBe = valuesOfNodes.Item2;
                        TreeProof(tree.childNodeRight.Item, tree.childNodeRight);
                    }
                }
            }
            else
            {
                myFinals.Add(new(node.valueMustBe, node.mSentence));
            }
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
            if (part.Contains('∧'))
            {
                var parts = part.Split('∧');
                var part1 = parts[0];
                var part2 = parts[1];
                newPart.Append('¬' + part1 + "∨¬" + part2);
            }
            else if(part.Contains('∨'))
            {
                var parts = part.Split('∨');
                var part1 = parts[0];
                var part2 = parts[1];
                newPart.Append('¬' + part1 + "∧¬" + part2);
            }
            else if(part.Contains('>'))
            {
                var parts = part.Split('>');
                var part1 = parts[0].Substring(1);
                var part2 = parts[1];
                newPart.Append(part1 + "∧¬" + part2);
            }
            return newPart.ToString();
        }

        public void PrepareDAG()
        {
            DAG dagConvert = new DAG(Dag);
            dagConvert.PrepareDAG();
            DAGNodes = dagConvert.DAGNodes;
            TreeConnections = dagConvert.TreeConnections;
        }
    }


}
