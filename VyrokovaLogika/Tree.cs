using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
    }
}
