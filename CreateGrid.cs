

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse {

    public class CreateGrid
    {

      
        public const string StudentAuthorName = "Andy Zhao";


        // Helper method provided to help you implement this file. Leave as is.
        // Returns true if point p is inside (or on edge) the polygon defined by pts (CCW winding). False, otherwise
        static bool IsPointInsidePolygon(Vector2Int[] pts, Vector2Int p)
        {
            return CG.InPoly1(pts, p) != CG.PointPolygonIntersectionType.Outside;
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Returns float converted to int according to default scaling factor (1000)
        static int Convert(float v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
        static Vector2Int Convert(Vector2 v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns true is segment AB intersects CD properly or improperly
        static bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            return CG.Intersect(a, b, c, d);
        }


        // IsPointInsideBoundingBox(): Determines whether a point (Vector2Int:p) is On/Inside a bounding box (such as a grid cell) defined by
        // minCellBounds and maxCellBounds (both Vector2Int's).
        // Returns true if the point is ON/INSIDE the cell and false otherwise
        // This method should return true if the point p is on one of the edges of the cell.
        // This is more efficient than PointInsidePolygon() for an equivalent dimension poly
        // Preconditions: minCellBounds <= maxCellBounds, per dimension
        static bool IsPointInsideAxisAlignedBoundingBox(Vector2Int minCellBounds, Vector2Int maxCellBounds, Vector2Int p)
        {
             return p.x >= minCellBounds.x && p.x <= maxCellBounds.x && p.y >= minCellBounds.y && p.y <= maxCellBounds.y;
        }




        // IsRangeOverlapping(): Determines if the range (inclusive) from min1 to max1 overlaps the range (inclusive) from min2 to max2.
        // The ranges are considered to overlap if one or more values is within the range of both.
        // Returns true if overlap, false otherwise.
        // Preconditions: min1 <= max1 AND min2 <= max2
        static bool IsRangeOverlapping(int min1, int max1, int min2, int max2)
        {
            return min1 <= max2 && max1 >= min2;
        }

        // IsAxisAlignedBouningBoxOverlapping(): Determines if the AABBs defined by min1,max1 and min2,max2 overlap or touch
        // Returns true if overlap, false otherwise.
        // Preconditions: min1 <= max1, per dimension. min2 <= max2 per dimension
        static bool IsAxisAlignedBoundingBoxOverlapping(Vector2Int min1, Vector2Int max1, Vector2Int min2, Vector2Int max2)
        {

            bool Check_X_Overlapping = IsRangeOverlapping(min1.x, max1.x, min2.x, max2.x);
            bool Check_Y_Overlapping = IsRangeOverlapping(min1.y, max1.y, min2.y, max2.y);
            return Check_X_Overlapping && Check_Y_Overlapping;
        }

        // IsTraversable(): returns true if the grid is traversable from grid[x,y] in the direction dir, false otherwise.
        // The grid boundaries are not traversable. If the grid position x,y is itself not traversable but the grid cell in direction
        // dir is traversable, the function will return false.
        // returns false if the grid is null, grid rank is not 2 dimensional, or any dimension of grid is zero length
        // returns false if x,y is out of range
        // Note: public methods are autograded
        public static bool IsTraversable(bool[,] grid, int x, int y, TraverseDirection dir)
        {
            if (grid == null || grid.Rank != 2 || grid.GetLength(0) == 0 || grid.GetLength(1) == 0)
                return false;
            int MAX_X = grid.GetLength(0);
            int MAX_Y = grid.GetLength(1);
            if (x < 0 || y < 0 || x >= MAX_X || y >= MAX_Y || !grid[x, y])
                return false;
            int NEW_X = x, NEW_Y = y;
            switch (dir)
            {
                case TraverseDirection.Up:
                    NEW_Y += 1;
                    break;
                case TraverseDirection.Down:
                    NEW_Y -= 1;
                    break;
                case TraverseDirection.Left:
                    NEW_X -= 1;
                    break;
                case TraverseDirection.Right:
                    NEW_X += 1;
                    break;
                case TraverseDirection.UpLeft:
                    NEW_X -= 1;
                    NEW_Y += 1;
                    break;
                case TraverseDirection.UpRight:
                    NEW_X += 1;
                    NEW_Y += 1;
                    break;
                case TraverseDirection.DownLeft:
                    NEW_X -= 1;
                    NEW_Y -= 1;
                    break;
                case TraverseDirection.DownRight:
                    NEW_X += 1;
                    NEW_Y -= 1;
                    break;
            }

            return NEW_X >= 0 && NEW_Y >= 0 && NEW_X < MAX_X && NEW_Y < MAX_Y && grid[NEW_X, NEW_Y];
        }


        // Create(): Creates a grid lattice discretized space for navigation.
        // canvasOrigin: bottom left corner of navigable region in world coordinates
        // canvasWidth: width of navigable region in world dimensions
        // canvasHeight: height of navigable region in world dimensions
        // cellWidth: target cell width (of a grid cell) in world dimensions
        // obstacles: a list of collider obstacles
        // grid: an array of bools. A cell is true if navigable, false otherwise
        //    Example: grid[x_pos, y_pos]

        public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight, float cellWidth, List<Polygon> obstacles, out bool[,] grid)
         {
            int X_GRID_SIZE = Mathf.FloorToInt(canvasWidth / cellWidth);
            int Y_GRID_SIZE = Mathf.FloorToInt(canvasHeight / cellWidth);

            X_GRID_SIZE = X_GRID_SIZE > 0 ? X_GRID_SIZE : 1;
            Y_GRID_SIZE = Y_GRID_SIZE > 0 ? Y_GRID_SIZE : 1;

            grid = new bool[X_GRID_SIZE, Y_GRID_SIZE];
            for (int i = 0; i < X_GRID_SIZE; i++)
            {
                for (int j = 0; j < Y_GRID_SIZE; j++)
                {
                    Vector2Int minCellBounds = new Vector2Int(Convert(canvasOrigin.x + i * cellWidth), Convert(canvasOrigin.y + j * cellWidth));
                    Vector2Int maxCellBounds = new Vector2Int(Convert(canvasOrigin.x + (i + 1) * cellWidth), Convert(canvasOrigin.y + (j + 1) * cellWidth));
                    bool Blocking = false;
                    foreach (var obstacle in obstacles)
                    {
                        Vector2Int[] obstaclePoints = obstacle.getIntegerPoints();
                        if (IsPointInsidePolygon(obstaclePoints, minCellBounds) || IsAxisAlignedBoundingBoxOverlapping(minCellBounds, maxCellBounds, obstacle.MinIntBounds, obstacle.MaxIntBounds))
                        {
                            isBlocked = true;
                            break;
                        }
                    }
                    grid[i, j] = !Blocking;
                }
            }
        }
    }
}