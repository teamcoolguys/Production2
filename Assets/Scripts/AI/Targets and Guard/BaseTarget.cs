//Jack Ng
//Nov 5th, 2014


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(GraphSearch))]
[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(Node))]

[RequireComponent(typeof(CatGentleman))]
[RequireComponent(typeof(PenguinScientist))]
[RequireComponent(typeof(SafeBot))]


public class BaseTarget : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Normal,
		Run,
		Die,
		Count
	}
	//State Currently in
	public State mState;
	public int mPanicTimer;
	private int mPanic;
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
	public string mName;
	public int mDefence;
	public int mMovement;
	public int mRunMovement;
	public int mInfamy = 1;

	//Node currently going to
	public int mTowardChoice;
	//4 nodes to move around
	public int mNodeA;
	public int mWeightA;

	public int mNodeB;
	public int mWeightB;

	public int mNodeC;
	public int mWeightC;

	public int mNodeD;
	public int mWeightD;

	private int mTowardNodeX;
	private int mTowardNodeY;

	public int mNodeE;
	public int mWeightE;
	
	public int mNodeF;
	public int mWeightF;
	
	public int mNodeG;
	public int mWeightG;
	
	public int mNodeH;
	public int mWeightH;

	private bool mWalkPathTrue;
	private bool mRunPathTrue;
	//List to Track Graph
	public List<Node>mTowardPath;
	public List<Node>mCurrentPath;

	private bool firstTime;

	public void SetPanic()
	{
		mPanic = mPanicTimer;
	}
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
		mState = State.Spawn;
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
				}
				else
				{
					mTileMapObject=GameObject.Find("CurrentTileMap(Clone)");
					mManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>(); // thats how you get infromation from the manager

				}
			}
			//Debug.Log ("TargetX::" + mPositionX);
			//Debug.Log ("TargetY::" + mPositionY);
			if(mTileMapObject)
			{
				mTileMap = mTileMapObject.GetComponent<TileMap>();
				//mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 5);
				Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(mMouseX, mMouseY);
				Move(v3Temp);
			}
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
		case State.Spawn:
			UpdateSpawn ();
			break;
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
		}
		return true;
	}
	void UpdateSpawn()
	{
		//Character Import:
		int randomInt = Random.Range (0, 3);
		switch(randomInt)
		{
		case 0:
			CatGentleman mCatGentleman = new CatGentleman ();
			mName = mCatGentleman.mCharacterName;
			mMovement = mCatGentleman.mInputMovement;
			mRunMovement = mCatGentleman.mInputRunMovement;
			mInfamy = mCatGentleman.mInputInfamy;
			break;
		case 1:
			SafeBot mSafeBot = new SafeBot ();
			mName = mSafeBot.mCharacterName;
			mMovement = mSafeBot.mInputMovement;
			mRunMovement = mSafeBot.mInputRunMovement;
			mInfamy = mSafeBot.mInputInfamy;
			break;
		case 2:
			PenguinScientist mPenguinScientist = new PenguinScientist ();
			mName = mPenguinScientist.mCharacterName;
			mMovement = mPenguinScientist.mInputMovement;
			mRunMovement = mPenguinScientist.mInputRunMovement;
			mInfamy = mPenguinScientist.mInputInfamy;
			break;
		}
		PathDecision ();
		while(mTileMap.MapInfo.GetTileType (mTowardNodeX, mTowardNodeY) != DTileMap.TileType.Floor)
		{
			PathDecision();
		}
		Debug.Log ("Target: Spawn Choice: " + mTowardNodeX + ", " + mTowardNodeY);
		Travel (mTowardNodeX, mTowardNodeY);
		mState = State.Normal;
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
			PathDecision ();
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
		if(mTileMap.MapInfo.GetTileType (mTowardNodeX, mTowardNodeY) != DTileMap.TileType.Floor)
		{
			mWalkPathTrue = false;
		}
		if(mWalkPathTrue==false)
		{
			ResetPath(ref mTowardPath);
			PathDecision ();
			mWalkPathTrue = true;
		}
		//Find Range, and find current path
		mTowardPath = PathFind (mPositionX, mPositionY, mTowardNodeX, mTowardNodeY);
		int index = PathFindRange (ref mTowardPath, mRunMovement);
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
		if(mPanic == 0)
		{
			mState = State.Normal;
		}
		else
		{
			mPanic--;
		}
	}

	public void UpdateDie()
	{
		mTileMap.MapInfo.SetTileType (mPositionX, mPositionY, DTileMap.TileType.Floor, true);
		mManager.curDefending = DTileMap.TileType.TargetSpot;
		PhotonNetwork.Destroy (gameObject);
		Destroy (gameObject);
		mManager.RemoveTarget (this);
	}

	void PathDecision()//Decision on Paths
	{
		//Weight calulation for decision on which path
		int totalWeight = mWeightA + mWeightB + mWeightC + mWeightD + mWeightE + mWeightF + mWeightG + mWeightH;
		int randomInt = Random.Range (0, totalWeight);
		int tempAB = mWeightA + mWeightB;
		int tempABC = tempAB + mWeightC;
		int tempABCD = tempABC + mWeightD;
		int tempABCDE = tempABCD + mWeightE;
		int tempABCDEF = tempABCDE + mWeightF;
		int tempABCDEFG = tempABCDEF + mWeightG;
		int tempABCDEFGH = tempABCDEFG + mWeightH;
		if(randomInt<mWeightA)
		{
			mTileMap.MapInfo.IndexToXY( mNodeA, out mTowardNodeX, out mTowardNodeY);
			mTowardChoice = 0;
		}
		else if(randomInt >= mWeightA && randomInt < tempAB)
		{
			mTileMap.MapInfo.IndexToXY( mNodeB, out mTowardNodeX, out mTowardNodeY);
			mTowardChoice = 1;
		}
		else if(randomInt >= tempAB && randomInt < tempABC)
		{
			mTileMap.MapInfo.IndexToXY( mNodeC, out mTowardNodeX, out mTowardNodeY);
			mTowardChoice = 2;
		}
		else if(randomInt >= tempABC && randomInt < tempABCD)
		{
			mTileMap.MapInfo.IndexToXY( mNodeD, out mTowardNodeX, out mTowardNodeY);
			mTowardChoice = 3;
		}
		else if(randomInt >= tempABCD && randomInt < tempABCDE)
		{
			mTileMap.MapInfo.IndexToXY( mNodeE, out mTowardNodeX, out mTowardNodeY);
			mTowardChoice = 4;
		}
		else if(randomInt >= tempABCDE && randomInt < tempABCDEF)
		{
			mTileMap.MapInfo.IndexToXY( mNodeF, out mTowardNodeX, out mTowardNodeY);
			mTowardChoice = 5;
		}
		else if(randomInt >= tempABCDEF && randomInt < tempABCDEFG)
		{
			mTileMap.MapInfo.IndexToXY( mNodeG, out mTowardNodeX, out mTowardNodeY);
			mTowardChoice = 6;
		}
		else if(randomInt >= tempABCDEFG && randomInt < tempABCDEFGH)
		{
			mTileMap.MapInfo.IndexToXY( mNodeH, out mTowardNodeX, out mTowardNodeY);
			mTowardChoice = 7;
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
		if (range >= totalPath.Count) 
		{
			DTileMap.TileType check = mTileMap.MapInfo.GetTileTypeIndex (totalPath[0].mIndex);
			if(check == DTileMap.TileType.Floor)
			{
				return  totalPath[0].mIndex;
			}
		}
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
