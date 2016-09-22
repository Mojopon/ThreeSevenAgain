using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using System.Collections.Generic;

namespace ThreeSeven.Helper {
    public static class DropHelper
    {
        public static TwoDimensionalMovement[] DropThreeSevenGrid(this bool[,] grid)
        {
            // this grid returns true when the cell is empty
            // and returns false when the cell is occupied

            var size = grid.Size();
    
            List<TwoDimensionalMovement> objectMovements = new List<TwoDimensionalMovement>();
            //TwoDimensionalMovements objectMovements = new TwoDimensionalMovements(Size);

            for (int y = size.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    if (grid[x, y]) continue;

                    int i = 0;
                    int verticalMove = 0;
                    while (true)
                    {
                        i++;

                        // add movement when the space below the block is empty to know
                        // how many times drops the block
                        if (grid.IsOutOfRange(Point<int>.At(x, y + i)) ||
                           !grid[x, y + i])
                        {
                            break;
                        }

                        verticalMove++;
                    }

                    if (verticalMove == 0) continue;

                    var movement = new TwoDimensionalMovement(Point<int>.At(x, y), Point<int>.At(x, y + verticalMove));
                    objectMovements.Add(movement);
                    grid.Swap(movement);
                }
            }

            return objectMovements.ToArray();
        }
    }

}