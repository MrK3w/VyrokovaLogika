using VyrokovaLogika;

//my sentence - negation & and | or > implication

#pragma warning disable CS0219 // Variable is assigned but its value is never used
string vl = "(-a>-B)|-B&C";

string vl1 = "(a>b)|(a>b)";
string vl3 = "(a|b)>((a&c) | ((a|b) & (a|b)))";
string vl2 = "-((-a&-B)&(B&-a))|-B";
string vl5 = "(p∨(q∧r))∧((p∨q)∧(p∨r))";
string vl4 = "(p∨(q∧r))>((p∨q)∧(p∧r))";
string vl7 = "(A&B) | C";
string vl8 = "(p∧(q>r))>((p∧q)∨(p∧r))"; //nejde
string vl9 = "(p∨(q∧r))>((p∨q)∧(p∨r))"; //jde
#pragma warning disable IDE0059 // Unnecessary assignment of a value
string vl10 = "(-p>q)∨(-q>p)";
string vl11 = "(p>q)≡(-q>-p)";
#pragma warning restore CS0219 // Variable is assigned but its value is never used
//create instance of engine which will proceed my sentece/s?
Engine tree1 = new(vl11);
tree1.ProcessSentence();
