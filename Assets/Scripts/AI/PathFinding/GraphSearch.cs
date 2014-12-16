//Jack Ng
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
	public void Run(int startX, int startY, int endX, int endY)
	{
		Node startNode = mGraph.GetNode (startX, startY);
		Node endNode = mGraph.GetNode (endX, endY);

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
			Node node = GetEndNode();
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
	//A* code//
	public Node GetNextNode()
	{
		Node node;
		node = null;
		return node;
	}
	public void ExpandNode(Node node, Node neighbor)
	{

	}
	//A* code//
	public List<Node> GetPathList()
	{
		List<Node>path;
		path = null;
		Node endNode=mEndNode;
		while(endNode!=null)
		{
			path.Add(endNode);
			endNode=endNode.mParent;
		}
		return path;
	}


	public bool IsFound()				{return mFound;}

	public Node GetEndNode()			{return mEndNode;}

	public List<Node> GetCloseList()	{return mCloseList;}


}
