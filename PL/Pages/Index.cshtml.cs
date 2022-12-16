using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VyrokovaLogika;

namespace PL.Pages
{
    public class IndexModel : PageModel
    {
        private string vl;
        private List<string> htmlTree = new List<string>();
        public string ConvertedTree { get; set; }
        public IndexModel()
        {

        }

        public void OnPost()
        {
            vl = Request.Form["formula"];
            Engine tree1 = new Engine(vl);
            tree1.ProcessSentence();
            var tree = tree1.tree;
            PrintTree(tree);
            string div = "<div class='tf-tree tf-gap-lg'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
        }

        private void PrintTree(Tree tree)
        {
            htmlTree.Add("<li>");
            htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");

            if (tree.childNodeLeft != null)
            {
                htmlTree.Add("<ul>");
                PrintTree(tree.childNodeLeft);
                if (tree.childNodeRight != null)
                {
                    PrintTree(tree.childNodeRight);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }
    }
}