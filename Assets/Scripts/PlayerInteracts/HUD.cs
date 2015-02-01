using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour 
{
	//publix
	public List<GameObject> deck = new List<GameObject>();
	public List<GUITexture> infamsprites = new List<GUITexture>();
	public double uoff = 0;
	public GUITexture bar, backbar, turns, stats;
	public GUITexture combar, atkbar, defbar, atkprt, defprt, cardslots;
	public GUIText str, def, mov, inf;

	//privates
	private baseCharacter ch;
	private List<GameObject> discard = new List<GameObject>();
	private List<GameObject> cards = new List<GameObject>();
	private List<GameObject> hand = new List<GameObject>();
	private int cardsDealt = 0;
	private int cardsheld = 0;
	private bool[] cs = new bool[3];
	private bool showR = false;
	private bool combui = false;

	//wyatt
	private GameManager mManager;
	//

	public float maxinfamy, infamy, percent;

	void Awake()
	{
		if(PhotonNetwork.isMasterClient)
		{
			mManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>(); // thats how you get infromation from the manager
		}
	}
	void Start ()
	{

		for (int i = 0; i < maxinfamy; i++)
		{

			GameObject go = GameObject.Instantiate(infamsprites[i]) as GameObject;

		}
		maxinfamy = 8; infamy = 0;
		//Compute Player stats here

		//str.text = player.GetComponent<GameClient> ().playerPrefab.mCharacter.mInputAttack.ToString();
		//def.text = player.GetComponent<GameClient> ().playerPrefab.mCharacter.mInputDefence.ToString();
		//mov.text = player.GetComponent<GameClient>().playerPrefab.mCharacter.mInputMovement.ToString();

		ResetDeck ();
	}

	void ResetDeck()
	{
		cardsDealt = 0;
		for (int i = 0; i < hand.Count; i++) 
		{
			Destroy(hand[i]);	
		}
		for (int i = 0; i < discard.Count; i++) 
		{
			Destroy(discard[i]);
		}
		discard.Clear ();
		hand.Clear ();
		cards.Clear ();
		cards.AddRange (deck);
		showR = false;
		cardsheld = 0;
		cardsDealt = 0;
	}

	void Playcard(GameObject cardd)
	{
		GameObject al;
		al = cardd.transform.parent.gameObject;
		discard.Add (al);
		hand.Remove (al);
		Destroy (al);
		//do card stuff
	}

	GameObject DealCard()
	{
		if (cards.Count == 0) 
		{
			showR = true;
			return null;
			// alternately reset deck with ResetDeck();
		}
		int card = Random.Range (0, cards.Count - 1);
		GameObject go = GameObject.Instantiate (cards [card]) as GameObject;
		cards.RemoveAt (card);
		cardsheld++;
		return go;
	}

	void Gameover()
	{
		cardsDealt = 0;
		for (int i = 0; i < hand.Count; i++) 
		{
			Destroy(hand[i]);	
		}
		for (int i = 0; i < discard.Count; i++) 
		{
			Destroy(discard[i]);
		}
		discard.Clear ();
		hand.Clear ();
		cards.Clear ();
		cards.AddRange (deck);
		showR = false;
		cardsheld = 0;
		cardsDealt = 0;
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
		
		if (combui) 
		{
			//int screenAspectRatio = (screenWidth / screenHeight);
			//int textureAspectRatio = (textureWidth / textureHeight);
			GUI.Box(new Rect((Screen.width/2) - 180,210,100,150), ""); //attacker card slot
			GUI.Box(new Rect((Screen.width/2) + 140,40,100,150), "");	//target card slot
			GUI.Box(new Rect((Screen.width/2) + 260,40,100,150), "");//target card slot
			GUI.Box(new Rect((Screen.width/2) + 140,210,100,150), "");//target card slot
			GUI.Box(new Rect((Screen.width/2) + 260,210,100,150), "");//target card slot
			if (GUI.Button(new Rect((Screen.width/2) - 180,380,100, 25), "Fight!")) //Fight button
			{
				//Combatstuff
			}
			GUI.Box(new Rect((Screen.width/2),30,60,380), "");//Middle back bar
			GUI.Box(new Rect((Screen.width/2) + 5,35,50,185), "");//Middle Attack bar
			GUI.Box(new Rect((Screen.width/2) + 5,220,50,185), "");//Middle Defense bar
			GUI.Box(new Rect((Screen.width/2) - 20,215,100,10), "");//Middle Clash bar
			GUI.Box(new Rect((Screen.width/2) - 180,80,100,100), "");//Attacker Port box
			GUI.Box(new Rect((Screen.width/2) - 180,40,100,20), "PlayerName");//Attacker name box
			GUI.Box(new Rect((Screen.width/2) + 210,430,100,100), "");//Def port box
			GUI.Box(new Rect((Screen.width/2) + 210,390,100,20), "Defendername");//Def name box
		}
		else if (combui == false)
		{
			if (infamy == 0)
				percent = 0;
			else if (infamy >= maxinfamy)
			{percent = 190; infamy = maxinfamy;}
			else
				percent = 190 * (infamy/maxinfamy);

			GUI.DrawTexture(new Rect((Screen.width/2) - 100, (Screen.height/2) - (Screen.height/2) + 25, 200, 30), backbar.texture , ScaleMode.StretchToFill, true, 0.0f);
			GUI.DrawTexture(new Rect((Screen.width/2) - 95, (Screen.height/2) - (Screen.height/2) + 30, percent, 20), bar.texture , ScaleMode.StretchToFill, true, 0.0f);
			inf.text = "Infamy"; //Infamy text and spacing it out
			//turns
			GUI.DrawTexture(new Rect((Screen.width/2) - 470, (Screen.height/2) - 240, 200, 300), turns.texture , ScaleMode.StretchToFill, true, 0.0f);
			//stats
			GUI.DrawTexture(new Rect((Screen.width/2) + 260, (Screen.height/2) - 240, 200, 300), stats.texture , ScaleMode.StretchToFill, true, 0.0f);

			if (GUI.Button(new Rect(40,40,50, 30), "INFAMY BOOOST"))
			{
				infamy = infamy+1;
			}
			
			if (!showR) 
			{
				if (GUI.Button(new Rect(10,10,100, 20), "Deal"))
				{
					MoveDealtCard();
				}
			}
			else
			{
				if (GUI.Button(new Rect(10, 10, 100, 20), "Reset"))
				{
					ResetDeck();
				}
			}
			if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 20), "GameOver"))
			{
				Gameover();
			}
		}
		//GameObject go = GameObject.Instantiate

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
		hand.Add (newCard);
		cardsDealt++;
	}

	void Rearrangehand()
	{
		GameObject hudd = GameObject.FindGameObjectWithTag("HUD");
		float offset = (float)-.5;
		offset = offset + (float)uoff;
		if (cardsheld == 1)
		{
			offset = offset + 2;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward * 6;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x, hand[0].transform.position.y - 5, hand[0].transform.position.z); 
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		if (cardsheld == 2)
		{
			offset = offset + (float)1.5;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[0].transform.position = hand[0].transform.position + Camera.main.transform.right * (float)-.7;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x, hand[0].transform.position.y - 5, hand[0].transform.position.z);
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[1].transform.position = Camera.main.transform.position + Camera.main.transform.forward * 6;
			hand[1].transform.position = hand[1].transform.position + Camera.main.transform.right * (float).7;
			hand[1].transform.position = new Vector3(hand[1].transform.position.x, hand[1].transform.position.y - 5, hand[1].transform.position.z);
			hand[1].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		if (cardsheld == 3)
		{
			offset = offset + 1;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[0].transform.position = hand[0].transform.position + Camera.main.transform.right * (float)-1.4;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x , hand[0].transform.position.y- 5, hand[0].transform.position.z );
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;

			hand[1].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;

			hand[1].transform.position = new Vector3(hand[1].transform.position.x , hand[1].transform.position.y- 5, hand[1].transform.position.z );
			hand[1].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[2].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[2].transform.position = hand[2].transform.position + Camera.main.transform.right * (float)1.4;
			hand[2].transform.position = new Vector3(hand[2].transform.position.x , hand[2].transform.position.y- 5, hand[2].transform.position.z );
			hand[2].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		if (cardsheld == 4)
		{
			offset = offset + (float).5;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[0].transform.position = hand[0].transform.position + Camera.main.transform.right * (float)-2.1;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x , hand[0].transform.position.y - 5, hand[0].transform.position.z );
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[1].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[1].transform.position = hand[1].transform.position + Camera.main.transform.right * (float)-.7;
			hand[1].transform.position = new Vector3(hand[1].transform.position.x , hand[1].transform.position.y - 5, hand[1].transform.position.z );
			hand[1].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[2].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[2].transform.position = hand[2].transform.position + Camera.main.transform.right * (float).7;
			hand[2].transform.position = new Vector3(hand[2].transform.position.x , hand[2].transform.position.y - 5, hand[2].transform.position.z );
			hand[2].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[3].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[3].transform.position = hand[3].transform.position + Camera.main.transform.right * (float)2.1;
			hand[3].transform.position = new Vector3(hand[3].transform.position.x , hand[3].transform.position.y - 5, hand[3].transform.position.z );
			hand[3].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		if (cardsheld == 5)
		{
			offset = offset + (float).5;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[0].transform.position = hand[0].transform.position + Camera.main.transform.right * (float)-2.8;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x , hand[0].transform.position.y - 5, hand[0].transform.position.z );
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[1].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[1].transform.position = hand[1].transform.position + Camera.main.transform.right * (float)-1.4;
			hand[1].transform.position = new Vector3(hand[1].transform.position.x , hand[1].transform.position.y - 5, hand[1].transform.position.z );
			hand[1].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[2].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[2].transform.position = new Vector3(hand[2].transform.position.x , hand[2].transform.position.y - 5, hand[2].transform.position.z );
			hand[2].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[3].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[3].transform.position = hand[3].transform.position + Camera.main.transform.right * (float)1.4;
			hand[3].transform.position = new Vector3(hand[3].transform.position.x , hand[3].transform.position.y - 5, hand[3].transform.position.z );
			hand[3].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[4].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[4].transform.position = hand[4].transform.position + Camera.main.transform.right * (float)2.8;
			hand[4].transform.position = new Vector3(hand[4].transform.position.x , hand[4].transform.position.y - 5, hand[4].transform.position.z );
			hand[4].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}


	}

//	void Rearrangeinfamy()
//	{
//		int w =24, h =24, x = -128, y = -80;
//		for (int i=0; i < maxinfamy; i++)
//		{
//			transform.position = Vector3.zero;
//			transform.localScale = Vector3.zero;
//			infamsprites[i].pixelInset = new Rect(x,y,w,h);
//				x = x + 24;
//		}
//	}

	void Update()
	{
		if(!mManager)
		{
			mManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();
		}
		str.text = infamy.ToString();
		if (Input.GetKeyDown("space"))
		{
			if (combui == false)
				combui = true;
			else
				combui = false;
		}

		if (combui) 
		{

		} 
		else 
		{

		}

		//Rearrangeinfamy ();
		//bar = GameObject.Find("frontbr").guiTexture;


		Rearrangehand ();
		if (cardsheld == 5 || cards.Count == 0) 
		{
			showR = true;
		}
		else
			showR = false;
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))			
			{ 
				Debug.Log("clicked it");
				
				if(hit.collider.CompareTag("Card"))
				{
					Debug.Log("hit it");
					Playcard(hit.collider.gameObject);
					cardsheld= cardsheld - 1;
					cardsDealt = cardsDealt - 1;
				}
			} 
		}
		
	}



}


