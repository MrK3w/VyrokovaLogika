using VyrokovaLogika;

//my sentence - negation & and | or > implication

string vl = "-(-a>-B)|-B";
string vl1 = "a>b";

string vl2 = "-((-a&-B)&(B&-a))|-B";
string vl3 = "(-((-x|b)&(x|a)) | (x&B)) >((a|b)&(b&c))";
string vl4 = "A>B";
//create instance of engine which will proceed my sentece/s?
Engine tree1 = new Engine(vl);
tree1.ProcessSentence();
