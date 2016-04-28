using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour {

    public Node node1, node2;
    Vector3 pos1, pos2;
    public float force;
    public static Main main;
    Renderer rend;
    public Color color;
    Color[] colors;

    void Start()
    {
        rend = GetComponent<Renderer>();
        colors = new Color[10];
        colors[9] = new Color(255, 0, 0);
        colors[8] = new Color(255, 69, 0);
        colors[7] = new Color(255, 140, 0);
        colors[6] = new Color(255, 165, 0);
        colors[5] = new Color(255, 215, 0);
        colors[4] = new Color(240, 230, 140);
        colors[3] = new Color(189, 183, 107);
        colors[2] = new Color(154, 205, 50);
        colors[1] = new Color(107, 142, 35);
        colors[0] = new Color(85, 107, 47);

        for (int i=0; i<colors.Length; i++)
        {
            colors[i].r /= 255f;
            colors[i].g /= 255f;
            colors[i].b /= 255f;
        }
    }



    // Update is called once per frame
    void Update()
    {
        updateCylinder();
        updateColor();
        rend.material.color = color;
    }

    void updateCylinder()
    {
        pos1 = node1.getPosition();
        pos2 = node2.getPosition();
        float width = 0.15f;

        Vector3 offset = pos2 - pos1;
        Vector3 scale = new Vector3(width, offset.magnitude / 2.0f, width);
        Vector3 position = pos1 + (offset / 2f);

        transform.position = position;
        transform.rotation = Quaternion.identity;
        transform.up = offset;
        transform.localScale = scale;
    }

    public void setNodes(Node start, Node end)
    {
        node1 = start;
        node2 = end;
    }

    public void delete()
    {
        node1.removeNeighbor(node2);
        node2.removeNeighbor(node1);
        node1.removeEdge(this);
        node2.removeEdge(this);
        Main.edges.Remove(this);
        Destroy(gameObject);
    }

    public void rename()
    {
        transform.name = node1.transform.name + node2.transform.name;
    }

    public int getIndex()
    {
        int index = Main.edges.FindIndex((Edge e) => { return e.transform.name == this.transform.name; });
        return index;
    }

    public float getAngleFrom(Node n)
    {
        float angle = -1;
        Vector3 dir;
        if(node1 == n)
        {
            dir = node2.transform.position - node1.transform.position;
            dir = node2.transform.InverseTransformDirection(dir);
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        if(node2 == n)
        {
            dir = node1.transform.position - node2.transform.position;
            dir = node1.transform.InverseTransformDirection(dir);
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        return angle;
    }

    void OnMouseDown()
    {
        if (Main.tool == Main.Tools.DELETE)
            delete();
    }

    void updateColor()
    {
        int otro = (int)((force) / 10f);
        color = colors[otro];
    }

}
