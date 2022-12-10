using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VyrokovaLogika
{
    public class Node
    {
        public string mSentence { get; set; }
        public Operator.OperatorEnum mOperator { get; set; }
        int upperEquals;

        public Node(string sentence)
        {
            mSentence = sentence;

        }
    }
}
