using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Planner
{
    public abstract class BasePathPlanner
    {
        public Stack<Node> Plan(Node[,] nodes, Node src, Node dest)
        {
            double startTime = Time.realtimeSinceStartup;
            FindPlan(nodes, src, dest);
            double endTime = (Time.realtimeSinceStartup - startTime);
            Stack<Node> path = getPlannedPath(dest);
            //Debug.Log("Total Path length: " + path.Count + "\nCompute time: " + endTime);

            return getPlannedPath(dest);
        }

        protected abstract float CalculateCost(Node currentNode, Node neighbor, Node destination);

        private void FindPlan(Node[,] nodes, Node src, Node dest)
        {
            // Create an unexplored set
            HashSet<Vector3Int> unexploredNodes = new HashSet<Vector3Int>();
            List<Node> costSetNodes = new List<Node>();

            foreach (Node node in nodes)
            {
                node.setParentNode(null);
                node.setCost(float.MaxValue);
                if (!node.isObstacle())
                {
                    unexploredNodes.Add(node.GridLocation);
                }
            }

            // Set starting node's cost to zero
            src.setCost(0);
            costSetNodes.Add(src);


            // Iterate through unexplored set
            while (unexploredNodes.Count > 0)
            {
                // Sort by cost
                costSetNodes.Sort((x, y) => x.getCost().CompareTo(y.getCost()));

                // Get the current node
                Node currentNode = costSetNodes[0];

                // Remove from set
                unexploredNodes.Remove(currentNode.GridLocation);
                costSetNodes.Remove(currentNode);

                // Check completed
                if (currentNode == dest)
                {
                    return;
                }

                // Go through each neighbor
                foreach (Node neighbor in currentNode.getNeighborNodes())
                {
                    // Check for existence in unexplored set and not an obstacle
                    if (unexploredNodes.Contains(neighbor.GridLocation) && !neighbor.isObstacle())
                    {
                        // Calculate the new cost
                        float newDist = CalculateCost(currentNode, neighbor, dest);
                        // Check for shorter cost
                        if (newDist < neighbor.getCost())
                        {
                            // Set the new cost
                            neighbor.setCost(newDist);
                            neighbor.setParentNode(currentNode);
                            costSetNodes.Add(neighbor);
                        }
                    }
                }
            }
        }

        private Stack<Node> getPlannedPath(Node dest)
        {
            Stack<Node> plannedPath = new Stack<Node>();
            plannedPath.Push(dest);
            Node parentNode = dest.getParentNode();
            while (parentNode != null)
            {
                plannedPath.Push(parentNode);
                parentNode = parentNode.getParentNode();
            }

            return plannedPath;
        }
    }
}
