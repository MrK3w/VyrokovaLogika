using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class TruthTree
    {
        public TruthTree ChildNodeLeft;
        public TruthTree ChildNodeRight;
        public TruthTree Parent { get; set; }

        public Operator.OperatorEnum mOperator { get; set; }
        public string literal;
        public int Item { get; set; }
        public bool invalid = false;
        public int number = 0;
        public bool contradiction = false;
        public TruthTree(int item)
        {
            Item = item;
        }

        public TruthTree()
        {

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

        public TruthTree AddChild(TruthTree child, string side)
        {

            if (side == "left")
            {
                ChildNodeLeft = child;
                ChildNodeLeft.Parent = this;
                return ChildNodeLeft;
            }
            else if (side == "right")
            {
                ChildNodeRight = child;
                ChildNodeRight.Parent = this;
                return ChildNodeRight;
            }
            return null;
        }

        public void AddChild(TruthTree childNodeLeft, TruthTree childNodeRight)
        {

            ChildNodeLeft = childNodeLeft;
            ChildNodeRight = childNodeRight;
            ChildNodeLeft.Parent = this;
            ChildNodeRight.Parent = this;
        }

        // Method to return the leaf nodes of the tree
        public List<TruthTree> GetLeafNodes()
        {
            List<TruthTree> leafNodes = new List<TruthTree>();
            Traverse(this, leafNodes);
            return leafNodes;
        }

        public void MarkContradiction(string searchedLiteral)
        {
            Traverse(this,null, searchedLiteral);
        }

        // Private helper method to recursively traverse the tree
        private void Traverse(TruthTree node, List<TruthTree> leafNodes = null, string searchedLiteral = null)
        {
            if (node == null) return;

            if (node.ChildNodeLeft == null && node.ChildNodeRight == null)
            {
                if(leafNodes != null)
                // Node has no children, so it's a leaf node
                leafNodes.Add(node);
                if(searchedLiteral != null)
                {
                    if(searchedLiteral == node.literal) node.invalid = true;
                }

            }
            else
            {
                // Node has children, so recursively traverse its children
                Traverse(node.ChildNodeLeft, leafNodes,searchedLiteral);
                Traverse(node.ChildNodeRight, leafNodes, searchedLiteral);
            }
        }

        internal TruthTree AddChild(string side)
        {
            var tree = new TruthTree();
            if(side == "left")
            {
                tree.Parent = this;
                this.ChildNodeLeft = tree;
            }
            else
            {
                tree.Parent = this;
                this.ChildNodeRight = tree;
            }
            return tree;
           
        }

        public void AddNumbers(Tree tree)
        {
            AddNumbers(tree, this);
        }

        private TruthTree AddNumbers(Tree tree, TruthTree truthTree)
        {
            if(tree.IsLeaf)
            {
               truthTree.number = tree.Item.number;
               return truthTree;
            }
            if (tree.childNodeLeft != null) AddNumbers(tree.childNodeLeft, truthTree.ChildNodeLeft);
            if(tree.childNodeRight != null) AddNumbers(tree.childNodeRight, truthTree.ChildNodeRight);
            truthTree.number = tree.Item.number;
            return truthTree;
        }
    }
}

