using VyrokovaLogika;

//my sentence - negation & and | or > implication

string vl = "(-a>-B)|-B&C";
string vl1 = "(a>b)|(a>b)";
string vl3 = "(a|b)>((a&c) | ((a|b) & (a|b)))";
string vl2 = "-((-a&-B)&(B&-a))|-B";
string vl5 = "(p∨(q∧r))>((p∨q)∧(p∨r))";
string vl4 = "(p∨(q∧r))⇒((p∨q)∧(p∧r))";
string vl7 = "(p∧(q∨r))>((p∨q)∧(p∧r))";
//create instance of engine which will proceed my sentece/s?
Engine tree1 = new Engine(vl7);
tree1.ProcessSentence();
