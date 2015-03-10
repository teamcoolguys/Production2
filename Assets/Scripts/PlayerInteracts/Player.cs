
//Created by Dylan Fraser
//November 3, 2014
//Updated by
//Jack Ng
//November 4, 2014
//Wyatt Gibbs
//December 10, 2014
//Jack Ng
//Jan 8th, 2015
//Rewritten by Jack Ng
//Feb 5th, 2015

//Engine usuage
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Scripts requirement
[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(TileMapMouse))]
//[RequireComponent(typeof(GameManager))]

[RequireComponent(typeof(GraphSearch))]
[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(Node))]
//[RequireComponent(typeof(HUD))]

[RequireComponent(typeof(WeldingtonAutomaton))]
[RequireComponent(typeof(ThordrannDwarf))]
[RequireComponent(typeof(CalamityNinja))]
[RequireComponent(typeof(AnchorbeardPirate))]

public class Player : MonoBehaviour
{
	public enum PlayerPhase
	{
		Respawn,
		Sewer,
		Start,
		MayBeMove,
		Move,
		Play,
		Special,
		Attack,
		AttackMaybe,
		End
	};

	public enum Character
	{
		Weldington,
		Thordrann,
		Calamity,
		Anchorbeard
	};

	//public enum  ClickPhase
	//{
	//	Zero,
	//	First,		// Show Path
	//	Second,		// moving
	//	Third,		// Show Attack
	//	Fourth,		// attacking
	//};

	//Information needed in the game 
	public Character mCharacter;					//Character base stats
	private TileMap mTileMap;						//TileMap information
	TileMapMouse mMouse;							//Current Mouse information
	GameObject mTileMapObject;						//TileMap Object
	public Transform[] mAttackSelect = new Transform[4];	//Attack Transform

	//Current Stats			
	public bool mAlive = true;
	public int mAttack;								//Current Player Attack
	public int mDefence;							//Current Player Defence
	public int mMovement;							//Current Player Movment
	public int mRange;								//Current Player Attack Range
	public int mInfamy;							//Current Player Infamy
	public int mSkillsCD = 0;
	public int mRandomMovement = 0;
	public bool mIsU = true;
	//Mouse Info			   		
	private int mMouseX;							//MouseOnTile info on X
	private int mMouseY;							//MouseOnTile info on Y
	public DTileMap.TileType curTarget;
	int mMouseClickPhase = 0;

	//Tracking current Spot//
	public int mStorePositionX;						//Previous TileMap Position X
	public int mStorePositionY;                  	//Previous TileMap Position Y
	public int mPositionX;							//Current TileMap Position X
	public int mPositionY;							//Current TileMap Position Y
	
	//List to Track Graph	  		
	public List<Node>mWalkRangeList;				//List for finding walking range
	public List<Node>mPath;							//List for the actual path for player
	public List<Node>mAttackRangeList;				//List for finding walking range
	public List<DTileMap.TileType>mAttackList;
	public List<Vector3>mAttackPosition;
	public List<int>mIntList;
	public List<int>mAllRespawnIndex;				//Respawn, 256, 73, 330, 10, 198, 314, 86, 386, 252, 114
	public List<int>mAllSewerIndex;					//Sewer: 325, 118, 101, 333, 383, 129, 296, 66
	//Player Loop
	public DTileMap.TileType mPlayerIndex;			//Current Player information
	public PlayerPhase mPlayerPhase = PlayerPhase.Start;
	public bool mMoved;
	public bool mPlayed;
	public bool mTurn = false;
	public bool mOnSewer;
	bool mTurn1 = true;
	bool mSewerWalkable = false;
	public Hand mHand;
	public bool mAttacked;
	private Vector3 syncEndPosition;
	private GameManager mManager;
	//
	public Deck mDeck;
	public GameObject Self;							//GameObject itself
	
	void Start()
	{
		//Hack
		mAllRespawnIndex.Add (256);
		mAllRespawnIndex.Add (73);
		mAllRespawnIndex.Add (330);
		mAllRespawnIndex.Add (10);
		mAllRespawnIndex.Add (198);
		mAllRespawnIndex.Add (314);
		mAllRespawnIndex.Add (86);
		mAllRespawnIndex.Add (386);
		mAllRespawnIndex.Add (252);
		mAllRespawnIndex.Add (114);

		mAllSewerIndex.Add (325);
		mAllSewerIndex.Add (118);
		mAllSewerIndex.Add (101);
		mAllSewerIndex.Add (333);
		mAllSewerIndex.Add (383);
		mAllSewerIndex.Add (129);
		mAllSewerIndex.Add (296);
		mAllSewerIndex.Add (66);
		//Hack
		mTurn1 = true;
		if(mPlayerIndex==DTileMap.TileType.Floor)
		{
			mPlayerIndex = DTileMap.TileType.Player1;
		}

		switch(mCharacter)
		{
		case Character.Anchorbeard:
			AnchorbeardPirate x1 = new AnchorbeardPirate();
			mAttack = x1.mInputAttack;
			mDefence = x1.mInputDefence;
			mMovement = x1.mInputMovement;
			mRange =x1.mInputRange;
			break;
			
		case Character.Calamity:
			CalamityNinja x2 = new CalamityNinja(); 
			mAttack = x2.mInputAttack;
			mDefence = x2.mInputDefence;
			mMovement = x2.mInputMovement;
			mRange =x2.mInputRange;
			break;
			
		case Character.Thordrann:
			ThordrannDwarf x3 = new ThordrannDwarf(); 
			mAttack = x3.mInputAttack;
			mDefence = x3.mInputDefence;
			mMovement = x3.mInputMovement;
			mRange =x3.mInputRange;
			break;
			
		case Character.Weldington:
			WeldingtonAutomaton x4 = new WeldingtonAutomaton(); 
			mAttack = x4.mInputAttack;
			mDefence = x4.mInputDefence;
			mMovement = x4.mInputMovement + 3;
			mRange =x4.mInputRange;
			break;
			
		}
		mInfamy = 1;
		mPlayerPhase = PlayerPhase.Respawn;
		mMoved = false;
		mPlayed = false;
		mTurn = false;
		Debug.Log ("Player: Created");
	}

