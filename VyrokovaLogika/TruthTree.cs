using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class TruthTree
    {
        public TruthTree ChildNodeLeft;
        public TruthTree ChildNodeRight;
        public TruthTree Parent { get; set; }

        public string literal;
        public int Item { get; set; }
        public TruthTree(int item)
        {
            Item = item;
        }

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public bool IsLeaf
        {
            get { return ChildNodeLeft == null && ChildNodeRight == null; }
        }



        public TruthTree GetParent(TruthTree tree)
        {
            if (tree.Parent != null) return GetParent(tree.Parent);
            return tree;
        }
        public TruthTree AddChild(int child, string side)
        {

            if (side == "left")
            {
                ChildNodeLeft = new TruthTree(child);
                ChildNodeLeft.Parent = this;
                return ChildNodeLeft;
            }
            else if (side == "right")
            {
                ChildNodeRight = new TruthTree(child);
                ChildNodeRight.Parent = this;
                return ChildNodeRight;
            }
            return null;
        }

        // Method to return the leaf nodes of the tree
        public List<TruthTree> GetLeafNodes()
        {
            List<TruthTree> leafNodes = new List<TruthTree>();
            Traverse(this, leafNodes);
            return leafNodes;
        }

        // Private helper method to recursively traverse the tree
        private void Traverse(TruthTree node, List<TruthTree> leafNodes)
        {
            if (node == null) return;

            if (node.ChildNodeLeft == null && node.ChildNodeRight == null)
            {
                // Node has no children, so it's a leaf node
                leafNodes.Add(node);
            }
            else
            {
                // Node has children, so recursively traverse its children
                Traverse(node.ChildNodeLeft, leafNodes);
                Traverse(node.ChildNodeRight, leafNodes);
            }
        }

        public TruthTree Clone()
        {
            TruthTree clonedTree = new TruthTree(this.Item);

            if (ChildNodeLeft != null)
            {
                clonedTree.ChildNodeLeft = ChildNodeLeft.Clone();
                clonedTree.ChildNodeLeft.Parent = clonedTree;
            }

            if (ChildNodeRight != null)
            {
                clonedTree.ChildNodeRight = ChildNodeRight.Clone();
                clonedTree.ChildNodeRight.Parent = clonedTree;
            }

            // The cloned tree will have the same parent as the original tree
            clonedTree.Parent = this.Parent;

            return clonedTree;
        }
    }
}

