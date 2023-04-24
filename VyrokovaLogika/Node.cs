using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VyrokovaLogika
{
    public class Node
    {
        public string MSentence { get; set; }
        public int number;
        public Operator.OperatorEnum MOperator { get; set; }
        public int level;

        public Node(string sentence, int level = 1)
        {
            MSentence = sentence;
            this.level = level;
        }
    }
}
