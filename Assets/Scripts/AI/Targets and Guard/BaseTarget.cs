﻿//Jack Ng
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
	public Transform curTargetNode;
	TileMap mTileMap;
	TileMapMouse mMouse;
	
	GameObject mTileMapObject;
	GameObject mPlayer;
	//privates
	private int mPositionX;		//Current Position
	private int mPositionY;		//Current Position
	private int mMouseX;		//Mouse Location
	private int mMouseY;		//Mouse Location
	public DTileMap.TileType mTargetIndex;
	//Network Stuff
	GameManager mManager;
	public bool mTargetTurn;
	
	//Current Stats
	public int mDefence;
	public int mMovement;
	public int mRunMovement;
	public int mInfamy = 1;
	
	//Node currently going to
	public int mTowardChoice;
	//4 nodes to move around
	public int mNodeAX;
	public int mNodeAY;
	public int mWeightA;
	
	public int mNodeBX;
	public int mNodeBY;
	public int mWeightB;
	
	public int mNodeCX;
	public int mNodeCY;
	public int mWeightC;
	
	public int mNodeDX;
	public int mNodeDY;
	public int mWeightD;
	
	private int mTowardNodeX;
	private int mTowardNodeY;
	
	
	//4 nodes to Run around
	public int mNodeRAX;
	public int mNodeRAY;
	public int mWeightRA;
	
	public int mNodeRBX;
	public int mNodeRBY;
	public int mWeightRB;
	
	public int mNodeRCX;
	public int mNodeRCY;
	public int mWeightRC;
	
	public int mNodeRDX;
	public int mNodeRDY;
	public int mWeightRD;
	
	private bool mWalkPathTrue;
	private bool mRunPathTrue;
	//List to Track Graph
	public List<Node>mTowardPath;
	public List<Node>mCurrentPath;
	
	private bool firstTime;

	//Sounds Stuff
	public AudioClip mBlockSound;
	public AudioClip mAttackSound;

	void Start () 
	{
		if(mTargetIndex==DTileMap.TileType.Floor)
		{
			mTargetIndex = DTileMap.TileType.Target1;
		}
		firstTime = true;
		mWalkPathTrue = false;
		mRunPathTrue = false;
		mTargetTurn = false;
	}
	void Update () 
	{
		if(firstTime==true)
		{
			firstTime = false;
			mPositionX = 0;
			mPositionY = 0;
			mTargetTurn = false;
			if(!mManager)
			{
				if(PhotonNetwork.offlineMode)
				{
					mTileMapObject=GameObject.Find("CurrentTileMap");
					mManager = GameObject.Find("GameManager").GetComponent<GameManager>(); // thats how you get infromation from the manager
					mManager.AddTarget (this);	
				}
				else
				{
					mTileMapObject=GameObject.Find("CurrentTileMap(Clone)");
					mManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>(); // thats how you get infromation from the manager
					mManager.AddTarget (this);	
				}
			}
			Debug.Log ("TargetX::" + mPositionX);
			Debug.Log ("TargetY::" + mPositionY);
			if(mTileMapObject)
			{
				mTileMap = mTileMapObject.GetComponent<TileMap>();
				//mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 5);
				Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(mMouseX, mMouseY);
				Move(v3Temp);
			}
			mState = State.Normal;
		}
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;
		
		Vector3 temp = mTileMap.MapInfo.GetTileLocation (mTowardNodeX, mTowardNodeY);
		curTargetNode.position = temp;
	}
	public bool UpdateTarget()
	{
		switch (mState)
		{
		case State.Normal:
			UpdateNormal ();
			mTargetTurn = false;
			break;
		case State.Run:
			UpdateRun ();
			mTargetTurn = false;
			break;
		case State.Die:
			UpdateDie ();
			mTargetTurn = false;
			break;
		default:
			Debug.Log ("Unknown state!");
			break;
		}
		if (!mManager)
		{
			mManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>(); // thats how you get infromation from the manager
			if(mManager)
			{
				mManager.AddTarget(this);
			}
		}
		return true;
	}
	void UpdateNormal()
	{
		if(mTileMap.MapInfo.GetTileType (mTowardNodeX, mTowardNodeY) != DTileMap.TileType.Floor)
		{
			mWalkPathTrue = false;
		}
		if(mWalkPathTrue==false)
		{
			ResetPath(ref mTowardPath);
			PathDecision (true);
			mWalkPathTrue = true;
		}
		//Find Range, and find current path
		mTowardPath = PathFind (mPositionX, mPositionY, mTowardNodeX, mTowardNodeY);
		int index = PathFindRange (ref mTowardPath, mMovement);
		if(index == -1)
		{
			Travel (mPositionX, mPositionY);
		}
		else
		{
			int x = 0;
			int y = 0;
			mTileMap.MapInfo.IndexToXY (index, out x, out y);
			Travel (x, y);
		}
		if(mPositionX == mTowardNodeX && mPositionY == mTowardNodeY)
		{
			mRunPathTrue = false;
		}
	}
	void UpdateRun()
	{
		if(mRunPathTrue==false)
		{
			ResetPath(ref mTowardPath);
			//Decide on a Path;
			PathDecision (false);
			//Find Target Node path;
			
			Debug.Log("CurrentChoice : " + mTowardChoice);
			mRunPathTrue = true;
		}
		mTowardPath = PathFind (mPositionX, mPositionY, mTowardNodeX, mTowardNodeY);
		int index = PathFindRange (ref mTowardPath, mRunMovement);
		int tempX = 0;
		int tempY = 0;
		mTileMap.MapInfo.IndexToXY (index, out tempX, out tempY);
		Travel (tempX, tempY);
		//Reset currentPath
		ResetPath (ref mCurrentPath);
		if(mPositionX == mTowardNodeX && mPositionY == mTowardNodeY)
		{
			ResetPath (ref mTowardPath);
			mRunPathTrue = false;
		}
		if(Input.GetKey ("n"))
		{
			ResetPath (ref mCurrentPath);
			ResetPath (ref mTowardPath);
			mWalkPathTrue = false;
			mState=State.Run;
		}
	}
	
	public void UpdateDie()
	{
		PhotonNetwork.Destroy (gameObject);
	}
	
	void PathDecision(bool WalkTrue)//Decision on Paths
	{
		if(WalkTrue == true)
		{
			//Weight calulation for decision on which path
			int totalWeight = mWeightA + mWeightB + mWeightC + mWeightD;
			int randomInt = Random.Range (0, totalWeight);
			int tempAB = mWeightA + mWeightB;
			int tempABC = mWeightA + mWeightB + mWeightC;
			if(randomInt<mWeightA)
			{
				mTowardNodeX = mNodeAX;
				mTowardNodeY = mNodeAY;
				mTowardChoice = 0;
			}
			else if(randomInt >= mWeightA && randomInt < tempAB)
			{
				mTowardNodeX = mNodeBX;
				mTowardNodeY = mNodeBY;
				mTowardChoice = 1;
			}
			else if(randomInt >= tempAB && randomInt < tempABC)
			{
				mTowardNodeX = mNodeCX;
				mTowardNodeY = mNodeCY;
				mTowardChoice = 2;
			}
			else
			{
				mTowardNodeX = mNodeDX;
				mTowardNodeY = mNodeDY;
				mTowardChoice = 3;
			}
		}
		else
		{
			int totalWeight = mWeightRA + mWeightRB + mWeightRC + mWeightRD;
			int randomInt = Random.Range (0, totalWeight);
			int tempAB = mWeightRA + mWeightRB;
			int tempABC = mWeightRA + mWeightRB + mWeightRC;
			if(randomInt<mWeightRA)
			{
				mTowardNodeX = mNodeRAX;
				mTowardNodeY = mNodeRAY;
				mTowardChoice = 4;
			}
			else if(randomInt >= mWeightRA && randomInt < tempAB)
			{
				mTowardNodeX = mNodeRBX;
				mTowardNodeY = mNodeRBY;
				mTowardChoice = 5;
			}
			else if(randomInt >= tempAB && randomInt < tempABC)
			{
				mTowardNodeX = mNodeRCX;
				mTowardNodeY = mNodeRCY;
				mTowardChoice = 6;
			}
			else
			{
				mTowardNodeX = mNodeRDX;
				mTowardNodeY = mNodeRDY;
				mTowardChoice = 7;
			}
		}
	}
	//Path Find Parts
	List<Node> PathFind(int startX, int startY, int endX, int endY)
	{
		List<Node> Path = null;
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.PathFind(startX, startY, endX, endY);
		if(mSearch.IsFound())
		{
			//mCloseList = mSearch.GetCloseList();
			Path= mSearch.GetPathList();
			//foreach(Node i in Path)
			//{
			//	mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,DTileMap.TileType.Path);
			//}
		}
		else
		{
			Debug.Log("No Path is found");
		}
		return Path;
	}	 
	int PathFindRange(ref List<Node> totalPath, int range)
	{
		foreach(Node i in totalPath )
		{
			if(i.g == range)
			{
				DTileMap.TileType temp = mTileMap.MapInfo.GetTileTypeIndex (i.mIndex);
				if(temp == DTileMap.TileType.Floor)
				{
					return i.mIndex;
				}
				else
				{
					range--;
				}
			}
		}
		return -1;
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
			mTileMap.MapInfo.SetTileTypeIndex (x,DTileMap.TileType.Floor, true);
		}
		Path.Clear ();
	}
	void Travel(int x, int y)
	{
		mTileMap.MapInfo.SetTileType(mPositionX, mPositionY, DTileMap.TileType.Floor, true);
		Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(x, y);		
		Move(v3Temp);
		mPositionX = x;
		mPositionY = y;
		mTileMap.MapInfo.SetTileType(mPositionX, mPositionY, mTargetIndex, false);
	}
	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}
}
