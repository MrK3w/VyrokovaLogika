using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VyrokovaLogika
{
    public class TreeProof
    {
        List<TruthTree> Trees = new List<TruthTree>();
        public TreeProof()
        {
        }

        public List<Tuple<TruthTree,TruthTree>> ProcessTree(Tree tree, int truthValue = 0)
        {
            List<TruthTree> listOftrees = new List<TruthTree>();
            if(tree.IsLeaf)
            {
                TruthTree tTree = new TruthTree(truthValue);
                List<Tuple<TruthTree, TruthTree>> treeees = new List<Tuple<TruthTree, TruthTree>>();

                // Add a tuple with tTree as the first element and null as the second element to the list
                treeees.Add(new Tuple<TruthTree, TruthTree>(tTree, null));

                return treeees;
            }
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            var sideValues = Rule.GetValuesOfBothSides(truthValue, tree.Item.mOperator);
            foreach (var sideValue in sideValues)
            {
                int leftSide = 0;
                int rightSide = 0;
                if (tree.childNodeLeft != null)
                {
                    leftSide = sideValue.Item1;  
                }
                if (tree.childNodeRight != null)
                {
                    rightSide = sideValue.Item2;
                }
                list.Add(new Tuple<int,int>(leftSide, rightSide));
            }

            foreach (var newTree in list)
            {
                List<TruthTree> tempTreeListLeft = new List<TruthTree> ();
                List<TruthTree> tempTreeListRight = new List<TruthTree>();
                TruthTree tTree = new TruthTree(truthValue);
                if (tree.childNodeLeft != null)
                {
                    var xd = ProcessTree(tree.childNodeLeft, newTree.Item1);
                    foreach (var tyhlestromy in xd)
                    {
                        tTree = new TruthTree(truthValue);
                        if (tyhlestromy.Item1 != null)
                        {
                            tTree.AddChild(tyhlestromy.Item1.Item, "left");
                        }
                        if (tyhlestromy.Item2 != null)
                        {
                            tTree.AddChild(tyhlestromy.Item2.Item, "right");
                        }

                        tempTreeListLeft.Add(tTree);
                    }


                }
                if(tree.childNodeRight != null)
                {
                    var xd = ProcessTree(tree.childNodeRight, newTree.Item2);
                    foreach (var tyhlestromy in xd)
                    {
                        tTree = new TruthTree(truthValue);
                        if (tyhlestromy.Item1 != null)
                        {
                            tTree.AddChild(tyhlestromy.Item1.Item, "left");
                        }
                        if (tyhlestromy.Item2 != null)
                        {
                            tTree.AddChild(tyhlestromy.Item2.Item, "right");
                        }

                        tempTreeListRight.Add(tTree);
                    }
                }

                List<Tuple<TruthTree, TruthTree>> treePairs = new List<Tuple<TruthTree, TruthTree>>();

                // Iterate through the list of trees
                for (int i = 0; i < tempTreeListLeft.Count; i++)
                {
                    for (int j = 0; j < tempTreeListRight.Count; j++)
                    {
                        // Create a tuple with the current pair of trees
                        Tuple<TruthTree, TruthTree> treePair = new Tuple<TruthTree, TruthTree>(tempTreeListLeft[i], tempTreeListRight[j]);

                        // Add the tuple to the list of tree pairs
                        treePairs.Add(treePair);
                    }
                }
                return treePairs;
            }
            return null;
        }

        public bool isTautology()
        {
            foreach (var tree in Trees)
            {
                int number = 0;
                bool contradiction = false;
                var treeParent = tree.GetParent(tree);
                var elementalNodes = treeParent.GetLeafNodes();

                foreach (var node in elementalNodes)
                {
                    number++;
                    if (contradiction) break;
                    foreach (var node1 in elementalNodes)
                    {
                        if (node1.literal == node.literal && node1.Item != node.Item)
                            contradiction = true;
                    }
                    }
                if (contradiction == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
