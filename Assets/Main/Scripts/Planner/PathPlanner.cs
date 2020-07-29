using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planner;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BaseMovement))]
public class PathPlanner : MonoBehaviour
{
    public bool drawPotentialPath = false;
    private Transform player;
    private BaseMovement baseMovement;

    private Vector3 spriteCompensation;

    private Grid worldGrid;
    private Tilemap[] grids;
    private Tilemap groundGrid;
    private List<Tilemap> obstacleGrids = new List<Tilemap>();

    private Node[,] nodes;
    private Vector3Int nodeLocationOffset;

    // Current Planned Path
    private Stack<Node> plannedPath;
    private LinkedList<Direction> planToExecute = new LinkedList<Direction>();
    private LinkedList<Node> planToExecuteNodes = new LinkedList<Node>();

    // Planners
    private enum Planner
    {
        DIJKSTRA,
        GREEDY_BEST_FIRST,
        A_STAR
    }
    private BasePathPlanner planner;
    private DijkstraPlanner dijkstra;
    private GreedyBestFirstPlanner greedyBestFirst;
    private AStarPlanner aStar;

    // World Coords
    //        23
    //-37     0      +37
    //       -23
    // Local Coords
    //        46
    //0       37      74
    //        0   

    void Start()
    {
        Debug.Log("Path Planner Start!");
        Debug.Log("nodes: " + (nodes == null ? -1 : nodes.Length));
        player = transform;
        baseMovement = player.GetComponent<BaseMovement>();
        
        worldGrid = FindObjectOfType<Grid>();
        spriteCompensation = worldGrid.cellSize / 2.0f;

        grids = worldGrid.GetComponentsInChildren<Tilemap>();
        groundGrid = grids[0];

        foreach (Tilemap tm in grids)
        {
            if (tm.tag.Equals("Obstacle"))
            {
                obstacleGrids.Add(tm);
            }
        }

        Debug.Log("Obstacle Grids found: " + obstacleGrids.Count);

        initializePlanningSpace();
        initializePlanners();

        currentMouseCell = getPlayerLocationNode().GridLocation;
    }

    public void planPath(Vector3Int worldCellDestination, bool appendToPlan)
    {
        // Determine source and destination nodes
        Node src = getPlayerLocationNode();
        Node dst = getLocationNode(worldCellDestination);

        if (appendToPlan)
        {
            if (planToExecuteNodes.Last != null)
            {
                src = planToExecuteNodes.Last.Value;
            }
            // Execute the planner
            Stack<Node> newPlan = planner.Plan(nodes, src, dst);
            // Save an execution plan
            LinkedList<Direction> newPlanToExecute = getExecutionPlan(newPlan);
            foreach (Node node in newPlan)
            {
                plannedPath.Push(node);
            }
            foreach (Direction dir in newPlanToExecute)
            {
                planToExecute.AddLast(dir);
            }
        } else
        {
            planToExecuteNodes.Clear();
            // Execute the planner
            plannedPath = planner.Plan(nodes, src, dst);
            // Save an execution plan
            planToExecute = getExecutionPlan(plannedPath);
        }
    }

    public Node getLocationNode(Vector3Int worldCellLocation)
    {
        // Convert to local position for look up in the array
        Vector3Int localCellLocation = getLocalGridFromWorldGrid(worldCellLocation);
        return nodes[localCellLocation.x, localCellLocation.y];
    }

    private void initializePlanners()
    {
        dijkstra = new DijkstraPlanner();
        greedyBestFirst = new GreedyBestFirstPlanner();
        aStar = new AStarPlanner();

        planner = dijkstra;
    }

