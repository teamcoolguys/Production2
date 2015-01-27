//Jack Ng
//Nov 5th, 2014


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(GraphSearch))]
[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(Node))]
public class BaseTarget : MonoBehaviour
{
	public enum State
	{
		Normal,
		Run,
		Die,
		Count
	}
	//State Currently in
	private State mState;
	//Imformation Checking for the Game
	TileMap mTileMap;
	TileMapMouse mMouse;
	GameObject mTileMapObject;
	GameObject mPlayer;
								//privates
	private int mPositionX;		//Current Position
	private int mPositionY;		//Current Position
	private int mMouseX;		//Mouse Location
	private int mMouseY;		//Mouse Location

	//Network Stuff
	GameManager mManager;
	public bool mTargetTurn;

	//Current Stats
	public int mDefense;
	public int mMovement;
	public int mRunMovement;
	public int mInfamy;

	//3 nodes to move around
	public int mNodeAX;
	public int mNodeAY;
	public int mWeightA;

	public int mNodeBX;
	public int mNodeBY;
	public int mWeightB;

	public int mNodeCX;
	public int mNodeCY;
	public int mWeightC;

	public int mTowardNodeX;
	public int mTowardNodeY;
	public int mTowardChoice;
	
	private bool mPathTrue;
	//List to Track Graph
	public List<Node>mTowardPath;
	public List<Node>mCurrentPath;

	private bool firstTime;

	void Start () 
	{

		firstTime = true;
	}
	void Update () 
	{
		if(firstTime==true)
		{
			firstTime = false;
			mPositionX = 0;
			mPositionY = 0;
			mTargetTurn = false;
			mTileMapObject=GameObject.Find("CurrentTileMap");
			//mManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
			mTileMap = mTileMapObject.GetComponent<TileMap>();
			//mManager.AddTarget (this);
			mState = State.Normal;
			//mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 5);
			Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(mMouseX, mMouseY);
			Move(v3Temp);
		}
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;

		if (Input.GetKey ("b"))
		{
			UpdateTarget ();
		}
	}
	public bool UpdateTarget()
	{
		bool rc = false;
		bool walk = false;
		switch (mState)
		{
		case State.Normal:
			UpdateNormal ();
			break;
		case State.Run:
			UpdateRun ();
			break;
		case State.Die:
			UpdateDie ();
			break;
		default:
			Debug.Log ("Unknown state!");
			break;
		}
		return true;
		/*
		int temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
			//Random moveMent;
			switch(temp)
			{
				case 1:
					{
						Debug.Log ("Target::Floor");
						mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 1);
						Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(mMouseX, mMouseY);
						Move(v3Temp);
						mPositionX = mMouseX;
						mPositionY = mMouseY;
						mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 4);
						mTargetTurn = true;
						rc = true;
						walk = true;
						break;
					}
				case 2:
					{
						Debug.Log ("Target::Wall");
						break;
					}
				default:
					{
						Debug.Log ("Target::Fuck Off");
						break;
					}
			}
			if(walk == true)
			{
				Debug.Log("Walking");
			}
		//}
		return rc;
		*/
	}

	void UpdateNormal()
	{
		//Decide on a Path;
		PathDecision ();
		Debug.Log("CurrentChoice : " + mTowardChoice);
		//Find Path;
		mTowardPath = PathFind (mPositionX, mPositionY, mTowardNodeX, mTowardNodeY);
		mCurrentPath = PathFindRange (ref mTowardPath, mMovement);
		int index = mCurrentPath.Count;

		if(Input.GetKey ("v"))
		{
			mState=State.Run;
		}
	}
	void UpdateRun()
	{


	}
	void UpdateDie()
	{

	}
	void PathDecision()//Decision on Paths
	{
		//Weight calulation for decision on which path
		int totalWeight = mWeightA + mWeightB + mWeightC;
		int randomInt = Random.Range (0, totalWeight);
		int temp = mWeightA + mWeightB;
		if(randomInt<mWeightA)
		{
			mTowardNodeX = mNodeAX;
			mTowardNodeY = mNodeAY;
			mTowardChoice = 0;
		}
		else if(randomInt>=mWeightA&&randomInt<temp)
		{
			mTowardNodeX = mNodeBX;
			mTowardNodeY = mNodeBY;
			mTowardChoice = 1;
		}
		else
		{
			mTowardNodeX = mNodeCX;
			mTowardNodeY = mNodeCY;
			mTowardChoice = 2;
		}

	}
	//Path Find Parts
	List<Node> PathFind(int startX, int startY, int endX, int endY)
	{
		List<Node> Path = null;
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.Run(startX, startY, endX, endY);
		if(mSearch.IsFound())
		{
			//mCloseList = mSearch.GetCloseList();
			Path= mSearch.GetPathList();
			foreach(Node i in Path)
			{
				mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,3);
			}
		}
		else
		{
			Debug.Log("No Path is found");
		}
		return Path;
	}	
	List<Node> PathFindRange(ref List<Node> totalPath, int range)
	{
		List<Node> Path = new List<Node>();
		if (totalPath.Count >= range) 
		{
			return Path;
		}
		else
		{
			int temp = totalPath.Count-range;
			for (int i=(totalPath.Count-1); i>=temp; i--)
			{
				Path.Add (totalPath[i]);
				totalPath.RemoveAt (i);
			}
			foreach(Node i in Path)
			{
				mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,1);
			}
			return Path;
		}

		//foreach(Node i in totalPath)
		//{
		//
		//}
		//foreach(Node i in Path)
		//{
		//	mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,1);
		//}
		return Path;
	}	
	void ResetPath(ref List<Node> Path)
	{
		if (Path == null) 
		{
			return;
		}
		for (int i=0; i<Path.Count; i++)
		{
			int x = Path[i].mIndex;
			mTileMap.MapInfo.SetTileTypeIndex (x,0);
		}
		Path.Clear ();
	}
	void Travel(int x, int y)
	{
		mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 5);
		Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(x, y);
		Move(v3Temp);
		mPositionX = x;
		mPositionY = y;
	}
	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}
}
