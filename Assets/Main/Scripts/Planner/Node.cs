using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planner
{
    public class Node
    {
        [SerializeField] private Vector3Int gridLocation;
        [SerializeField] private float cost = int.MaxValue;
        [SerializeField] private Node parentNode = null;
        [SerializeField] private List<Node> neighborNodes = new List<Node>();
        [SerializeField] private bool obstacle = false;

        public Vector3Int GridLocation { get => gridLocation;}
        public bool Visited { get; set; }

        public Node(Vector3Int gridLocation)
        {
            this.gridLocation = gridLocation;
        }

        /// <summary>
        /// Reset all the values in the nodes.
        /// </summary>
        public void resetNode()
        {
            cost = int.MaxValue;
            parentNode = null;
        }

        // -------------------------------- Setters --------------------------------

        /// <summary>
        /// Set the parent node.
        /// </summary>
        /// <param name="node">Set the node for parent node.</param>
        public void setParentNode(Node node)
        {
            this.parentNode = node;
        }

        /// <summary>.
        /// Set the cost value
        /// </summary>
        /// <param name="value">cost value</param>
        public void setCost(float value)
        {
            this.cost = value;
        }

        /// <summary>
        /// Set is node is an Obstacle.
        /// </summary>
        /// <param name="value">boolean</param>
        public void setObstacle(bool value)
        {
            this.obstacle = value;
        }

        /// <summary>
        /// Adding neighbor node objects.
        /// </summary>
        /// <param name="nodes">Nodes</param>
        public void addNeighborNodes(List<Node> nodes)
        {
            this.neighborNodes.AddRange(nodes);
        }

        // -------------------------------- Getters --------------------------------

        


        /// <summary>
        /// Get neighbor nodes.
        /// </summary>
        /// <returns>All the nodes.</returns>
        public List<Node> getNeighborNodes()
        {
            return this.neighborNodes;
        }

        /// <summary>
        /// Get weight
        /// </summary>
        /// <returns>get cost in float.</returns>
        public float getCost()
        {
            float result = this.cost;
            return result;

        }

        /// <summary>
        /// Get the parent Node.
        /// </summary>
        /// <returns>Return the parent node.</returns>
        public Node getParentNode()
        {
            return this.parentNode;
        }


        // -------------------------------- Checkers --------------------------------

        /// <summary>
        /// Get if the node is an obstacle.
        /// </summary>
        /// <returns>boolean</returns>
        public bool isObstacle()
        {
            return obstacle;
        }

    }

    // Defines a comparer to create a sorted set
    // that is sorted by the node's cost.
    public class ByCost : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            if (x.GridLocation.Equals(y.GridLocation))
            {
                return 0;
            }
            return x.getCost().CompareTo(y.getCost());
            /*// Parse the extension from the file name.
            xExt = x.Substring(x.LastIndexOf(".") + 1);
            yExt = y.Substring(y.LastIndexOf(".") + 1);

            // Compare the file extensions.
            int vExt = caseiComp.Compare(xExt, yExt);
            if (vExt != 0)
            {
                return vExt;
            }
            else
            {
                // The extension is the same,
                // so compare the filenames.
                return caseiComp.Compare(x, y);
            }*/
        }
    }
}