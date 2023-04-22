
var stepNumber = 1;

function makeDAG(myList, treeConnections, exercise = false, issueIndex = -1, timer = 999) {
    var nodes = new vis.DataSet([]);
    var edges = new vis.DataSet([]);
    for (let i = 0; i < myList.length; i++) {
        if (i> timer) break;
        var node = { id: myList[i], label: myList[i], size: 100, font: { color: 'white', size: 16 } };
        if (i == 0) {
            node.color = 'purple';
        }
        if (i == issueIndex) {
            node.color = 'red';
        }
        nodes.add(node);
    }

    for (let i = 0; i < treeConnections.length; i++) {
         if (i > timer) break;
        let edgeColor = { color: 'orange' };
        var step = "Rozdělíme " + treeConnections[i].item1 + " pravá strana " + treeConnections[i].item2;
        console.log(treeConnections[i]);
        if (i < treeConnections.length - 1 && treeConnections[i].item1 === treeConnections[i + 1].item1) {
            edgeColor.color = 'blue';
            step = "Rozdělíme  " + treeConnections[i].item1 + " levá strana " + treeConnections[i].item2;
        }
        let firstChar = treeConnections[i].item1.charAt(0);
        let thirdChar = treeConnections[i].item1.charAt(2);
        
        if (firstChar.charCodeAt(0) === 172 && treeConnections[i].item1.length === 2) {
            edgeColor.color = 'blue';   
            step = "Rozdělíme  " + treeConnections[i].item1 + " levá strana " + treeConnections[i].item2;
        }
        if (firstChar.charCodeAt(0) === 172 && thirdChar.charCodeAt(0) === 61) {
            edgeColor.color = 'blue';
            step = "Rozdělíme  " + treeConnections[i].item1 + " levá strana " + treeConnections[i].item2;
        }
        else if (i < treeConnections.length - 1 && treeConnections[i].item2 === treeConnections[i + 1].item2) {
            edgeColor.color = 'blue';
            step = "Rozdělíme  " + treeConnections[i].item1 + " levá strana " + treeConnections[i].item2;
        }
        
        edges.add({ from: treeConnections[i].item1, to: treeConnections[i].item2, arrows: 'from', color: edgeColor });
    }

    var container = document.getElementById("mynetwork");
    var data = {
        nodes: nodes,
        edges: edges
    };
    var options = {};
    network = new vis.Network(container, data, options);
    console.log(step);
    if (step != undefined) {
        if (!exercise)
    var stepAlert = document.createElement("div");
    stepAlert.classList.add("alert", "alert-primary");
        stepAlert.innerHTML = '<p>' + stepNumber + ". " + step + '</p>';
        stepNumber++;
        document.getElementById("mynetwork").parentNode.insertBefore(stepAlert, document.getElementById("mynetwork").nextSibling);
    }

    if (exercise) {
        network.on('click', function (params) {
            if (params.nodes.length > 0) {
                var nodeId = params.nodes[0];
                var node = nodes.get(nodeId);

                var currentLabel = node.label;
                var currentParts = currentLabel.split('=');
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
                var newLabel = currentLabel.split('=')[0] + '=' + newValue;
                node.label = newLabel;
                nodes.update(node); // Call update() to apply the changes to the node
            }
        });
    }
}