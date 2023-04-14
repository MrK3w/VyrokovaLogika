using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VyrokovaLogika;
using static System.Net.Mime.MediaTypeNames;

public static class ListItemsHelper
{
    private static List<SelectListItem> _listItems;

    public static List<SelectListItem> ListItems
    {
        get
        {
            if (_listItems == null)
            {
                // Initialize _listItems here, e.g. from a database or other data source
                _listItems = new List<SelectListItem>();
                SetListItems();
            }
            return _listItems;
        }
    }
    public static void SetListItems(string mSentence = null)
    {
        if (_listItems.Count == 0)
        {
            string mPropositionalSentence = "(p>q)≡(-q>-p)";
            Converter.ConvertLogicalOperators(ref mPropositionalSentence);


            string mPropositionalSentence1 = "(((-x|b)&(x|a)) | (x&B)) >((a|b)&(b&c))";
            Converter.ConvertLogicalOperators(ref mPropositionalSentence1);

            string mPropositionalSentence2 = "(-x|b)&(x|a)";
            Converter.ConvertLogicalOperators(ref mPropositionalSentence2);

            SelectListItem item1 = new SelectListItem(mPropositionalSentence, mPropositionalSentence);
            SelectListItem item2 = new SelectListItem(mPropositionalSentence1, mPropositionalSentence1);
            SelectListItem item3 = new SelectListItem(mPropositionalSentence2, mPropositionalSentence2);
            _listItems.Add(item1);
            _listItems.Add(item2);
            _listItems.Add(item3);
        }
        else
        {
            // Check if the item already exists in the list
            if (!_listItems.Any(item => item.Text == mSentence))
            {
                SelectListItem item = new SelectListItem(mSentence, mSentence);
                _listItems.Add(item);
            }
          

        }
    }
}