//truth dag if we need to print dag with truth values also
var stepNumber = 1;
var firstTime = true;
function truthDAG(myList, treeConnections, exercise = false, issueIndex = -1, timer = 999) {
    var nodes = new vis.DataSet([]);
    var edges = new vis.DataSet([]);
    for (let i = 0; i < myList.length; i++) {
        if (i > timer) break;
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
        var step = "Formule " + treeConnections[i].item1 + "dosadíme pravou stranu " + treeConnections[i].item2;
        console.log(treeConnections[i]);
        if (i < treeConnections.length - 1 && treeConnections[i].item1 === treeConnections[i + 1].item1) {
            edgeColor.color = 'blue';
            step = "Formule  " + treeConnections[i].item1 + "dosadíme levou stranu " + treeConnections[i].item2;
        }
        let firstChar = treeConnections[i].item1.charAt(0);
        let thirdChar = treeConnections[i].item1.charAt(2);

        if (firstChar.charCodeAt(0) === 172 && treeConnections[i].item1.length === 2) {
            edgeColor.color = 'blue';
            step = "Formule  " + treeConnections[i].item1 + "dosadíme levou stranu " + treeConnections[i].item2;
        }
        if (firstChar.charCodeAt(0) === 172 && thirdChar.charCodeAt(0) === 61) {
            edgeColor.color = 'blue';
            step = "Formule  " + treeConnections[i].item1 + "dosadíme levou stranu " + treeConnections[i].item2;
        }
        else if (i < treeConnections.length - 1 && treeConnections[i].item2 === treeConnections[i + 1].item2) {
            edgeColor.color = 'blue';
            step = "Formule  " + treeConnections[i].item1 + "dosadíme levou stranu " + treeConnections[i].item2;
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
    if (step != undefined) {
            var stepAlert = document.createElement("div");
        stepAlert.classList.add("alert", "alert-primary");


            stepAlert.innerHTML = '<p>' + stepNumber + ". " + step + '</p>';
            stepNumber++;
        document.getElementById("mynetwork").parentNode.insertBefore(stepAlert, document.getElementById("mynetwork").nextSibling);

    }

   
    }