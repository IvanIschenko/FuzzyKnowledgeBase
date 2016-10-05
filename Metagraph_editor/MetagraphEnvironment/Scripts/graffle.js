//"use strict";
var paper = Raphael(document.getElementById("workZone"), 1600, 900);
var mode = "addVertex"; //Початковий режим
var pointSet = paper.set();
var vertices = []; //oX, oY, value
var edges = []; // v1, v2
var nodeN = 0;

var MetaGraph = {
    metaVerticesCenters: []
};

function getJson(p) {

}

var jsonData = '{ ' +
                    '"name": "JSON!!", ' +
                    '"vertices": [ ' +
                        '{"x": 118, "y": 287}, ' +
                        '{"x": 295, "y": 161}, ' +
                        '{"x": 346, "y": 155}, ' +
                        '{"x": 276, "y": 291}, ' +
                        '{"x": 179, "y": 245}, ' +
                        '{"x": 156, "y": 124} ' +
                    '], ' +
                    '"metaVertices": [ ' +
                        '[5], ' +
                        '[1, 2]' +
                        //'[4, 1, 2]'+
                    '], ' +
                    '"edges": [ ' +
                        '{"start": "m0", "finish": "v1"}, ' +
                        '{"start": "v2", "finish": "v4"}, ' +
                        '{"start": "m1", "finish": "v3"}, ' +
                        '{"start": "v4", "finish": "m0"} ' +
                    '] ' +
                '}';

function build() {

    var obj = JSON.parse(jsonData);
    //alert(getJson());
    alert("Yea!!");

    for (var i = 0 ; i < obj.vertices.length; ++i) {
        addVertex(obj.vertices[i].x, obj.vertices[i].y);
    }

    for (var j = 0; j < obj.metaVertices.length; ++j) {

        var left = { x: 10000, y: 10000 }, top = { x: 10000, y: 10000 }, right = { x: 0, y: 0 }, bottom = { x: 0, y: 0 };

        for (var k = 0; k < obj.metaVertices[j].length; ++k) {
            if (obj.vertices[obj.metaVertices[j][k]].x < left.x) {
                left.x = obj.vertices[obj.metaVertices[j][k]].x;
                left.y = obj.vertices[obj.metaVertices[j][k]].y;
            }
            if (obj.vertices[obj.metaVertices[j][k]].y < top.y) {
                top.y = obj.vertices[obj.metaVertices[j][k]].y;
                top.x = obj.vertices[obj.metaVertices[j][k]].x;
            }
            if (obj.vertices[obj.metaVertices[j][k]].x > right.x) {
                right.x = obj.vertices[obj.metaVertices[j][k]].x;
                right.y = obj.vertices[obj.metaVertices[j][k]].y;
            }
            if (obj.vertices[obj.metaVertices[j][k]].y > bottom.y) {
                bottom.y = obj.vertices[obj.metaVertices[j][k]].y;
                bottom.x = obj.vertices[obj.metaVertices[j][k]].x;
            }
        }

        var R_shtrih = 10;
        paper.path("M " + (left.x - R_shtrih) + " " + left.y + " R" + top.x + " "
        + (top.y - R_shtrih) + " " + (right.x + R_shtrih) + " " + right.y + " " + bottom.x + " " + (bottom.y + R_shtrih) + " z").toBack().attr({ fill: "green" });
        //paper.circle(left.x-30, left.y, 2);
        //paper.circle(right.x+30, right.y, 2);
        //paper.circle(top.x, top.y-30, 2);
        //paper.circle(bottom.x, bottom.y+30, 2);

        MetaGraph.metaVerticesCenters[j] = new Object();
        MetaGraph.metaVerticesCenters[j].x = (right.x + left.x) / 2;
        MetaGraph.metaVerticesCenters[j].y = (top.y + bottom.y) / 2;
        //console.log(MetaGraph.metaVerticesCenters[j]);
    }

    for (var l = 0; l < obj.edges.length; ++l) {

        var startX, startY, finishX, finishY;
        var nS = obj.edges[l].start[1]; //
        var nF = obj.edges[l].finish[1]; //
        // Додати пізніше try-catch
        if (obj.edges[l].start[0] == "v") {
            startX = obj.vertices[nS].x;
            startY = obj.vertices[nS].y;
        }
        else {
            startX = MetaGraph.metaVerticesCenters[nS].x;
            startY = MetaGraph.metaVerticesCenters[nS].y;
        }

        if (obj.edges[l].finish[0] == "v") {
            finishX = obj.vertices[nF].x;
            finishY = obj.vertices[nF].y;
        }
        else {
            finishX = MetaGraph.metaVerticesCenters[nF].x;
            finishY = MetaGraph.metaVerticesCenters[nF].y;
        }

        //console.log(obj.edges[l].start[0]);
        paper.path("M " + startX + " " + startY + " " + finishX + " " + finishY);
    }
}

