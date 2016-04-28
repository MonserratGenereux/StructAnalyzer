using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node:MonoBehaviour {

    //Class variables
    public enum SupportType { NONE, HINGED, MOVABLE }
    public static SupportType type;
    public static Main main;

    public GameObject prefabHinged, prefabMovable, prefabForce;
    public List<Node> neighbors = new List<Node>();
    public List<Edge> edges = new List<Edge>();
    public List<Force> forces = new List<Force>();

    Renderer rend;
    Color color;

    public SupportType supType;
    public Reaction x, y;
    public bool supported = false;
    public float supportRotation = 0;
    public float px, py;
    public double[] equation;
    GameObject sup;

    void Awake()
    {
        type = SupportType.NONE;
        px = 0;
        py = 0;
    }

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = Color.gray;
    }

    void Update()
    {
        if (supType != SupportType.NONE)
            sup.transform.position = transform.position;

        calculateForces();
    }
    
    void calculateForces()
    {
        px = 0;
        py = 0;
        for(int i=0; i<forces.Count; i++)
        {
            float tmpx = forces[i].magnitude * Mathf.Cos(Mathf.Deg2Rad * forces[i].angle);
            float tmpy = forces[i].magnitude * Mathf.Sin(Mathf.Deg2Rad * forces[i].angle);
            //Debug.Log("Calculate forces " + i + " " + tmpx + " " + tmpy);
            px += tmpx;
            py += tmpy;
        }
    }

    public void addNeighbor(Node n, Edge e)
    {
        neighbors.Add(n);
        edges.Add(e);
    }

    public void removeNeighbor(Node n)
    {
        neighbors.Remove(n);
    }

    public void removeEdge(Edge e)
    {
        Main.edges.Remove(e);
        edges.Remove(e);
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    void delete()
    {
        int i = 0;
        while(edges.Count > 0)
        { 
        
            edges[0].delete();  
            i++;
        }
        Main.nodes.Remove(this);
        Destroy(gameObject);
    }

    void deleteSupport()
    {
        Destroy(sup);
        Main.reactions.Remove(x);
        Main.reactions.Remove(y);
        x = null;
        y = null;
        supType = SupportType.NONE;
        
    }

    void rotateSupport()
    {
        if (!supported)
        {
            sup.transform.Rotate(new Vector3(0, 0, 90));
            supportRotation += 90;

            if(supType == SupportType.MOVABLE)
            {
                
                if(x == null)
                {
                    Main.reactions.Remove(y);
                    x = new Reaction(this, Reaction.Direction.X);
                    y = null;
                    Main.reactions.Add(x);
                }
                else
                {
                    Main.reactions.Remove(x);
                    y = new Reaction(this, Reaction.Direction.Y);
                    x = null;
                    Main.reactions.Add(y);
                }
            }

        }
    }

    void addHingedSupport()
    {
        supType = type;
        sup = (GameObject)Instantiate(prefabHinged, transform.position, transform.rotation);
        x = new Reaction(this, Reaction.Direction.X);
        y = new Reaction(this, Reaction.Direction.Y);
        sup.transform.name = this.transform.name + "Hinged";
        Main.reactions.Add(x);
        Main.reactions.Add(y);
        sup.transform.SetParent(Main.Supports.transform);   
    }

    void addMovableSupport()
    {
        supType = type;
        sup = (GameObject)Instantiate(prefabMovable, transform.position, transform.rotation);
        y = new Reaction(this, Reaction.Direction.Y);
        sup.transform.name = this.transform.name + "Movable";
        Main.reactions.Add(y);
        sup.transform.SetParent(Main.Supports.transform);
    }

    void addForce()
    {
        GameObject go = (GameObject)Instantiate(prefabForce, transform.position, Quaternion.identity);
        Force f = go.GetComponent<Force>();
        Main.selectedForce = f;
        forces.Add(f);
        f.setNode(this);
    }

    void OnMouseDrag()
    {
        if (Main.tool == Main.Tools.MOVE)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objectPosition - new Vector3(0, 0, objectPosition.z);
        }
    }

    void OnMouseDown()
    {
        if (Main.tool == Main.Tools.DELETE)
        {
            if (supType != SupportType.NONE)
                deleteSupport();
            else
                delete();
        }

        if(Main.tool == Main.Tools.FORCE)
        {
            addForce();
        }

        if(supType == SupportType.NONE)
        {
            if (Main.tool == Main.Tools.HINGEDSUPP)
                addHingedSupport();

            if (Main.tool == Main.Tools.MOVSUPP)
                addMovableSupport();
        }

        else
            if(Main.tool == Main.Tools.HINGEDSUPP || Main.tool == Main.Tools.MOVSUPP)
                rotateSupport();
    }

    public double[] equationX()
    {
        int size = Main.edges.Count + Main.reactions.Count + 1;
        equation = new double[size];
        equation[size - 1] = -1 * px;
        
        for(int i=0; i<edges.Count; i++)
        {
            int index = edges[i].getIndex();
            float angle = edges[i].getAngleFrom(this);
            equation[index] = Mathf.Cos(angle);

        }

        if(x != null)
        {
            int index = x.getIndex();
            equation[index] = 1;
        }

        return equation;
    }

    public double[] equationY()
    {
        int size = Main.edges.Count + Main.reactions.Count + 1;
        equation = new double[size];
        equation[size - 1] = -1 * py;

        for (int i = 0; i < edges.Count; i++)
        {
            int index = edges[i].getIndex();
            float angle = edges[i].getAngleFrom(this);
            equation[index] = Mathf.Sin(angle);

        }

        if (y != null)
        {
            int index = y.getIndex();
            equation[index] = 1;
        }

        return equation;
    }
}
