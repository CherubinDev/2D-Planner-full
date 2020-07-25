using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planner
{
    public class DijkstraPlanner : BasePathPlanner
    {

        protected override float CalculateCost(
            Node currentNode, 
            Node neighbor, 
            Node destination)
        {
            return currentNode.getCost() + 1.0f;
        }
    }
}