using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VyrokovaLogika.IndexModel;

namespace VyrokovaLogika
{
    public class DAG
    {
        public List<string> DAGNodes { get; set; } = new List<string>();
        private List<Tuple<int, int>> TreeConnectionsNumbered = new List<Tuple<int, int>>();

        public List<Tuple<string, string>> TreeConnections { get; } = new List<Tuple<string, string>>();

        private List<Tuple<string, int>> DAGNodesNumbered = new List<Tuple<string, int>>();
        private List<Tuple<string, int>> DAGNodesNumberedTruthTree = new List<Tuple<string, int>>();
        private DAGNode dag;

        public DAG(DAGNode dag)
        {
            this.dag = dag;
        }

        internal void PrepareDAG(TruthTree counterModel = null)
        {
            PrepareDAGNodesList(this.dag, counterModel);
            DAGNodes = DAGNodes.Distinct().ToList();

            PrepareDAGNodesListConnection(this.dag);
            RemoveDuplicates();
            ReplaceConnectionNumbersForString();
        }

        private void PrepareDAGNodesList(DAGNode tree, TruthTree truthTree)
        {
            DAGNodesNumbered.Add((new Tuple<string, int>(tree.Item.mSentence, tree.Item.number)));
            DAGNodesNumberedTruthTree.Add((new Tuple<string, int>(truthTree.Item.ToString(), truthTree.number)));
            DAGNodes.Add(tree.Item.mSentence);
            if (tree.LeftChild != null)
            {
                PrepareDAGNodesList(tree.LeftChild,truthTree.ChildNodeLeft);
                if (tree.RightChild != null)
                {
                    PrepareDAGNodesList(tree.RightChild, truthTree.ChildNodeRight);
                }
            }
        }

        private void PrepareDAGNodesListConnection(DAGNode tree)
        {
            if (tree.LeftChild != null)
            {
                TreeConnectionsNumbered.Add(new Tuple<int, int>(tree.Item.number, tree.LeftChild.Item.number));
            }
            if (tree.RightChild != null)
            {
                TreeConnectionsNumbered.Add(new Tuple<int, int>(tree.Item.number, tree.RightChild.Item.number));
            }
            if (tree.LeftChild != null)
            {
                PrepareDAGNodesListConnection(tree.LeftChild);
                if (tree.RightChild != null)
                {
                    PrepareDAGNodesListConnection(tree.RightChild);
                }
            }
        }

        private void RemoveDuplicates()
        {
            TreeConnectionsNumbered = TreeConnectionsNumbered.Distinct(new TupleEqualityComparer<int, int>()).ToList();
        }

        private void ReplaceConnectionNumbersForString()
        {
            foreach (var connection in TreeConnectionsNumbered)
            {
                var ConnectionsTuple = new Tuple<string, string>(SearchByNumber(connection.Item1), SearchByNumber(connection.Item2));
                var firstItem = AddEvaluationValuesToTuple(ConnectionsTuple.Item1);
                var secondItem = AddEvaluationValuesToTuple(ConnectionsTuple.Item2);
                ConnectionsTuple = new Tuple<string, string>(firstItem,secondItem);
                TreeConnections.Add(ConnectionsTuple);
            }
        }

        private string AddEvaluationValuesToTuple(string item)
        {
            List<int> result = DAGNodesNumbered.Where(t => t.Item1 == item).Select(t => t.Item2).ToList();
            result = result.Distinct().ToList();
            List<string> newValuesList = new List<string>();
            foreach (var r in result)
            {
                var valueToAdd = DAGNodesNumberedTruthTree
                                .SingleOrDefault(tuple => tuple.Item2 == r)?.Item1; 

                if (valueToAdd != null && !newValuesList.Contains(valueToAdd))
                {
                    newValuesList.Add(valueToAdd);
                }

            }
            foreach (var newValue in newValuesList)
            item += $" {newValue} ";
            return item;
           
        }

        public string SearchByNumber(int number)
        {
            return DAGNodesNumbered.FirstOrDefault(t => t.Item2 == number).Item1;
        }
    }
}
