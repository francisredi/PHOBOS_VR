using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Wayfinding;

public class SharkNavigator : MonoBehaviour
{
    //public Graph graph; // graph reference
    public Graph graph = new Graph(); // waypoint graph
    public GameObject[] globalPathPlanningWaypoints;
    public ArrayList destinationList = new ArrayList(); // possible destinations

    private GameObject currentNode = null;
    private GameObject destinationNode = null;
    private GameObject destinationOffsetNode = null;
    private float gSpeed = 3.0f;

    private GameObject offsetWaypoint;
    private float targetSpeed = 3.0f;
    private float turnSpeed = 1.5f;
    private string dest = "SharkPath-1";
    private bool destinationReached = false;
    private List<Node> pathList = new List<Node>();
    private int currentWP;
    private Vector3 startNodePosition;
    private Vector3 direction;
    private float scale = 1.0f;

    // Use this for initialization
    void Start () {
        print("Loading waypoint graph");
        CreateWaypointGraph();   // create waypoint graph for global navigation by group agents
        CreateDestinationList();

        offsetWaypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(offsetWaypoint.GetComponent<Collider>());
        offsetWaypoint.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
        offsetWaypoint.GetComponent<Renderer>().material.color = Color.red;
        offsetWaypoint.transform.localScale = new Vector3(0.5f / scale, 0.5f / scale, 0.5f / scale);

        if (dest != "") FindStartNode(dest);
        do
        {
            int ind = Random.Range(0, destinationList.Count); // 0 to count - 1
            dest = (string)destinationList[ind]; // decide destination from list
            FindDestinationNode(dest);
            destinationOffsetNode = GameObject.Find(dest); // 12/3/2014
        } while (currentNode.name.Equals(destinationNode.name, System.StringComparison.Ordinal)); // do not allow origin to be destination
                                                                                                  //print (currentNode.name + " " + destinationNode.name);
        graph.AStar(currentNode, destinationNode, pathList);
        currentWP = 0;
        destinationReached = false;

        startNodePosition = graph.getPathPoint(currentWP, pathList).transform.position;
        offsetWaypoint.transform.position = startNodePosition;
    }

    // Update is called once per frame
    void Update () {
        graph.debugDraw(); // draw the paths in the scene view of the editor while playing
    }

    #region Waypoint Graph Routines

    private void CreateBiPath(string name1, string name2, int a, int b, float narrowness)
    {
        GameObject w1 = GameObject.Find(name1 + "-" + a);
        GameObject w2 = GameObject.Find(name2 + "-" + b);

        if (w1 && w2) // if both objects exist
        {
            // create edges between the waypoints in both directions
            graph.AddEdge(w1, w2, narrowness);
            graph.AddEdge(w2, w1, narrowness);
        }
    }

    private void CreateWaypointGraph()
    {
        globalPathPlanningWaypoints = GameObject.FindGameObjectsWithTag("waypoint");

        // add all the waypoints to the graph
        foreach (GameObject go in globalPathPlanningWaypoints)
        {
            graph.AddNode(go, true, true);
        }

        for (int i = 1; i < 22; i++)
        { // 1 to 22
            CreateBiPath("SharkPath", "SharkPath", i, i + 1, 1.0f);
        }
        CreateBiPath("SharkPath", "SharkPath", 22, 1, 1.0f);
    }

    void CreateDestinationList()
    {
        destinationList.Add("SharkPath-5");
        destinationList.Add("SharkPath-12");
        destinationList.Add("SharkPath-20");
    }

    #endregion

    #region Moving Agent and Adjusting Speed

    public void FindStartNode(string start)
    {
        if (currentNode != null) return;
        currentNode = GameObject.Find(start);
    }

    public void FindDestinationNode(string destination)
    {
        destinationNode = GameObject.Find(destination);
    }

    private void AdjustSpeed()
    {
        if (gSpeed < targetSpeed) gSpeed += 0.1f;
        else if (gSpeed > targetSpeed) gSpeed -= 0.1f;
    }


    GameObject nextNode = null;
    GameObject nextNextNode = null;
    GameObject prevOffsetWaypoint;

    void UpdateOffSetWaypointTransform()
    {

        if (currentWP == graph.getPathLength(pathList))
        { // if reached end, there is no next node
            return;
        }

        if ((currentWP + 1) == graph.getPathLength(pathList))
        {
            nextNextNode = null;
        }
        else
        {
            nextNextNode = graph.getPathPoint(currentWP + 1, pathList);
        }

        // calculate direction must go
        prevOffsetWaypoint = offsetWaypoint;
        nextNode = graph.getPathPoint(currentWP, pathList);
        Vector3 dir = nextNode.transform.position - currentNode.transform.position;
        offsetWaypoint.transform.position = nextNode.transform.position;
        /*if (false)
        {
            offsetWaypoint.transform.rotation = Quaternion.LookRotation(dir, Vector3.up); // orient the offset transform's forward direction
                                                                                          // now calculate an offset on x-axis that is within the width of the passage, random stopping closer or farther than waypoint on local z-axis
            offsetWaypoint.transform.Translate(Random.Range(-5.0f, 5.0f), 0, Random.Range(-3.0f, 3.0f)); // translate relative to local x-axis, also z-axis
        }*/
    }

    void FixedUpdate()
    {
        // if at end of path
        if (currentWP == graph.getPathLength(pathList))
        {
            currentNode = GameObject.Find(dest);
            offsetWaypoint.transform.position = currentNode.transform.position;

            do
            {
                int ind = Random.Range(0, destinationList.Count); // 0 to count - 1
                dest = (string)destinationList[ind]; // decide destination from list
                FindDestinationNode(dest);
                destinationOffsetNode = GameObject.Find(dest); // 12/3/2014
            } while (currentNode.name.Equals(destinationNode.name, System.StringComparison.Ordinal)); // do not allow origin to be destination
                                                                                                      //print (currentNode.name + " " + destinationNode.name);
            graph.AStar(currentNode, destinationNode, pathList);
            currentWP = 0;
            destinationReached = false;

            startNodePosition = graph.getPathPoint(currentWP, pathList).transform.position;
            offsetWaypoint.transform.position = startNodePosition;

            return;
        }

        // node closest to at moment
        currentNode = graph.getPathPoint(currentWP, pathList);

        // if close enough to current waypoint start moving toward the next
        //float turnSpeed = 1.5f;

        if (Vector3.Distance(offsetWaypoint.transform.position, transform.position) < 1.5f)
        {
            currentWP++;
            nextNode = null; // reset
            UpdateOffSetWaypointTransform(); // update offset transform for diverse paths for group agents
        }

        // if not at end of path, keep on moving
        if (currentWP < graph.getPathLength(pathList))
        {
            direction = offsetWaypoint.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), turnSpeed * Time.deltaTime);
            transform.Translate(0, 0, Time.deltaTime * gSpeed / scale); // move straight in current direction
            AdjustSpeed();
        }

    }

    #endregion
}
