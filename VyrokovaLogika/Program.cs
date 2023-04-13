using VyrokovaLogika;

//my sentence - negation & and | or > implication

string vl = "(-a>-B)|-B&C";
string vl1 = "(a>b)|(a>b)";
string vl3 = "(a|b)>((a&c) | ((a|b) & (a|b)))";
string vl2 = "-((-a&-B)&(B&-a))|-B";
string vl5 = "(p∨(q∧r))∧((p∨q)∧(p∨r))";
string vl4 = "(p∨(q∧r))>((p∨q)∧(p∧r))";
string vl7 = "(A&B) | C";
string vl8 = "(p∧(q>r))>((p∧q)∨(p∧r))"; //nejde
string vl9 = "(p∨(q∧r))>((p∨q)∧(p∨r))"; //jde
string vl10 = "(-p>q)∨(-q>p)";
string vl11 = "(p>q)≡(-q>-p)";
//create instance of engine which will proceed my sentece/s?
Engine tree1 = new Engine(vl11);
tree1.ProcessSentence();
