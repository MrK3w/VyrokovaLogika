using System;
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
        Node mainNode;
        List<Tuple<int, string>> myFinals = new List<Tuple<int, string>>();
        public List<string> DAGNodes { get; private set; } = new List<string>();
        public List<Tuple<string, string>> TreeConnections { get; private set; } = new List<Tuple<string, string>>();


        int number = 1;

        public Engine(string propositionalSentence)
        {
            mPropositionalSentence = propositionalSentence;
        }

        public bool ProcessSentence()
        {
            //Replace white spaces to better organize this sentence
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty).ToLowerInvariant();
            Converter.ConvertLogicalOperators(ref mPropositionalSentence);
            Converter.ConvertParenthessis(ref mPropositionalSentence);
            //check if sentence is valid
            if (!Validator.ValidateParenthesses(mPropositionalSentence)) return false;
            mainNode = new Node(mPropositionalSentence);
            tree = new Tree(mainNode);
            BuildTree(mainNode, tree);
            TreeProof(mainNode, tree);
            Tautology = CheckIfIsItTautology();
            var dagConverter = new ASTtoDAGConverter();
            Dag = dagConverter.Convert(tree);
            return true;
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
            Splitter splitter = new Splitter(node);
            node.mOperator = splitter.mNode.mOperator;
            splitter.Split();
            if (splitter.mLeftNode != null)
            {
                number++;
                var first = tree.AddChild(splitter.mLeftNode, "left", number);
                BuildTree(splitter.mLeftNode, first);
            }
            if (splitter.mRightNode != null)
            {
                number++;
                var second = tree.AddChild(splitter.mRightNode, "right", number);
                BuildTree(splitter.mRightNode, second);
            }
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
     
        public void PrepareDAG()
        {
            DAG dagConvert = new DAG(Dag);
            dagConvert.PrepareDAG();
            DAGNodes = dagConvert.DAGNodes;
            TreeConnections = dagConvert.TreeConnections;
        }
    }


}
