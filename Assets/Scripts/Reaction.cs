using UnityEngine;
using System.Collections;

public class Reaction {

    public enum Direction { X, Y };
    public Direction dir;
    public float value;
    public Node node;
    public string name;
    public GameObject gameObject;

    public Reaction(Node n, Direction d)
    {
        node = n;
        dir = d;
        rename();
    }

    public void rename()
    {
        if (dir == Direction.X)
            name = "RX" + node.transform.name;

        else
            name = "RY" + node.transform.name;
    }

    public void print()
    {

    }

    public int getIndex()
    {
        int index = Main.reactions.FindIndex((Reaction r) => { return r.name == this.name; });
        index += Main.edges.Count;
        return index;
    }

}
