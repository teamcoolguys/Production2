using UnityEngine;
using System.Collections;

public class CalamityNinja
{
	public string mCharacterName = "Calmity";
	public int mInputAttack = 5;
	public int mInputDefence = 4;
	public int mInputMovement = 6;
	public int mInputRange = 1;
	public Faction mFaction = Faction.Clockwork;
	
	public enum Faction
	{
		SkyPirates,
		Clockwork,
		Tesla,
		Automaton
	};
}
