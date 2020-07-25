using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planner
{
    public class AStarPlanner : BasePathPlanner
    {

        protected override float CalculateCost(
            Node currentNode, 
            Node neighbor, 
            Node destination)
        {
            // f(n) = g(n) + h(n)
            // Weight(n) = Cost(n) + Distance to Goal(n)
            return (currentNode.getCost() + 1.0f) +
                Vector3Int.Distance(neighbor.GridLocation, destination.GridLocation);
        }
    }
}