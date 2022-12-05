/**
 * Autor : Sébastien Duruz
 * Date : 03.12.2022
 * Description : Map generator for Eve Intel Checker
 */

var container
var map

var links
var nodes

var options = {
    height: '100%',
    width: '100%',
    clickToUse: false,
    physics: {
        enabled: false,
    },
    nodes: {
        borderWidth: 1,
        borderWidthSelected: 1,
        chosen: false,
        color: {
            border: '#E0E0E0',
            background: '#272c34ff',
            highlight: {
                border: '#E0E0E0',
                background: '#272c34ff'
            },
            hover: {
                border: '#E0E0E0',
                background: '#272c34ff'
            }
        },
        font: {
            color: '#E0E0E0',
            size: 14, // px
            face: 'arial'
        },
        fixed: true,
        shape: 'box'
    },
    layout: {
        randomSeed: '0.45384121392550325:1670224726446',
        improvedLayout: true,
        hierarchical: {
            enabled: true,
            levelSeparation: 100,
            nodeSpacing: 100,
            treeSpacing: 100,
            blockShifting: true,
            edgeMinimization: true,
            parentCentralization: true,
            direction: 'UD',        // UD, DU, LR, RL
            sortMethod: 'hubsize',  // hubsize, directed
            shakeTowards: 'roots'  // roots, leaves
        }
    }
};

function setLinks(links) {
    links = links;
}

function setNodes(nodes) {
    nodes = nodes;
}


function buildMap() {
    // create an array with nodes


    // provide the data in the vis format
    var data = {
        nodes: nodes,
        edges: edges
    };

    container = document.getElementById('canvasMap');
    map = new vis.Network(container, data, options);
}
