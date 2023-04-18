var network;

function makeDAG(myList, treeConnections, exercise = false) {
    var nodes = new vis.DataSet([]);
    var edges = new vis.DataSet([]);

    for (let i = 0; i < myList.length; i++) {
        var node = { id: myList[i], label: myList[i], size: 100, font: { color: 'white', size: 16 } };
        if (i == 0) {
            node.color = 'red';
        }
        nodes.add(node);
    }

    for (let i = 0; i < treeConnections.length; i++) {
        edges.add({ from: treeConnections[i].item1, to: treeConnections[i].item2, arrows: 'to' });
    }

    var container = document.getElementById("mynetwork");
    var data = {
        nodes: nodes,
        edges: edges
    };
    var options = {};
    network = new vis.Network(container, data, options);

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