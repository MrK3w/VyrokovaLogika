using System.Data;

namespace VyrokovaLogika
{
    public class TreeProof
    {
        public List<Tuple<string, int>> DistinctNodes { get; set; } = new List<Tuple<string, int>>();
        public TruthTree CounterModel { get; set; } = new TruthTree(0);

        public bool Green { get; set; }
        public TreeProof()
        {
        }

        public List<TruthTree> ProcessTree(Tree tree, int truthValue = 0)
        {
            List<TruthTree> combinedTrees = new();

            //if leaf return value of leaf
            if (tree.IsLeaf)
            {
                return GetLeave(tree, truthValue);
            }
            var sideValues = Rule.GetValuesOfBothSides(truthValue, tree.Item.MOperator);
            List<Tuple<int, int>> listOfTruthValues = GetValuesFromBothSides(tree,sideValues);

            //we must iterate for each available option which can tree childs evaluate to get parent value
            foreach (var truthValues in listOfTruthValues)
            {
                //create new list for left and right side
                List<TruthTree> currentTreeListFromLeftSide = new();
                List<TruthTree> currentTreeListFromRightSide = new();
                //if tree has childNodeLeft or childNodeRight do recursion for this tree
                if (tree.childNodeLeft != null)
                {
                    currentTreeListFromLeftSide = ProcessTree(tree.childNodeLeft, truthValues.Item1);
                }
                if (tree.childNodeRight != null) {
                    currentTreeListFromRightSide = ProcessTree(tree.childNodeRight, truthValues.Item2);
                }
                //connect right and left trees this dont iterate if we dont have any of this lists
                for (int i = 0; i < currentTreeListFromLeftSide.Count; i++) {
                    for (int j = 0; j < currentTreeListFromRightSide.Count; j++)
                    {
                        //create new tempTree where we store trees
                        TruthTree tempTree = new();
                        //if tree is root take value from parameter
                        if (tree.IsRoot) tempTree.Item = truthValue;
                        //set parameter on tree
                        tempTree.MOperator = tree.Item.MOperator;
                        //set values of trees based on truthValues, which we get from Rule
                        currentTreeListFromLeftSide[i].Item = truthValues.Item1;
                        currentTreeListFromRightSide[j].Item = truthValues.Item2;
                        //add to tempTree his tree childs
                        tempTree.AddChild(currentTreeListFromLeftSide[i], currentTreeListFromRightSide[j]);
                        //add this childs to combinedTree list
                        combinedTrees.Add(tempTree);
                    }
                }
                //if tree don't have right side to the same 
                if(currentTreeListFromRightSide.Count == 0)
                {
                    for (int i = 0; i < currentTreeListFromLeftSide.Count; i++)
                    {
                        TruthTree tempTree = new();
                        if (tree.IsRoot) tempTree.Item = truthValue;
                        tempTree.MOperator = tree.Item.MOperator;
                        currentTreeListFromLeftSide[i].Item = truthValues.Item1;
                        tempTree.AddChild(currentTreeListFromLeftSide[i], "left");
                        combinedTrees.Add(tempTree);
                    }
                }
                //if tree don't have left side to the same 
                if (currentTreeListFromLeftSide.Count == 0)
                {
                    for (int i = 0; i < currentTreeListFromRightSide.Count; i++)
                    {
                        TruthTree tempTree = new();
                        if (tree.IsRoot) tempTree.Item = truthValue;
                        currentTreeListFromRightSide[i].Item = truthValues.Item2;
                        tempTree.MOperator = tree.Item.MOperator;
                        tempTree.AddChild(currentTreeListFromRightSide[i], "right");
                        combinedTrees.Add(tempTree);
                    }
                }
            }
            return combinedTrees;
        }

        //get values for both sides based on side values
        private static List<Tuple<int, int>> GetValuesFromBothSides(Tree tree,List<(int, int)> sideValues)
        {
            List<Tuple<int, int>> list = new();
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

        //get leaf in tree
        private static List<TruthTree> GetLeave(Tree tree, int truthValue)
        {
            TruthTree leafTree = new(truthValue)
            {
                literal = tree.Item.MSentence
            };
            List<TruthTree> treeees = new List<TruthTree>();
            return new List<TruthTree> { leafTree};
        }

        public bool FindContradiction(List<TruthTree> Trees)
        {
            List<TruthTree> elementalNodes = new();
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
                    DistinctNodes = elementalNodes
            .DistinctBy(node => new { node.literal, node.Item })
            .Select(node => new Tuple<string, int>(node.literal, node.Item))
            .ToList();
                    CounterModel = tree;
                    return false;

                }
            }
            CounterModel = Trees.LastOrDefault();
            DistinctNodes = elementalNodes.
                DistinctBy(node => new { node.literal, node.Item })
          .Select(node => new Tuple<string, int>(node.literal, node.Item))
          .ToList();
            return true;
        }
    }
}
