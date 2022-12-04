/**
 * Autor : Sébastien Duruz
 * Date : 03.12.2022
 * Description : Map generator for Eve Intel Checker
 */

// create an array with nodes
var nodes = new vis.DataSet([
    { id: 1, label: 'PBD' },
    { id: 2, label: '9GI' },
    { id: 3, label: 'L-1' },
    { id: 4, label: 'DBT' },
    { id: 5, label: 'PND' },
    { id: 6, label: '3G-' }
]);

// create an array with edges
var edges = new vis.DataSet([
    { from: 1, to: 2 },
    { from: 2, to: 4 },
    { from: 2, to: 3 },
    { from: 4, to: 5 },
    { from: 4, to: 6 }
]);

// provide the data in the vis format
var data = {
    nodes: nodes,
    edges: edges
};
var options = {
    height: '100%',
    width: '100%'
};

var container
var map

function buildMap() {
    container = document.getElementById('canvasMap');
    map = new vis.Network(container, data, options);
}

function destroyMap() {
    map.hide()
}

console.log("hello")
