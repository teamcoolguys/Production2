//Created by Dylan Fraser
//Prodcution 1
//Updated by Jack Ng
//Production 1
//Updated by Jack Ng
//Jan 8th
using UnityEngine;

public class DTileMap {
	
	/*protected class DTile {
		bool isWalkable = false;
		int tileGraphicId = 0;
		string name = "Unknown";
	}
	
	List<DTile> tileTypes;
	
	void InitTiles() {
		tileType[1].name = "Floor";
		tileType[1].isWalkable = true;
		tileType[1].tileGraphicId = 1;
		tileType[1].damagePerTurn = 0;
	}*/
	
	int size_x;
	int size_y;

	public struct V2
	{
		public float x;
		public float y;
	}

	public Vector3[] mPositionData;

	public int[] map_data;

	
	public enum TileType
	{
		Unknown,	//0
		Floor,		//1
		Wall,		//2
		Player,		//3
		Target,		//4		
		Guard,		//5
		Shop,		//6
		Buildings	//7
	};
	public DTileMap(int sizex, int sizey)
	{

		this.size_x = sizex;
		this.size_y = sizey;
		
		map_data = new int[sizex*sizey];
		for(int y=0;y<sizey;y++)
		{
			for(int x=0;x<sizex;x++) 
			{
				map_data[(y*size_x)+x] = 1;
			}
		}
		map_data[9] = 0;
		//Position
		Vector3 startLocation;
		startLocation.x = 0.5f;
		startLocation.y = 0.5f;
		startLocation.z = 0.5f;

		mPositionData = new Vector3[sizex*sizey];
		for(int y=0;y<sizey;y++)
		{
			for(int x=0;x<sizex;x++) 
			{
				Vector3 currentLocation = startLocation;
				currentLocation.x += x*1.0f;
				currentLocation.z += y*1.0f;
				mPositionData[(y*size_x)+x] =currentLocation;
			}
		}
	}

	public int GetTileType(int x, int y)
	{
		return map_data[(y*size_x)+x];
	}
	public void SetTileType(int x, int y, int i)
	{
		map_data[(y*size_x)+x]=i;
		return;
	}
	public Vector3 GetTileLocation(int x, int y)
	{
		return mPositionData[(y*size_x)+x];
	}
	public V2 GetTileXY(Vector3 location)
	{
		for(int y=0;y<size_x;y++)
		{
			for(int x=0;x<size_y;x++) 
			{
				Vector3 tempPosition=mPositionData[(y*size_x)+x];
				if(location.x == tempPosition.x
				   && location.y == tempPosition.y
				   && location.z == tempPosition.z)
				{
					V2 tileXY;
					tileXY.x=x;
					tileXY.y=y;
					return tileXY;
				}
			}
		}
		V2 error;
		error.x = -1;
		error.y = -1;
		return error;
	}
}
