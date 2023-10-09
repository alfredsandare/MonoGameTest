﻿using Microsoft.Xna.Framework;
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
	private int hitboxXOffset;
	private int hitboxYOffset;

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

	public void Move (string xDirection, string yDirection, double speed, double deltaTime, List<VisualObject> visualObjects, int thisObjectIndex)
	{
		if (xDirection != "" && yDirection != "")
		{
			speed *= 1 / (Math.Pow(2, 0.5));
        }

		int xMultiplier = 0; if (xDirection == "w") xMultiplier = 1; else if (xDirection == "e") xMultiplier = -1;
		int yMultiplier = 0; if (yDirection == "s") yMultiplier = 1; else if (yDirection == "n") yMultiplier = -1;

		this.xPos += (int)(speed * deltaTime * xMultiplier);
		this.yPos += (int)(speed * deltaTime * yMultiplier);

		for (int i = 0; i < visualObjects.Count; i++)
		{
			if (i != thisObjectIndex && EntitiesOverlap(visualObjects[i].xPos, visualObjects[i].yPos, visualObjects[i].hitboxWidth, visualObjects[i].hitboxHeight))
			{
				//Debug.WriteLine("{0} {1} {2} {3}", visualObjects[i].xPos, visualObjects[i].yPos, visualObjects[i].hitboxWidth, visualObjects[i].hitboxHeight);
				if (this.isSolid && visualObjects[i].isSolid) 
				{
					int[] distances =
					{
						Math.Abs(visualObjects[i].xPos - (this.xPos + this.hitboxXOffset + this.hitboxWidth)), //snap to the left
						Math.Abs(visualObjects[i].yPos - (this.yPos + this.hitboxYOffset + this.hitboxHeight)), //snap to the top
						Math.Abs(visualObjects[i].xPos + visualObjects[i].hitboxWidth - this.xPos - this.hitboxXOffset), //snap to the right
						Math.Abs(visualObjects[i].yPos + visualObjects[i].hitboxHeight - this.yPos - this.hitboxYOffset), //snap to the bottom
					};

					int smallestDistance = 999999999;
					int direction = 0;
					for (int j=0; j<4; j++) 
					{
						if (distances[j] < smallestDistance)
						{
							smallestDistance = distances[j];
							direction = j;
						}
					}
					switch (direction)
					{
						case 0:
							this.xPos -= smallestDistance;
							break;
						case 1:
							this.yPos -= smallestDistance;
							break;
						case 2:
							this.xPos += smallestDistance;
							break;
						case 3:
							this.yPos += smallestDistance;
							break;
						default:
							break;
					}
				}	
			}
		}
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