function changeMode(newMode) {
    mode = newMode;
    console.log(newMode);
    pointSet.undrag();
}

function addEdge() {
    changeMode("addEdge");
    var newEdge = { v1: null, v2: null, path: null };
    var click = function () {

        if (mode == "addEdge") {
            if (newEdge.v1 == null) {
                newEdge.v1 = this.id;
                console.log(newEdge.v1);
            } else {
                newEdge.v2 = this.id;
                newEdge.path = paper.path("M " + vertices[newEdge.v1].vX + " " + vertices[newEdge.v1].vY + " "
                + vertices[newEdge.v2].vX + " " + vertices[newEdge.v2].vY).toBack();
                edges.push(newEdge);

                console.log(newEdge.v1 + ":" + newEdge.v2);
                newEdge.v1 = null;
            }
        }
    };
    pointSet.click(click);

}

function seeNodes() {
    for (var i = 0; i < vertices.length; ++i) {
        if (pointSet[i].id != null) {
            console.log(vertices[i].vX + ":" + vertices[i].vY + "| id=" + vertices[i].id + " value=" + vertices[i].value);
            //console.log(vertices[i].vX + ":" + vertices[i].vY + "| id=" + vertices[i].id + " value=" + vertices[i].value);
            //console.log("pID:"+pointSet[i].id);
        }
    }
}

function moveVertex() {
    changeMode("moveVertex");

    if (mode == "moveVertex") {
        var start = function () {
            this.ox = this.attr("cx");
            this.oy = this.attr("cy");
            this.animate({
                //r: 20,
                "stroke-width": 2
            }, 100, ">");
            if (this.attr.r == 80) {
                this.toBack();
            } else {
                this.toFront();
            }
            // Взята вершина завжди стає на передній план
            //return this.id;
        },

            move = function (dx, dy) {
                this.attr({
                    cx: this.ox + dx,
                    cy: this.oy + dy
                });
            },

            end = function () {
                this.animate({
                    //r: 15,
                    "stroke-width": 1
                }, 100, ">");
                console.log("move id:" + this.id);
                vertices[this.id].vX = this.attr("cx");
                vertices[this.id].vY = this.attr("cy");
            };

        pointSet.drag(move, start, end);
    }
}

function rmVertex() {
    changeMode("rmVertex");

    var click = function () {
        if (mode == "rmVertex") {
            this.remove();
        }
    };
    pointSet.click(click);
}

function addVertex(x, y) {
    changeMode("addVertex");

    if (arguments.length == 0) {
        paper.canvas.onclick = function () {
            if (mode == "addVertex") {
                var mX = event.offsetX;
                var mY = event.offsetY;

                var point = paper.circle(mX, mY, 5);
                point.attr({
                    fill: "yellow",
                    id: nodeN
                });
                pointSet.push(point);

                vertices.push({
                    vX: mX,
                    vY: mY,
                    value: null
                    //id: nodeN
                });
                nodeN++;

                //point.node.onclick = function () {
                //};
                console.log("added new NODE: " + mX + '-' + mY);
                return 0; ///ВОЗМОЖНО ИЗ ЗА ТОГО, ЧТО НЕ СТАВИЛ RETURN БЫЛИ ОШИБКИ!!!!!
            }
        }
    }
    var point = paper.circle(x, y, 5);
    point.attr({
        fill: "yellow",
        id: nodeN
    });
    pointSet.push(point);
    return 0;
}


function addC() {
    changeMode("addC");
    var i;
    for (i = 0; i < pointSet.length; ++i) {
        if (pointSet[i].attr("r") > 35) {
            paper.circle(pointSet[i].attr("cx"), pointSet[i].attr("cy"), pointSet[i].attr("r"));
        }

    }
}

