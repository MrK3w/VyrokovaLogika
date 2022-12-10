using VyrokovaLogika;

//my sentence
string vl = "((a&B)|(b&C)) > (b|c)";
//create instance of engine which will proceed my sentece/s?
Engine tree1 = new Engine(vl);
tree1.ProcessSentence();
