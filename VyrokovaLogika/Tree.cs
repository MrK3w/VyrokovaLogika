using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class Tree
    {
        string mSentence = string.Empty;
        Node mainNode;
        public Tree(string sentence)
        {
            mSentence = sentence;
        }

        public void Process()
        {
           //create first main node of this tree
           mainNode = new Node(mSentence);
           mainNode.Process();
        }
    }
}