function addMetaVertex() {
    changeMode("addMetaVertex");

    paper.canvas.onclick = function () {
        if (mode == "addMetaVertex") {
            var mX = event.offsetX;
            var mY = event.offsetY;

            var point = paper.circle(mX, mY, 5);
            //var point2 = paper.circle(mX, mY, 80);

            point.attr({
                fill: "green",
                id: nodeN
            });
            pointSet.push(point);



            vertices.push({
                vX: mX,
                vY: mY,
                value: null
                //id: nodeN
            });
            nodeN++;

            //point.node.onclick = function () {
            //};
            console.log("added new NODE: " + mX + '-' + mY);
        }
    }
}


function addInfo() {
    //pointSet.undrag(this);
    changeMode("addInfo");

    var click1 = function () {
        if (mode == "addInfo") {
            vertices[this.id].value = prompt("Вершина " + this.id + ": ", "значення");
        }

        // ---???--- console.log(this.value+"121");
    };
    pointSet.click(click1);
}

function seeInfo() {

    changeMode("seeInfo");

    var click1 = function () {
        //if (mode == "seeInfo"){
        alert(vertices[this.id].vX + ":" + vertices[this.id].vY + "| id=" + vertices[this.id].id + " value="
        + vertices[this.id].value);
        //}
    };
    pointSet.click(click1);
}



