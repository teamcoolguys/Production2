﻿//Created by Dylan Fraser
//Prodcution 1
//Updated by Jack Ng
//Production 1
//Updated by Jack Ng
//Jan 8th
using UnityEngine;


[RequireComponent(typeof(TileMap))]
public class DTileMap
{	
	//Jack Ng
	//Search Graph
	public Graph mGraph;
	GameObject mTileMapObject;
	TileMap mInfo;

	//>>>>>>>>>>>>>>>>>>>
	//<<<<<<<<<<<<<<<<<<<
	public int size_x;
	public int size_y;

	public Vector3[] mPositionData;

	public int[] map_data;
	
	public enum TileType
	{
		Floor,			//0
		Walkable,		//1
		Path,			//2
		Wall,			//3
		Sewer,			//4
		PlayerSpawn,	//5		
		Player1,		//6
		Player2,	   	//7
		Player3,		//8
		Player4,		//9
    	Target1,		//10
		Target2,		//11
		Target3,		//12
		TrueSewer,		//13
		TargetSpot,		//14
	};

	public void CreateMap(int sizex, int sizey)
	{
		if(PhotonNetwork.offlineMode)
		{
			mTileMapObject=GameObject.Find("CurrentTileMap");
		}
		else
		{
			mTileMapObject = GameObject.Find("CurrentTileMap(Clone)");
		}

		if(mTileMapObject)
		{
			mInfo = mTileMapObject.GetComponent<TileMap>();
			map_data = new int[size_x*size_y];

			//Graph Search Structure
			mGraph=new Graph();
			mGraph.Create (size_x, size_y);
			
			//Map position structure
			Vector3 startLocation;
			startLocation.x = 0.5f;
			startLocation.y = 0.5f;
			startLocation.z = 0.5f;	
			
			mPositionData = new Vector3[sizex*sizey];
			
			int[] readData = new int[size_x * size_y];
			
			readData = ReadMap ();
			
			for(int y=0;y<sizey;y++)
			{
				for(int x=0;x < sizex;x++) 
				{
					//Setting Map
					map_data[(y*size_x)+x] = readData[(y*size_x)+x];
					
					
					//Setting Map position
					Vector3 currentLocation = startLocation;
					currentLocation.x += x*1.0f;
					currentLocation.z += y*1.0f;
					mPositionData[(y*size_x)+x] =currentLocation;
					//Seting Graph Search
					
					Node node = mGraph.GetNodeInfo(x,y);
					int type = map_data[(y*size_x)+x];
					int i= (y*size_x)+x;
					if(type==(int)TileType.Floor || type == (int)TileType.Sewer)
					{
						node.walkable = true;
					}
					else
					{
						node.walkable = false;
					}
				}
			}
		}
		else
		{
			Debug.Log("TileMap Non-Existent");
		}
	}

	public int[] ReadMap()
	{
		int[] readData = new int[size_x * size_y];
		
		string[] lines = System.IO.File.ReadAllLines (Application.dataPath + "/map.txt");
		char[] delimiterChars = {' ', ',', '.'};
		
		for(int i = 0; i < lines.Length; ++i)
		{
			string[] entries = lines[i].Split (delimiterChars);
			
			int len = entries.Length;
			if(len > 0)
			{
				for(int j = 0; j < len; ++j)
				{
					int entry = int.Parse (entries[j]);
					
					readData[(i * len) + j] = entry;
				}
			}
		}
		return readData;
	}

	public DTileMap(int sizex, int sizey)
	{
		//Map Structure
		this.size_x = sizex;
		this.size_y = sizey;
		CreateMap (sizex, sizey);
	}

	public int XYToIndex(int x, int y)
	{
		return ((y * size_x) + x);
	}

	public TileType GetTileType(int x, int y)
	{
		if(map_data != null)
		{
			return (TileType)map_data[(y*size_x)+x];
		}
		else
		{
			return TileType.TargetSpot;
		}
	}

	public TileType GetTileTypeIndex(int index)
	{
		int x= 0, y = 0;
		IndexToXY (index,out x, out y);
		return (TileType)map_data[(y*size_x)+x];
	}

	public void SetTileType(int x, int y, TileType t, bool walkable)
	{
		Node node = mGraph.GetNodeInfo (x, y);
		//if(walkable)
		//{
		//	node.walkable = true;
		//}
		//else
		//{
		//	node.walkable = false;
		//}
		map_data[(y*size_x)+x]= (int)t;

		mInfo.UpdateTexture (x,y);
	}

	public void SetTileTypeIndex(int index, TileType t, bool walkable)
	{

		map_data[index]= (int)t;
		int x= 0, y = 0;
		IndexToXY (index,out x, out y);
		Node node = mGraph.GetNodeInfo (x, y);
		//if(walkable)
		//{
		//	node.walkable = true;
		//}
		//else
		//{
		//	node.walkable = false;
		//}
		mInfo.UpdateTexture (x,y);
	}

	public void IndexToXY(int index, out int x, out int y)
	{
		int count = 0;
		while(index>=size_x)
		{
			index-=size_x;
			count++;
		}
		y = count;
		x = index;
	}

	public Vector3 GetTileLocation(int x, int y)
	{
		return mPositionData[(y*size_x)+x];
	}

	public Vector3 GetTileLocationIndex(int index)
	{
		int x= 0, y = 0;
		IndexToXY (index, out x, out y);
		return mPositionData [(y * size_x) + x];
	}
	//public V2 GetTileXY(Vector3 location)
	//{
	//	for(int y=0;y<size_x;y++)
	//	{
	//		for(int x=0;x<size_y;x++) 
	//		{
	//			Vector3 tempPosition=mPositionData[(y*size_x)+x];
	//			if(location.x == tempPosition.x
	//			   && location.y == tempPosition.y
	//			   && location.z == tempPosition.z)
	//			{
	//				V2 tileXY;
	//				tileXY.x=x;
	//				tileXY.y=y;
	//				return tileXY;
	//			}
	//		}
	//	}
	//	V2 error;
	//	error.x = -1;
	//	error.y = -1;
	//	return error;
	//}
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//Debug.Log ("Writing");
			//We own this player: send the others our data			
		}
		else
		{
			//Debug.Log ("Receiving");
			//Network player, receive data
		}
	}
}
