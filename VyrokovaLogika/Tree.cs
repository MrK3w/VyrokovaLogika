using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VyrokovaLogika
{
    public class Tree
    {
        public Tree childNodeLeft;
        public Tree childNodeRight;
        public Tree Parent { get; set; }

        
        public Node Item { get; set; }
        public Tree(Node item)
        {
            Item = item;
            Item.number = 1;
        }

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public bool IsLeaf
        {
            get { return childNodeLeft == null && childNodeRight == null; }
        }

      
        public Tree GetParent(Tree tree)
        {
            if(tree.Parent != null) return GetParent(tree.Parent);
            return tree;
        }
        public Tree AddChild(Node child, string side, int number)
        {
  
            if (side == "left")
            {
                childNodeLeft = new Tree(child);
                childNodeLeft.Item.number = number;
                childNodeLeft.Parent = this;
                return childNodeLeft;
            }
            else if (side == "right")
            {
                childNodeRight = new Tree(child);
                childNodeRight.Item.number = number;
                childNodeRight.Parent = this;
                return childNodeRight;
            }
            return null;
        }

        // Method to return the leaf nodes of the tree
        public List<Tree> GetLeafNodes()
        {
            List<Tree> leafNodes = new List<Tree>();
            Traverse(this, leafNodes);
            return leafNodes;
        }

        // Private helper method to recursively traverse the tree
        private void Traverse(Tree node, List<Tree> leafNodes)
        {
            if (node == null) return;

            if (node.childNodeLeft == null && node.childNodeRight == null)
            {
                // Node has no children, so it's a leaf node
                leafNodes.Add(node);
            }
            else
            {
                // Node has children, so recursively traverse its children
                Traverse(node.childNodeLeft, leafNodes);
                Traverse(node.childNodeRight, leafNodes);
            }
        }
    }
}
