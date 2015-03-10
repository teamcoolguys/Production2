﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour 
{
	//publix
	public GameObject[] deck1 = new GameObject[30];
	public GameObject[] deck2 = new GameObject[30];
	public GameObject[] deck3 = new GameObject[30];
	public GameObject[] deck4 = new GameObject[30];
	public double uoff = 0;
	public Texture bar, backbar;
	public GUITexture combar, atkbar, defbar, atkprt, defprt, cardslots;
	public Texture2D turns, stats;
	
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
	private GameObject stat, estat;
	private GameObject set1, set2, set3, set4, set5, set6, set7, set8, set9, set10, set11, set12;
	private int decksize, cdel;
	private GameObject[] discard = new GameObject[30];
	private GameObject[] cards = new GameObject[30];
	private GameObject[] hand = new GameObject[15];
	private int cardsheld = 0;
	private int cardsDealt = 0, rotint = 0;
	private bool[] cs = new bool[3];
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
	private int attackeratk, attackerdef, defenderatk, defenderdef, bartotal, barpercent;
	
	//wyatt
	private GameManager mManager;
	//
	
	public float percent;

	//Sound Stuff
	public AudioSource Source = new AudioSource();
	
	public AudioClip SoundPlayCard;
	public AudioClip SoundSelectCard;
	public AudioClip SoundDrawCard;

	
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
				
		stat = new GameObject();
		stat.AddComponent<GUITexture> ();
		stat.transform.localScale = Vector3.zero;
		stat.guiTexture.pixelInset = new Rect((Screen.width/2), (Screen.height/2), 200, 300);
		
		estat = new GameObject();
		estat.AddComponent<GUITexture> ();
		estat.transform.localScale = Vector3.zero;
		estat.guiTexture.pixelInset = new Rect((Screen.width/2), (Screen.height/2), 200, 300);
		
		set1 = new GameObject();
		set1.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set1.AddComponent<GUIText> ();
		
		set2 = new GameObject();
		set2.AddComponent<GUIText>();
		set2.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set2.guiText.pixelOffset = new Vector2 (250, 0);
		
		set3 = new GameObject ();
		set3.AddComponent<GUIText>();
		set3.transform.position = new Vector3(0.5f, 0.5f,  1.0f);
		set3.guiText.pixelOffset = new Vector2 (350, 0);
		
		set4 = new GameObject();
		set4.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set4.AddComponent<GUIText>();
		
		set5 = new GameObject();
		set5.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set5.AddComponent<GUIText>();
		
		set6 = new GameObject();
		set6.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set6.AddComponent<GUIText>();
		
		set7 = new GameObject();
		set7.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set7.AddComponent<GUIText>();
		
		set8 = new GameObject();
		set8.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set8.AddComponent<GUIText>();
		
		set9 = new GameObject();
		set9.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set9.AddComponent<GUIText>();
		
		set10 = new GameObject();
		set10.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set10.AddComponent<GUIText>();
		
		set11 = new GameObject();
		set11.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set11.AddComponent<GUIText>();
		
		set12 = new GameObject();
		set12.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set12.AddComponent<GUIText>();
		
		ResetDeck ();
		
	}
	
	void ResetDeck()
	{
		for (int i = 0; i < hand.Length; i++) 
		{
			if (hand[i] != null)
				Destroy(hand[i]);
			hand[i] = null;	
		}
		for (int i = 0; i < discard.Length; i++) 
		{
			discard[i] = null;	
		}
		
		System.Array.Copy (deck1, cards, cards.Length);

		showR = false;
		cdel = 0;
		cardsheld = 0;
		cardsDealt = 0;
	}
	
	void Playcard(GameObject cardd)
	{
		GameObject al;
		al = cardd.transform.parent.gameObject;
		
		for (int i = 0; i <discard.Length; i++)
		{
			if (discard[i] == null)
			{
				discard[i] = cardd;
			}
		}
		Destroy (al);
		//do card stuff
	}
	
	GameObject DealCard()
	{
		Source.clip = SoundDrawCard;
		Source.Play();
		
		System.Random rand = new System.Random();
		int card = rand.Next (30);
		while(true)
		{
			if (cards [card] == null)
			{
				card = rand.Next (30);
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
		
		cardsheld++;
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
		for (int l = 0; l < 5; l++)
		{
			if (p1c[l] != null)
				p1c[l].SetActive(false);
			
			if (p2c[l] != null)
				p2c[l].SetActive(false);
			
			if (p3c[l] != null)
				p3c[l].SetActive(false);
			
			if (p4c[l] != null)
				p4c[l].SetActive(false);
			
			if (t1c[l] != null)
				t1c[l].SetActive(false);
			
			if (t2c[l] != null)
				t2c[l].SetActive(false);
			
			if (t3c[l] != null)
				t3c[l].SetActive(false);
		}
		
		estat.guiTexture.pixelInset = new Rect(Screen.width - Screen.width, (Screen.height/2) - 150, 200, 300);
		stat.guiTexture.pixelInset = new Rect(Screen.width - 200, (Screen.height/2) - 150, 200, 300);
		stat.guiTexture.texture = stats;
		//estat.guiTexture.pixelInset = new Rect (100, 100, 100, 100);
		estat.guiTexture.texture = stats;
		//GUI.DrawTexture(new Rect((Screen.width/2) + 275, 100 , 200, 300), stats , ScaleMode.StretchToFill, true, 0.0f);
		//GUI.DrawTexture(new Rect((Screen.width/2) - 475, 100 , 200, 300), stats , ScaleMode.StretchToFill, true, 0.0f);
		
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
			if(!PhotonNetwork.offlineMode)
			{
				set5.guiText.text = mManager.CurrentPlayer().gameObject.GetPhotonView().photonView.owner.name;
			}
			else
			{
				set5.guiText.text = mManager.CurrentPlayer().name;
			}
			
			set6.guiText.text = mManager.CurrentPlayer().mAttack.ToString();
			set7.guiText.text = mManager.CurrentPlayer().mDefence.ToString();
			set8.guiText.text = mManager.CurrentPlayer().mMovement.ToString();
		}
		
		set4.guiText.pixelOffset = new Vector2 (-70, (Screen.height/2));
		set5.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 100, 0 + 120);
		set6.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 100, 0 + 65);
		set7.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 100, 0 + 20);
		set8.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 100, 0 - 25);
		
		//Target/Other player stats
		if (choosing)
		{
			if (GUI.Button(new Rect(50,(Screen.height/2) - 200,100, 40), "Play Card") )
			{
				choosing = false;
				
				Source.clip = SoundPlayCard;
				Source.Play();
				
				switch(mManager.curDefending)
				{
				case(DTileMap.TileType.Player1):
					
					for(int i = 0; i < 4; ++i)
					{
						if (p1c[i] == null)
						{
							PlayCardOnPlayer(p1c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Player2):
					for(int i = 0; i < 4; ++i)
					{
						if (p2c[i] == null)
						{
							PlayCardOnPlayer(p2c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Player3):
					for(int i = 0; i < 4; ++i)
					{
						if (p3c[i] == null)
						{
							PlayCardOnPlayer(p3c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Player4):
					for(int i = 0; i < 4; ++i)
					{
						if (p4c[i] == null)
						{
							PlayCardOnPlayer(p4c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Target1):
					for(int i = 0; i < 4; ++i)
					{
						if (t1c[i] == null)
						{
							PlayCardOnPlayer(t1c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Target2):
					for(int i = 0; i < 4; ++i)
					{
						if (t2c[i] == null)
						{
							PlayCardOnPlayer(t2c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Target3):
					for(int i = 0; i < 4; ++i)
					{
						if (t3c[i] == null)
						{
							PlayCardOnPlayer(t3c[i]);
							
							break;
						}
					}
					break;
					
				}
			}
			
			
			
			if (GUI.Button(new Rect((Screen.width) - 100,(Screen.height/2)- 200, 100, 40), "Play Card"))
			{
				Debug.Log ("Logan Code Sucks" + mManager.curAttacking);
				choosing = false;
				
				Source.clip = SoundPlayCard;
				Source.Play();
				
				switch(mManager.curAttacking)
				{
				case(DTileMap.TileType.Player1):
					
					for(int i = 0; i < 4; ++i)
					{
						if (p1c[i] == null)
						{
							PlayCardOnPlayer(p1c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Player2):
					for(int i = 0; i < 4; ++i)
					{
						if (p2c[i] == null)
						{
							PlayCardOnPlayer(p2c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Player3):
					for(int i = 0; i < 4; ++i)
					{
						if (p3c[i] == null)
						{
							PlayCardOnPlayer(p3c[i]);
							
							break;
						}
					}
					break;
					
				case(DTileMap.TileType.Player4):
					for(int i = 0; i < 4; ++i)
					{
						if (p4c[i] == null)
						{
							PlayCardOnPlayer(p4c[i]);
							
							break;
						}
					}
					break;
				}
			}
		}
		
		if (mManager.curDefending == DTileMap.TileType.TargetSpot || mManager.curDefending == mManager.curAttacking)
		{
			set9.guiText.text = "None Selected";
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
					Attack();
					mManager.CurrentPlayer().mPlayerPhase = Player.PlayerPhase.End;
					// infamy boost infamy = infamy+1;
				}
			}
		}
		set9.guiText.pixelOffset  = new Vector2(-(Screen.width/2) + 100, 0 + 120);
		set10.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 100, 0 + 65);
		set11.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 100, 0 + 20);
		set12.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 100, 0 - 25);
		
		
		if (!showR)
		{
			if (GUI.Button(new Rect(10,10,100, 20), "Deal"))
			{
				MoveDealtCard();
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
		
		Dispcards ();
	}
	
	void ChangeDefenderGUI(DTileMap.TileType TypeofDefender)
	{
		//Debug.Log ("TypeOfDefender" + TypeofDefender);
		if(TypeofDefender >= DTileMap.TileType.Target1)
		{
			//target logic
			//Debug.Log("TargetThatIsDefending::" + TypeofDefender);
			if(!PhotonNetwork.offlineMode)
			{
				set9.guiText.text = mManager.CurrentTargetDefender().name;
			}
			else
			{
				set9.guiText.text = mManager.CurrentTargetDefender().name;
			}
			
			set10.guiText.text = "N/A";
			set11.guiText.text = mManager.CurrentTargetDefender().mDefence.ToString();
			set12.guiText.text = mManager.CurrentTargetDefender().mMovement.ToString();
		}
		else
		{
			//player logic
			//Debug.Log("PlayerThatIsDefending::" + mManager.curDefending);
			if(!PhotonNetwork.offlineMode)
			{
				set9.guiText.text = mManager.CurrentPlayerDefender().gameObject.GetPhotonView().photonView.owner.name;
			}
			else
			{
				set9.guiText.text = mManager.CurrentPlayerDefender().name;
			}
			
			set10.guiText.text = mManager.CurrentPlayerDefender().mAttack.ToString();
			set11.guiText.text = mManager.CurrentPlayerDefender().mDefence.ToString();
			set12.guiText.text = mManager.CurrentPlayerDefender().mMovement.ToString();
		}
	} 
	
	GameObject PlayCardOnPlayer(GameObject gObject)
	{
		curcard.collider.gameObject.tag = "Untagged";
		gObject = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
		cardsheld--;
		Destroy(curcard.collider.gameObject.transform.parent.gameObject);
		return gObject;
	}
	
	void MoveDealtCard()
	{
		GameObject newCard = DealCard ();
		
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
		//hand.Add (newCard);
		cardsDealt++;
	}
	
	void Rearrangehand()
	{
		GameObject[] tempdeck = new GameObject[15];
		int cintd = 0;
		for (int i = 0; i < hand.Length; i++) 
		{
			if (hand[i] != null)
			{
				tempdeck[cintd] = hand[i];
				cintd++;
			}
		}
		
		for (int i = 0; i < hand.Length; i++) 
		{
			hand[i] = tempdeck[i];
		}
		
		GameObject hudd = GameObject.FindGameObjectWithTag("HUD");
		float offset = (float)-5.6;
		offset = offset + (float)uoff;
		
		//Find farthest card left based on how many cards total
		
		for (int i = 0; i < cardsheld; i++)
		{
			hand[i].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[i].transform.position = hand[i].transform.position + Camera.main.transform.right * offset;
			hand[i].transform.position = new Vector3(hand[i].transform.position.x, hand[i].transform.position.y - 5, hand[i].transform.position.z);
			hand[i].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + (float)1.4;
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
		
		
		if (choosing) 
		{		
			if (Input.GetMouseButtonDown (0)) 
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) 
				{ 
					curcard = hit;
				} 
			}
			if (Input.GetMouseButtonDown (1)) 
			{
				choosing = false;
			}
			
		}
		else
		{	
			Rearrangehand ();
			Quaternion targetRotation;
			targetRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
			
			if (Input.GetKeyDown("space"))
			{
				if (rotr == false)
					rotr = true;
				else
					rotr = false;
				
			}
			if (rotr && rotint <= 330)
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
			else if (rotr && rotint > 320)
			{
				
				secs -= 1* Time.deltaTime;
				if (secs <=0)
				{
					secs = 5;
					Attack();
				}
			}
			
			//DEBUG Purposes
			//===================================================
			if (cdel >= 15) 
			{
				showR = true;
			}
			else
				showR = false;
			//===================================================

			if(Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))			
				{ 
					Debug.Log("clicked it");
					
					if(hit.collider.CompareTag("Card"))
					{
						Source.clip = SoundSelectCard;
						Source.Play();
						
						curcard = hit;
						choosing = true;
					}
				} 
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
			return Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
		case(1):
			return Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
		case(2):
			return Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
		case(3):
			return Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
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
			return Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 300, 6.0f) );
		case(1):
			return Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 300, 6.0f) );
		case(2):
			return Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 175, 6.0f) );
		case(3):
			return Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 175, 6.0f) );
		default:
			return new Vector3();
		}
	}
	IEnumerator DelaySound(float x) 
	{
		//print(Time.time);
		yield return new WaitForSeconds(x);
		//print(Time.time);
	}
	void Attack()
	{
		int tempatk, 
		tempdef, 
		taratk,
		tardef;
		
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
		if (tempatk > tardef)
		{
			Source.PlayOneShot(mManager.CurrentPlayer().mAttackSound);
			
			if(mManager.curDefending >= DTileMap.TileType.Target1)
			{
				
				Source.clip = mManager.CurrentTargetDefender ().mBlockSound;
				Source.PlayDelayed (mManager.CurrentPlayer().mAttackSound.length);
				//mManager.CurrentTargetDefender().gameObject.renderer.enabled = false;
				mManager.AttackWorked = true;
				//Kill target.
			}
			else
			{
				Source.clip = mManager.CurrentPlayerDefender ().mBlockSound;
				Source.PlayDelayed (mManager.CurrentPlayer().mAttackSound.length);
				
				//mManager.CurrentPlayerDefender().gameObject.renderer.enabled = false;
				mManager.AttackWorked = true;
				//Kill Player
			}
		}
		else if (taratk > tempdef)
		{
			if(mManager.curDefending >= DTileMap.TileType.Target1)
			{
				AudioSource.PlayClipAtPoint (mManager.CurrentTargetDefender().mAttackSound, transform.position);
			}
			else
			{
				AudioSource.PlayClipAtPoint (mManager.CurrentPlayerDefender().mAttackSound, transform.position);
			}
			AudioSource.PlayClipAtPoint (mManager.CurrentPlayer().mBlockSound, transform.position);
			//mManager.CurrentPlayer ().gameObject.renderer.enabled = false;
			mManager.CounterAttackWorked = true;
			//Kill player.
		}
		else
		{
			//Do nothing.
		}
		
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
}