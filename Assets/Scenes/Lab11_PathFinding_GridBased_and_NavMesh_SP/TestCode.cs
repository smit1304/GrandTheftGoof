using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    private Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }
    public List<Node> pathArray;
    GameObject objStartCube, objEndCube;
    private float elapsedTime = 0.0f;
    //Interval time between pathfinding
    public float intervalTime = 1.0f;
    private LineRenderer lineRenderer;

    void Start()
    {
        objStartCube = GameObject.FindGameObjectWithTag("Player");
        objEndCube = GameObject.FindGameObjectWithTag("End");
        lineRenderer = GetComponent<LineRenderer>();

        pathArray = new List<Node>();
        FindPath();
    }
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            FindPath();
            
        }
    }
    void FindPath()
    {
        if (objEndCube == null)
        {
            objEndCube = GameObject.FindGameObjectWithTag("End");
           return;
        }

        startPos = objStartCube.transform;
        endPos = objEndCube.transform;

        //Assign StartNode and Goal Node
        var (startColumn, startRow) =
        GridManager.instance.GetGridCoordinates(startPos.position);
        var (goalColumn, goalRow) = GridManager.instance.GetGridCoordinates(endPos.position);
        startNode = new Node(GridManager.instance.GetGridCellCenter(startColumn, startRow));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(goalColumn, goalRow));
        pathArray = new AStar().FindPath(startNode, goalNode);

        if (pathArray == null)
        {
            Debug.Log("Path array is null");
        }
        DrawPath();
    }
    void OnDrawGizmos()
    {
        if (pathArray == null)
            return;
        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            }
            
        }
    }
    void DrawPath()
    {
        if (pathArray == null || pathArray.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = pathArray.Count;

        for (int i = 0; i < pathArray.Count; i++)
        {
            lineRenderer.SetPosition(i, pathArray[i].position);
        }
    }


}
