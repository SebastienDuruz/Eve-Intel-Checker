/**
 * Autor : Sébastien Duruz
 * Date : 03.12.2022
 * Description : Map generator for Eve Intel Checker
 */

// create an array with nodes
var nodes = new vis.DataSet([
    {id: 1, label: 'PBD'},
    {id: 2, label: '9GI'},
    {id: 3, label: 'L-1'},
    {id: 4, label: 'DBT'},
    {id: 5, label: 'PND'},
    {id: 6, label: '3G-'}
]);

// create an array with edges
var edges = new vis.DataSet([
    {from: 1, to: 2},
    {from: 2, to: 4},
    {from: 2, to: 3},
    {from: 4, to: 5},
    {from: 4, to: 6}
]);

var container = document.getElementById('canvasMap');
var options = {};

function buildMap(nodes, edges) {
    let map = new vis.network(container, {nodes: nodes, edges: edges}, options);
}