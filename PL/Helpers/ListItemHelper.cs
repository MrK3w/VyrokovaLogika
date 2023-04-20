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
                _listItems = new List<SelectListItem>();
                SetListItems();
            }
            return _listItems;
        }
    }

    private static List<SelectListItem> _exerciseTypes;

    public static List<SelectListItem> ExerciseTypes
    {
        get
        {
            if (_exerciseTypes == null)
            {
                _exerciseTypes = new List<SelectListItem>();
                SetListItemExercises();
            }
            return _exerciseTypes;
        }
    }

    private static void SetListItemExercises()
    {
        SelectListItem item1 = new SelectListItem("Tautology", "Tautology");
        SelectListItem item2 = new SelectListItem("Not Tautology", "Not Tautology");
        SelectListItem item3 = new SelectListItem("Contradiction", "Contradiction");
        SelectListItem item4 = new SelectListItem("Not Contradiction", "Not Contradiction");
        _exerciseTypes.Add(item1);
        _exerciseTypes.Add(item2);
        _exerciseTypes.Add(item3);
        _exerciseTypes.Add(item4);
    }

    public static void SetListItems(string mSentence = null)
    {
        if (_listItems.Count == 0)
        {
            SelectListItem defaultItem = new SelectListItem { Text = "select formula", Value = "" };
            string mPropositionalSentence = "(p>q)≡(-q>-p)";
            Validator.ValidateSentence(ref mPropositionalSentence);


            string mPropositionalSentence1 = "((-x|b)&(x|a))";
            Validator.ValidateSentence(ref mPropositionalSentence1);

            string mPropositionalSentence2 = "(-x|b)&(x|a)";
            Validator.ValidateSentence(ref mPropositionalSentence2);

            string mPropositionalSentence3 = "(P&-P)";
            Validator.ValidateSentence(ref mPropositionalSentence3);
            string mPropositionalSentence4 = " (A|B)&(-A&-B)";
            Validator.ValidateSentence(ref mPropositionalSentence4);
            SelectListItem item1 = new SelectListItem(mPropositionalSentence, mPropositionalSentence);
            SelectListItem item2 = new SelectListItem(mPropositionalSentence1, mPropositionalSentence1);
            SelectListItem item3 = new SelectListItem(mPropositionalSentence2, mPropositionalSentence2);
            SelectListItem item4 = new SelectListItem(mPropositionalSentence3, mPropositionalSentence3);
            SelectListItem item5 = new SelectListItem(mPropositionalSentence4, mPropositionalSentence4);

            _listItems.Add(defaultItem);
            _listItems.Add(item1);
            _listItems.Add(item2);
            _listItems.Add(item3);
            _listItems.Add(item4);
            _listItems.Add(item5);
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