function buildGraph(obj) {

    //paper.rect(5, 5, 100, 100);

    //var obj = JSON.parse(jsonData);
    var metaVerticesLenghts = obj.Vertices.length;

    for (var i = 0 ; i < obj.Vertices.length; ++i) {
        //console.log(obj.Vertices[i].IncludedVertices);
        if (obj.Vertices[i].IncludedVertices.length == 0) {
            metaVerticesLenghts--;
            addVertex(obj.Vertices[i].Coordinates.X, obj.Vertices[i].Coordinates.Y);
            //paper.print(obj.Vertices[i].Coordinates.X, obj.Vertices[i].Coordinates.Y, "test", paper.getFont("Times", 800), 30, "baseline", -0.8);
            var p1 = "", p2 = "", p3 = "", p4 = "";

            for (var k = 0; k < 24; k++) {
                //console.log("k" + k + ": " + obj.Vertices[i].Name[k]);

                if (k < 12 && obj.Vertices[i].Name[k] != undefined) {
                    p1 = p1 + obj.Vertices[i].Name[k];
                }
                                     
                if (k >= 12 && k < 24 && obj.Vertices[i].Name[k] != undefined) {
                    p2 = p2 + obj.Vertices[i].Name[k];
                }

                if (k < 12 && obj.Vertices[i].NameLP[k] != undefined) {
                    p3 = p3 + obj.Vertices[i].NameLP[k];
                }

                if (k >= 12 && k < 24 && obj.Vertices[i].NameLP[k] != undefined) {
                    p4 = p4 + obj.Vertices[i].NameLP[k];
                }
                    
            }

            //paper.text(obj.Vertices[i].Coordinates.X, obj.Vertices[i].Coordinates.Y, obj.Vertices[i].Name);
            //paper.text(obj.Vertices[i].Coordinates.X, obj.Vertices[i].Coordinates.Y - 10, p3);
            //paper.text(obj.Vertices[i].Coordinates.X, obj.Vertices[i].Coordinates.Y - 3, p4);
            //paper.text(obj.Vertices[i].Coordinates.X, obj.Vertices[i].Coordinates.Y + 3, p1);
            //paper.text(obj.Vertices[i].Coordinates.X, obj.Vertices[i].Coordinates.Y + 10, p2);
        }  
    }

    //for (var j = 0; j < obj.metaVertices.length; ++j) {
    for (var j = 0; j < metaVerticesLenghts; ++j) {

        var left = { x: 10000, y: 10000 }, top = { x: 10000, y: 10000 }, right = { x: 0, y: 0 }, bottom = { x: 0, y: 0 };

        for (var k = 0; k < obj.Vertices[j].IncludedVertices.length; ++k) {

            console.log("Metavertices: " + obj.Vertices[j].IncludedVertices.length);

            if (obj.Vertices[j].IncludedVertices[k].Coordinates.X < left.x) {
                left.x = obj.Vertices[j].IncludedVertices[k].Coordinates.X;
                left.y = obj.Vertices[j].IncludedVertices[k].Coordinates.Y;
                console.log("X: " + obj.Vertices[j].IncludedVertices[k].Coordinates.X + "Y: " + obj.Vertices[j].IncludedVertices[k].Coordinates.Y);
            }
            if (obj.Vertices[j].IncludedVertices[k].Coordinates.Y < top.y) {
                top.y = obj.Vertices[j].IncludedVertices[k].Coordinates.Y;
                top.x = obj.Vertices[j].IncludedVertices[k].Coordinates.X;
            }
            if (obj.Vertices[j].IncludedVertices[k].Coordinates.X > right.x) {
                right.x = obj.Vertices[j].IncludedVertices[k].Coordinates.X;
                right.y = obj.Vertices[j].IncludedVertices[k].Coordinates.Y;
            }
            if (obj.Vertices[j].IncludedVertices[k].Coordinates.Y > bottom.y) {
                bottom.y = obj.Vertices[j].IncludedVertices[k].Coordinates.Y;
                bottom.x = obj.Vertices[j].IncludedVertices[k].Coordinates.X;
            }
        }

        //Побудова та розфарбування метавершин
        var CONST_META_DELTA = 10;
        var myColor = "rgba(" + obj.Vertices[j].Color.R + "%, " + obj.Vertices[j].Color.G + "%, " + obj.Vertices[j].Color.B + "%, " + "30%)";
        paper.path("M " + (left.x - CONST_META_DELTA) + " " + left.y + " R" + top.x + " "
        + (top.y - CONST_META_DELTA) + " " + (right.x + CONST_META_DELTA) + " " + right.y + " " + bottom.x + " " + (bottom.y + CONST_META_DELTA) + " z").toBack().attr({ fill: myColor });

        //paper.circle(left.x-30, left.y, 2);
        //paper.circle(right.x+30, right.y, 2);
        //paper.circle(top.x, top.y-30, 2);
        //paper.circle(bottom.x, bottom.y+30, 2);

        MetaGraph.metaVerticesCenters[j] = new Object();
        MetaGraph.metaVerticesCenters[j].x = (right.x + left.x) / 2;
        MetaGraph.metaVerticesCenters[j].y = (top.y + bottom.y) / 2;
        //console.log(MetaGraph.metaVerticesCenters[j]);

        //Центр метавершини
        var myColorCenter = "rgba(" + obj.Vertices[j].Color.R + "%, " + obj.Vertices[j].Color.G + "%, " + obj.Vertices[j].Color.B + "%, " + "100%)";
        paper.circle(MetaGraph.metaVerticesCenters[j].x, MetaGraph.metaVerticesCenters[j].y, 1).attr({ fill: myColorCenter });

    }

    for (var l = 0; l < obj.Edges.length; ++l) {

        var startX, startY, finishX, finishY;
        var nS = obj.Edges[l].StartVertex.Content; //
        var nF = obj.Edges[l].EndVertex.Content; //
        // Додати пізніше try-catch
        if (obj.Edges[l].StartVertex.Name[0] != "M") {
            startX = obj.Edges[l].StartVertex.Coordinates.X;
            startY = obj.Edges[l].StartVertex.Coordinates.Y;
        }
        else {
            startX = MetaGraph.metaVerticesCenters[nS].x;
            startY = MetaGraph.metaVerticesCenters[nS].y;
        }
        

        if (obj.Edges[l].EndVertex.Name[0] != "M") {
            finishX = obj.Edges[l].EndVertex.Coordinates.X;
            finishY = obj.Edges[l].EndVertex.Coordinates.Y;
        }
        else {
            finishX = MetaGraph.metaVerticesCenters[nF].x;
            finishY = MetaGraph.metaVerticesCenters[nF].y;
            
        }

        //console.log(obj.edges[l].start[0]);
        paper.path("M " + startX + " " + startY + " " + finishX + " " + finishY);
    }
}

