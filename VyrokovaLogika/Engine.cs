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
        public List<string> DAGNodes { get; private set; } = new List<string>();
        public List<Tuple<string, string>> TreeConnections { get; private set; } = new List<Tuple<string, string>>();


        int number = 1;

        public Engine(string propositionalSentence)
        {
            mPropositionalSentence = propositionalSentence;
        }

        public bool ProcessSentence()
        {
            ConvertSentenceToRightFormat();
            //check if sentence is valid
            if (!Validator.ValidateParenthesses(mPropositionalSentence)) return false;
            mainNode = new Node(mPropositionalSentence);
            //BUILD TREE
            tree = new Tree(mainNode);
            BuildTree(mainNode, tree);
            //CONVERT TREE TO DAG
            var dagConverter = new ASTtoDAGConverter();
            Dag = dagConverter.Convert(tree);
            //TODO TreeProof
            TreeProof proofSolver = new TreeProof();
            var xdd = proofSolver.ProcessTree(tree);
            Tautology = proofSolver.isTautology(xdd);
            return true;
        }

        private void ConvertSentenceToRightFormat()
        {
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty).ToLowerInvariant();
            Converter.ConvertLogicalOperators(ref mPropositionalSentence);
            Converter.ConvertParenthessis(ref mPropositionalSentence);
        }

        private void BuildTree(Node node, Tree tree)
        {
            Splitter splitter = new Splitter(node);
            node.mOperator = splitter.mNode.mOperator;
            splitter.Split();
            if (splitter.mLeftNode != null)
            {
                number++;
                var leftTree = tree.AddChild(splitter.mLeftNode, "left", number);
                BuildTree(splitter.mLeftNode, leftTree);
            }
            if (splitter.mRightNode != null)
            {
                number++;
                var rightTree = tree.AddChild(splitter.mRightNode, "right", number);
                BuildTree(splitter.mRightNode, rightTree);
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