    private void initializePlanningSpace()
    {
        Debug.Log("initializePlanningSpace!");
        nodeLocationOffset = groundGrid.cellBounds.max;
        Debug.Log(String.Format("Ground Grid: {0}, {1}", groundGrid.cellBounds.size.x, groundGrid.cellBounds.size.y));
        nodes = new Node[groundGrid.cellBounds.size.x, groundGrid.cellBounds.size.y];
        Debug.Log("Nodes: X=" + nodes.GetLength(0) + ", Y=" + nodes.GetLength(1));

        for (int x = 0; x < nodes.GetLength(0); x++) {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                //Debug.Log(String.Format("Setting up Nodes X={0}, Y={1}", x, y));
                Vector3Int gridLocation = new Vector3Int(x, y, 1) - nodeLocationOffset;
                Node newNode = new Node(gridLocation);
                newNode.setParentNode(null);
                newNode.setCost(float.MaxValue);
                newNode.setObstacle(isObstacleTile(gridLocation));
                nodes[x, y] = newNode;
            }
        }
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                //Debug.Log(String.Format("Setting up Neighbors X={0}, Y={1}", x, y));
                Node node = nodes[x, y];
                List<Node> neighborNodes = getNeighborNodes(node, ref nodes);
                node.addNeighborNodes(neighborNodes);
                nodes[x, y] = node;
            }
        }
        Debug.Log("initializePlanningSpace done!");
    }

    private bool isObstacleTile(Vector3Int location)
    {
        foreach(Tilemap tm in obstacleGrids)
        {
            TileBase tile = tm.GetTile(location);
            if (location.x == 3 && location.y == 1)
            {
                Debug.Log("Tile: " + (tile == null ? "null" : tile.ToString()));
                Debug.Log("HasTile: " + tm.HasTile(location));
            }
            if (tm.HasTile(location))
            {
                return true;
            }
        }
        return false;
    }

    private List<Node> getNeighborNodes(Node newNode, ref Node[,] existingNodes)
    {
        int maxX = nodes.GetLength(0);
        int maxY = nodes.GetLength(1);
        Vector3Int nodeLoc = getLocalGridFromWorldGrid(newNode.GridLocation);
        List<Node> neighbors = new List<Node>();
        Node left = LookupNode(nodeLoc + Vector3Int.left, ref existingNodes);
        if (left != null)
        {
            neighbors.Add(left);
        }
        Node right = LookupNode(nodeLoc + Vector3Int.right, ref existingNodes);
        if (right != null)
        {
            neighbors.Add(right);
        }
        Node up = LookupNode(nodeLoc + Vector3Int.up, ref existingNodes);
        if (up != null)
        {
            neighbors.Add(up);
        }
        Node down = LookupNode(nodeLoc + Vector3Int.down, ref existingNodes);
        if (down != null)
        {
            neighbors.Add(down);
        }
        return neighbors;
    }

    private Node LookupNode(Vector3Int location, ref Node[,] nodes)
    {
        // Check for out of bounds location
        if (location.x < 0 || location.y < 0 ||
            location.x >= nodes.GetLength(0) ||
            location.y >= nodes.GetLength(1))
        {
            return null;
        }
        // Look up the node
        return nodes[location.x, location.y];
    }

    // Update is called once per frame
    void Update()
    {
        checkInput();
        detectDestination();
        executePlan();
        drawExecutingPlan();
    }

    private void checkInput()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            planner = dijkstra;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            planner = greedyBestFirst;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            planner = aStar;
        }
    }

    private LinkedList<Direction> getExecutionPlan(Stack<Node> plannedPath)
    {
        LinkedList<Direction> directions = new LinkedList<Direction>();
        Node origin = null;
        foreach (Node next in plannedPath)
        {
            if (origin != null)
            {
                planToExecuteNodes.AddLast(next);
                Vector3Int dir = (next.GridLocation - origin.GridLocation);
                if (dir == Vector3Int.left)
                {
                    directions.AddLast(Direction.LEFT);
                }
                else if (dir == Vector3Int.right)
                {
                    directions.AddLast(Direction.RIGHT);
                }
                else if (dir == Vector3Int.up)
                {
                    directions.AddLast(Direction.UP);
                }
                else if (dir == Vector3Int.down)
                {
                    directions.AddLast(Direction.DOWN);
                }
            }
            origin = next;
        }
        return directions;
    }

    private void drawPlan()
    {
        if (plannedPath != null && plannedPath.Count > 0)
        {
            Node start = null;
            foreach (Node loc in plannedPath)
            {
                if (start != null)
                {
                    Color color = Color.white;
                    if (typeof(DijkstraPlanner).Equals(planner.GetType()))
                    {
                        color = Color.red;
                    }
                    else if (typeof(GreedyBestFirstPlanner).Equals(planner.GetType()))
                    {
                        color = Color.blue;
                    }
                    else if (typeof(AStarPlanner).Equals(planner.GetType()))
                    {
                        color = Color.magenta;
                    }
                    Debug.DrawLine(
                        nodeLocationToWorldCenterLocation(start.GridLocation),
                        nodeLocationToWorldCenterLocation(loc.GridLocation), 
                        color
                    );
                }
                start = loc;
            }
        }
    }

    private void drawExecutingPlan()
    {
        if (planToExecuteNodes.Count > 0)
        {
            Node start = null;
            foreach (Node loc in planToExecuteNodes)
            {
                if (start != null)
                {
                    Debug.DrawLine(
                        nodeLocationToWorldCenterLocation(start.GridLocation),
                        nodeLocationToWorldCenterLocation(loc.GridLocation),
                        Color.white
                    );
                }
                start = loc;
            }
        }
    }

    private Vector3 nodeLocationToWorldCenterLocation(Vector3Int worldGridLocation)
    {
        return worldGrid.CellToLocal(worldGridLocation) + spriteCompensation;
    }

    private void executePlan()
    {
        // Check for a plan to execute
        if (planToExecute.Count > 0 && !baseMovement.isMoving())
        {
            planToExecuteNodes.RemoveFirst();
            baseMovement.Move(planToExecute.First.Value);
            planToExecute.RemoveFirst();
        }
    }

    private Vector3Int currentMouseCell;
    private Node destination;
    private void detectDestination()
    {
        // Check to see if we need to find a planned path
        if (!drawPotentialPath)
        {
            return;
        }
        // Get the mouse position
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0.5f;
        Vector3 mousePointWorld = Camera.main.ScreenToWorldPoint(mousePoint);
        // Calculate mouse position in the world grid
        Vector3Int worldCellLocation = worldGrid.WorldToCell(mousePointWorld);

        if (worldCellLocation != currentMouseCell)
        {
            currentMouseCell = worldCellLocation;
            // Convert to local position for look up in the array
            Vector3Int localCellLocation = getLocalGridFromWorldGrid(worldCellLocation);
            // Get the destination location node
            destination = nodes[localCellLocation.x, localCellLocation.y];
            // Get current player location node
            Node playerLocation = getPlayerLocationNode();
            plannedPath = planner.Plan(nodes, playerLocation, destination);
        }
        drawPlan();
    }

    private Node getPlayerLocationNode()
    {
        // Calculate current player position in the world grid
        Vector3Int playerWorldPos = worldGrid.WorldToCell(player.localPosition);
        // Convert to local position for look up in the array
        Vector3Int playerLocalPos = getLocalGridFromWorldGrid(playerWorldPos);
        return nodes[playerLocalPos.x, playerLocalPos.y];
    }

    private Vector3Int getLocalGridFromWorldGrid(Vector3Int worldGridLoc)
    {
        return worldGridLoc + nodeLocationOffset;
    }

    private Vector3Int getWorldGridFromLocalGrid(Vector3Int localGridLoc)
    {
        return localGridLoc - nodeLocationOffset;
    }
}
