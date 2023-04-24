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
            if (proofSearch == "Tautology")
            {
                var pathTrees = proofSolver.ProcessTree(Tree);
                isTautologyOrContradiction = proofSolver.FindContradiction(pathTrees);
            }
            else if (proofSearch == "Contradiction")
            {
                var pathTrees = proofSolver.ProcessTree(Tree, 1);
                isTautologyOrContradiction = proofSolver.FindContradiction(pathTrees);
            }
            DistinctNodes = proofSolver.DistinctNodes;
            CounterModel = proofSolver.CounterModel;
            return isTautologyOrContradiction;
        }

        private void BuildTree(Node node, Tree tree)
        {
            if (Validator.IsLiteralWithNegation(node.MSentence))
            {
                node.MSentence = node.MSentence.Replace("(", "").Replace(")", "");
            }
            Splitter splitter = new(node);
            splitter.Split();
            node.MOperator = splitter.MNode.MOperator;


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
            DAG dagConvert = new(Dag);
            if (CounterModel.MOperator != Operator.OperatorEnum.EMPTY)
            {
                AddNumbersToTruhTree();
                dagConvert.PrepareDAG(CounterModel, exercise);
            }
            else dagConvert.PrepareDAG(null);
          
            TreeConnections = dagConvert.TreeConnections;

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
