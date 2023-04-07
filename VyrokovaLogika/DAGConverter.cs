namespace VyrokovaLogika
{
    class ASTtoDAGConverter
    {
        Dictionary<string, DAGNode> seen = new Dictionary<string, DAGNode>();

        public DAGNode Convert(Tree root)
        {
            return ReplaceSubtreeWithDAG(root);
        }

        string ComputeHash(Tree node)
        {
            if (node.childNodeLeft == null && node.childNodeRight == null)
            {
                return node.Item.mSentence;
            }
            string leftHash = node.childNodeLeft == null ? "" : ComputeHash(node.childNodeLeft);
            string rightHash = node.childNodeRight == null ? "" : ComputeHash(node.childNodeRight);
            return $"{node.Item.mSentence}{leftHash}{rightHash}";
        }

        DAGNode ReplaceSubtreeWithDAG(Tree node)
        {
            if (node.childNodeLeft == null && node.childNodeRight == null)
            {
                return new DAGNode { Item = node.Item };
            }
            string subtreeHash = ComputeHash(node);
            if (seen.TryGetValue(subtreeHash, out DAGNode dagNode))
            {
                dagNode.RefCounts.Add(dagNode.RefCounts.Count);
                return dagNode;
            }
            dagNode = new DAGNode { Item = node.Item };
            seen[subtreeHash] = dagNode;
            if(node.childNodeLeft != null)
            { 
                dagNode.LeftChild = ReplaceSubtreeWithDAG(node.childNodeLeft);
            }
            if (node.childNodeRight != null)
            {
                dagNode.RightChild = ReplaceSubtreeWithDAG(node.childNodeRight);
            }
            dagNode.RefCounts = Enumerable.Repeat(1, 2).ToList();
            return dagNode;
        }
    }
}