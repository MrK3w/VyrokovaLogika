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
        public List<Tuple<string, int>> distinctNodes { get; set; } = new List<Tuple<string, int>>();

        public TreeProof()
        {
        }

        public List<TruthTree> ProcessTree(Tree tree, int truthValue = 0)
        {
            List<TruthTree> treePairsFinal = new List<TruthTree>();
            List<TruthTree> listOftrees = new List<TruthTree>();
           
            if (tree.IsLeaf)
            {
                return GetLeave(tree, truthValue);
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
                TruthTree tTree;
                if (tree.childNodeLeft != null)
                {
                    var xd = ProcessTree(tree.childNodeLeft, newTree.Item1);
                    foreach (var tyhlestromy in xd)
                    {
                       tTree = new TruthTree(tyhlestromy.Item);
                        tTree.mOperator = tree.Item.mOperator;
                        //check here if treees have right and left
                            if(tyhlestromy.ChildNodeLeft != null)
                            {
                                tTree.AddChild(tyhlestromy.ChildNodeLeft, "left");
                            }
                            if(tyhlestromy.ChildNodeRight != null)
                            {
                                tTree.AddChild(tyhlestromy.ChildNodeRight, "right");
                            }
                            if(tyhlestromy.ChildNodeLeft == null && tyhlestromy.ChildNodeRight == null)
                            {
                                tTree.AddChild(tyhlestromy.Item, "left");
                                tTree.ChildNodeLeft.literal = tyhlestromy.literal;
                                tTree.ChildNodeLeft.mOperator = tyhlestromy.mOperator;
                        }
                            tempTreeListLeft.Add(tTree);
                    }


                }
                if(tree.childNodeRight != null)
                {
                    var xd = ProcessTree(tree.childNodeRight, newTree.Item2);
                    foreach (var tyhlestromy in xd)
                    {
                        tTree = new TruthTree(tyhlestromy.Item);
                        tTree.mOperator = tree.Item.mOperator;
                        //check here if treees have right and left
                        if (tyhlestromy.ChildNodeLeft != null)
                        {
                            tTree.AddChild(tyhlestromy.ChildNodeLeft, "left");
                        }
                        if(tyhlestromy.ChildNodeRight != null)
                        {
                            tTree.AddChild(tyhlestromy.ChildNodeRight, "right");
                        }
                        if (tyhlestromy.ChildNodeLeft == null && tyhlestromy.ChildNodeRight == null)
                        {
                            tTree.AddChild(tyhlestromy.Item, "right");
                            tTree.ChildNodeRight.literal = tyhlestromy.literal;
                            tTree.ChildNodeRight.mOperator = tyhlestromy.mOperator;
                        }
                        tempTreeListRight.Add(tTree);
                    }
                }

                List<TruthTree> treePairs = new List<TruthTree>();

                // Iterate through the list of trees
                for (int i = 0; i < tempTreeListLeft.Count; i++)
                {
                    for (int j = 0; j < tempTreeListRight.Count; j++)
                    {
                        var newtru = new TruthTree(truthValue);

                        if (tempTreeListRight[j].ChildNodeLeft != null && tempTreeListRight[j].ChildNodeRight == null && tempTreeListLeft[i].ChildNodeLeft
                            != null && tempTreeListLeft[i].ChildNodeRight == null)
                        {
                            newtru.AddChild(tempTreeListLeft[i], tempTreeListRight[j]);
                        }
                        if (tempTreeListRight[j].ChildNodeLeft != null && tempTreeListRight[j].ChildNodeRight != null && tempTreeListLeft[i].ChildNodeLeft
                            != null && tempTreeListLeft[i].ChildNodeRight != null)
                        {
                            newtru.AddChild(tempTreeListLeft[i], tempTreeListRight[j]);
                        }
                        if (tempTreeListRight[j].ChildNodeLeft == null && tempTreeListRight[j].ChildNodeRight != null && tempTreeListLeft[i].ChildNodeLeft
                            != null && tempTreeListLeft[i].ChildNodeRight != null)
                        {
                            newtru.AddChild(tempTreeListLeft[i], tempTreeListRight[j].ChildNodeRight);
                        }
                        if (tempTreeListRight[j].ChildNodeLeft != null && tempTreeListRight[j].ChildNodeRight == null && tempTreeListLeft[i].ChildNodeLeft
                           != null && tempTreeListLeft[i].ChildNodeRight != null)
                        {
                            newtru.AddChild(tempTreeListLeft[i], tempTreeListRight[j].ChildNodeLeft);
                        }
                        if (tempTreeListRight[j].ChildNodeLeft != null && tempTreeListRight[j].ChildNodeRight != null && tempTreeListLeft[i].ChildNodeLeft
                         == null && tempTreeListLeft[i].ChildNodeRight != null)
                        {
                            newtru.AddChild(tempTreeListLeft[i].ChildNodeRight, tempTreeListRight[j]);
                        }
                        if (tempTreeListRight[j].ChildNodeLeft != null && tempTreeListRight[j].ChildNodeRight != null && tempTreeListLeft[i].ChildNodeLeft
                        != null && tempTreeListLeft[i].ChildNodeRight == null)
                        {
                            newtru.AddChild(tempTreeListLeft[i].ChildNodeLeft, tempTreeListRight[j]);
                        }

                        if (tempTreeListRight[j].ChildNodeLeft == null && tempTreeListLeft[i].ChildNodeRight == null)
                        {
                            if (tempTreeListLeft[i].ChildNodeLeft.mOperator == Operator.OperatorEnum.NEGATION || tempTreeListLeft[i].ChildNodeLeft.mOperator == Operator.OperatorEnum.DOUBLENEGATION)
                            {
                                newtru.AddChild(tempTreeListLeft[i], tempTreeListRight[j].ChildNodeRight);
                            }
                            else if (tempTreeListRight[j].ChildNodeRight.mOperator == Operator.OperatorEnum.NEGATION || tempTreeListRight[j].ChildNodeRight.mOperator == Operator.OperatorEnum.DOUBLENEGATION)
                            {
                                newtru.AddChild(tempTreeListLeft[i].ChildNodeLeft, tempTreeListRight[j]);
                            }
                            else if ((tempTreeListRight[j].ChildNodeRight.mOperator == Operator.OperatorEnum.NEGATION || tempTreeListRight[j].ChildNodeRight.mOperator == Operator.OperatorEnum.DOUBLENEGATION)
                                && (tempTreeListLeft[i].ChildNodeLeft.mOperator == Operator.OperatorEnum.NEGATION || tempTreeListLeft[i].ChildNodeLeft.mOperator == Operator.OperatorEnum.DOUBLENEGATION))
                            {
                                newtru.AddChild(tempTreeListLeft[i], tempTreeListRight[j]);
                            }
                            else
                            {
                                newtru.AddChild(tempTreeListLeft[i].ChildNodeLeft, tempTreeListRight[j].ChildNodeRight);
                            }
                        }
                        treePairs.Add(newtru);
                    }
                    if (tempTreeListRight.Count == 0)
                    {
                        var newtru = new TruthTree(truthValue);
                        if (tempTreeListLeft[i].ChildNodeLeft != null)
                        {
                            newtru.AddChild(tempTreeListLeft[i].ChildNodeLeft, "left");
                        }
                        if (tempTreeListLeft[i].ChildNodeRight != null)
                        {
                            newtru.AddChild(tempTreeListLeft[i].ChildNodeRight, "right");
                        }
                        treePairs.Add(newtru);
                    }
                }
                treePairsFinal.AddRange(treePairs);
            }
            return treePairsFinal;
        }

        private static List<TruthTree> GetLeave(Tree tree, int truthValue)
        {
            TruthTree tTree = new TruthTree(truthValue);
            tTree.literal = tree.Item.mSentence;
            tTree.mOperator = tree.Parent.Item.mOperator;
            List<TruthTree> treeees = new List<TruthTree>();

            // Add a tuple with tTree as the first element and null as the second element to the list
            treeees.Add(tTree);

            return treeees;
        }

        public bool isTautology(List<TruthTree> Trees)
        {
            foreach (var tree in Trees)
            {
                int number = 0;
                bool contradiction = false;
                var elementalNodes = tree.GetLeafNodes();
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
                    distinctNodes = elementalNodes
            .DistinctBy(node => new { node.literal, node.Item })
            .Select(node => new Tuple<string, int>(node.literal, node.Item))
            .ToList();
                    return false;
                }
            }
            return true;
        }
    }
}
