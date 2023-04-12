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

        public void ProcessTree(Tree tree, TruthTree truthTree = null)
        {
            List<TruthTree> TempTrees = new List<TruthTree>();
            if (tree.IsRoot)
            {
                truthTree = new TruthTree(0);
            }
            var sideValues = Rule.GetValuesOfBothSides(truthTree.Item, tree.Item.mOperator);
            //it's just a copy of previous tree must be redone
            foreach (var sideValue in sideValues)
            {
                //TruthTree newTree = truthTree.Clone();
                if(tree.childNodeLeft != null)
                {
                    newTree.AddChild(sideValue.Item1, "left");   
                }
                if (tree.childNodeRight != null)
                {
                    newTree.AddChild(sideValue.Item2, "right");
                }
                TempTrees.Add(newTree);
            }
            
            foreach (var newTree in TempTrees)
            {
                if (tree.childNodeLeft != null)
                {
                    ProcessTree(tree.childNodeLeft, newTree.ChildNodeLeft);
                }
                if(tree.childNodeRight != null)
                {
                    ProcessTree(tree.childNodeRight, newTree.ChildNodeRight);
                }
               
            }
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
