using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;
    public int smoothingPasses;
    public int[,] map;

    private const int EMPTY = 0;
    private const int WALL = 1;

    void Start() {
        GenerateMap();
    }

    void GenerateMap() {
        map = new int[width, height];
        RandomFillMap();
        for(int i = 0; i < smoothingPasses; i++) {
            SmoothMap();
        }

        int borderSize = 5;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++) {
            for (int y = 0; y < borderedMap.GetLength(1); y++) {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                } else {
                    borderedMap[x, y] = WALL;
                }
            }
        }

        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(map, 1);
    }

    void Update() {
        if(Input.GetButtonUp("Fire1")) {
            GenerateMap();
        }
    }

    void RandomFillMap() {
        if(useRandomSeed) {
            seed = Time.time.ToString();
        }

        System.Random randomNumberGenerator = new System.Random(seed.GetHashCode());

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if(x == 0 || x == width-1 || y == 0 || y == height - 1) {
                    map[x, y] = WALL;
                } else {
                    map[x, y] = randomNumberGenerator.Next(0, 100) < randomFillPercent ? WALL : EMPTY;
                }
            }
        }
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if(neighbourWallTiles > 4) {
                    map[x, y] = WALL;
                } else if(neighbourWallTiles < 4) {
                    map[x, y] = EMPTY;
                }
            }
        }
    }

    int GetSurroundingWallCount(int x, int y) {
        int wallCount = 0;

        for(int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++) {
                if(neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    if (neighbourX != x || neighbourY != y) {
                        wallCount += map[neighbourX, neighbourY];
                    }
                } else {
                    wallCount++;
                }
            } 
        }

        return wallCount;
    }
}
