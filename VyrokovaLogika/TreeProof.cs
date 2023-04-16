using System.Data;

namespace VyrokovaLogika
{
    public class TreeProof
    {
        public List<Tuple<string, int>> distinctNodes { get; set; } = new List<Tuple<string, int>>();
        public TruthTree counterModel { get; set; } = new TruthTree(0);

        public TreeProof()
        {
        }

        public List<TruthTree> ProcessTree(Tree tree, int truthValue = 0)
        {
            List<TruthTree> combinedTrees = new List<TruthTree>();

            //if leaf return value of leaf
            if (tree.IsLeaf)
            {
                return GetLeave(tree, truthValue);
            }
            var sideValues = Rule.GetValuesOfBothSides(truthValue, tree.Item.mOperator);
            List<Tuple<int, int>> list = GetValuesFromBothSides(tree,sideValues);

            //we must iterate for each available option which can tree childs evaluate to get parent value
            foreach (var newTree in list)
            {
                List<TruthTree> currentTreeListFromLeftSide = new List<TruthTree>();
                List<TruthTree> currentTreeListFromRightSide = new List<TruthTree>();
                TruthTree currentTree;
                if (tree.childNodeLeft != null)
                {
                    var childrenTrees = ProcessTree(tree.childNodeLeft, newTree.Item1);
                    foreach (var childrenTree in childrenTrees)
                    {
                        currentTree = new TruthTree(childrenTree.Item);
                        currentTree.mOperator = childrenTree.mOperator;
                        if (childrenTree.ChildNodeLeft != null)
                        {
                            currentTree.AddChild(childrenTree.ChildNodeLeft, "left");
                        }
                        if (childrenTree.ChildNodeRight != null)
                        {
                            currentTree.AddChild(childrenTree.ChildNodeRight, "right");
                        }
                        if (childrenTree.ChildNodeLeft == null && childrenTree.ChildNodeRight == null)
                        {
                            currentTree.AddChild(childrenTree.Item, "left");
                            currentTree.ChildNodeLeft.literal = childrenTree.literal;
                            currentTree.ChildNodeLeft.mOperator = childrenTree.mOperator;
                        }
                        currentTreeListFromLeftSide.Add(currentTree);
                    }


                }
                if (tree.childNodeRight != null)
                {
                    var childrenTrees = ProcessTree(tree.childNodeRight, newTree.Item2);
                    foreach (var childrenTree in childrenTrees)
                    {
                        currentTree = new TruthTree(childrenTree.Item);
                        currentTree.mOperator = childrenTree.mOperator;
                        if (childrenTree.ChildNodeLeft != null)
                        {
                            currentTree.AddChild(childrenTree.ChildNodeLeft, "left");
                        }
                        if (childrenTree.ChildNodeRight != null)
                        {
                            currentTree.AddChild(childrenTree.ChildNodeRight, "right");
                        }
                        if (childrenTree.ChildNodeLeft == null && childrenTree.ChildNodeRight == null)
                        {
                            currentTree.AddChild(childrenTree.Item, "right");
                            currentTree.ChildNodeRight.literal = childrenTree.literal;
                            currentTree.ChildNodeRight.mOperator = childrenTree.mOperator;
                        }
                        currentTreeListFromRightSide.Add(currentTree);
                    }
                }



                // Iterate through the list of trees right and left side we need to combine them into new trees
                for (int i = 0; i < currentTreeListFromLeftSide.Count; i++)
                {
                    for (int j = 0; j < currentTreeListFromRightSide.Count; j++)
                    {
                        var newTemporaryTreeForCombining = new TruthTree(truthValue);
                        newTemporaryTreeForCombining.mOperator = tree.Item.mOperator;
                        if (currentTreeListFromRightSide[j].ChildNodeLeft != null && currentTreeListFromRightSide[j].ChildNodeRight == null && currentTreeListFromLeftSide[i].ChildNodeLeft
                            != null && currentTreeListFromLeftSide[i].ChildNodeRight == null)
                        {
                            newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i], currentTreeListFromRightSide[j]);
                        }
                        if (currentTreeListFromRightSide[j].ChildNodeLeft != null && currentTreeListFromRightSide[j].ChildNodeRight != null && currentTreeListFromLeftSide[i].ChildNodeLeft
                            != null && currentTreeListFromLeftSide[i].ChildNodeRight != null)
                        {
                            newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i], currentTreeListFromRightSide[j]);
                        }
                        if (currentTreeListFromRightSide[j].ChildNodeLeft == null && currentTreeListFromRightSide[j].ChildNodeRight != null && currentTreeListFromLeftSide[i].ChildNodeLeft
                            != null && currentTreeListFromLeftSide[i].ChildNodeRight != null)
                        {
                            newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i], currentTreeListFromRightSide[j].ChildNodeRight);
                        }
                        if (currentTreeListFromRightSide[j].ChildNodeLeft != null && currentTreeListFromRightSide[j].ChildNodeRight == null && currentTreeListFromLeftSide[i].ChildNodeLeft
                           != null && currentTreeListFromLeftSide[i].ChildNodeRight != null)
                        {
                            newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i], currentTreeListFromRightSide[j].ChildNodeLeft);
                        }
                        if (currentTreeListFromRightSide[j].ChildNodeLeft != null && currentTreeListFromRightSide[j].ChildNodeRight != null && currentTreeListFromLeftSide[i].ChildNodeLeft
                         == null && currentTreeListFromLeftSide[i].ChildNodeRight != null)
                        {
                            newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i].ChildNodeRight, currentTreeListFromRightSide[j]);
                        }
                        if (currentTreeListFromRightSide[j].ChildNodeLeft != null && currentTreeListFromRightSide[j].ChildNodeRight != null && currentTreeListFromLeftSide[i].ChildNodeLeft
                        != null && currentTreeListFromLeftSide[i].ChildNodeRight == null)
                        {
                            newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i].ChildNodeLeft, currentTreeListFromRightSide[j]);
                        }

                        if (currentTreeListFromRightSide[j].ChildNodeLeft == null && currentTreeListFromLeftSide[i].ChildNodeRight == null)
                        {
                            if (currentTreeListFromLeftSide[i].ChildNodeLeft.mOperator == Operator.OperatorEnum.NEGATION || currentTreeListFromLeftSide[i].ChildNodeLeft.mOperator == Operator.OperatorEnum.DOUBLENEGATION)
                            {
                                newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i], currentTreeListFromRightSide[j].ChildNodeRight);
                            }
                            else if (currentTreeListFromRightSide[j].ChildNodeRight.mOperator == Operator.OperatorEnum.NEGATION || currentTreeListFromRightSide[j].ChildNodeRight.mOperator == Operator.OperatorEnum.DOUBLENEGATION)
                            {
                                newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i].ChildNodeLeft, currentTreeListFromRightSide[j]);
                            }
                            else if ((currentTreeListFromRightSide[j].ChildNodeRight.mOperator == Operator.OperatorEnum.NEGATION || currentTreeListFromRightSide[j].ChildNodeRight.mOperator == Operator.OperatorEnum.DOUBLENEGATION)
                                && (currentTreeListFromLeftSide[i].ChildNodeLeft.mOperator == Operator.OperatorEnum.NEGATION || currentTreeListFromLeftSide[i].ChildNodeLeft.mOperator == Operator.OperatorEnum.DOUBLENEGATION))
                            {
                                newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i], currentTreeListFromRightSide[j]);
                            }
                            else
                            {
                                newTemporaryTreeForCombining.AddChild(currentTreeListFromLeftSide[i].ChildNodeLeft, currentTreeListFromRightSide[j].ChildNodeRight);
                            }
                        }
                        combinedTrees.Add(newTemporaryTreeForCombining);
                    }
                    //if three don't have right side
                    if (currentTreeListFromRightSide.Count == 0)
                    {
                        var newTemporaryTree = new TruthTree(truthValue);
                        newTemporaryTree.mOperator = tree.Item.mOperator;
                        if (newTemporaryTree.mOperator != Operator.OperatorEnum.NEGATION)
                        {
                            if (currentTreeListFromLeftSide[i].ChildNodeLeft != null)
                            {
                                newTemporaryTree.AddChild(currentTreeListFromLeftSide[i].ChildNodeLeft, "left");
                            }
                            if (currentTreeListFromLeftSide[i].ChildNodeRight != null)
                            {
                                newTemporaryTree.AddChild(currentTreeListFromLeftSide[i].ChildNodeRight, "right");
                            }
                        }
                        else
                        {
                            newTemporaryTree.AddChild(currentTreeListFromLeftSide[i].ChildNodeLeft, "left");

                        }
                        combinedTrees.Add(newTemporaryTree);
                    }
                }
                //treePairsFinal.AddRange(combinedTrees);
            }
            return combinedTrees;
        }

        private List<Tuple<int, int>> GetValuesFromBothSides(Tree tree,List<(int, int)> sideValues)
        {
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
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
                list.Add(new Tuple<int, int>(leftSide, rightSide));
            }
            return list;
        }

        private static List<TruthTree> GetLeave(Tree tree, int truthValue)
        {
            TruthTree leafTree = new TruthTree(truthValue);
            leafTree.literal = tree.Item.mSentence;
            if(tree.Parent != null)
            leafTree.mOperator = tree.Parent.Item.mOperator;
            List<TruthTree> treeees = new List<TruthTree>();
            return new List<TruthTree> { leafTree};
        }

        public bool FindContradiction(List<TruthTree> Trees)
        {
            List<TruthTree> elementalNodes = new List<TruthTree>();
            foreach (var tree in Trees)
            {
                int number = 0;
                bool contradiction = false;
                elementalNodes = tree.GetLeafNodes();
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
                    counterModel = tree;
                    return false;

                }
            }
            counterModel = Trees.LastOrDefault();
            distinctNodes = elementalNodes.
                DistinctBy(node => new { node.literal, node.Item })
          .Select(node => new Tuple<string, int>(node.literal, node.Item))
          .ToList();
            return true;
        }
    }
}
