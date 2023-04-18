namespace VyrokovaLogika
{
    class ASTtoDAGConverter
    {
        Dictionary<string, DAGNode> seen = new Dictionary<string, DAGNode>();

        public DAGNode Convert(Tree root)
        {
            return ReplaceSubtreeWithDAG(root);
        }



        DAGNode ReplaceSubtreeWithDAG(Tree node)
        {
            if (seen.TryGetValue(node.Item.mSentence, out DAGNode dagNode))
            {
                return dagNode;
            }
            dagNode = new DAGNode { Item = node.Item };
            seen[node.Item.mSentence] = dagNode;
            if (node.childNodeLeft == null && node.childNodeRight == null)
            {
                return new DAGNode { Item = node.Item };
            }
          

            if(node.childNodeLeft != null)
            { 
                dagNode.LeftChild = ReplaceSubtreeWithDAG(node.childNodeLeft);
            }
            if (node.childNodeRight != null)
            {
                dagNode.RightChild = ReplaceSubtreeWithDAG(node.childNodeRight);
            }
            return dagNode;
        }
    }
}