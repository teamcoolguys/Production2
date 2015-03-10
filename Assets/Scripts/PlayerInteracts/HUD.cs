using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour 
{
	//publix
	public static int decksize = 30;
	public GameObject[] deck1 = new GameObject[decksize];
	public GameObject[] deck2 = new GameObject[decksize];
	public GameObject[] deck3 = new GameObject[decksize];
	public GameObject[] deck4 = new GameObject[decksize];
	public double uoff = 0;
	public Texture bar, backbar;
	public GUITexture combar, atkbar, defbar, atkprt, defprt, cardslots;

	//Jack
	//So the ints are player 0,1,2,3
	// and the targets are 4,5,6,7,8
	//public int curAttacking;
	//public int curDefending;
	//I made these two varible in the manager, so just grab the number there(default to be -1 if nothing is happening)
	//--------------------//

	//privates
	private RaycastHit curcard;
	private float secs = 5;
	private float incr = 0, sizer = 0; 
	private int ignorecard = -1, te = -1;
	private Vector3 temp, getbigger;
	private GameObject set1, set2, set3, set4, set5, set6, set7, set8, set9, set10, set11, set12;
	private int cdel;
	private GameObject[] cards1 = new GameObject[decksize], cards2 = new GameObject[decksize], cards3 = new GameObject[decksize], cards4 = new GameObject[decksize];
	private GameObject[] hand1 = new GameObject[decksize], hand2 = new GameObject[decksize], hand3 = new GameObject[decksize], hand4 = new GameObject[decksize];
	private int rotint = 0;
	private bool rotr = false ;
	private List<GameObject> p1c = new List<GameObject>(), 
								p2c= new List<GameObject>(), 
								p3c= new List<GameObject>(), 
								p4c= new List<GameObject>(), 
								t1c= new List<GameObject>(), 
								t2c= new List<GameObject>(), 
								t3c= new List<GameObject>();	
	private bool showR = false;
	private bool choosing = false;
	private int attackeratk, attackerdef, defenderatk, defenderdef;
	
	//wyatt
	private GameManager mManager;
	//
	
	public float percent;

	void Start ()
	{
		for (int j = 0; j < 5; j++)
		{
			p1c.Add (null);

			p2c.Add (null);
			
			p3c.Add (null);
			
			p4c.Add (null);
			
			t1c.Add (null);
			
			t2c.Add (null);
			
			t3c.Add (null);
		}

		decksize = deck1.Length;

		set1 = new GameObject();
		set1.AddComponent<GUIText> ();
		set1.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set1.guiText.text = "N/A";
		set1.guiText.color = Color.black;

		set2 = new GameObject();
		set2.AddComponent<GUIText>();
		set2.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set2.guiText.pixelOffset = new Vector2 (250, 0);
		set2.guiText.text = "N/A";
		set2.guiText.color = Color.black;

		set3 = new GameObject ();
		set3.AddComponent<GUIText>();
		set3.transform.position = new Vector3(0.5f, 0.5f,  1.0f);
		set3.guiText.pixelOffset = new Vector2 (350, 0);
		set3.guiText.text = "N/A";
		set3.guiText.color = Color.black;

		set4 = new GameObject();
		set4.AddComponent<GUIText>();
		set4.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set4.guiText.text = "N/A";
		set4.guiText.color = Color.black;

		set5 = new GameObject();
		set5.AddComponent<GUIText>();
		set5.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set5.guiText.text = "N/A";
		set5.guiText.color = Color.black;

		set6 = new GameObject();
		set6.AddComponent<GUIText>();
		set6.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set6.guiText.text = "N/A";
		set6.guiText.color = Color.black;

		set7 = new GameObject();
		set7.AddComponent<GUIText>();
		set7.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set7.guiText.text = "N/A";
		set7.guiText.color = Color.black;

		set8 = new GameObject();
		set8.AddComponent<GUIText>();
		set8.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set8.guiText.text = "N/A";
		set8.guiText.color = Color.black;

		set9 = new GameObject();
		set9.AddComponent<GUIText>();
		set9.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set9.guiText.text = "N/A";
		set9.guiText.color = Color.black;

		set10 = new GameObject();
		set10.AddComponent<GUIText>();
		set10.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set10.guiText.text = "N/A";
		set10.guiText.color = Color.black;

		set11 = new GameObject();
		set11.AddComponent<GUIText>();
		set11.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set11.guiText.text = "N/A";
		set11.guiText.color = Color.black;

		set12 = new GameObject();
		set12.AddComponent<GUIText>();
		set12.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set12.guiText.text = "N/A";
		set12.guiText.color = Color.black;

		ResetDeck ();

	}
	
	void ResetDeck()
	{
		for (int i = 0; i < hand1.Length; i++) 
		{
			if (hand1[i] != null)
				Destroy(hand1[i]);
			hand1[i] = null;	
		}

		for (int i = 0; i < hand2.Length; i++) 
		{
			if (hand2[i] != null)
				Destroy(hand2[i]);
			hand2[i] = null;	
		}

		for (int i = 0; i < hand3.Length; i++) 
		{
			if (hand3[i] != null)
				Destroy(hand3[i]);
			hand3[i] = null;	
		}

		for (int i = 0; i < hand4.Length; i++) 
		{
			if (hand4[i] != null)
				Destroy(hand4[i]);
			hand4[i] = null;	
		}



					System.Array.Copy (deck1, cards1, cards1.Length);
					System.Array.Copy (deck2, cards2, cards2.Length);
					System.Array.Copy (deck3, cards3, cards3.Length);
					System.Array.Copy (deck4, cards4, cards4.Length);


		showR = false;
		cdel = 0;
	}
	
	void Playcard(GameObject cardd)
	{
		GameObject al;
		al = cardd.transform.parent.gameObject;
		Destroy (al);
		//do card stuff
	}

	GameObject DealCard(GameObject[] cards, GameObject[] hand)
	{
		System.Random rand = new System.Random();
		int card = rand.Next (decksize);
		while(true)
		{
			if (cards [card] == null)
			{
				card = rand.Next (decksize);
				Debug.Log(card);
			}
			else
				break;
		}

		GameObject go = GameObject.Instantiate (cards [card]) as GameObject;
		cards [card] = null;

		for(int i = 0; i < 15; ++i)
		{
			if (hand[i] == null)
			{
				hand[i] = go;
				break;
			}
		}
		cdel++;
		return go;
	}
	
	void Gameover()
	{
		ResetDeck();
	}
	
	
	Rect ResizeGUI(Rect _rect)
	{
		float FilScreenWidth = _rect.width / 800;
		float rectWidth = FilScreenWidth * Screen.width;
		float FilScreenHeight = _rect.height / 600;
		float rectHeight = FilScreenHeight * Screen.height;
		float rectX = (_rect.x / 800) * Screen.width;
		float rectY = (_rect.y / 600) * Screen.height;
		
		return new Rect(rectX,rectY,rectWidth,rectHeight);
	}
	
	void OnGUI()
	{
		//if (mManager.curDefending == mManager.curAttacking)
		//{
		//	mManager.curDefending = -1;
		//}
		if (rotr == false) 
		{
					for (int l = 0; l < 5; l++) {
							if (p1c [l] != null)
									p1c [l].SetActive (false);

						if (p2c [l] != null)
									p2c [l].SetActive (false);
		
						if (p3c [l] != null)
									p3c [l].SetActive (false);
		
						if (p4c [l] != null)
									p4c [l].SetActive (false);
		
						if (t1c [l] != null)
									t1c [l].SetActive (false);
		
						if (t2c [l] != null)
									t2c [l].SetActive (false);
		
						if (t3c [l] != null)
									t3c [l].SetActive (false);
					}
				}



		float infamy = 0;

		//TODO
		if(mManager.CurrentPlayer () != null)
		{
			infamy = mManager.CurrentPlayer().mInfamy;
		}

		float maxinfamy = 5;

		if (infamy == 0)
			percent = 0;
		else if (infamy >= maxinfamy)
		{
			percent = 190; infamy = maxinfamy;
		}
		else
			percent = 190 * (infamy/maxinfamy);
		
		GUI.DrawTexture(new Rect((Screen.width/2) - 100, 20, 200, 30), backbar , ScaleMode.StretchToFill, true, 0.0f);
		GUI.DrawTexture(new Rect((Screen.width/2) - 95, 25 , percent, 20), bar , ScaleMode.StretchToFill, true, 0.0f);

		//Infamy text
		set1.guiText.text = infamy.ToString ();
		set2.guiText.text = "/";
		set3.guiText.text = maxinfamy.ToString();
		set1.guiText.pixelOffset = new Vector2 (0,  (Screen.height/2));
		set2.guiText.pixelOffset = new Vector2 (10, (Screen.height/2));
		set3.guiText.pixelOffset = new Vector2 (20, (Screen.height/2));
	
		//Players Stats
		set4.guiText.text = "Infamy:";
		if(mManager.CurrentPlayer() != null)
		{

			set5.guiText.text = mManager.CurrentPlayer().mRange.ToString();
			set6.guiText.text = mManager.CurrentPlayer().mAttack.ToString();
			set7.guiText.text = mManager.CurrentPlayer().mDefence.ToString();
			set8.guiText.text = mManager.CurrentPlayer().mMovement.ToString();
		}

		set4.guiText.pixelOffset = new Vector2 (-70, (Screen.height/2));
		set5.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 120, 0 - 175);
		set6.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 210, 0 - 130);
		set7.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 120, 0 - 130);
		set8.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 210, 0 - 175);

		//Target/Other player stats
		if (choosing)
		{
			if (GUI.Button(new Rect(50,(Screen.height/2) - 200,100, 40), "Play Card") )
			{
				ignorecard = -1;
				choosing = false;
				switch(mManager.curDefending)
				{
					case(DTileMap.TileType.Player1):

					for(int i = 0; i < 4; ++i)
					{
						if (p1c[i] == null)
						{
							p1c[i] = PlayCardOnPlayer(p1c[i]);
							p1c[i].gameObject.tag = "Untagged";
							break;
						}
					}
					break;

					case(DTileMap.TileType.Player2):
						for(int i = 0; i < 4; ++i)
						{
							if (p2c[i] == null)
							{
								p2c[i] = PlayCardOnPlayer(p2c[i]);
								p2c[i].gameObject.tag = "Untagged";
							break;
							}
						}
					break;
					
					case(DTileMap.TileType.Player3):
					for(int i = 0; i < 4; ++i)
						{
							if (p3c[i] == null)
							{
								p3c[i] = PlayCardOnPlayer(p3c[i]);
							p3c[i].gameObject.tag = "Untagged";
							break;
							}
						}
					break;
					
					case(DTileMap.TileType.Player4):
						for(int i = 0; i < 4; ++i)
						{
							if (p4c[i] == null)
							{
								p4c[i] = PlayCardOnPlayer(p4c[i]);
							p4c[i].gameObject.tag = "Untagged";
							break;
							}
						}
					break;
					
					case(DTileMap.TileType.Target1):
						for(int i = 0; i < 4; ++i)
						{
							if (t1c[i] == null)
							{
								t1c[i] = PlayCardOnPlayer(t1c[i]);
							t1c[i].gameObject.tag = "Untagged";
							break;
							}
						}
					break;
					
					case(DTileMap.TileType.Target2):
						for(int i = 0; i < 4; ++i)
						{
							if (t2c[i] == null)
							{
								t2c[i] = PlayCardOnPlayer(t2c[i]);
							t2c[i].gameObject.tag = "Untagged";
							break;
							}
						}
					break;
					
					case(DTileMap.TileType.Target3):
						for(int i = 0; i < 4; ++i)
						{
							if (t3c[i] == null)
							{
								t3c[i] = PlayCardOnPlayer(t3c[i]);
							t3c[i].gameObject.tag = "Untagged";
							break;
							}
						}
					break;
					
				}
			}
			if (GUI.Button(new Rect((Screen.width) - 100,(Screen.height/2)- 200, 100, 40), "Play Card"))
			{
				Debug.Log ("Wyatt Gargles " + 99 + " fat cocks on the " + mManager.curAttacking + " all day");
				choosing = false;
				ignorecard = -1;
				switch(mManager.curAttacking)
				{
					case(DTileMap.TileType.Player1):
					
						for(int i = 0; i < 4; ++i)
						{
							if (p1c[i] == null)
							{
								p1c[i] = PlayCardOnPlayer(p1c[i]);
							p1c[i].gameObject.tag = "Untagged";
							break;
							}
						}
						break;
					
				case(DTileMap.TileType.Player2):
						for(int i = 0; i < 4; ++i)
						{
							if (p2c[i] == null)
							{
								p2c[i] = PlayCardOnPlayer(p2c[i]);
							p2c[i].gameObject.tag = "Untagged";
							break;
							}
						}
						break;
					
				case(DTileMap.TileType.Player3):
						for(int i = 0; i < 4; ++i)
						{
							if (p3c[i] == null)
							{
								p3c[i] = PlayCardOnPlayer(p3c[i]);
							p3c[i].gameObject.tag = "Untagged";
							break;
							}
						}
						break;
					
				case(DTileMap.TileType.Player4):
						for(int i = 0; i < 4; ++i)
						{
							if (p4c[i] == null)
							{
								p4c[i] = PlayCardOnPlayer(p4c[i]);
							p4c[i].gameObject.tag = "Untagged";
							break;
							}
						}
						break;
				}
			}
		}

		if (mManager.curDefending == DTileMap.TileType.TargetSpot || mManager.curDefending == mManager.curAttacking)
		{
			set9.guiText.text = "N/A";
			set10.guiText.text = "N/A";
			set11.guiText.text = "N/A";
			set12.guiText.text = "N/A";
		}
		else
		{
			switch(mManager.curDefending)
			{
			case DTileMap.TileType.Player1:
				//Debug.Log(DTileMap.TileType.Player1);
				ChangeDefenderGUI(DTileMap.TileType.Player1);
				break;
			case DTileMap.TileType.Player2:
				//Debug.Log(DTileMap.TileType.Player2);
				ChangeDefenderGUI(DTileMap.TileType.Player2);
				break;
			case DTileMap.TileType.Player3:
				//Debug.Log(DTileMap.TileType.Player3);
				ChangeDefenderGUI(DTileMap.TileType.Player3);
				break;
			case DTileMap.TileType.Player4:
				//Debug.Log(DTileMap.TileType.Player4);
				ChangeDefenderGUI(DTileMap.TileType.Player4);
				break;
			case DTileMap.TileType.Target1:
				//Debug.Log(DTileMap.TileType.Target1);
				ChangeDefenderGUI(DTileMap.TileType.Target1);
				break;
			case DTileMap.TileType.Target2:
				//Debug.Log(DTileMap.TileType.Target2);
				ChangeDefenderGUI(DTileMap.TileType.Target2);
				break;
			case DTileMap.TileType.Target3:
				//Debug.Log(DTileMap.TileType.Target3);
				ChangeDefenderGUI(DTileMap.TileType.Target3);
				break;
			}
			if(mManager.CurrentPlayer() != null)
			{
				if (mManager.CurrentPlayer().mPlayerPhase == Player.PlayerPhase.Attack)
				{
					rotr = true;
					
					// infamy boost infamy = infamy+1;
				}
			}
		}
		set9.guiText.pixelOffset  = new Vector2(-(Screen.width/2) + 150, 0 - 175);
		set10.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 80, 0 - 130);
		set11.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 150, 0 - 130);
		set12.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 80, 0 - 175);

		//atk def
		//mov rng
		//set9.guiText.text =  "N/A";	range		
		//set10.guiText.text = "N/A";   attack
		//set11.guiText.text = mManager.sTargets[(int)mManager.curDefending].mDefence.ToString();
		//set12.guiText.text = mManager.sTargets[(int)mManager.curDefending].mMovement.ToString();

		
		if (!showR)
		{
			if (GUI.Button(new Rect(10,10,100, 20), "Deal1"))
			{
				MoveDealtCard(cards1, hand1);
			}
			if (GUI.Button(new Rect(10,40,100, 20), "Deal2"))
			{
				MoveDealtCard(cards2, hand2);
			}
			if (GUI.Button(new Rect(10,70,100, 20), "Deal3"))
			{
				MoveDealtCard(cards3, hand3);
			}
			if (GUI.Button(new Rect(10,100,100, 20), "Deal4"))
			{
				MoveDealtCard(cards4, hand4);
			}
		}
		
		if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 20), "GameOver"))
		{
			Gameover();
		}
		
		if (GUI.Button(new Rect(Screen.width - 110, 40, 100, 20), "Quit"))
		{
			Application.Quit();
		}

		if (rotr == false)
		Dispcards ();
	}

	void ChangeDefenderGUI(DTileMap.TileType TypeofDefender)
	{
		//Debug.Log ("TypeOfDefender" + TypeofDefender);
		if(TypeofDefender >= DTileMap.TileType.Target1)
		{
			//target logic
			//Debug.Log("TargetThatIsDefending::" + TypeofDefender);
			mManager.curDefending -= DTileMap.TileType.Target1;

			set9.guiText.text =  "N/A";			
			set10.guiText.text = "N/A";
			set11.guiText.text = mManager.sTargets[(int)mManager.curDefending].mDefence.ToString();
			set12.guiText.text = mManager.sTargets[(int)mManager.curDefending].mMovement.ToString();
			mManager.curDefending += (int)DTileMap.TileType.Target1;
		}
		else
		{
			//player logic
			//Debug.Log("PlayerThatIsDefending::" + mManager.curDefending);
			mManager.curDefending -= DTileMap.TileType.Player1;

			set9.guiText.text = mManager.sPlayers[(int)mManager.curDefending].mRange.ToString();
			set10.guiText.text = mManager.sPlayers[(int)mManager.curDefending].mAttack.ToString();
			set11.guiText.text = mManager.sPlayers[(int)mManager.curDefending].mDefence.ToString();
			set12.guiText.text = mManager.sPlayers[(int)mManager.curDefending].mMovement.ToString();
			mManager.curDefending += (int)DTileMap.TileType.Player1;
		}
	} 
	GameObject PlayCardOnPlayer(GameObject gObject)
	{
		curcard.collider.gameObject.tag = "Untagged";
		gObject = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
		Destroy(curcard.collider.gameObject.transform.parent.gameObject);
		return gObject;
	}
	
	void MoveDealtCard(GameObject[] cards, GameObject[] hand)
	{
		GameObject newCard = DealCard (cards, hand);
		
		if (newCard == null)
		{
			Debug.Log("Out of Cards");
			showR = true;
			return;
		}
		float offset = 0;
		GameObject hudd = GameObject.FindGameObjectWithTag("HUD");
		newCard.transform.position = hudd.transform.position;
		newCard.transform.rotation = hudd.transform.rotation;
		newCard.transform.position = new Vector3(newCard.transform.position.x - offset, newCard.transform.position.y, newCard.transform.position.z + offset);
	}
	
	void Rearrangehand(GameObject[] hand, int ignore = -1, int ignore2 = -1)
	{
		GameObject[] tempo = new GameObject[15];
		int k = 0;
		for (int j = 0; j < hand.Length; j++)
		{
			if (hand[j] != null)
			{tempo[k] = hand[j]; k++;}
		}
		hand = tempo;
		float offset = (float)-5.6;
		offset = offset + (float)uoff;
		float distinc = 0.0f;
		int cardsheld = 0;
		for (int j = 0; j < hand.Length; j++)
		{
			if (hand [j] != null)
			cardsheld++;
		}

		for (int i = 0; i < cardsheld; i++)
		{
			if (i != 0 && hand[i] != null)
			{
				BoxCollider box = hand[i].GetComponentInChildren<BoxCollider>();
				box.size = new Vector3(0.5f, 3.0f, 1.0f);
				box.center = new Vector3(-0.25f, 0.0f, 0.0f);
			}
			if (i == 0)
			{
				BoxCollider box = hand[i].GetComponentInChildren<BoxCollider>();
				box.size = new Vector3(1.0f, 3.0f, 1.0f);
			}
			if (i != ignore && i != ignore2 && hand[i] != null)
			{
				hand[i].transform.position = new Vector3 (0,0,0);
				hand[i].transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 12) + (Camera.main.transform.forward * distinc);
				hand[i].transform.position = hand[i].transform.position + Camera.main.transform.right * offset;
				hand[i].transform.position = hand[i].transform.position + (Camera.main.transform.up * -5) + ((Camera.main.transform.up * -distinc) / 2);
				hand[i].transform.localScale = new Vector3(Camera.main.fieldOfView/120,Camera.main.fieldOfView/120,Camera.main.fieldOfView/120);
				hand[i].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			}
			distinc = distinc + 0.01f;
			offset = offset + 0.7f;
		}
	}

	
	void Update()
	{
		if(PhotonNetwork.offlineMode)
		{
			if(!mManager)
			{
				mManager = GameObject.Find("GameManager").GetComponent<GameManager>(); // thats how you get infromation from the manager
			}
		}
		else
		{
			if(!mManager)
			{
				mManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>(); // thats how you get infromation from the manager
			}
		}

		switchdisp ();

		if (mManager.curAttacking == DTileMap.TileType.Player1)
			Rearrangehand(hand1, ignorecard, te);
		else if (mManager.curAttacking == DTileMap.TileType.Player2)
			Rearrangehand(hand2, ignorecard, te);
		else if (mManager.curAttacking == DTileMap.TileType.Player3)
			Rearrangehand(hand3, ignorecard, te);
		else if (mManager.curAttacking == DTileMap.TileType.Player4)
			Rearrangehand(hand4, ignorecard, te);
		
		Ray mouse = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit mhit;
		if (Physics.Raycast (mouse, out mhit) && (mhit.collider.CompareTag("Card") || mhit.collider.CompareTag("Mouseover")))
		{
			if (mManager.CurrentPlayer() == mManager.sPlayers[0])
				lookat(hand1, mhit);
			else if (mManager.CurrentPlayer() == mManager.sPlayers[1])
				lookat(hand2, mhit);
			else if (mManager.CurrentPlayer() == mManager.sPlayers[2])
				lookat(hand3, mhit);
			else if (mManager.CurrentPlayer() == mManager.sPlayers[3])
				lookat(hand4, mhit);
		}
		else
		{
			if (mManager.CurrentPlayer() == mManager.sPlayers[0])
				settagtocard(hand1);
			else if (mManager.CurrentPlayer() == mManager.sPlayers[1])
				settagtocard(hand2);
			else if (mManager.CurrentPlayer() == mManager.sPlayers[2])
				settagtocard(hand3);
			else if (mManager.CurrentPlayer() == mManager.sPlayers[3])
				settagtocard(hand4);
			te = -1;
		}
		
		if (choosing == false)
		{	
			ignorecard = -1;	
			Quaternion targetRotation;
			targetRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
			
			if (Input.GetKeyDown("space"))
			{
				if (rotr == false)
					rotr = true;
				else
					rotr = false;
				
			}
			if (rotr && rotint <= 350)
			{
				if (mManager.curAttacking == DTileMap.TileType.Player1)
					rotate180(p1c);
				else if (mManager.curAttacking == DTileMap.TileType.Player2)
					rotate180(p2c);
				else if (mManager.curAttacking == DTileMap.TileType.Player3)
					rotate180(p3c);
				else if (mManager.curAttacking == DTileMap.TileType.Player4)
					rotate180(p4c);
				
				if (mManager.curDefending == DTileMap.TileType.Player1)
					rotate180(p1c);
				else if (mManager.curDefending == DTileMap.TileType.Player2)
					rotate180(p2c);
				else if (mManager.curDefending == DTileMap.TileType.Player3)
					rotate180(p3c);
				else if (mManager.curDefending == DTileMap.TileType.Player4)
					rotate180(p4c);
				else if (mManager.curDefending == DTileMap.TileType.Target1)
					rotate180(t1c);
				else if (mManager.curDefending == DTileMap.TileType.Target2)
					rotate180(t2c);
				else if (mManager.curDefending == DTileMap.TileType.Target3)
					rotate180(t3c);
				
				rotint++;
			}
			else if (rotr && rotint > 350)
			{
				
				secs -= 1* Time.deltaTime;
				if (secs <=0)
				{
					secs = 5;
					Attack();
				}
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))			
				{ 
					clickcheck(hand1, hit);
				} 
			}
		}
		
		if (choosing == true) 
		{	
			curcard.transform.parent.position = Vector3.Lerp(temp, (Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)) + Camera.main.transform.forward * 5), incr );
			incr += 0.1f;
			if (Input.GetMouseButtonDown (1)) 
			{
				curcard = new RaycastHit(); 
				choosing = false;
				ignorecard = -1;
			}
			
		}
	}
	
	void Dispcards()
	{
		//CARDS ABOVE CURRENT PLAYER

		if (mManager.curAttacking == DTileMap.TileType.Player1)
		{
			for (int l = 0; l < p1c.Count - 1; l++)
			{
				if (p1c[l] != null)
					p1c[l].SetActive(true);
			}

			for(int i = 0; i < p1c.Count - 1; ++i)
			{
				ShowAttackingCard(p1c[i], i);
			}
		}
		else if (mManager.curAttacking == DTileMap.TileType.Player2)
		{
			for (int l = 0; l < p2c.Count - 1; l++)
			{
				if (p2c[l] != null)
					p2c[l].SetActive(true);
			}

			for(int i = 0; i < p2c.Count - 1; ++i)
			{
				ShowAttackingCard(p2c[i], i);
			}
		}
		else if (mManager.curAttacking == DTileMap.TileType.Player3)
		{
			for (int l = 0; l < p3c.Count - 1; l++)
			{
				if (p3c[l] != null)
					p3c[l].SetActive(true);
			}

			for(int i = 0; i < p3c.Count - 1; ++i)
			{
				ShowAttackingCard(p3c[i], i);
			}
		}
		else if (mManager.curAttacking == DTileMap.TileType.Player4)
		{
			for (int l = 0; l < p4c.Count - 1; l++)
			{
				if (p4c[l] != null)
					p4c[l].SetActive(true);
			}

			for(int i = 0; i < p4c.Count - 1; ++i)
			{
				ShowAttackingCard(p4c[i], i);
			}
		}

		//SHOW CARDS ABOVE DEFENDING WINDOW
		if (mManager.curDefending == DTileMap.TileType.Player1)
		{
			for (int l = 0; l < p1c.Count - 1; l++)
			{
				if (p1c[l] != null)
					p1c[l].SetActive(true);
			}

			for(int i = 0; i < p1c.Count - 1; ++i)
			{
				ShowDefendingCard(p1c[i], i);
			}
			
			//if (p1c[0] != null)
			//{
			//	Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
			//	p1c[0].transform.position = p;
			//	
			//	p1c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
			//	p1c[0].transform.rotation = Camera.main.transform.rotation;
			//}
			//if (p1c[1] != null)
			//{
			//	Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
			//	p1c[1].transform.position = p;
			//	
			//	p1c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
			//	p1c[1].transform.rotation = Camera.main.transform.rotation;
			//}
			//if (p1c[2] != null)
			//{
			//	Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
			//	p1c[2].transform.position = p;
			//	
			//	p1c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
			//	p1c[2].transform.rotation = Camera.main.transform.rotation;
			//}
			//if (p1c[3] != null)
			//{
			//	Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
			//	p1c[3].transform.position = p;
			//	
			//	p1c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
			//	p1c[3].transform.rotation = Camera.main.transform.rotation;
			//}
		}
		else if (mManager.curDefending == DTileMap.TileType.Player2)
		{
			for (int l = 0; l < p2c.Count - 1; l++)
			{
				if (p2c[l] != null)
					p2c[l].SetActive(true);
			}

			for(int i = 0; i < p2c.Count - 1; ++i)
			{
				ShowDefendingCard(p2c[i], i);
			}
			

		}
		else if (mManager.curDefending == DTileMap.TileType.Player3)
		{
			for (int l = 0; l < p3c.Count - 1; l++)
			{
				if (p3c[l] != null)
					p3c[l].SetActive(true);
			}
			
			for(int i = 0; i < p3c.Count - 1; ++i)
			{
				ShowDefendingCard(p3c[i], i);
			}
		}
		else if (mManager.curDefending == DTileMap.TileType.Player4)
		{
			for (int l = 0; l < p4c.Count - 1; l++)
			{
				if (p4c[l] != null)
					p4c[l].SetActive(true);
			}
			
			for(int i = 0; i < p4c.Count - 1; ++i)
			{
				ShowDefendingCard(p4c[i], i);
			}
		}
		else if (mManager.curDefending == DTileMap.TileType.Target1)
		{
			for (int l = 0; l < t1c.Count - 1; l++)
			{
				if (t1c[l] != null)
					t1c[l].SetActive(true);
			}
			
			for(int i = 0; i < t1c.Count - 1; ++i)
			{
				ShowDefendingCard(t1c[i], i);
			}
		}
		else if (mManager.curDefending == DTileMap.TileType.Target2)
		{
			for (int l = 0; l < t2c.Count - 1; l++)
			{
				if (t2c[l] != null)
					t2c[l].SetActive(true);
			}
			
			for(int i = 0; i < t2c.Count - 1; ++i)
			{
				ShowDefendingCard(t2c[i], i);
			}
		}
		else if (mManager.curDefending == DTileMap.TileType.Target3)
		{
			for (int l = 0; l < t3c.Count - 1; l++)
			{
				if (t3c[l] != null)
					t3c[l].SetActive(true);
			}
			
			for(int i = 0; i < t3c.Count - 1; ++i)
			{
				ShowDefendingCard(t3c[i], i);
			}
		}
	}
																			
	void ShowDefendingCard(GameObject gObject, int i)
	{
		if(gObject)
		{
			Vector3 p = CreateDefendReadVector(i);
			gObject.transform.position = p;
			
			gObject.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
			gObject.transform.rotation = Camera.main.transform.rotation;
		}
	}

	Vector3 CreateDefendReadVector(int i)
	{
		switch(i)
		{
		case(0):
			return Camera.main.ScreenToWorldPoint( new Vector3(0 + 95, Screen.height/2 - 20, 6.0f) );
		case(1):
			return Camera.main.ScreenToWorldPoint( new Vector3(0 + 180, Screen.height/2 - 20, 6.0f) );
		case(2):
			return Camera.main.ScreenToWorldPoint( new Vector3(0 + 95, Screen.height/2 - 80, 6.0f) );
		case(3):
			return Camera.main.ScreenToWorldPoint( new Vector3(0 + 180, Screen.height/2 - 80, 6.0f) );
		default:
			return new Vector3();
		}
	}

	void ShowAttackingCard(GameObject gObject, int i)
	{
		if(gObject)
		{
			Vector3 p = CreateAttackReadVector(i);
			gObject.transform.position = p;
			
			gObject.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
			gObject.transform.rotation = Camera.main.transform.rotation;
		}
	}

	Vector3 CreateAttackReadVector(int i)
	{
		switch(i)
		{
		case(0):
			return Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 200, Screen.height/2 - 20, 6.0f) );
		case(1):
			return Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 120, Screen.height/2 - 20, 6.0f) );
		case(2):
			return Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 200, Screen.height/2 - 80, 6.0f) );
		case(3):
			return Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 120, Screen.height/2 - 80, 6.0f) );
		default:
			return new Vector3();
		}
	}

	void Attack()
	{
		int tempatk, tempdef, taratk, tardef;

		tempatk = mManager.CurrentPlayer().mAttack;
		tempdef = mManager.CurrentPlayer().mDefence;

		tardef = 0;
		taratk = 0;

		if(mManager.curDefending >= DTileMap.TileType.Target1 && mManager.curDefending <= DTileMap.TileType.Target3)
		{
			//Targets DO NOT ATTACK
			taratk = 0;
			tardef = mManager.CurrentTargetDefender().mDefence;
		}
		else if (mManager.curDefending >= DTileMap.TileType.Player1 && mManager.curDefending < DTileMap.TileType.Target1)
		{
			taratk = mManager.CurrentPlayerDefender().mAttack;
			tardef = mManager.CurrentPlayerDefender().mDefence;
		}
		switch(mManager.curAttacking)
		{
			//When cards are defined do card stuff
			case DTileMap.TileType.Player1:
			{
				for (int h = 0; h < 5; h++)
				{
					Destroy(p1c[h]);
				}
			}
			break;
			case DTileMap.TileType.Player2:
			{
				
			for (int h = 0; h < 5; h++)
				{
					Destroy(p2c[h]);
				}
			}
			break;
			case DTileMap.TileType.Player3:
			{
				
				for (int h = 0; h < 5; h++)
				{
					Destroy(p3c[h]);
				}
			}
			break;
			case DTileMap.TileType.Player4:
			{
				
				for (int h = 0; h < 5; h++)
				{
					Destroy(p4c[h]);
				}
			}
			break;
		}

		switch(mManager.curDefending)
		{
			case DTileMap.TileType.Player1:
			{
				
				for (int h = 0; h < 5; h++)
				{
					Destroy(p1c[h]);
				}
			}
			break;
			case DTileMap.TileType.Player2:
			{
				for (int h = 0; h < 5; h++)
				{
					Destroy(p2c[h]);
				}
			}
			break;
			case DTileMap.TileType.Player3:
			{
				
				for (int h = 0; h < 5; h++)
				{
					Destroy(p3c[h]);
				}
			}
			break;
			case DTileMap.TileType.Player4:
			{
				for (int h = 0; h < 5; h++)
				{
					Destroy(p4c[h]);
				}
			}
			break;
			case DTileMap.TileType.Target1:
			{
				for (int h = 0; h < 5; h++)
				{
					Destroy(t1c[h]);
				}
			}
			break;
			case DTileMap.TileType.Target2:
			{
				for (int h = 0; h < 5; h++)
				{
					Destroy(t2c[h]);
				}
			}
			break;
			case DTileMap.TileType.Target3:
			{
				for (int h = 0; h < 5; h++)
				{
					Destroy(t3c[h]);
				}
			}
			break;
		}

		Debug.Log ("Attack: " + tempatk + "Defence: " + tardef);
		if (tempatk > tardef)
		{
			if(mManager.curDefending >= DTileMap.TileType.Target1)
			{

				//mManager.CurrentTargetDefender().gameObject.renderer.enabled = false;
				mManager.AttackWorked = true;
				//Kill target.
			}
			else
			{
				//mManager.CurrentPlayerDefender().gameObject.renderer.enabled = false;
				mManager.AttackWorked = true;
			}
		}
		else if (taratk > tempdef)
		{
			//mManager.CurrentPlayer ().gameObject.renderer.enabled = false;
			mManager.CounterAttackWorked = true;
			//Kill player.
		}
		else
		{
			//Do nothing.
		}
		mManager.HudUpdated = true;
		rotr = false;
	}

	void rotate180(List<GameObject> a)
	{

		for (int b = 0; b< a.Count; b++)
		{
			if (a[b] != null)
			{
				a[b].transform.Rotate(0, Time.deltaTime * 30, 0, UnityEngine.Space.Self);
			}
			
		}
		
	}

	void lookat(GameObject[] hand, RaycastHit mhit)
	{
		if(mhit.collider.CompareTag("Card"))
		{
			for (int o = 0; o < 15; o++)
			{
				if (hand[o])
				{
					sizer = 0;
					hand[o].transform.gameObject.tag = "Card";
					hand[o].transform.Find("Card").gameObject.tag = "Card";
				}
				
			}
			te = -1;
			for (int p = 0; p < hand.Length; p++)
			{
				if (hand[p] != null)
					if (mhit.collider.transform.position == hand[p].transform.position)
						te = p;
				
			}
			mhit.collider.tag = "Mouseover";
			getbigger = mhit.collider.transform.position;
			sizer = 0;
			
		}
		else if (mhit.collider.CompareTag("Mouseover"))
		{
			
			mhit.collider.transform.parent.position = Vector3.Lerp(getbigger, (getbigger + (Camera.main.transform.up * 2.0f)), sizer);
			sizer += 0.1f;
			
		}
		
	}

	void settagtocard(GameObject[] hand)
	{
		for (int o = 0; o < 15; o++)
		{
			if (hand[o] && o != ignorecard)
			{
				sizer = 0;
				hand[o].transform.gameObject.tag = "Card";
				hand[o].transform.Find("Card").gameObject.tag = "Card";
			}
			
		}
	}

	void clickcheck(GameObject[] hand, RaycastHit hit)
	{
		if(hit.collider.CompareTag("Mouseover"))
		{
			incr = 0;
			curcard = hit;
			choosing = true;
			temp = curcard.transform.position;
			hit.collider.tag = "Selected";
			for (int p = 0; p < hand.Length; p++)
			{
				if (hand[p] != null)
					if (hit.collider.transform.position == hand[p].transform.position)
						ignorecard = p;
				
			}
			
		}
	}

	void switchdisp()
	{
		for (int o = 0; o < 15; o++)
		{
			if (hand1[o] != null)
			{
				hand1[o].SetActive(false);
			}
			if (hand2[o] != null)
			{
				hand2[o].SetActive(false);
			}
			if (hand3[o] != null)
			{
				hand3[o].SetActive(false);
			}
			if (hand4[o] != null)
			{
				hand4[o].SetActive(false);
			}
			if (mManager.CurrentPlayer() == mManager.sPlayers[0])
			{
				if (hand1[o] != null)
				hand1[o].SetActive(true);
			}
			else if (mManager.CurrentPlayer() == mManager.sPlayers[1])
			{
				if (hand2[o] != null)
				hand2[o].SetActive(true);
			}
			else if (mManager.CurrentPlayer() == mManager.sPlayers[2])
			{
				if (hand3[o] != null)
				hand3[o].SetActive(true);
			}
			else if (mManager.CurrentPlayer() == mManager.sPlayers[3])
			{
				if (hand4[o] != null)
				hand4[o].SetActive(true);
			}
		}
	}
	
}