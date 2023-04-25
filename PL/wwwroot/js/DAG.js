
var stepNumber = 1;
//print dag
function makeDAG(myList, treeConnections, exercise = false, issueIndex = -1, timer = 999) {
    //create empty dataset for nodes and edges
    var nodes = new vis.DataSet([]);
    var edges = new vis.DataSet([]);
    //print always one level more than in previous iteration
    for (let i = 0; i < myList.length; i++) {
        if (i > timer) break;
        //create node which has size 100 and white color
        var node = { id: myList[i], label: myList[i], size: 100, font: { color: 'white', size: 16 } };
        //if it is root node print that in purple color
        if (i == 0) {
            node.color = 'purple';
        }
        //if we want to print issue in that dag print it with red color, it's used on exercises with dag
        if (i == issueIndex) {
            node.color = 'red';
        }
        //add node to nodes list
        nodes.add(node);
    }
    //used for path connections in DAG 
    for (let i = 0; i < treeConnections.length; i++) {
         if (i > timer) break;
        let edgeColor = { color: 'orange' };
        var step = "Rozdělíme " + treeConnections[i].item1 + " pravá strana " + treeConnections[i].item2;
        console.log(treeConnections[i]);
        //if tree has left and right side we will use this step
        if (i < treeConnections.length - 1 && treeConnections[i].item1 === treeConnections[i + 1].item1) {
            edgeColor.color = 'blue';
            step = "Rozdělíme  " + treeConnections[i].item1 + " levá strana " + treeConnections[i].item2;
        }
        //chars for ¬ and = without this it cannot be equaled 
        let firstChar = treeConnections[i].item1.charAt(0);
        let thirdChar = treeConnections[i].item1.charAt(2);
        //if first char ¬ and item has only two places then this e.g. ¬a
        if (firstChar.charCodeAt(0) === 172 && treeConnections[i].item1.length === 2) {
            edgeColor.color = 'blue';   
            step = "Rozdělíme  " + treeConnections[i].item1 + " levá strana " + treeConnections[i].item2;
        }
         //if first char ¬ and third = then use this e.g ¬a= 1
        if (firstChar.charCodeAt(0) === 172 && thirdChar.charCodeAt(0) === 61) {
            edgeColor.color = 'blue';
            step = "Rozdělíme  " + treeConnections[i].item1 + " levá strana " + treeConnections[i].item2;
        }
        //if it will be literal use this
        else if (i < treeConnections.length - 1 && treeConnections[i].item2 === treeConnections[i + 1].item2) {
            edgeColor.color = 'blue';
            step = "Rozdělíme  " + treeConnections[i].item1 + " levá strana " + treeConnections[i].item2;
        }
        //add this paths to list of edges
        edges.add({ from: treeConnections[i].item1, to: treeConnections[i].item2, arrows: 'from', color: edgeColor });
    }
    //create container for that svg element
    var container = document.getElementById("mynetwork");
    var data = {
        nodes: nodes,
        edges: edges
    };
    var options = {};
    network = new vis.Network(container, data, options);
    console.log(step);
    //if we have steps and it is not exercise print steps in alert-primary divs
    if (step != undefined) {
        if (!exercise) {
            var stepAlert = document.createElement("div");
            stepAlert.classList.add("alert", "alert-primary");
            stepAlert.innerHTML = '<p>' + stepNumber + ". " + step + '</p>';
            stepNumber++;
            document.getElementById("mynetwork").parentNode.insertBefore(stepAlert, document.getElementById("mynetwork").nextSibling);
        }
    }
    //if it is exercise
    if (exercise) {
        //add on click event
        network.on('click', function (params) {
            if (params.nodes.length > 0) {
                //get node by it id
                var nodeId = params.nodes[0];
                var node = nodes.get(nodeId);
                //get label of it
                var currentLabel = node.label;
                //split node by =
                var currentParts = currentLabel.split('=');
                //current value is after =
                var currentValue = currentParts[1].trim();

                // Toggle the value between 1, 0, and 0 = 1
                var newValue;
                if (currentParts.length === 3) {
                    newValue = "0";
                } else if (currentValue === "0") {
                    newValue = "1";
                } else if (currentValue === "1") {
                    newValue = "0 = 1";
                }
                //split label = add new = and new value after click
                var newLabel = currentLabel.split('=')[0] + '=' + newValue;
                node.label = newLabel;
                nodes.update(node); // Call update() to apply the changes to the node
            }
        });
    }
}