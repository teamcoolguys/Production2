//Created by Jack ng
//Nov 1st 2014 
//
using UnityEngine;
using System.Collections;

public class baseCharacter : MonoBehaviour
{
	public string characterName;
	public int mInputAttack;
	public int mInputDefence;
	public int mInputMovement;
	public int mInputRange;
	public Fraction mFraction;

	public enum Fraction
	{
		SkyPirates,
		Clockwork,
		Tesla,
		Automaton
	};
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
