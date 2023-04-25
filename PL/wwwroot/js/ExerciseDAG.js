//function to get dag from page
function myFunctionDAG() {
    var updatedNodes = network.body.data.nodes.get();
    var updatedEdges = network.body.data.edges.get();

    var nodeData = updatedNodes.map(function (node) {
        return {
            label: node.label
        };
    });

    var edgeData = updatedEdges.map(function (edge) {
        return {
            from: edge.from,
            to: edge.to
        };
    });

    var nodeDataJson = JSON.stringify(nodeData);
    var edgeDataJson = JSON.stringify(edgeData);
    console.log(nodeDataJson);
    console.log(edgeDataJson);
    document.getElementById('hiddenNumber2').value = nodeDataJson;
    document.getElementById('hiddenNumber3').value = edgeDataJson;
}