﻿//Jack Ng
//Dec 5th, 2014

//North,	// x,y-1
//East,	// x+1, y
//South,	// x, y+1
//West,	// x-1, y


using System;
using System.Collections.Generic;


public class GraphSearch
{
	public Graph mGraph;
	public Node mEndNode;

	public List<Node>mOpenList;
	public List<Node>mCloseList;
	public List<Node>mPath;

	public bool mFound;

	//variables//-------------------------------
	public GraphSearch (Graph graph)
	{
		mGraph = graph;
		mFound = false;
	}
	//AttackRange
	public void AttackRange(int startX, int startY, int range)
	{
		mOpenList = new List<Node>();
		mCloseList = new List<Node>();
		mPath = new List<Node>();
		
		Node startNode = mGraph.GetNodeInfo (startX, startY);
		//Reset graph Node
		mGraph.ResetNodes();
		//Add start Node to open List
		mOpenList.Add (startNode);
		startNode.open = true;
		//Search//
		
		while (mOpenList.Count!=0) 
		{
			Node node = GetNextNode();
			//if(range==-1)
			//{
			//}
			//else
			//{
			//	node = GetNextNode(range);
			//}
			for(int n = 0; n < 4; ++n)
			{
				Node neighbor = node.mNeighbors[n];
				if(neighbor!=null)
				{
					ExpandNode(node, neighbor);
				}
				
			}
			if(node.g == range + 1)
			{
				break;
			}
			mCloseList.Add (node);
			node.close = true;
		}
	}
	//Serpeate the Run Function into PathFind and RangeSearch
	public void RangeSearch(int startX, int startY, int range)
	{
		mOpenList = new List<Node>();
		mCloseList = new List<Node>();
		mPath = new List<Node>();
		
		Node startNode = mGraph.GetNodeInfo (startX, startY);
		//Reset graph Node
		mGraph.ResetNodes();
		//Add start Node to open List
		mOpenList.Add (startNode);
		startNode.open = true;
		//Search//
		
		while (mOpenList.Count!=0) 
		{
			Node node = GetNextNode();
			//if(range==-1)
			//{
			//}
			//else
			//{
			//	node = GetNextNode(range);
			//}
			for(int n = 0; n < 4; ++n)
			{
				Node neighbor = node.mNeighbors[n];
				if(neighbor!=null && neighbor.walkable==true)
				{
					ExpandNode(node, neighbor);
				}
			
			}
			if(node.g == range + 1)
			{
				break;
			}
				mCloseList.Add (node);
				node.close = true;
		}
}
	public void PathFind(int startX, int startY, int endX, int endY)
	{
		mOpenList = new List<Node>();
		mCloseList = new List<Node>();
		mPath = new List<Node>();

		Node startNode = mGraph.GetNodeInfo (startX, startY);
		Node endNode = mGraph.GetNodeInfo (endX, endY);

		mEndNode = endNode;

		if(startNode == null || endNode == null)
		{
			return;
		}
		//Reset graph Node
		mGraph.ResetNodes();
		//Add start Node to open List
		mOpenList.Add (startNode);
		startNode.open = true;
		//Search//
		bool done = false;

		while (done==false && mOpenList.Count!=0) 
		{
			Node node = GetNextNode();
			//if(range==-1)
			//{
			//}
			//else
			//{
			//	node = GetNextNode(range);
			//}
			if(node == endNode)
			{
				done = true;
				mFound = true;
			}
			else
			{
				for(int n = 0; n < 4; ++n)
				{
					Node neighbor = node.mNeighbors[n];
					if(neighbor!=null && neighbor.walkable==true)
					{
						ExpandNode(node, neighbor);
					}

				}
			}
			mCloseList.Add (node);
			node.close = true;
		}
	}
	// code//
	public Node GetNextNode()
	{
		Node lowestIter;
		lowestIter = null;
		float lowestCost = 99999.99f;	//just equal to FLT_MAX
		foreach (Node i in mOpenList)
		{
			Node node = i;

			if(node.g < lowestCost)
			{
				lowestCost = node.g;
				lowestIter = i;
			}
		}
		Node lowestNode = lowestIter;
		mOpenList.Remove (lowestIter);
		return lowestNode;
	}
	public void ExpandNode(Node node, Node neighbor)
	{
		if(!neighbor.close)
		{
			float g = node.g + 1.0f;
			if(!neighbor.open)
			{
				neighbor.g = g;
				neighbor.mParent = node;
				mOpenList.Add (neighbor);
				neighbor.open = true;
			}
			else if(g < neighbor.g)
			{
				neighbor.g = g;
				neighbor.mParent = node;
			}
		}
	}
	//A* code//
	public List<Node> GetPathList()
	{
		List<Node>path;
		path = new List<Node>();
		while(mEndNode!=null)
		{
			path.Add(mEndNode);
			mEndNode=mEndNode.mParent;
		}
		return path;
	}


	public bool IsFound()				{return mFound;}

	public Node GetEndNode()			{return mEndNode;}

	public List<Node> GetCloseList()	{return mCloseList;}


}
