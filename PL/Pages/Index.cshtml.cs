using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VyrokovaLogika;

namespace PL.Pages
{
    public class IndexModel : PageModel
    {
        private string vl;
        private string vl1;
        private List<string> htmlTree = new List<string>();
        public string ConvertedTree { get; set; }
        public string TautologyDecision { get; set; }

        public List<SelectListItem> listItems { get;set; }  = new List<SelectListItem>();

        public IndexModel()
        {
            string mPropositionalSentence = "(B > A) > A";
            Converter.ConvertSentence(ref mPropositionalSentence);

            string mPropositionalSentence1 = "(((-x|b)&(x|a)) | (x&B)) >((a|b)&(b&c))";
            Converter.ConvertSentence(ref mPropositionalSentence1);

            string mPropositionalSentence2 = "((((-x|b)&(x|a))))";
            Converter.ConvertSentence(ref mPropositionalSentence2);

            SelectListItem item1 = new SelectListItem(mPropositionalSentence, mPropositionalSentence);
            SelectListItem item2 = new SelectListItem(mPropositionalSentence1, mPropositionalSentence1);
            SelectListItem item3 = new SelectListItem(mPropositionalSentence2, mPropositionalSentence2);
            listItems.Add(item1);
            listItems.Add(item2);
            listItems.Add(item3);
        }

        public void OnPost()
        {
            vl = Request.Form["formula"];
            vl1 = Request.Form["UserInput"];
            Engine engine = new Engine("");
            if (vl == "" && vl1 == "") return;
            htmlTree.Clear();

            if (vl1 != "")
            {
                listItems.Add(new SelectListItem(vl1, vl1));
                var selected = listItems.Where(x => x.Value == vl1).First();
                selected.Selected = true;
                engine = new Engine(vl1);
            }

            else if (vl != "")
            {
                foreach (var item in listItems)
                {
                    item.Selected = false;
                    if (vl == item.Value) item.Selected = true;
                }
                engine = new Engine(vl);
            }
           
            engine.ProcessSentence();
            var tree = engine.tree;
            PrintTree(tree);
            string div = "<div class='tf-tree tf-gap-lg'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            if (engine.Tautology)
            {
                TautologyDecision = "Propositional sentence is Tautology";
            }
            else TautologyDecision = "Propositional sentence is not Tautology";
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