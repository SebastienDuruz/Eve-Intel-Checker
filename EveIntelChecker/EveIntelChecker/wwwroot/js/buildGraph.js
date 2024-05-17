/**
 * Autor : Sébastien Duruz
 * Date : 03.12.2022
 * Description : Map generator for Eve Intel Checker
 */

let container
let map
let edges
let nodes
let options = {
    autoResize: true,
    height: '100%',
    width: '100%',
    clickToUse: false,
    physics: {
        enabled: true,
        solver: 'barnesHut',
        barnesHut: {
            avoidOverlap: 0.1
        },
        stabilization: {
            enabled: true,
            fit: true
        }
    },
    nodes: {
        borderWidth: 1,
        borderWidthSelected: 1,
        margin: 5,
        chosen: false,
        color: {
            border: '#FFFFFF',
            background: '#1c1c1c',
            highlight: {
                border: '#FFFFFF',
                background: '#1c1c1c'
            },
        },
        font: {
            color: '#FFFFFF',
            size: 18, // px
            face: 'roboto',
            multi: true
        },
        fixed: false,
        shape: 'box'
    },
    layout: {
        improvedLayout: true,
        randomSeed: 2
    },
    interaction: {
        dragNodes: true,
        dragView: true,
        hideEdgesOnDrag: false,
        hideEdgesOnZoom: false,
        hideNodesOnDrag: false,
        hover: false,
        hoverConnectedEdges: false,
        multiselect: false,
        navigationButtons: false,
        selectable: false,
        selectConnectedEdges: false,
        tooltipDelay: 50,
        zoomSpeed: 1,
        zoomView: true
    },
    edges: {
        smooth: {
            type: 'discrete',
            forceDirection: 'none'
        }
    }
};

/**
 * Build new map by settings new data
 * */
function buildMap(nodesToBuild, edgesToBuild) {

    nodes = new vis.DataSet(nodesToBuild)
    edges = new vis.DataSet(edgesToBuild)
    const data = {
        nodes: nodes,
        edges: edges
    };

    container = document.getElementById('canvasMap');

    if (container !== null)
        map = new vis.Network(container, data, options);
}
function setData(nodesToUpdate) {
    for (let i = 1; i < nodesToUpdate.length + 1; ++i) {
        nodes.update([{ id: i, color: { background: nodesToUpdate[i - 1].color.background } }]);
        nodes.update([{ id: i, label: nodesToUpdate[i - 1].label }]);
    }
}

