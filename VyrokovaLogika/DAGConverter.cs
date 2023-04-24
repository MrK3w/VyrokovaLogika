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
            //We arre looking into dictionary if we didn't already seen this node, 
            //if yes return that node and don't continue
            if (seen.TryGetValue(node.Item.MSentence, out DAGNode dagNode))
            {
                return dagNode;
            }
            //we set content of this node to item of tree
            dagNode = new DAGNode { Item = node.Item };
            //we add node to dictionary
            seen[node.Item.MSentence] = dagNode;
            //if it is leaf return new node otherwise we will use recursion
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