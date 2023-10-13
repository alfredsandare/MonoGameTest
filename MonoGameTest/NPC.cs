using System;
using System.Collections.Generic;

public class NPC : Creature
{
	public NPC(IDictionary<string, Animation> animations, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, string defaultSprite)
        : base(animations, xPos, yPos, hitBoxWidth, hitboxHeight, layer, true, defaultSprite)
    {

		
	}

	public void Update()
	{

	}
}