	void Update()
	{
		//Manager Loop
		if(!mManager)
		{
			if(!PhotonNetwork.offlineMode)
			{
				//Connect the the TIleMap
				mTileMapObject = GameObject.Find ("CurrentTileMap(Clone)");
				mTileMap = mTileMapObject.GetComponent<TileMap>();
				
				//Connect with the Mouse
				mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
				mMouseX = mMouse.mMouseHitX;
				mMouseY = mMouse.mMouseHitY;

				mManager = GameObject.Find ("GameManager(Clone)").GetComponent<GameManager>();
				mManager.AddPlayer (this);//allows gamemanager to know that a new player is active
			}
			else
			{
				//Connect the the TIleMap
				mTileMapObject = GameObject.Find ("CurrentTileMap");
				mTileMap = mTileMapObject.GetComponent<TileMap>();
				
				//Connect with the Mouse
				mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
				mMouseX = mMouse.mMouseHitX;
				mMouseY = mMouse.mMouseHitY;
				mManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
				mManager.AddPlayer (this);//allows gamemanager to know that a new player is active
			}
		}

		//Grabing the Current Mouse and Tile Information
		mMouse = mTileMapObject.GetComponent<TileMapMouse>();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;
		//Debug.Log ("MouseX Info: " + mMouse.mMouseHitX );
		//Debug.Log ("MouseY Info: " + mMouse.mMouseHitY );
		//Put Player on Map at Starting Position
		Teleport (mPositionX, mPositionY);
		//Quick button checks

		//Update the whole player function
		//Wall building code
		//Wall building code
		if (Input.GetKey ("o")) 
		{
			int temp = mTileMap.MapInfo.XYToIndex(mMouseX,mMouseY);
			int x = 0; 
			int y = 0; 
			mTileMap.MapInfo.IndexToXY(temp, out x, out y);
			DTileMap.TileType tempType = mTileMap.MapInfo.GetTileType(x,y);
			Debug.Log ("Player:Index" + temp +" x: " + x + " y: " + y + " TileType: " + tempType);

		}
		//if (Input.GetKey ("p")) 
		//{
		//	mTileMap.MapInfo.SetTileType(mMouseX,mMouseY, DTileMap.TileType.Floor, true);
		//}
		//if(Input.GetMouseButtonDown(0))
		//{	
		//	if(Input.GetMouseButtonUp(0))
		//	{
		//		if(mClick)
		//		{
		//			mMouseClickPhase++;
		//		}
		//	}
		//}
	}
	IEnumerator WaitAndPrint(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
	}
	
	
	public bool UpdatePlayer()
	{
		switch (mPlayerPhase)
		{
			case PlayerPhase.Sewer:
				UpdateSewer ();
				break;
			case PlayerPhase.Respawn:
				UpdateRespawn();
				break;
			case PlayerPhase.Start:
				UpdateStart ();
				break;
			case PlayerPhase.Move:
				UpdateMove ();
				break;
			case PlayerPhase.Special:
				UpdateSpecial();
				break;
			case PlayerPhase.Attack:
				UpdateAttack();
				break;
			case PlayerPhase.AttackMaybe:
				UpdateAttackMaybe();
				break;
			case PlayerPhase.Play:
				UpdatePlay ();
				break;
			case PlayerPhase.End:
				mManager.curDefending = DTileMap.TileType.TargetSpot;
				UpdateEnd ();
				break;
			case PlayerPhase.MayBeMove:
				UpdateMayBeMove();
				break;
			default:
				Debug.Log ("Player:Unknown state!");
				break;
		}

		return true;
	}
	void UpdateRespawn()
	{
		foreach(int i in mAllRespawnIndex)
		{
			DTileMap.TileType temp= mTileMap.MapInfo.GetTileTypeIndex (i) ;
			if(temp == DTileMap.TileType.Floor)
			{
				mTileMap.MapInfo.SetTileTypeIndex (i, DTileMap.TileType.PlayerSpawn, true) ;
			}
		}
		DTileMap.TileType curValue = mTileMap.MapInfo.GetTileType (mMouseX, mMouseY);
		if(Input.GetMouseButtonDown(0) && curValue == DTileMap.TileType.PlayerSpawn)
		{
			mAlive = true;
			gameObject.collider.renderer.enabled = true;
			Teleport (mMouseX, mMouseY);
			foreach(int i in mAllRespawnIndex)
			{
				DTileMap.TileType temp= mTileMap.MapInfo.GetTileTypeIndex (i) ;
				if(temp == DTileMap.TileType.PlayerSpawn)
				{
					mTileMap.MapInfo.SetTileTypeIndex (i, DTileMap.TileType.Floor, true) ;
				}
			}
			mPlayerPhase = PlayerPhase.Start;
			if(mTurn1 == true)
			{
				mTurn1 = false;
				mPlayerPhase = PlayerPhase.End;
			}
		}
	}
	void UpdateSewer()
	{
		if(mSewerWalkable == false)
		{
			foreach(int i in mAllSewerIndex)
			{
				DTileMap.TileType temp= mTileMap.MapInfo.GetTileTypeIndex (i) ;
				if(temp == DTileMap.TileType.Sewer)
				{
					mTileMap.MapInfo.SetTileTypeIndex (i, DTileMap.TileType.TrueSewer, true) ;
				}
			}
		}
		DTileMap.TileType curValue = mTileMap.MapInfo.GetTileType (mMouseX, mMouseY);
		if(Input.GetMouseButtonDown(0)&& curValue == DTileMap.TileType.TrueSewer)
		{

			Teleport (mMouseX, mMouseY);
			foreach(int i in mAllSewerIndex)
			{
				DTileMap.TileType temp= mTileMap.MapInfo.GetTileTypeIndex (i) ;
				if(temp == DTileMap.TileType.TrueSewer)
				{
					mTileMap.MapInfo.SetTileTypeIndex (i, DTileMap.TileType.Sewer, true) ;
				}
			}
			FindWalkRange (1);
			curValue = mTileMap.MapInfo.GetTileType (mMouseX, mMouseY);
			mSewerWalkable = true;
		}
		if(Input.GetMouseButtonDown(0)&& curValue == DTileMap.TileType.Walkable && mSewerWalkable == true)
		{
			Teleport (mMouseX, mMouseY);
			foreach(int i in mIntList)
			{
				if(mTileMap.MapInfo.GetTileTypeIndex(i) == DTileMap.TileType.Walkable )
				{
					mTileMap.MapInfo.SetTileTypeIndex(i, DTileMap.TileType.Floor,true);
				}
			}
			mOnSewer = false;
			gameObject.renderer.enabled = true;
			mPlayerPhase = PlayerPhase.Start;
			mSewerWalkable = false;
		}
	}

