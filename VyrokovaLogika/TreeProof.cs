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
        List<Tree> Trees = new List<Tree>();
        public TreeProof()
        {
        }

        public void ProcessTree(Tree tree)
        {
            List<Tree> TempTrees = new List<Tree>();
            if (tree.IsRoot)
            {
                tree.Item.valueMustBe = 0;
            }
            if(tree.IsLeaf)
            {
                return;
            }    
            var sideValues = Rule.GetValuesOfBothSides(tree.Item.valueMustBe, tree.Item.mOperator);
            //it's just a copy of previous tree must be redone
            foreach (var sideValue in sideValues)
            {
                Tree newTree = tree.Clone();
                if(newTree.childNodeLeft != null)
                {
                    newTree.childNodeLeft.Item.valueMustBe = sideValue.Item1;    
                }
                if (newTree.childNodeRight != null)
                {
                    newTree.childNodeRight.Item.valueMustBe = sideValue.Item2;
                }
                TempTrees.Add(newTree);
            }
            foreach (var newTree in TempTrees)
            {
                var parent = newTree.GetParent(newTree);
                Trees.Add(parent); 
                if (newTree.childNodeLeft != null)
                {
                    ProcessTree(newTree.childNodeLeft);
                }
                if(newTree.childNodeRight != null)
                {
                    ProcessTree(newTree.childNodeRight);
                }
            }
        }

        public bool isTautology()
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
                        if (node1.Item.mSentence == node.Item.mSentence && node1.Item.valueMustBe != node.Item.valueMustBe)
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
