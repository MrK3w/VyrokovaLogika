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
        Node mainNode;
        public TruthTree counterModel { get; set; } = new TruthTree(0);
        public List<string> DAGNodes { get; private set; } = new List<string>();
        public List<Tuple<string, string>> TreeConnections { get; private set; } = new List<Tuple<string, string>>();

        public List<Tuple<string, int>> distinctNodes { get; set; } = new List<Tuple<string, int>>();


        int number = 1;

        public Engine(string propositionalSentence)
        {
            mPropositionalSentence = propositionalSentence;
        }

        public bool ValidateSentence()
        {
            ConvertSentenceToRightFormat();
            //check if sentence is valid
            if (!Validator.ValidateParenthesses(mPropositionalSentence)) return false;
            if (!Validator.RightCharacters(mPropositionalSentence)) return false;
            return true;
        }

        public void ProcessSentence()
        {
            
            mainNode = new Node(mPropositionalSentence);
            //BUILD TREE
            tree = new Tree(mainNode);
            BuildTree(mainNode, tree);
            //CONVERT TREE TO DAG
            var dagConverter = new ASTtoDAGConverter();
            Dag = dagConverter.Convert(tree);
        }

        public bool ProofSolver(string proofSearch)
        {
            bool isTautologyOrContradiction = false;
            //TODO TreeProof
            TreeProof proofSolver = new TreeProof();
            if (proofSearch == "Tautology")
            {
                var pathTrees = proofSolver.ProcessTree(tree);
                isTautologyOrContradiction = proofSolver.FindContradiction(pathTrees);       
            }
            else if(proofSearch == "Contradiction")
            {
                var pathTrees = proofSolver.ProcessTree(tree,1);
                isTautologyOrContradiction = proofSolver.FindContradiction(pathTrees);
            }
            if (!isTautologyOrContradiction)
            {
                distinctNodes = proofSolver.distinctNodes;
                counterModel = proofSolver.counterModel;
            }
            return isTautologyOrContradiction;
        }

        private void ConvertSentenceToRightFormat()
        {
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty).ToLowerInvariant();
            Converter.ConvertLogicalOperators(ref mPropositionalSentence);
            Converter.ConvertParenthessis(ref mPropositionalSentence);
        }

        private void BuildTree(Node node, Tree tree)
        {
            if (Validator.isVariableWithNegation(node.mSentence))
            {
                node.mSentence = node.mSentence.Replace("(", "").Replace(")", "");
            }
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
