using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class VisualObject
{
    public List<SpriteComponent> spriteComponents;
	public int xPos;
	public int yPos;
	public int hitboxWidth;
	public int hitboxHeight;
	public int layer;
	public bool isSolid;
	public int hitboxXOffset;
	public int hitboxYOffset;

    public VisualObject(List<SpriteComponent> spriteComponents, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, bool isSolid)
	{
		this.spriteComponents = spriteComponents;
		this.xPos = xPos;
		this.yPos = yPos;
		this.hitboxWidth = hitBoxWidth;
		this.hitboxHeight = hitboxHeight;
		this.layer = layer;
		this.isSolid = isSolid;
		this.hitboxXOffset = 0;
		this.hitboxYOffset = 0;
	}

	public void SetHitboxOffset(int x, int y)
	{
		this.hitboxXOffset = x;
		this.hitboxYOffset = y;
	}

	

    public bool EntitiesOverlap(int x, int y, int width, int height)
    {
		int selfXPos = this.xPos + this.hitboxXOffset;
		int selfYPos = this.yPos + this.hitboxYOffset;
		//if any of the first rectangle's 4 corners is inside the second rectangle
        int[] coords1 = { selfXPos, selfYPos, selfXPos + this.hitboxWidth, selfYPos, selfXPos, selfYPos + this.hitboxHeight, selfXPos + this.hitboxWidth, selfYPos + this.hitboxHeight };
        for (int i = 0; i < 8; i += 2)
        {
            if (x < coords1[i] && coords1[i] < x + width && y < coords1[i + 1] && coords1[i + 1] < y + height)
            {
                return true;
            }
        }

		//if any of the second rectangle's 4 corners is inside the first rectangle
		
		int[] coords2 = { x, y, x + width, y, x, y + height, x + width, y + height };
		for (int i = 0; i < 8; i+=2)
		{
			if (selfXPos < coords2[i] && coords2[i] <= selfXPos + this.hitboxWidth && selfYPos < coords2[i + 1] && coords2[i + 1] <= selfYPos + this.hitboxHeight)
			{
				return true;
			}
		}
        return false;
    }
}