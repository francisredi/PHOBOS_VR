using UnityEngine;
using System.Collections;

namespace Wayfinding {
	public class Edge
	{
		public Node startNode;
		public Node endNode;
		public float passageThickness; // 0.0f is river, 1.0f is wall
		
		public Edge(Node fromNode, Node toNode, float narrowness)
		{
			startNode = fromNode;
			endNode = toNode;
			passageThickness = narrowness;
		}
	}
}