//Jack Ng
//Dec 8th, 2014


//North,	// x,y-1
//East,	// x+1, y
//South,	// x, y+1
//West,	// x-1, y

using System;

public class Graph
{
	public Node[] mNodeArray;
	public int mWidth;
	public int mHeight;

	//Constructor
	public Graph()

	{
		mNodeArray = null;
		mWidth = 0;
		mHeight = 0;
	}
	//Destructor
	~Graph()
	{
		Destory ();
	}
	//Creating array
	public void Create(int width, int height)
	{
		Destory ();
		mNodeArray = new Node[width*height];
		mWidth = width;
		mHeight = height;

		for (int y=0; y<height; ++y) 
		{
			for (int x=0; x<height; ++x)
			{
				Node node = GetNode (x,y);
				node.mNeighbors[0] = GetNode( x , y - 1);	//North
				node.mNeighbors[1] = GetNode( x + 1 , y);	//East
				node.mNeighbors[2] = GetNode( x , y + 1);	//South
				node.mNeighbors[3] = GetNode( x - 1 , y);	//West
				node.close						= false;
			}
		}
	}
	//Destory Node
	public void Destory()
	{
		mNodeArray =null;
		mWidth=0;
		mHeight=0;
	}

	public void ResetNodes()
	{
		for(int i = 0; i < mWidth * mHeight; ++i)
		{
			mNodeArray[i].mParent=null;
			mNodeArray[i].open=false;
			mNodeArray[i].close=false;
			mNodeArray[i].g=0.0f;
			mNodeArray[i].h=0.0f;
		}
	}

	public Node GetNode(int x, int y)
	{
		Node node=null;
		if(x >= 0 && x < mWidth &&
		   y >= 0 && x < mHeight)
		{
			int index = x + (y*mWidth);
			node = mNodeArray[index];
		}
		return node;
	}
}
