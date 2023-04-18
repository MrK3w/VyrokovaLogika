function makeDAG(myList, treeConnections) {
    var nodes = new vis.DataSet([]);
    var edges = new vis.DataSet([]);

    for (let i = 0; i < myList.length; i++) {
        console.log("nody " + myList[i])
        if (i == 0) {
            nodes.add({
                id: myList[0], label: myList[0], color: 'red',  size: 100, font: {
                    color: 'white',
                    size: 16,// Set text color to white
                }
            });
        }
        else {
            nodes.add({
                id: myList[i], label: myList[i], size: 100, font: {
                    color: 'white',
                    size: 16,// Set text color to white
                }
            });
        }
    }

    for (let i = 0; i < treeConnections.length; i++) {
        console.log("cesty " + treeConnections[i].item1 + "|" + treeConnections[i].item2)
        edges.add({ from: treeConnections[i].item1, to: treeConnections[i].item2, arrows: 'to' });
    }

    var container = document.getElementById("mynetwork");
    var data = {
        nodes: nodes,
        edges: edges
    };
    var options = {

    }
    var network = new vis.Network(container, data, options);
}