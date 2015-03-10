using UnityEngine;
using System.Collections;

public class charboxdisplay : MonoBehaviour {

	private GameObject stat, estat;
	public Texture2D stats;

	// Use this for initialization
	void Start () {
		stat = new GameObject();
		stat.AddComponent<GUITexture> ();
		stat.transform.localScale = Vector3.zero;
		stat.guiTexture.pixelInset = new Rect((Screen.width/2) , (Screen.height/2) - 150, 300, 400);
		stat.layer = 8;
		estat = new GameObject();
		estat.AddComponent<GUITexture> ();
		estat.transform.localScale = Vector3.zero;
		estat.guiTexture.pixelInset = new Rect((Screen.width/2) , (Screen.height/2) - 150, 300, 400);
		estat.layer = 8;

	}
	
	// Update is called once per frame
	void OnGUI () {
		estat.guiTexture.pixelInset = new Rect(Screen.width - Screen.width, (Screen.height/2) - 250, 300, 400);
		estat.guiTexture.texture = stats;
		
		stat.guiTexture.pixelInset = new Rect(Screen.width - 300, (Screen.height/2) - 250, 300, 400);
		stat.guiTexture.texture = stats;
	}
}
