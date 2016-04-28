using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    public enum Tools { CREATE, DELETE, MOVE, HINGEDSUPP, MOVSUPP, FORCE};
    public GameObject prefabNode, prefabEdge, inputsGo;
    public static GameObject Nodes, Edges, Supports;

    public static List<Node> nodes;
    public static List<Edge> edges;
    public static List<Reaction> reactions;
    public static Tools tool;
    public static Force selectedForce;

    InputFields input;
    Node node1, node2;
    Edge edge;
    bool mouseDown, mouseUp, balanced;
    int index, reactionsNumber;
    Matrix matrixSolver;
    double[][] matrix;
    double[] solutions, normalized;

	// Use this for initialization
	void Start () {
        tool = Tools.CREATE;
        StartCoroutine(create());
        mouseDown = false;
        mouseUp = false;
        nodes = new List<Node>();
        edges = new List<Edge>();
        reactions = new List<Reaction>();
        Nodes = new GameObject("Nodes");
        Edges = new GameObject("Edges");
        Supports = new GameObject("Supports");
        Node.main = this;
        Edge.main = this;
        index = 0;
        input = inputsGo.GetComponent<InputFields>();
	}
	
	// Update is called once per frame
	void Update () {
        mouseDown = Input.GetMouseButtonDown(0);
        mouseUp = Input.GetMouseButtonUp(0);

        balanced = isStableStructure();
        BalancedIndicator.balanced = balanced;

        if (nodes.Count > 0)
            //Debug.Log(nodes[0].px + " , " + nodes[0].py);

        if (balanced)
        {
            setUpMatrix();
            solveSystem();
            printSolutions();

            normalizeSolutions();
            Results.result.text = getResultsString();
            for (int i = 0; i < edges.Count; i++)
                edges[i].force = (float)normalized[i];
        }
        else
            Results.result.text = "";

    }

    public void selectForce(Force f)
    {
        selectedForce = f;
        input.angleInput.text = f.angle.ToString();
        input.magnitudeInput.text = f.magnitude.ToString();
    }

    void printStructureInfo()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Debug.Log(nodes[i].transform.name + " " + nodes[i].edges.Count);
            for (int j = 0; j < nodes[i].edges.Count; j++)
            {
                Debug.Log(" " + nodes[i].edges[j].transform.name);
            }
        }
    }

    bool isStableStructure()
    {
        return 2 * nodes.Count == edges.Count + reactions.Count && nodes.Count > 0;
    }

    void setUpMatrix()
    {
        matrix = new double[2*nodes.Count][];
        double[] eqX, eqY;
        
        for(int i=0; i<nodes.Count; i++)
        {
            eqX = nodes[i].equationX();
            eqY = nodes[i].equationY();

            matrix[2*i] = eqX;
            matrix[2 * i + 1] = eqY;     
        }
    }

    int solveSystem()
    {
        matrixSolver = new Matrix(ref matrix);
        int solved = matrixSolver.solveByGaussJordan();

        if(solved == 0)
            solutions = matrixSolver.getSolutions();

        return solved;
    }

    string getResultsString()
    {
        string r = "";

        for(int i=0; i<edges.Count; i++)
        {
            r += edges[i].transform.name + " = " + solutions[i].ToString("F3") + "\n";
        }

        for (int i = 0; i < reactions.Count; i++)
        {
            r += reactions[i].name + " = " + solutions[i + edges.Count].ToString("F3") + "\n";
        }

        return r;
    }

    void normalizeSolutions()
    {
        double max = 0;
        normalized = new double[solutions.Length];
        for(int i=0; i<solutions.Length; i++)
        {
            if (solutions[i] < 0)
                normalized[i] = -1 * solutions[i];
            else
                normalized[i] = solutions[i];

            if (normalized[i] > max)
                max = normalized[i];
        }

        max += 1;

        for (int i = 0; i < normalized.Length; i++)
            normalized[i] *= (100.0 / max);
    }

    void printMatrix()
    {
        string lines = matrix.Length + " " + matrix[0].Length + "\n";

        for(int i=0; i<matrix.Length; i++)
        {
            for(int j=0; j<matrix[i].Length; j++)
            {
                lines += matrix[i][j] + " ";
            }
            lines += "\n";
        }

        Debug.Log(lines);
    }

    void printSolutions()
    {
        string line = "";

        for (int i = 0; i < solutions.Length; i++)
        {
            line += solutions[i] + " ";
        }

        line += "\n";
        Debug.Log(line);
    }

    public void createTool()
    {
        Debug.Log("Create mode");
        tool = Tools.CREATE;
        StartCoroutine(create());
    }

    public void moveTool()
    {
        Debug.Log("Move mode");
        tool = Tools.MOVE;
    }

    public void deleteTool()
    {
        Debug.Log("Delete mode");
        tool = Tools.DELETE;
    }

    public void movableSupportTool()
    {
        Debug.Log("Support movable");
        tool = Tools.MOVSUPP;
        Node.type = Node.SupportType.MOVABLE;
    }

    public void hingedSupportTool()
    {
        Debug.Log("Support hinged");
        tool = Tools.HINGEDSUPP;
        Node.type = Node.SupportType.HINGED;
    }

    public void forceTool()
    {
        Debug.Log("Force");
        tool = Tools.FORCE;
    }

    Node createNode()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.name.Equals("Plane"))
            {
                GameObject newNode = (GameObject)Instantiate(prefabNode, hit.point, transform.rotation);
                newNode.transform.position -= new Vector3(0, 0, newNode.transform.position.z);
                newNode.transform.name = ((char)(index + 65)).ToString();
                newNode.transform.SetParent(Nodes.transform);
                index++;
                Node node = newNode.GetComponent<Node>();
                nodes.Add(node);
                return node;
            }
            else
            {
                return hit.transform.gameObject.GetComponent<Node>();
            }
        }

        return null;
    }

    Edge createEdge(Node node1, Node node2)
    {
        Vector3 pos1, pos2;
        pos1 = node1.getPosition();
        pos2 = node2.getPosition();
        Vector3 midpoint = (pos1 + pos2) / 2;
        GameObject e = (GameObject)Instantiate(prefabEdge, midpoint, Quaternion.Euler(0, 0, Vector3.Angle(pos1, pos2)));
        e.transform.SetParent(Edges.transform);
        edge = e.GetComponent<Edge>();
        edge.setNodes(node1, node2);
        edge.rename();
        edges.Add(edge);
        return edge;
    }

    void setUpNeighbors(Node node1, Node node2)
    {
        if(node1 != node2 && !existEdge(node1, node2) )
        {
            Edge edge = createEdge(node1, node2);
            node1.addNeighbor(node2, edge);
            node2.addNeighbor(node1, edge);
        }
    }

    bool existEdge(Node node1, Node node2)
    {
        string name1 = node1.transform.name + node2.transform.name;
        string name2 = node2.transform.name + node1.transform.name;

        bool exist = GameObject.Find(name1) != null || GameObject.Find(name2) != null;
        return exist;
    }

    IEnumerator create()
    {
        while(tool == Tools.CREATE)
        {
            if (mouseDown)
            {
                node1 = createNode();
            }
            if (mouseUp)
            {
                node2 = createNode();
                setUpNeighbors(node1, node2);
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
