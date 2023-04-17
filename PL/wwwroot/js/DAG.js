function makeDAG(myList, treeConnections) {
    //inicialize graph from Viva library
    var graph = Viva.Graph.graph();
    //get myList from controller to use it for later

    var graphics = Viva.Graph.View.svgGraphics(),
        nodeSize = 16;



    graphics.node(function (node) {
        // This time it's a group of elements: http://www.w3.org/TR/SVG/struct.html#Groups
        var ui = Viva.Graph.svg('g'),
            // Create SVG text element with user id as content
            svgText = Viva.Graph.svg('text').attr('y', '16px').attr('x','-15px').text(node.id),
            img = Viva.Graph.svg('rect')
                .attr('width', 4)
                .attr('height', 4)
                .attr('fill', "blue")
                .attr('font-size', '12px');

        ui.append(svgText);
        ui.append(img);
        return ui;
    }).placeNode(function (nodeUI, pos) {
        // 'g' element doesn't have convenient (x,y) attributes, instead
        // we have to deal with transforms: http://www.w3.org/TR/SVG/coords.html#SVGGlobalTransformAttribute
        nodeUI.attr('transform',
            'translate(' +
            (pos.x - 4 / 2) + ',' + (pos.y - 4 / 2) +
            ')');
    });

    // Rendering arrow shape is achieved by using SVG markers, part of the SVG
    // standard: http://www.w3.org/TR/SVG/painting.html#Markers
    var createMarker = function (id) {
        return Viva.Graph.svg('marker')
            .attr('id', id)
            .attr('viewBox', "0 0 10 10")
            .attr('refX', "10")
            .attr('refY', "5")
            .attr('markerUnits', "strokeWidth")
            .attr('markerWidth', "10")
            .attr('markerHeight', "5")
            .attr('orient', "auto");
    },

        marker = createMarker('Triangle');
    marker.append('path').attr('d', 'M 0 0 L 10 5 L 0 10 z');

    // Marker should be defined only once in <defs> child element of root <svg> element:
    var defs = graphics.getSvgRoot().append('defs');
    defs.append(marker);

    var geom = Viva.Graph.geom();

    graphics.link(function (link) {
        // Notice the Triangle marker-end attribe:
        return Viva.Graph.svg('path')
            .attr('stroke', 'gray')
            .attr('marker-end', 'url(#Triangle)');
    }).placeLink(function (linkUI, fromPos, toPos) {
        // Here we should take care about
        //  "Links should start/stop at node's bounding box, not at the node center."

        // For rectangular nodes Viva.Graph.geom() provides efficient way to find
        // an intersection point between segment and rectangle
        var toNodeSize = nodeSize,
            fromNodeSize = nodeSize;

        var from = geom.intersectRect(
            // rectangle:
            fromPos.x - fromNodeSize / 2, // left
            fromPos.y - fromNodeSize / 2, // top
            fromPos.x + fromNodeSize / 2, // right
            fromPos.y + fromNodeSize / 2, // bottom
            // segment:
            fromPos.x, fromPos.y, toPos.x, toPos.y)
            || fromPos; // if no intersection found - return center of the node

        var to = geom.intersectRect(
            // rectangle:
            toPos.x - toNodeSize / 2, // left
            toPos.y - toNodeSize / 2, // top
            toPos.x + toNodeSize / 2, // right
            toPos.y + toNodeSize / 2, // bottom
            // segment:
            toPos.x, toPos.y, fromPos.x, fromPos.y)
            || toPos; // if no intersection found - return center of the node

        var data = 'M' + from.x + ',' + from.y +
            'L' + to.x + ',' + to.y;

        linkUI.attr("d", data);
    });

    for (let i = 0; i < myList.length; i++) {
        graph.addNode(myList[i]);
    }

    for (let i = 0; i < treeConnections.length; i++) {
        graph.addLink(treeConnections[i].item1, treeConnections[i].item2);
    }

    var container = document.createElement('div');
    container.style.position = 'relative'; // Set the container's position property to 'relative'
    container.style.width = '800px'; // Set the container's width
    container.style.height = '600px'; // Set the container's height
    container.style.left = '0px'; // Set the initial left position to 0px
    document.body.appendChild(container); // Append the container to the DOM

    container.style.left = '100px';

    container.addEventListener('wheel', function (event) {
        event.preventDefault(); // Prevent scrolling on the container div
    });

    // Render the graph
    var renderer = Viva.Graph.View.renderer(graph, {
        graphics: graphics,
        container: container,
        width: 800, // Set the desired width
        height: 600 // Set the desired height
    });
    renderer.run();
}