using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public SquareGrid squareGrid;

    public void GenerateMesh(int[,] map, float squareSize) {
        squareGrid = new SquareGrid(map, squareSize);
    }

    void OnDrawGizmos() {
        if(squareGrid != null) {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
                    drawGizmoForNode(squareGrid.squares[x, y].topLeft);
                    drawGizmoForNode(squareGrid.squares[x, y].topRight);
                    drawGizmoForNode(squareGrid.squares[x, y].bottomRight);
                    drawGizmoForNode(squareGrid.squares[x, y].bottomLeft);

                    drawGizmoForNode(squareGrid.squares[x, y].centerTop);
                    drawGizmoForNode(squareGrid.squares[x, y].centerRight);
                    drawGizmoForNode(squareGrid.squares[x, y].centerBottom);
                    drawGizmoForNode(squareGrid.squares[x, y].centerLeft);
                }
            }
        }
    }

    void drawGizmoForNode(ControlNode node) {
        Gizmos.color = node.active ? Color.black : Color.white;
        Gizmos.DrawCube(node.position, Vector3.one * 0.4f);
    }

    void drawGizmoForNode(Node node) {
        Gizmos.color = Color.gray;
        Gizmos.DrawCube(node.position, Vector3.one * 0.15f);
    }

    public class SquareGrid {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize) {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for(int x = 0; x < nodeCountX; x++) {
                for(int y = 0; y < nodeCountY; y++) {
                    Vector3 position = new Vector3(-mapWidth / 2 + x * squareSize + squareSize/2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(position, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++) {
                for (int y = 0; y < nodeCountY - 1; y++) {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }

    public class Square {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft) {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            centerTop = topLeft.rightNode;
            centerRight = bottomRight.aboveNode;
            centerBottom = bottomLeft.rightNode;
            centerLeft = bottomLeft.aboveNode;
        }
    }

	public class Node {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 position) {
            this.position = position;
        }
    }

    public class ControlNode : Node {
        public bool active;
        public Node aboveNode;
        public Node rightNode;

        public ControlNode(Vector3 position, bool active, float size) : base(position) {
            this.active = active;
            aboveNode = new Node(position + Vector3.forward * size/2);
            rightNode = new Node(position + Vector3.right * size / 2);
        }
    }
}