	void UpdateStart ()
	{
		if(mAlive == false)
		{
			mPlayerPhase = PlayerPhase.Respawn;
		}
		else if(mOnSewer)
		{
			mPlayerPhase = PlayerPhase.Sewer;
		}
		else
		{
			if(Input.GetMouseButtonDown(1))
			{
				ResetWalkRange ();
				mPlayerPhase = PlayerPhase.End;
			}
			if(mMouse.cubeActive == true)
			{
				ResetFindAttackRange();
				FindAttackRange();
				if(mAttackList!=null)
				{
					int count = 0;
					foreach (Vector3 i in mAttackPosition)
					{
						mAttackSelect[count].position = i;
						mAttackSelect[count].renderer.enabled = true;
						count++;
					}
				}
				else
				{
					for(int i = 0; i<4; i++)
					{
						mAttackSelect[i].renderer.enabled = false;
					}
				}
				//Debug.Log ("Player::StateStart");
				if(mMoved==false)
				{
					FindWalkRange (mMovement);	//FInd all walkable Tiles
				}
				else
				{
					ResetWalkRange ();
					ResetPath();
				}
				if(Input.GetMouseButtonDown(0))
				{	
					mStorePositionX = mMouseX;
					mStorePositionY = mMouseY;
					DTileMap.TileType temp=mTileMap.MapInfo.GetTileType(mStorePositionX, mStorePositionY);
					switch(temp)
					{
					case DTileMap.TileType.Floor:
						Debug.Log ("Player::Floor(out of range) "+mMouseClickPhase);
						mMouseClickPhase = 0;
						break;
					case DTileMap.TileType.Walkable:
						Debug.Log ("Player::Walkable");
						if(mMoved == false)
						{
							mPlayerPhase = PlayerPhase.MayBeMove;
						}
						break;			
					case DTileMap.TileType.Path:
						Debug.Log ("Player::Path: Invalid");
						if(mMoved == false)
						{
							mPlayerPhase = PlayerPhase.MayBeMove;
						}
						break;
					case DTileMap.TileType.Wall:
						Debug.Log ("Player::Wall: Can't travel");
						break;
					case DTileMap.TileType.Sewer:
						Debug.Log ("Player::Sewer: out of range");
						break;
					case DTileMap.TileType.PlayerSpawn:
						Debug.Log ("Player::PlayerSpawn");
						break;
					case DTileMap.TileType.Player1:
						Debug.Log ("Player::Player1");
						mManager.curDefending = DTileMap.TileType.Player1;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Player1)
							{
								mPlayerPhase = PlayerPhase.AttackMaybe;
							}
						}
						break;
					case DTileMap.TileType.Player2:
						Debug.Log ("Player::Player2");
						mManager.curDefending = DTileMap.TileType.Player2;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Player2)
							{
								mPlayerPhase = PlayerPhase.AttackMaybe;
							}
						}
						break;
					case DTileMap.TileType.Player3:
						Debug.Log ("Player::Player3");
			
						mManager.curDefending = DTileMap.TileType.Player3;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Player3)
							{
								mPlayerPhase = PlayerPhase.AttackMaybe;
							}
						}
						break;
					case DTileMap.TileType.Player4:
						Debug.Log ("Player::Player4");
						mManager.curDefending = DTileMap.TileType.Player4;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Player4)
							{
								mPlayerPhase = PlayerPhase.AttackMaybe;
							}
						}
						break;
					case DTileMap.TileType.Target1:
						Debug.Log ("Player::Target1");
						mManager.curDefending = DTileMap.TileType.Target1;
						curTarget = DTileMap.TileType.Target1;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Target1)
							{
								mPlayerPhase = PlayerPhase.AttackMaybe;
							}
						}
						break;
					case DTileMap.TileType.Target2:
						Debug.Log ("Player::Target2");
						mManager.curDefending = DTileMap.TileType.Target2;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Target2)
							{
								mPlayerPhase = PlayerPhase.AttackMaybe;
							}
						}
						break;
					case DTileMap.TileType.Target3:
						Debug.Log ("Player::Target3");
						mManager.curDefending = DTileMap.TileType.Target3;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Target3)
							{
								mPlayerPhase = PlayerPhase.AttackMaybe;
							}
						}
						break;
					case DTileMap.TileType.TrueSewer:		//Transfer to Sewer EndTurn
						Debug.Log ("Player::TrueSewer");
						mOnSewer = true;
						TravelToSewer (mStorePositionX, mStorePositionY);
						ResetPath ();
						break;
					}
				}
			}
			else if(mMouse.cubeActive == false)
			{
				mPlayerPhase = PlayerPhase.Play;
			}
			if(Input.GetKeyDown ("s") && mSkillsCD == 0 )
			{
				if(mCharacter == Character.Thordrann)
				{
					mRandomMovement =  Random.Range(1,8);	
					Debug.Log ("You roll a " + mRandomMovement);
				}
				mPlayerPhase = PlayerPhase.Special;
			}
		}
		//Debug.Log ("Player: "+ mPlayerIndex);
	}
	void UpdateMove()
	{
		//Debug.Log ("Player::StateMove");
		Travel (mStorePositionX, mStorePositionY);
		mMoved = true;
		mPlayerPhase = PlayerPhase.Start;
	}
	void UpdateMayBeMove()
	{
		//Debug.Log ("Player::StatePath");
		PathFind( mPositionX, mPositionY, mStorePositionX, mStorePositionY);
		DTileMap.TileType temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
		if(temp==DTileMap.TileType.Walkable|| temp==DTileMap.TileType.Path)
		{
			mStorePositionX = mMouseX;
			mStorePositionY = mMouseY;
		}
		else if(mMouseY==mStorePositionY && mMouseX==mStorePositionX&& Input.GetMouseButtonDown(0))
		{
			mPlayerPhase = PlayerPhase.Move;
		}
		else if(Input.GetMouseButtonDown(1))
		{
			ResetPath ();
			mPlayerPhase = PlayerPhase.Start;
		}
	}
	void UpdateSpecial()
	{
		switch(mCharacter)
		{
		case Character.Anchorbeard:
			AnchorbeardActive();
			break;
			
		case Character.Calamity:
			//Notdone
			break;
			
		case Character.Thordrann:
			ThordrannActive(mRandomMovement);
			break;
			
		case Character.Weldington:
			WeldingtonActive ();
			break;
		}
	}
	void UpdateAttackMaybe()
	{
		Debug.Log ("MayBeAttack");
		if(Input.GetMouseButtonDown(0))
		{
			mAttacked = true;
			mPlayerPhase = PlayerPhase.Attack;
		}
		else if (Input.GetMouseButtonDown (1))
		{
			mPlayerPhase = PlayerPhase.Attack;
		}
	}
	void UpdateAttack()
	{
		if(mManager.HudUpdated)
		{
			Debug.Log ("Attack" + mManager.HudUpdated);
			DTileMap.TileType AttackingTarget = mManager.curDefending;
			if(AttackingTarget>=DTileMap.TileType.Target1)
			{
				BaseTarget targetDefending = mManager.CurrentTargetDefender();
				if(mManager.AttackWorked)
				{
					Debug.Log ("Target Die");
					mInfamy+=targetDefending.mInfamy;
					targetDefending.UpdateDie();
				}
				else
				{
					Debug.Log ("Both Live");
					targetDefending.mState = BaseTarget.State.Run;
				}
			}
			else
			{
				Player playerDefending = mManager.CurrentPlayerDefender ();
				if(mManager.AttackWorked)
				{
					Debug.Log ("Attack Worked");
					Debug.Log ("you die");
					if(playerDefending.mInfamy>mInfamy)
					{
						mInfamy++;
						playerDefending.mInfamy--;
					}
					playerDefending.mAlive = false;
					playerDefending.gameObject.renderer.enabled = false;
					int positionX = playerDefending.mPositionX;
					int positionY = playerDefending.mPositionY;
					mTileMap.MapInfo.SetTileType (positionX, positionY, DTileMap.TileType.Floor, true);
				}
				else if(mManager.CounterAttackWorked)
				{
					Debug.Log ("Counter Attack Worked");
					Debug.Log ("I die");
					if(playerDefending.mInfamy < mInfamy)
					{
						mInfamy--;
						playerDefending.mInfamy++;
					}
					gameObject.renderer.enabled = false;
					mTileMap.MapInfo.SetTileType (mPositionX, mPositionY, DTileMap.TileType.Floor, true);
				}
				else
				{
					Debug.Log ("Both Live");
				}
			}
			if(mManager.HudUpdated)
			{
				mPlayerPhase = PlayerPhase.End;
			}
		}
	}
	void UpdatePlay()
	{
		//Debug.Log ("PlayerTurn::PlayCard");
		if(mMouse.cubeActive==true)
		{
			mPlayerPhase = PlayerPhase.Start;
		}
	}
	void UpdateEnd()
	{
		DTileMap.TileType temp = mTileMap.MapInfo.GetTileType (mPositionX, mPositionY);
		//Debug.Log ("TileType3" + temp);
		ResetFindAttackRange ();
		ResetWalkRange ();
		//Debug.Log ("PlayerTurn Ended");
		if(mSkillsCD!=0)
		{
			mSkillsCD--;
		}
		mTurn = true;
		temp = mTileMap.MapInfo.GetTileType (mPositionX, mPositionY);
		//Debug.Log ("TileType4" + temp);
	}
	public void FindWalkRange(int movement)
	{
		GraphSearch mSearch = new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.RangeSearch(mPositionX, mPositionY, movement);
		mWalkRangeList = mSearch.GetCloseList();
		foreach(Node i in mWalkRangeList)
		{
			int index = i.mIndex;
			DTileMap.TileType temp = mTileMap.MapInfo.GetTileTypeIndex(index);
			if(temp == DTileMap.TileType.Floor)
			{
				mTileMap.MapInfo.SetTileTypeIndex(index, DTileMap.TileType.Walkable, true);
			}
			if(temp == DTileMap.TileType.Sewer)
			{
				mTileMap.MapInfo.SetTileTypeIndex(index, DTileMap.TileType.TrueSewer, true);
			}
		}
	}
	public void ResetFindAttackRange()
	{
		if(mAttackRangeList==null)
		{
			return;
		}
		mAttackPosition.Clear ();
		mAttackList.Clear ();
		mAttackRangeList.Clear ();
		for(int i = 0; i<4; i++)
		{
			mAttackSelect[i].renderer.enabled = false;
		}
	}
	public void FindAttackRange()
	{
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.AttackRange(mPositionX, mPositionY, mRange);
		mAttackRangeList = mSearch.GetCloseList();
		//int positionIndex = mTileMap.MapInfo.XYToIndex (mPositionX, mPositionY);
		mAttackRangeList.RemoveAt(0);
		foreach(Node i in mAttackRangeList)
		{
			int index = i.mIndex;
			DTileMap.TileType temp = mTileMap.MapInfo.GetTileTypeIndex(index);
			if(temp==DTileMap.TileType.Player1)
			{
				mAttackList.Add (DTileMap.TileType.Player1);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Player2)
			{
				mAttackList.Add (DTileMap.TileType.Player2);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Player3)
			{
				mAttackList.Add (DTileMap.TileType.Player3);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Player4)
			{
				mAttackList.Add (DTileMap.TileType.Player4);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Target1)
			{
				mAttackList.Add (DTileMap.TileType.Target1);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Target2)
			{
				mAttackList.Add (DTileMap.TileType.Target2);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Target3)
			{
				mAttackList.Add (DTileMap.TileType.Target3);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
		}
	}
	void Travel(int TileX, int TileY)
	{
		mTileMap.MapInfo.SetTileType(mPositionX, mPositionY, DTileMap.TileType.Floor, true);
		PathFind (mPositionX, mPositionY, TileX, TileY);
		Teleport(TileX, TileY);
	}

	void Teleport(int TileX, int TileY)
	{
		Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(TileX, TileY);
		gameObject.transform.position = v3Temp + new Vector3(0.0f, 1.0f, 0.0f);
		gameObject.GetPhotonView ().RPC ("NetworkUpdatePosition", PhotonTargets.Others, transform.position);
		mPositionX=TileX;
		mPositionY=TileY;
		if(gameObject.renderer.enabled == true)
		{
			mTileMap.MapInfo.SetTileType(mPositionX,mPositionY,mPlayerIndex, false);
		}
	}
	void TravelToSewer(int TileX, int TileY)
	{
		DTileMap.TileType temp = mTileMap.MapInfo.GetTileType (TileX, TileY);
		Debug.Log ("TileType1" + temp);
		mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, DTileMap.TileType.Floor, true);
		gameObject.renderer.enabled = false;
		mPositionX = TileX;
		mPositionY = TileY;
		gameObject.renderer.enabled = false;
		mTileMap.MapInfo.SetTileType(TileX,TileY, DTileMap.TileType.Sewer, true);
		mPlayerPhase = PlayerPhase.End;
		temp = mTileMap.MapInfo.GetTileType (TileX, TileY);
		Debug.Log ("TileType2" + temp);
	}
	[RPC]
	void NetworkUpdatePosition(Vector3 newTransform)
	{
		transform.position = newTransform;
	}
	void PathFind(int startX, int startY, int endX, int endY)
	{
		ResetPath ();
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.PathFind(startX, startY, endX, endY);
		if(mSearch.IsFound())
		{
			mPath= mSearch.GetPathList();
		}
		if( mPath != null )
		{
			foreach(Node i in mPath)
			{
				if(mTileMap.MapInfo.GetTileTypeIndex(i.mIndex)== DTileMap.TileType.Walkable)
				{
					mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,DTileMap.TileType.Path, true);
				}
			}
			if(gameObject.renderer.enabled == true)
			{
				mTileMap.MapInfo.SetTileTypeIndex (mPath[0].mIndex, mPlayerIndex, true);
			}
		}
	}
	//Reset all Path back to Walkable
	void ResetPath()
	{
		if (mPath == null) 
		{
			return;
		}
		for (int i=0; i<mPath.Count; i++)
		{
			int x = mPath[i].mIndex;
			DTileMap.TileType tempType = mTileMap.MapInfo.GetTileTypeIndex (x);
			if(tempType == DTileMap.TileType.Path|| tempType == mPlayerIndex)
			{
				mTileMap.MapInfo.SetTileTypeIndex (x, DTileMap.TileType.Walkable, true);
			}
		}
		mPath.Clear ();
	}

	void ResetWalkRange()
	{
		if (mWalkRangeList == null) 
		{
			return;
		}
		for (int i=0; i<mWalkRangeList.Count; i++)
		{
			int x = mWalkRangeList[i].mIndex;
			DTileMap.TileType tempType = mTileMap.MapInfo.GetTileTypeIndex (x);
			if(tempType == DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileTypeIndex (x, DTileMap.TileType.Floor, true);
			}
			if(tempType == DTileMap.TileType.TrueSewer)
			{
				mTileMap.MapInfo.SetTileTypeIndex (x, DTileMap.TileType.Sewer, true);
			}
		}
		//Debug.Log ("Player: Walk Range Reset");
	}

	void AnchorbeardActive()
	{
		ResetWalkRange ();
		int rightX = 0;
     	int rightY = 0;
     	int leftX =  0;
     	int leftY =  0;
     	int upX = 0;
     	int upY = 0;
     	int downX =  0;
     	int downY =  0;
		//Still need to discard a card
		for(int hookamount = 3; hookamount>=2; hookamount--)
		{
			rightX = mPositionX + hookamount;
         	rightY = mPositionY;
         	leftX = mPositionX - hookamount;
			leftY = mPositionY;
			upX = mPositionX;
			upY = mPositionY + hookamount;
			downX = mPositionX;
			downY = mPositionY - hookamount;
			DTileMap.TileType hookRight = mTileMap.MapInfo.GetTileType (rightX, rightY);
			DTileMap.TileType hookLeft = mTileMap.MapInfo.GetTileType (leftX, leftY);
			DTileMap.TileType hookUp = mTileMap.MapInfo.GetTileType (upX, upY);
			DTileMap.TileType hookDown = mTileMap.MapInfo.GetTileType (downX, downY);
			if(hookRight==DTileMap.TileType.Wall || hookRight==DTileMap.TileType.Target1 ||hookRight==DTileMap.TileType.Target2||hookRight==DTileMap.TileType.Target3 )
			{
				DTileMap.TileType Check = mTileMap.MapInfo.GetTileType (rightX-1, rightY);
				if(Check == DTileMap.TileType.Floor)
				{
					mTileMap.MapInfo.SetTileType (rightX-1, rightY, DTileMap.TileType.Walkable, true);
				}
			}
			if(hookLeft==DTileMap.TileType.Wall || hookLeft==DTileMap.TileType.Target1 ||hookLeft==DTileMap.TileType.Target2||hookLeft==DTileMap.TileType.Target3 )
			{
				DTileMap.TileType Check = mTileMap.MapInfo.GetTileType (leftX+1, leftY);
				if(Check == DTileMap.TileType.Floor)
				{
					mTileMap.MapInfo.SetTileType (leftX+1, leftY, DTileMap.TileType.Walkable, true);
				}
			}
			if(hookUp==DTileMap.TileType.Wall || hookUp==DTileMap.TileType.Target1 ||hookUp==DTileMap.TileType.Target2||hookUp==DTileMap.TileType.Target3 )
			{
				DTileMap.TileType Check = mTileMap.MapInfo.GetTileType (upX, upY-1);
				if(Check == DTileMap.TileType.Floor)
				{
					mTileMap.MapInfo.SetTileType (upX, upY-1, DTileMap.TileType.Walkable, true);
				}
			}
			if(hookDown==DTileMap.TileType.Wall || hookDown==DTileMap.TileType.Target1 ||hookDown==DTileMap.TileType.Target2||hookDown==DTileMap.TileType.Target3)
			{
				DTileMap.TileType Check = mTileMap.MapInfo.GetTileType (downX, downY-1);
				if(Check == DTileMap.TileType.Floor)
				{
					mTileMap.MapInfo.SetTileType (downX, downY+1, DTileMap.TileType.Walkable, true);
				}
			}
		}
		DTileMap.TileType curType = mTileMap.MapInfo.GetTileType (mMouseX, mMouseY);
		if(Input.GetMouseButtonDown(0) && curType==DTileMap.TileType.Walkable)
		{
			Travel (mMouseX, mMouseY);
			if(mTileMap.MapInfo.GetTileType (downX, downY+1)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (downX, downY+1, DTileMap.TileType.Floor, true);
			}
			if(mTileMap.MapInfo.GetTileType (upX, upY-1)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (upX, upY-1, DTileMap.TileType.Floor, true);
			}
			if(mTileMap.MapInfo.GetTileType (leftX+1, leftY)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (leftX+1, leftY, DTileMap.TileType.Floor, true);
			}
			if(mTileMap.MapInfo.GetTileType (rightX-1, rightY)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (rightX-1, rightY, DTileMap.TileType.Floor, true);
			}
			mPlayerPhase = PlayerPhase.Start;
		}
		else if(Input.GetMouseButtonDown(1))
		{

			mPlayerPhase = PlayerPhase.Start;
		}
	}
	void ThordrannActive(int randomMovement)
	{
		int maxX = mTileMap.MapInfo.size_x;
		int maxY = mTileMap.MapInfo.size_y;
		int minX = 0;
		int minY = 0;
		
		int rightX = 0;
		int rightY = 0;
		int leftX =  0;
		int leftY =  0;
		int upX = 0;
		int upY = 0;
		int downX =  0;
		int downY =  0;
		ResetWalkRange ();
		//Discard a card
		//Check for movmement
		int rushMovement = randomMovement + mMovement;
		//Right
		int count = 0;
		bool Check = false;
		while(Check == false || count <= rushMovement)
		{
			rightX = mPositionX + count;
			rightY =  mPositionY;
			if(rightX > maxX)
			{
				rightX = maxX;
				Check =false;
				break;
			}
			DTileMap.TileType rushRight = mTileMap.MapInfo.GetTileType (rightX, rightY);
			if(rushRight != DTileMap.TileType.Floor || 
			   rushRight!=DTileMap.TileType.Sewer)
			{
				Check = true;
				break;
			}
			count++;
		}
		if(mTileMap.MapInfo.GetTileType (rightX-1, rightY)==DTileMap.TileType.Floor)
		{
			mTileMap.MapInfo.SetTileType (rightX-1, rightY, DTileMap.TileType.Walkable, true);
		}
		else if(mTileMap.MapInfo.GetTileType (rightX-1, rightY)==DTileMap.TileType.Sewer)
		{
			mTileMap.MapInfo.SetTileType (rightX-1, rightY, DTileMap.TileType.TrueSewer, true);
		}
		
		//Left
		count = 0;
		Check = false;
		while(Check == false || count <= rushMovement)
		{
			leftX = mPositionX - count;;
			leftY =  mPositionY;
			if(leftX < minX)
			{
				leftX = minX;
				Check =false;
				break;
			}
			DTileMap.TileType rushLeft = mTileMap.MapInfo.GetTileType (leftX, leftY);
			if(rushLeft != DTileMap.TileType.Floor || 
			   rushLeft!=DTileMap.TileType.Sewer)
			{
				Check = true;
				break;
			}
			count++;
		}
		if(mTileMap.MapInfo.GetTileType (leftX + 1, mPositionY)==DTileMap.TileType.Floor)
		{
			mTileMap.MapInfo.SetTileType (leftX + 1, mPositionY, DTileMap.TileType.Walkable, true);
		}
		else if(mTileMap.MapInfo.GetTileType (leftX + 1, mPositionY)==DTileMap.TileType.Sewer)
		{
			mTileMap.MapInfo.SetTileType (leftX + 1, mPositionY, DTileMap.TileType.TrueSewer, true);
		}
		//Up
		count = 0;
		Check = false;
		while(Check == false || count <= rushMovement)
		{
			upX = mPositionX ;
			upY =  mPositionY + count;
			if(upY > maxY)
			{
				upY = maxY;
				Check =false;
				break;
			}
			DTileMap.TileType rushUp = mTileMap.MapInfo.GetTileType (upX, upY);
			if(rushUp != DTileMap.TileType.Floor ||
			   rushUp !=DTileMap.TileType.Sewer)
			{
				Check = true;
				break;
			}
			count++;
		}
		if(mTileMap.MapInfo.GetTileType (upX , upY - 1)==DTileMap.TileType.Floor)
		{
			mTileMap.MapInfo.SetTileType (upX , upY - 1, DTileMap.TileType.Walkable, true);
		}
		else if(mTileMap.MapInfo.GetTileType (upX , upY - 1)==DTileMap.TileType.Sewer)
		{
			mTileMap.MapInfo.SetTileType (upX , upY - 1, DTileMap.TileType.TrueSewer, true);
		}
		
		//Down
		count = 0;
		Check = false;
		while(Check == false || count <= rushMovement)
		{
			downX = mPositionX ;
			downY =  mPositionY - count;
			if(downY > minY)
			{
				downY = minY;
				Check =false;
				break;
			}
			DTileMap.TileType rushDown = mTileMap.MapInfo.GetTileType (downX, downY);
			if(rushDown != DTileMap.TileType.Floor || 
			   rushDown !=DTileMap.TileType.Sewer)
			{
				Check = true;
				break;
			}
			count++;
		}
		if(mTileMap.MapInfo.GetTileType (downX , downY + 1)==DTileMap.TileType.Floor)
		{
			mTileMap.MapInfo.SetTileType (downX , downY + 1, DTileMap.TileType.Walkable, true);
		}
		else if(mTileMap.MapInfo.GetTileType (downX , downY + 1)==DTileMap.TileType.Sewer)
		{
			mTileMap.MapInfo.SetTileType (downX , downY + 1, DTileMap.TileType.TrueSewer, true);
		}
		DTileMap.TileType curType = mTileMap.MapInfo.GetTileType (mMouseX, mMouseY);
		if(Input.GetMouseButtonDown(0) && curType==DTileMap.TileType.Walkable)
		{
			Travel (mMouseX, mMouseY);
			if(mTileMap.MapInfo.GetTileType (downX, downY+1)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (downX, downY+1, DTileMap.TileType.Floor, true);
			}
			else if(mTileMap.MapInfo.GetTileType (downX, downY+1)==DTileMap.TileType.TrueSewer)
			{
				mTileMap.MapInfo.SetTileType (downX, downY+1, DTileMap.TileType.Sewer, true);
			}
			if(mTileMap.MapInfo.GetTileType (upX, upY-1)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (upX, upY-1, DTileMap.TileType.Floor, true);
			}
			else if(mTileMap.MapInfo.GetTileType (upX, upY-1)==DTileMap.TileType.TrueSewer)
			{
				mTileMap.MapInfo.SetTileType (upX, upY-1, DTileMap.TileType.Sewer, true);
			}
			if(mTileMap.MapInfo.GetTileType (leftX+1, leftY)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (leftX+1, leftY, DTileMap.TileType.Floor, true);
			}
			else if(mTileMap.MapInfo.GetTileType (leftX+1, leftY)==DTileMap.TileType.TrueSewer)
			{
				mTileMap.MapInfo.SetTileType (leftX+1, leftY, DTileMap.TileType.Sewer, true);
			}
			if(mTileMap.MapInfo.GetTileType (rightX-1, rightY)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (rightX-1, rightY, DTileMap.TileType.Floor, true);
			}
			else if(mTileMap.MapInfo.GetTileType (rightX-1, rightY)==DTileMap.TileType.TrueSewer)
			{
				mTileMap.MapInfo.SetTileType (rightX-1, rightY, DTileMap.TileType.Sewer, true);
			}
			mMoved = true;
			mPlayerPhase = PlayerPhase.Start;
		}
		else if(Input.GetMouseButtonDown(0) && curType==DTileMap.TileType.TrueSewer)
		{
			
			TravelToSewer (mMouseX, mMouseY);
			mOnSewer = true;
			mMoved = true;
			mPlayerPhase = PlayerPhase.Start;
		}
		else if(Input.GetMouseButtonDown(1))
		{
			
			mPlayerPhase = PlayerPhase.Start;
		}
	}
	
	void WeldingtonActive()
	{
		WeldingtonAutomaton i = new WeldingtonAutomaton ();
		if(mIsU == true)
		{
			mAttack = i.mInputAttack + 3;
			mMovement = i.mInputMovement;
			mIsU = false;
		}
		else
		{
			mMovement = i.mInputMovement+3;
			mAttack = i.mInputAttack;
			mIsU = true;
		}
		ResetWalkRange ();
		mSkillsCD = 3;
		mPlayerPhase = PlayerPhase.Start;
	}
	//added this to try to fix some issues
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
		}
		else
		{
			syncEndPosition = (Vector3)stream.ReceiveNext();
			transform.position = syncEndPosition;
		}
	}
}