namespace VyrokovaLogika
{
    public class DAGNode
    {
        public Node Item { get; set; }
        public DAGNode LeftChild { get; set; }
        public DAGNode RightChild { get; set; }
    }
}