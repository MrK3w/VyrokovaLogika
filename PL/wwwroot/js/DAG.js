function makeDAG(myList, treeConnections, exercise = false) {
    var nodes = new vis.DataSet([]);
    var edges = new vis.DataSet([]);

    for (let i = 0; i < myList.length; i++) {
        console.log("node " + myList[i]);
        var node = { id: myList[i], label: myList[i], size: 100, font: { color: 'white', size: 16 } };
        if (i == 0) {
            node.color = 'red';
        }
        nodes.add(node);
    }

    for (let i = 0; i < treeConnections.length; i++) {
        console.log("edges " + treeConnections[i].item1 + "|" + treeConnections[i].item2);
        edges.add({ from: treeConnections[i].item1, to: treeConnections[i].item2, arrows: 'to' });
    }

    var container = document.getElementById("mynetwork");
    var data = {
        nodes: nodes,
        edges: edges
    };
    var options = {};
    var network = new vis.Network(container, data, options);

    if (exercise) {

        network.on('click', function (params) {
            if (params.nodes.length > 0) {
                var nodeId = params.nodes[0];
                var node = nodes.get(nodeId);

                // Extract the current label and value from the node label
                var currentLabel = node.label;
                var currentValue = parseInt(currentLabel.split('=')[1]) || 0; // Extract the value after '=' and parse as integer, default to 0 if not present

                // Toggle the value between 0 and 1
                var newValue = currentValue === 0 ? 1 : 0;

                // Update the node label with the new value
                var newLabel = currentLabel.split('=')[0] + '=' + newValue; // Concatenate the label before '=' with the new value
                node.label = newLabel;
                nodes.update(node); // Call update() to apply the changes to the node
            }
        });
    }
}