window.designer = {
	package: {},
	items: [],
	selectedItem: {},
	isNewItemSelected: true,
	r: "",
	init: function(id) {
	    this.getPackage(id);
	},
	redraw: function() {
		var pkg = this.package;
		this.buildDesigner(pkg.width, pkg.height);
	},
	addNewItem: function() {
		this.clearEditValues();
		this.selectedItem = {
			id: (new Date()).getTime(), // generatenewId
			title: "",
			name: "",
			description: "",
			fields: [],
			positionX: 10,
			positionY: 10
		};
		this.isNewItemSelected = true;
		this.showItem();
	},
	addField: function() {
		var relatedItem = this.getItemValue("relatedItem").value;
		this.selectedItem.fields.push({
			name: this.getItemValue("fieldName"),
			title: this.getItemValue("fieldTitle"),
			type: "Connection",
			typeTitle: relatedItem,
			relatedItem: relatedItem
		});
		this.setItemValue("fieldName", "");
		this.setItemValue("fieldTitle", "");
		this.generateFieldsView();
		this.proceedTypeSelect();
	},
	generateFieldsView: function() {
		var fieldItems = this.selectedItem.fields;
		var html = "<ul>";
		fieldItems.forEach(function(item) {
			html = html + "<li style = 'cursor: pointer;' onclick='designer.showField(\"" + item.name + "\")'>" + item.title + " (" + item.typeTitle + ")</li>"
		}, this);
		html = html + "</ul>";
		this.setItemHTML("fieldsHost", html);
	},
	showField: function(name) {
		var field = "";
		var fieldItems = this.selectedItem.fields;
		fieldItems.forEach(function(item) {
			if (item.name === name) {
				field = item;
			}
		}, this);
		this.setItemValue("fieldName", field.name);
		this.setItemValue("fieldTitle", field.title);
		this.setItemValue("fieldType", field.type);
		this.proceedTypeSelect();
		this.setItemValue("relatedItem", field.relatedItem);
		this.setElementVisibility(true, "saveFieldButton");
		this.setElementVisibility(true, "deleteFieldButton");
		this.setElementVisibility(false, "addFieldButton");
	},
	proceedTypeSelect: function() {
		var select = document.getElementById("relatedItem");
		select.options.length = 0;
		var items = this.package.items;
		items.forEach(function(item) {
			var option = document.createElement("option");
			option.text = item.title;
			option.value = item.name;
			select.appendChild(option);
		}, this);
	},
	showItem: function(id) {
		if (id) {
			for (var i = 0; i < this.package.items.length; i++) {
				if (this.package.items[i].id === id) {
					this.selectedItem = this.package.items[i];
					this.isNewItemSelected = false;
				}
			}
		}
		if (this.isNewItemSelected) {
			this.setElementVisibility(false, "deleteItemButton");
			this.setElementVisibility(true, "cancelItemButton");
		} else {
			this.setItemValue("itemTitle", this.selectedItem.title);
			this.setItemValue("itemName", this.selectedItem.name);
			this.setItemValue("itemDescription", this.selectedItem.description);
			this.setElementVisibility(false, "cancelItemButton");
			this.setElementVisibility(true, "deleteItemButton");
		}
		this.generateFieldsView();
		this.proceedTypeSelect();
		this.setElementVisibility(true, "itemEditor");
	},
	cancelItem: function(){
		this.selectedItem = {};
		this.setElementVisibility(false, "itemEditor");
		this.clearEditValues();

	},
	deleteItem: function(){
		var index = -1;
		for (var i = 0; i < this.package.items.length; i++) {
			if (this.package.items[i].id === this.selectedItem.id) {
				index = i;
			}
		}
		if(index > -1) {
			this.selectedItem = {};
			this.package.items.splice(index, 1);
		}

		this.setElementVisibility(false, "itemEditor");
		this.clearEditValues();
		this.redraw();
	},
	applyItem: function() {
		this.selectedItem.title = this.getItemValue("itemTitle");
		this.selectedItem.name = this.getItemValue("itemName");
		this.selectedItem.description = this.getItemValue("itemDescription");
		if (this.isNewItemSelected) {
			this.package.items.push(this.selectedItem);
		} else {
			for (var i = 0; i < this.package.items.length; i++) {
				if (this.package.items[i].id === this.selectedItem.id) {
					this.package.items[i] = this.selectedItem;
				}
			}
		}
		this.setElementVisibility(false, "itemEditor");
		this.clearEditValues();
		this.redraw();
	},
	clearEditValues: function() {
		this.setItemValue("itemTitle", "");
		this.setItemValue("itemName", "");
		this.setItemValue("itemDescription", "");
	},
	getItemValue: function(elementName) {
		var item = document.getElementById(elementName);
		if (item) {
			if (item.options) {
				return {
					value: item.options[item.selectedIndex].value,
					displayValue: item.options[item.selectedIndex].text
				}
			}
			return item.value;
		}
	},
	setItemHTML: function(elementName, html) {
		var item = document.getElementById(elementName);
		if (item) {
			item.innerHTML = html;
		}
	},
	setItemValue: function(elementName, value) {
		var item = document.getElementById(elementName);
		if (item) {
			item.value = value;
		}
	},
	setElementVisibility: function(isVisible, elementName) {
		var itemEditor = document.getElementById(elementName);
		if (itemEditor) {
			if (isVisible) {
				itemEditor.style.display = 'block';
			} else {
				itemEditor.style.display = 'none';
			}
		}
	},
	buildDesigner: function(width, height) {
		var dragger = function() {
			this.ox = this.type == "ellipse" ? this.attr("cx") : this.attr("x");
			this.oy = this.type == "ellipse" ? this.attr("cy") : this.attr("y");
			this.pair.ox = this.pair.type == "ellipse" ? this.pair.attr("cx") : this.pair.attr("x");
			this.pair.oy = this.pair.type == "ellipse" ? this.pair.attr("cy") : this.pair.attr("y");
			designer.showItem(this.dataItemId);
		};
		var move = function(dx, dy) {
			var att = this.type == "ellipse" ? {cx: this.ox + dx, cy: this.oy + dy} :
			{x: this.ox + dx, y: this.oy + dy};
			this.attr(att);
			att = this.pair.type == "ellipse" ? {cx: this.pair.ox + dx, cy: this.pair.oy + dy} :
			{x: this.pair.ox + dx, y: this.pair.oy + dy};
			this.pair.attr(att);
			for (i = connections.length; i--;) {
				r.connection(connections[i]);
			}
			r.safari();
		};
		var up = function() {
			if (this.type != "text") {
				designer.setItemPosition(this.dataItemId, this.attrs.x, this.attrs.y);
			}
		};
		if (!this.r) {
			this.r = Raphael("holder", 10000, 10000);
		} else {
			this.r.clear();
		}
		var r = this.r;
		var packageItem = this.package;
		var items = packageItem.items;
		var shapes = [];
		var connections = [];
		items.forEach(function(item) {
			var shape = r.rect(item.positionX, item.positionY, 150, 40, 10);
			shape.attr({fill: "#2A83BD", cursor: "move", text: item.title});
			shape.dataItemId = item.id;
			var textDeviation = 75;//item.positionX + 75 - (item.title.length/2);
			var text = r.text(item.positionX + textDeviation, item.positionY + 20, item.title);
			text.attr({fill: "#EBEBEB", stroke: "none", "font-size": 12, "font-family": "Consolas", cursor: "move"});
			shape.drag(move, dragger, up);
			text.drag(move, dragger, up);
			shape.pair = text;
			text.pair = shape;
			shapes.push(shape);
		}, this);
		for (var k = 0; k < items.length; k++) {
			var item = items[k];
			for (var i = 0; i < item.fields.length; i++) {
				for (var j = 0; j < items.length; j++) {
					if (items[j].name === item.fields[i].relatedItem) {
						connections.push(r.connection(shapes[k], shapes[j], "#fff"));
					}
				}
			}
		}
	},
	setItemPosition: function(id, x, y) {
		this.package.items.forEach(function(item) {
			if (item.id === id) {
				item.positionX = x;
				item.positionY = y;
			}
		}, this);
	},
	getPackage: function(id) {
		var xmlhttp;
		if (window.XMLHttpRequest){// code for IE7+, Firefox, Chrome, Opera, Safari
			xmlhttp=new XMLHttpRequest();
		}
		else{// code for IE6, IE5
			xmlhttp=new ActiveXObject("Microsoft.XMLHTTP");
		}
		xmlhttp.onreadystatechange=function(){
			if (xmlhttp.readyState==4 && xmlhttp.status==200)
			{
			    var data = JSON.parse(xmlhttp.response);
			    var pkg = JSON.parse(data);
			    designer.package = pkg;
			    console.log(pkg);
			    buildGraph(pkg);
			    designer.buildDesigner(pkg.width, pkg.height);
			    designer.setItemValue("packageName", pkg.name);
			}
		}
		xmlhttp.open("GET","/api/values/4",true);
		xmlhttp.send();		
	}
};

window.onload = function() {
	designer.init("test1");
};
