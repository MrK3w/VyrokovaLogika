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
        private DAGNode dag;

        public DAG(DAGNode dag)
        {
            this.dag = dag;
        }

        internal void PrepareDAG()
        {
            PrepareDAGNodesList(this.dag);
            DAGNodes = DAGNodes.Distinct().ToList();

            PrepareDAGNodesListConnection(this.dag);
            RemoveDuplicates();
            ReplaceConnectionNumbersForString();
        }

        private void PrepareDAGNodesList(DAGNode tree)
        {
            DAGNodesNumbered.Add((new Tuple<string, int>(tree.Item.mSentence, tree.Item.number)));
            DAGNodes.Add(tree.Item.mSentence);
            if (tree.LeftChild != null)
            {
                PrepareDAGNodesList(tree.LeftChild);
                if (tree.RightChild != null)
                {
                    PrepareDAGNodesList(tree.RightChild);
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
                TreeConnections.Add(new Tuple<string, string>(SearchByNumber(connection.Item1), SearchByNumber(connection.Item2)));
            }
        }

        // Method to search for a corresponding tuple by number
        public string SearchByNumber(int number)
        {
            return DAGNodesNumbered.FirstOrDefault(t => t.Item2 == number).Item1;
        }
    }
}
