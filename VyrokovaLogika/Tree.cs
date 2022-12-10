using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class Tree<T> : IEnumerable<Tree<T>>
    {
        List<Tree<T>> childNode = new List<Tree<T>>();
        public T Item { get; set; }
        public Tree(T item)
        {
            Item = item;
        }

        public Tree<T> AddChild(T item)
        {
            Tree<T> nodeItem = new Tree<T>(item);
            childNode.Add(nodeItem);
            return nodeItem;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Tree<T>> GetEnumerator()
        {
            yield return this;
            for (int i = 0; i < childNode.Count; i++)
            {
                Tree<T>? directChild = childNode[i];
                foreach (var anyChild in directChild)
                {
                    Node node = anyChild.Item as Node;
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
                    Node node = anyChild.Item as Node;
                    if (node.level == level) list.Add(node);
                }
            }
            return list;
        }
    }
}
