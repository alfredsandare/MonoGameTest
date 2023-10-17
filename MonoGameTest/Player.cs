using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Player : Creature
{
	public Player(IDictionary<string, Animation> animations, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, string defaultSprite, double baseSpeed) 
		: base(animations, xPos, yPos, hitBoxWidth, hitboxHeight, layer, true, defaultSprite, baseSpeed)
	{

	}
}
