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

    public override string ToString()
    {
        return string.Format("from ({0}, {1}) to ({2}, {3})", source.X, source.Y, destination.X, destination.Y);
    }
}

/*
public class TwoDimensionalMovementsGrid
{
    private TwoDimensionalMovement[,] movements; 

    public TwoDimensionalMovementsGrid(Size<int> size)
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

    public void SetMovement(Point<int> source, Point<int> destination)
    {
        this[source.X, source.Y] = new TwoDimensionalMovement(source, destination);
    }

    public TwoDimensionalMovement[] ToArray()
    {
        List<TwoDimensionalMovement> movementsList = new List<TwoDimensionalMovement>();
        movements.ForEachFromBottomToTop((point, cell) =>
        {
            if(cell.HasMovement)
            {
                movementsList.Add(cell);
            }
        });

        return movementsList.ToArray();
    }
}
*/