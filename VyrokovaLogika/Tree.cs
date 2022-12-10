using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class Tree<T>
    {
        List<Tree<T>> childNode = new List<Tree<T>>();
        T Item { get; set; }
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
    }
}
