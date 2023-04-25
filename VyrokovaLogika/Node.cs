using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VyrokovaLogika
{
    public class Node
    {
        //formula of node
        public string MSentence { get; set; }
        //unique number of node
        public int number;
        //Operator of node
        public Operator.OperatorEnum MOperator { get; set; }
        //current level in tree
        public int level;

        public Node(string sentence, int level = 1)
        {
            MSentence = sentence;
            this.level = level;
        }
    }
}
