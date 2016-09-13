using UnityEngine;
using System.Collections;
using ThreeSeven.Model;
using System.Collections.Generic;

public struct TwoDimensionalMovement
{
    public Point<int> source      { get; private set; }
    public Point<int> destination { get; private set; }

    public TwoDimensionalMovement(Point<int> source, Point<int> destination)
    {
        this.source      = source;
        this.destination = destination;
    }

    // sourceとDestinationが同じでない場合は移動がある＝HasMovementでTrueを返す
    public bool HasMovement { get { return !source.Equals(destination); } }
}

public class TwoDimensionalMovements
{
    private TwoDimensionalMovement[,] movements; 

    public TwoDimensionalMovements(Size<int> size)
    {
        movements = new TwoDimensionalMovement[size.Width, size.Height];
    }

    public TwoDimensionalMovement this[int x, int y]
    {
        get
        {
            return movements[x, y];
        }
        set
        {
            movements[x, y] = value;
        }
    }

    public void SetDestination(Point<int> source, Point<int> destination)
    {
        this[source.X, source.Y] = new TwoDimensionalMovement(source, destination);
    }

    public TwoDimensionalMovement[] ToArray()
    {
        List<TwoDimensionalMovement> movementsList = new List<TwoDimensionalMovement>();
        movements.ForEach((point, cell) =>
        {
            if(cell.HasMovement)
            {
                movementsList.Add(cell);
            }
        });

        return movementsList.ToArray();
    }
}