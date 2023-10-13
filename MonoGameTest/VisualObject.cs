using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class VisualObject
{
    IDictionary<string, Animation> animations;
    public int xPos;
	public int yPos;
	public int hitboxWidth;
	public int hitboxHeight;
	public int layer;
	public bool isSolid;
	public int hitboxXOffset;
	public int hitboxYOffset;
	public string defaultSprite;
	public string currentSprite;
	public string currentAnimation;

    public VisualObject(IDictionary<string, Animation> animations, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, bool isSolid, string defaultSprite)
	{
		this.animations = animations;
		this.xPos = xPos;
		this.yPos = yPos;
		this.hitboxWidth = hitBoxWidth;
		this.hitboxHeight = hitboxHeight;
		this.layer = layer;
		this.isSolid = isSolid;
		this.hitboxXOffset = 0;
		this.hitboxYOffset = 0;
		this.defaultSprite = defaultSprite;
		this.currentSprite = defaultSprite;
        this.animations = animations;
        if (animations != null)
        {
            this.currentAnimation = new List<string>(animations.Keys)[0];
        }
        else
        {
            this.currentAnimation = null;
        }
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

    public void UpdateAnimation(double deltaTime)
    {
        if (animations == null) return;
        this.animations[this.currentAnimation].Update(deltaTime);
        this.currentSprite = this.animations[this.currentAnimation].currentSprite;
    }

    public void SwitchAnimation(string newAnimation)
    {
        if (this.currentAnimation == newAnimation) return;
        this.animations[this.currentAnimation].Reset();
        this.currentAnimation = newAnimation;
    }
}