/**
 * Autor : Sébastien Duruz
 * Date : 03.12.2022
 * Description : Map generator for Eve Intel Checker
 */

let container
let map
let links
let nodes
let options = {
    autoResize: true,
    height: '100%',
    width: '100%',
    clickToUse: false,
    physics: {
        enabled: true,
        barnesHut: {
            avoidOverlap: 0.1
        },
    },
    nodes: {
        borderWidth: 1,
        borderWidthSelected: 1,
        margin: 5,
        chosen: false,
        color: {
            border: '#E0E0E0',
            background: '#424242ff',
            highlight: {
                border: '#E0E0E0',
                background: '#424242ff'
            },
        },
        font: {
            color: '#FFFFFF',
            size: 18, // px
            face: 'roboto'
        },
        fixed: false,
        shape: 'box'
    },
    layout: {
        improvedLayout: true,
        randomSeed: 2
    },
    interaction: {
        dragNodes: false,
        dragView: false,
        hideEdgesOnDrag: false,
        hideEdgesOnZoom: false,
        hideNodesOnDrag: false,
        hover: false,
        hoverConnectedEdges: false,
        multiselect: false,
        navigationButtons: false,
        selectable: true,
        selectConnectedEdges: false,
        tooltipDelay: 50,
        zoomSpeed: 1,
        zoomView: false
    },
    edges: {
        smooth: {
            type: 'discrete',
            forceDirection: 'none'
        }
    }
};

/**
 * Autofit the StarMap
 * */
function fitMap() {
    if (map != undefined) {
        map.fit();
        map.stabilize();
    }
}

/**
 * Build new map by settings new data
 * */
function buildMap(nodesToBuild, linksToBuild) {

    var data = {
        nodes: nodesToBuild,
        edges: linksToBuild
    };

    container = document.getElementById('canvasMap');
    map = new vis.Network(container, data, options);

    // Set doubleclick event : Open Dotlan webpage
    map.on("doubleClick", function (params) {
        let nodeId = params.nodes[0];
        if (nodeId != null) {
            const node = data.nodes.find(element => element.id == nodeId);
            const region = node.region;
            const system = node.system;
            window.open("https://evemaps.dotlan.net/map/" + region.replace(' ', '_') + "/" + system, '_blank');
        }
    });
}

/**
 * Set new Nodes data to the graph
 * */
function setData(nodesToBuild) {
    nodes = new vis.DataSet(nodesToBuild);
    map.setData({ nodes: nodes, edges: links });
}
