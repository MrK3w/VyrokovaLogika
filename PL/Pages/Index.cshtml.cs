using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public List<SelectListItem> listItems { get;set; }

        SelectListItem item1 = new SelectListItem("-(-a>-B)|-B", "-(-a>-B)|-B");
        SelectListItem item2 = new SelectListItem("(((-x|b)&(x|a)) | (x&B)) >((a|b)&(b&c))", "(((-x|b)&(x|a)) | (x&B)) >((a|b)&(b&c))");
     
        public IndexModel()
        {
            listItems = new List<SelectListItem>();
            listItems.Add(item1);
            listItems.Add(item2);
        }

        public void OnPost()
        {
            vl = Request.Form["formula"];
            if (vl == "") return;
            htmlTree.Clear();
            foreach(var item in listItems)
            {
                item.Selected = false;
                if (vl == item.Value) item.Selected = true;
            }
            
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