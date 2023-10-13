using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class SpriteComponent
{
	public string defaultSprite;
	public int xOffset;
	public int yOffset;
	public string currentSprite;
	IDictionary<string, Animation> animations;
	string currentAnimation;

	public SpriteComponent(string defaultSprite, int xOffset, int yOffset, IDictionary<string, Animation> animations)
	{
		this.defaultSprite = defaultSprite;
		this.currentSprite = defaultSprite;
		this.xOffset = xOffset;
		this.yOffset = yOffset;
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
