using UnityEngine;
using System.Collections;

public class Force : MonoBehaviour {

    Transform cuerpo;
    Node node;
    public GameObject[] cubes;
    Color color;
    public float lenght;
    public float magnitude, angle;

	// Use this for initialization
	void Start () {
        cuerpo = transform.FindChild("Cuerpo");
        lenght = 2f;
        color = Color.blue;

        cubes = new GameObject[4];
        cubes[0] = transform.GetChild(0).GetChild(0).gameObject;
        cubes[1] = transform.GetChild(1).GetChild(0).gameObject;
        cubes[2] = transform.GetChild(1).GetChild(1).gameObject;
        cubes[3] = transform.GetChild(1).GetChild(2).gameObject;

        changeColor(color);
        magnitude = 50;
        angle = 0;
    }
	
	// Update is called once per frame
	void Update () {

        lenght = magnitude / 20f;
        cuerpo.localScale = new Vector3(cuerpo.localScale.x, lenght, cuerpo.localScale.z);
        transform.rotation = Quaternion.Euler(0, 0, angle+90);
        transform.position = node.transform.position;
        changeColor(color);
        if (Main.selectedForce != null)
            Main.selectedForce.changeColor(Color.green);
	}

    public void changeColor(Color c)
    {
        for(int i=0; i<4; i++)
        {
            cubes[i].GetComponent<Renderer>().material.color = c;
        }
    }

    public void setNode(Node n)
    {
        this.node = n;
    }

    void OnMouseDown()
    {
        if(Main.tool == Main.Tools.DELETE)
        {
            node.forces.Remove(this);
            Destroy(gameObject);
            Main.selectedForce = null;
        }

        if (Main.tool == Main.Tools.DELETE)
        {
            Main.selectedForce = this;
        }
            
    }


}
