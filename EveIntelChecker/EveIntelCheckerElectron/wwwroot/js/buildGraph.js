/**
 * Autor : Sébastien Duruz
 * Date : 03.12.2022
 * Description : Map generator for Eve Intel Checker
 */

var container
var map

var links
var nodes

let MIN_ZOOM = 1
let MAX_ZOOM = 1
let lastZoomPosition = { x: 0, y: 0 }

var options = {
    height: '100%',
    width: '100%',
    clickToUse: false,
    physics: {
        stabilization: {
            enabled: false,
            fit: false
        }
    },
    nodes: {
        borderWidth: 2,
        borderWidthSelected: 2,
        chosen: false,
        color: {
            border: '#E0E0E0',
            background: '#27272fff',
        },
        font: {
            color: '#FFFFFF',
            size: 18, // px
            face: 'roboto'
        },
        fixed: true,
        shape: 'box'
    },
    layout: {
        improvedLayout: true,
        randomSeed: 2,
        hierarchical: {
            enabled: false,
            levelSeparation: 150,
            nodeSpacing: 150,
            treeSpacing: 250,
            blockShifting: false,
            edgeMinimization: true,
            parentCentralization: true,
            direction: 'DU',        // UD, DU, LR, RL
            sortMethod: 'directed',  // hubsize, directed
            shakeTowards: 'roots'  // roots, leaves
        }
    },
    interaction: {
        dragNodes: false,
        dragView: false,
        hideEdgesOnDrag: false,
        hideEdgesOnZoom: false,
        hideNodesOnDrag: false,
        hover: false,
        hoverConnectedEdges: true,
        keyboard: {
            enabled: false,
            speed: { x: 10, y: 10, zoom: 0.02 },
            bindToWindow: true,
            autoFocus: true,
        },
        multiselect: false,
        navigationButtons: false,
        selectable: true,
        selectConnectedEdges: true,
        tooltipDelay: 300,
        zoomSpeed: 1,
        zoomView: false
    }
};

document.getElementsByTagName("BODY")[0].onresize = function () { fitMap() };

function fitMap() {
    if (map != null) {
        map.fit();
    }
}

function buildMap(nodesToBuild, linksToBuild) {
    var data = {
        nodes: nodesToBuild,
        edges: linksToBuild
    };

    container = document.getElementById('canvasMap');
    map = new vis.Network(container, data, options);
}


