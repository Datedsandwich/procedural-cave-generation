using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public SquareGrid squareGrid;
    List<Vector3> vertices;
    List<int> triangles;

    public void GenerateMesh(int[,] map, float squareSize) {
        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.SetVertices(vertices);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    void TriangulateSquare(Square square) {
        switch(square.configuration) {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 2:
                MeshFromPoints(square.centerRight, square.bottomRight, square.centerBottom);
                break;
            case 4:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }
    }

    void MeshFromPoints(params Node[] points) {
        AssignVertices(points);

        if(points.Length >= 3) {
            CreateTriangle(points[0], points[1], points[2]);
        }

        if(points.Length >= 4) {
            CreateTriangle(points[0], points[2], points[3]);
        }

        if(points.Length >= 5) {
            CreateTriangle(points[0], points[3], points[4]);
        }

        if(points.Length == 6) {
            CreateTriangle(points[0], points[4], points[5]);
        }
    }

    void AssignVertices(params Node[] points) {
        for(int i = 0; i < points.Length; i++) {
            if(points[i].vertexIndex == -1) {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c) {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
    }

    void OnDrawGizmos() {
        //if(squareGrid != null) {
        //    for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
        //        for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
        //            drawGizmoForNode(squareGrid.squares[x, y].topLeft);
        //            drawGizmoForNode(squareGrid.squares[x, y].topRight);
        //            drawGizmoForNode(squareGrid.squares[x, y].bottomRight);
        //            drawGizmoForNode(squareGrid.squares[x, y].bottomLeft);

        //            drawGizmoForNode(squareGrid.squares[x, y].centerTop);
        //            drawGizmoForNode(squareGrid.squares[x, y].centerRight);
        //            drawGizmoForNode(squareGrid.squares[x, y].centerBottom);
        //            drawGizmoForNode(squareGrid.squares[x, y].centerLeft);
        //        }
        //    }
        //}
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
        public int configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft) {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            centerTop = topLeft.rightNode;
            centerRight = bottomRight.aboveNode;
            centerBottom = bottomLeft.rightNode;
            centerLeft = bottomLeft.aboveNode;

            if(topLeft.active) {
                configuration += 8;
            }

            if(topRight.active) {
                configuration += 4;
            }

            if(bottomRight.active) {
                configuration += 2;
            }

            if(bottomLeft.active) {
                configuration += 1;
            }
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
