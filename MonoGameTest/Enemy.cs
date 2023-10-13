using System;
using System.Collections.Generic;

public class Enemy : Creature
{
	public Enemy(IDictionary<string, Animation> animations, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, string defaultSprite)
        : base(animations, xPos, yPos, hitBoxWidth, hitboxHeight, layer, true, defaultSprite)
    {
		
	}

}
