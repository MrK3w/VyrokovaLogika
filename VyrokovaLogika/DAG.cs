using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static VyrokovaLogika.IndexModel;

namespace VyrokovaLogika
{
    public class DAG
    {
        //all unique dagNodes
        public List<string> DAGNodes { get; set; } = new List<string>();
        //DAG connections by number
        private List<Tuple<int, int>> DAGConnectionsNumbered = new List<Tuple<int, int>>();

        //DAG connections by their formula value
        public List<Tuple<string, string>> DAGConnections { get; set; } = new List<Tuple<string, string>>();
        //DAG nodes
        private List<Tuple<string, int>> DAGNodesNumbered = new List<Tuple<string, int>>();
        private List<Tuple<string, int>> DAGNodesNumberedAddedTruthValues = new List<Tuple<string, int>>();
        private DAGNode dag;

        public DAG(DAGNode dag)
        {
            this.dag = dag;
        }

        public void PrepareDAG(TruthTree counterModel = null, bool exercise = false)
        {
            //if we need truth values use this option
            if(counterModel != null)
            PrepareDAGNodesList(this.dag, counterModel);
            else
            {
                PrepareDAGNodesList(this.dag);
            }
            //get DAGNodes 
            DAGNodes = DAGNodes.Distinct().ToList();
            //PrepareDAGNodesCOnnection
            PrepareDAGNodesListConnection(this.dag);
            //Replace this connections for their formula values
            ReplaceConnectionNumbersForString(exercise);
            //Distinct paths to not repeat
            DiscinctListOfPaths();
           

        }
        //Distinc paths so their don't repat
        private void DiscinctListOfPaths()
        {
            //to remove double arrow in case it leads to literal
            var filteredTuples = DAGConnections.Where(t =>
                (t.Item2.Length == 1 && char.IsLetter(t.Item2[0])) ||
                (t.Item2.Length >= 2 && char.IsLetter(t.Item2[0]) && t.Item2[1] == '=') ||
                (t.Item2.Length == 2 && t.Item2[0] == '¬' && char.IsLetter(t.Item2[1])) ||
                (t.Item2.Length == 3 && t.Item2[0] == '¬' && t.Item2[1] == '¬' && char.IsLetter(t.Item2[2])) ||
                (t.Item2.Length >= 2 && t.Item2[0] == '¬' && char.IsLetter(t.Item2[1]) && t.Item2[2] =='=') ||
                (t.Item2.Length == 3 && t.Item2[0] == '¬' && t.Item2[1] == '¬' && char.IsLetter(t.Item2[2])) ||
                (t.Item2.Length >= 3 && t.Item2[0] == '¬' && t.Item2[1] == '¬' && char.IsLetter(t.Item2[2]) && t.Item2[3] =='=')
            ).ToList(); 
            var distinctTuples = filteredTuples.GroupBy(t => t.Item2).Select(g => g.First()).ToList();
            var seenList = new List<Tuple<string,string>>();
            List<Tuple<string, string>> newConnections = new List<Tuple<string, string>>();
            foreach (var tuple in DAGConnections)
            {
                    if (distinctTuples.Contains(tuple))
                    {
                        if(seenList.Contains(tuple))
                        {
                        continue;
                        }
                        else
                        {
                            seenList.Add(tuple);
                        }
                    }
                    newConnections.Add(tuple);
                
            }
            DAGConnections = newConnections;
        }

        //prepare DAGNodesList add with numbers
        private void PrepareDAGNodesList(DAGNode dag)
        {
            DAGNodesNumbered.Add((new Tuple<string, int>(dag.Item.MSentence, dag.Item.number)));
            DAGNodes.Add(dag.Item.MSentence);
            if (dag.LeftChild != null)
            {
                PrepareDAGNodesList(dag.LeftChild);
                if (dag.RightChild != null)
                {
                    PrepareDAGNodesList(dag.RightChild);
                }
            }
        }

        //prepare DAGNodesList add with numbers and their truth values
        private void PrepareDAGNodesList(DAGNode tree, TruthTree truthTree)
        {
            DAGNodesNumbered.Add((new Tuple<string, int>(tree.Item.MSentence, tree.Item.number)));
            DAGNodesNumberedAddedTruthValues.Add((new Tuple<string, int>(truthTree.Item.ToString(), tree.Item.number)));
            DAGNodes.Add(tree.Item.MSentence);
            if (tree.LeftChild != null)
            {
                PrepareDAGNodesList(tree.LeftChild,truthTree.ChildNodeLeft);
                if (tree.RightChild != null)
                {
                    PrepareDAGNodesList(tree.RightChild, truthTree.ChildNodeRight);
                }
            }
        }

        //getDAgNodes connections numbered
        private void PrepareDAGNodesListConnection(DAGNode tree)
        {
            if (tree.LeftChild != null)
            {
                DAGConnectionsNumbered.Add(new Tuple<int, int>(tree.Item.number, tree.LeftChild.Item.number));
            }
            if (tree.RightChild != null)
            {
                DAGConnectionsNumbered.Add(new Tuple<int, int>(tree.Item.number, tree.RightChild.Item.number));
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

        //get this connections with string
        private void ReplaceConnectionNumbersForString(bool exercise = false)
        {
            foreach (var connection in DAGConnectionsNumbered)
            {
                var ConnectionsTuple = new Tuple<string, string>(SearchByNumber(connection.Item1), SearchByNumber(connection.Item2));
                var firstItem = AddEvaluationValuesToTuple(ConnectionsTuple.Item1,exercise);
                var secondItem = AddEvaluationValuesToTuple(ConnectionsTuple.Item2, exercise);
                ConnectionsTuple = new Tuple<string, string>(firstItem,secondItem);
                DAGConnections.Add(ConnectionsTuple);
            }
        }

        //add truth values to DAG
        private string AddEvaluationValuesToTuple(string item, bool exercise = false)
        {
            if(exercise) return item += $"= 0 ";
            List<int> result = DAGNodesNumbered.Where(t => t.Item1 == item).Select(t => t.Item2).ToList();
            result = result.Distinct().ToList();
            List<string> valueToAdd = new List<string>();
            foreach (var r in result)
            {
                valueToAdd = DAGNodesNumberedAddedTruthValues
                    .Where(tuple => tuple.Item2 == r)
                    .Select(tuple => tuple.Item1)
                    .ToList();

            }
            valueToAdd = valueToAdd.Distinct().ToList();
            foreach (var newValue in valueToAdd)
            item += $"= {newValue} ";
            return item;
           
        }

        //we searching for dag nodes by their number
        private string SearchByNumber(int number)
        {
            return DAGNodesNumbered.FirstOrDefault(t => t.Item2 == number).Item1;
        }
    }
}
