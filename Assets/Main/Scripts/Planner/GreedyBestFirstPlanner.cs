using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Planner
{
    public class GreedyBestFirstPlanner : BasePathPlanner
    {
        protected override float CalculateCost(Node currentNode, Node neighbor, Node destination)
        {
            return Vector3Int.Distance(neighbor.GridLocation, destination.GridLocation);
        }
    }
}
