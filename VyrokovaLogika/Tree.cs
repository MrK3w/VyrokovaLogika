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

        bool root = true;

        public Node Item { get; set; }
        public Tree(Node item)
        {

            Item = item;
        }

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public bool IsLeaf
        {
            get { return childNodeLeft == null && childNodeRight == null; }
        }

        public Tree AddChild(Node child, string side)
        {
            if (side == "left")
            {
                childNodeLeft = new Tree(child);
                childNodeLeft.Parent = this;
                return childNodeLeft;
            }
            else if (side == "right")
            {
                childNodeRight = new Tree(child);
                childNodeRight.Parent = this;
                return childNodeRight;
            }
            return null;
        }
        public List<string> getFinal()
        {
            List<string> list = new List<string>();
           
            //foreach (var directChild in childNode)
            //{
            //    foreach (var anyChild in directChild)
            //    {
            //        Node? node = anyChild.Item as Node;
            //    }
            //}
            return list;
        }

        public bool validateTree()
        {

            return true;
        }


        public List<Node> ReturnNode(int level)
        {
            List<Node> list = new List<Node>();
            //if ((Item as Node).level == level) list.Add(Item as Node);
            //foreach(var directChild in childNode)
            //{
            //    foreach (var anyChild in directChild)
            //    {
            //        Node? node = anyChild.Item as Node;
            //        if (node.level == level) list.Add(node);
            //    }
            //}
            return list;
        }
    }
}
