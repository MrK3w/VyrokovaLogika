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
        readonly string mPropositionalSentence;
        public Tree Tree { get; set; }
        public DAGNode Dag { get; set; }
        Node mainNode;
        public TruthTree CounterModel { get; set; } = new TruthTree(0);
        public List<string> DAGNodes { get; private set; } = new List<string>();
        public List<Tuple<string, string>> TreeConnections { get; private set; } = new List<Tuple<string, string>>();

        public List<Tuple<string, int>> DistinctNodes { get; set; } = new List<Tuple<string, int>>();


        int number = 1;

        public Engine(string propositionalSentence)
        {
            mPropositionalSentence = propositionalSentence;
        }

        public void ProcessSentence()
        {
            mainNode = new Node(mPropositionalSentence);
            //BUILD TREE
            Tree = new Tree(mainNode);
            BuildTree(mainNode, Tree);
        }

        public void ConvertTreeToDag()
        {
            //CONVERT TREE TO DAG
            var dagConverter = new ASTtoDAGConverter();
            Dag = dagConverter.Convert(Tree);
        }


        public bool ProofSolver(string proofSearch)
        {
            bool isTautologyOrContradiction = false;
            TreeProof proofSolver = new();
            //if we are searching for tautology we will proceed this way
            if (proofSearch == "Tautology")
            {
                //return all valid variants of tree for tautology
                var pathTrees = proofSolver.ProcessTree(Tree);
                //will find contradiction if trees if there is any
                isTautologyOrContradiction = proofSolver.FindContradiction(pathTrees);
            }
            else if (proofSearch == "Contradiction")
            {
                //in case of contradiction we will send 1 to search for tree, which evaluation will be 1
                var pathTrees = proofSolver.ProcessTree(Tree, 1);
                isTautologyOrContradiction = proofSolver.FindContradiction(pathTrees);
            }
            //all distinct nodes with literal for specific tree
            DistinctNodes = proofSolver.DistinctNodes;
            //tree counter model to show on page
            CounterModel = proofSolver.CounterModel;
            return isTautologyOrContradiction;
        }

        private void BuildTree(Node node, Tree tree)
        {
            //does literal has negation, then wwe will remove brackets
            if (Validator.IsLiteralWithNegation(node.MSentence))
            {
                node.MSentence = node.MSentence.Replace("(", "").Replace(")", "");
            }
            //new instance of splitter for current node
            Splitter splitter = new(node);
            //split sentence by it
            splitter.Split();
            //get operator from splitter instance
            node.MOperator = splitter.MNode.MOperator;

            //if MLeft node is not null we will proceed for it and add numbering
            if (splitter.MLeftNode != null)
            {
                number++;
                var leftTree = tree.AddChild(splitter.MLeftNode, "left", number);
                BuildTree(splitter.MLeftNode, leftTree);
            }
            if (splitter.MRightNode != null)
            {
                number++;
                var rightTree = tree.AddChild(splitter.MRightNode, "right", number);
                BuildTree(splitter.MRightNode, rightTree);
            }
        }

        public void PrepareDAG(bool exercise = false)
        {
            //create new instance of dag for this 
            DAG dagConvert = new(Dag);
            if (CounterModel.MOperator != Operator.OperatorEnum.EMPTY)
            {
                //add truth values to that dag and prepare him
                AddNumbersToTruhTree();
                dagConvert.PrepareDAG(CounterModel, exercise);
            }
            //if we dont need truth values in dag we will use this option
            else dagConvert.PrepareDAG(null);
            //get TreeConnections from dag class
            TreeConnections = dagConvert.DAGConnections;

            //we will get all nodes from treeConnections
            foreach (var treeConnect in TreeConnections)
            {
                if (!DAGNodes.Contains(treeConnect.Item1)) DAGNodes.Add(treeConnect.Item1);
                if (!DAGNodes.Contains(treeConnect.Item2)) DAGNodes.Add(treeConnect.Item2);
            }
        }
        private void AddNumbersToTruhTree()
        {
            CounterModel.AddNumbers(Tree);
        }
    }


}
