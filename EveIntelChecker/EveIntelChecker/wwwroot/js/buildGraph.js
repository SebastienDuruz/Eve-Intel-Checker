var graph = Viva.Graph.graph();
graph.addLink(1, 2);

// specify where it should be rendered:
var renderer = Viva.Graph.View.renderer(graph, {
    container: document.getElementById('graphDiv')
});
renderer.run();