using System.Data;

namespace VyrokovaLogika
{
    public class TreeProof
    {
        public List<Tuple<string, int>> distinctNodes { get; set; } = new List<Tuple<string, int>>();
        public TruthTree counterModel { get; set; } = new TruthTree(0);

        public bool green { get; set; }
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
            List<Tuple<int, int>> listOfTruthValues = GetValuesFromBothSides(tree,sideValues);

            //we must iterate for each available option which can tree childs evaluate to get parent value
            foreach (var truthValues in listOfTruthValues)
            {
                List<TruthTree> currentTreeListFromLeftSide = new List<TruthTree>();
                List<TruthTree> currentTreeListFromRightSide = new List<TruthTree>();
                TruthTree currentTree;
                if (tree.childNodeLeft != null)
                {
                    currentTreeListFromLeftSide = ProcessTree(tree.childNodeLeft, truthValues.Item1);
                }
                if (tree.childNodeRight != null) {
                    currentTreeListFromRightSide = ProcessTree(tree.childNodeRight, truthValues.Item2);
                }
                for (int i = 0; i < currentTreeListFromLeftSide.Count(); i++) {
                    for (int j = 0; j < currentTreeListFromRightSide.Count(); j++)
                    {
                        TruthTree tempTree = new TruthTree();
                        tempTree.mOperator = tree.Item.mOperator;
                        currentTreeListFromLeftSide[i].Item = truthValues.Item1;
                        currentTreeListFromRightSide[j].Item = truthValues.Item2;
                        tempTree.AddChild(currentTreeListFromLeftSide[i], currentTreeListFromRightSide[j]);
                        combinedTrees.Add(tempTree);
                    }
                }
                if(currentTreeListFromRightSide.Count() == 0)
                {
                    for (int i = 0; i < currentTreeListFromLeftSide.Count(); i++)
                    {
                        TruthTree tempTree = new TruthTree();
                        tempTree.mOperator = tree.Item.mOperator;
                        currentTreeListFromLeftSide[i].Item = truthValues.Item1;
                        tempTree.AddChild(currentTreeListFromLeftSide[i], "left");
                        combinedTrees.Add(tempTree);
                    }
                }

                if (currentTreeListFromRightSide.Count() == 0)
                {
                    for (int i = 0; i < currentTreeListFromRightSide.Count(); i++)
                    {
                        TruthTree tempTree = new TruthTree();
                        currentTreeListFromRightSide[i].Item = truthValues.Item2;
                        tempTree.mOperator = tree.Item.mOperator;
                        tempTree.AddChild(currentTreeListFromRightSide[i], "right");
                        combinedTrees.Add(tempTree);
                    }
                }
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
