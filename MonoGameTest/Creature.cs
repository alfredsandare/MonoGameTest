using System;
using System.Collections.Generic;

public class Creature : VisualObject
{
	public Creature(List<SpriteComponent> spriteComponents, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, bool isSolid) 
		: base(spriteComponents, xPos, yPos, hitBoxWidth, hitboxHeight, layer, isSolid)
    {

	}
}
