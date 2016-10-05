           <script type="text/javascript" src="raphael.js"></script>
<script type="text/javascript">
    var Points = new Array();
    var currentPX = 0, currentPY = 0, currentLine, flag = false;
    var paper = Raphael(document.getElementById("workZone"));
    paper.canvas.ondblclick = function () {
        var mX = event.offsetX;
        var mY = event.offsetY;
        if (mY >= 155) {
            mY = 310;
        }
        else {
            mY = 0;
        }
        Points.push(mX);
        Points.push(mY);
        document.getElementById("Coord2").textContent += mX;
        currentLine = paper.path("M" + currentPX + " " + currentPY + " L " + mX + " " + mY + "");
        currentPX = mX;
        currentPY = mY;
        var point = paper.circle(mX, mY, 5).attr({ fill: "#000000", stroke: "none", opacity: .5 });
        var start = function () {
            this.ox = this.attr("cx");
            this.oy = this.attr("cy");
            this.animate({ r: 5, opacity: .25 }, 500, ">");
        },
    move = function (dx, dy) {
        this.attr({ cx: this.ox + dx, cy: this.oy + dy });
    },
    up = function () {
        this.animate({ r: 5, opacity: .5 }, 500, ">");
    };

        paper.set(point).drag(move, start, up);
    }

    window.onload = function () {
        for (var x = 0.5; x < 500; x += 10) {
            paper.path("M" + x + " 0 L " + x + " 500");
        };
        for (var y = 0.5; y < 500; y += 10) {
            paper.path("M 0 " + y + " L 500 " + y + "");
        };
    }

</script>