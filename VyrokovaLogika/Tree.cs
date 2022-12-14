using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class Tree<T> : IEnumerable<Tree<T>>
    {
        List<Tree<T>> childNode = new List<Tree<T>>();

        public Tree<T> Parent { get; set; }

        bool root = true;

        public T Item { get; set; }
        public Tree(T item)
        {
            Item = item;
        }

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public bool IsLeaf
        {
            get { return childNode.Count == 0; }
        }

        public Tree<T> AddChild(T child)
        {
            Tree<T> childNode = new Tree<T>(child);
            childNode.Parent = this;
            if (childNode.Parent != null)
            {
                root = false;
            }
            this.childNode.Add(childNode);
            return childNode;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<string> getFinal()
        {
            List<string> list = new List<string>();
           
            foreach (var directChild in childNode)
            {
                foreach (var anyChild in directChild)
                {
                    Node? node = anyChild.Item as Node;
                }
            }
            return list;
        }

        public bool validateTree()
        {

            return true;
        }

        public IEnumerator<Tree<T>> GetEnumerator()
        {
            yield return this;
            for (int i = 0; i < childNode.Count; i++)
            {
                Tree<T>? directChild = childNode[i];
                foreach (var anyChild in directChild)
                {
                    Node? node = anyChild.Item as Node;
                    yield return anyChild;
                }
            }
        }

        public List<Node> ReturnNode(int level)
        {
            List<Node> list = new List<Node>();
            if ((Item as Node).level == level) list.Add(Item as Node);
            foreach(var directChild in childNode)
            {
                foreach (var anyChild in directChild)
                {
                    Node? node = anyChild.Item as Node;
                    if (node.level == level) list.Add(node);
                }
            }
            return list;
        }
    }
}
