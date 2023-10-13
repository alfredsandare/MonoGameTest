using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Player : Creature
{
	string faceDirection;
	public double speed;
	public Player(IDictionary<string, Animation> animations, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, string defaultSprite) 
		: base(animations, xPos, yPos, hitBoxWidth, hitboxHeight, layer, true, defaultSprite)
	{
		this.faceDirection = "e";
		this.speed = 150f;
	}

	public void MovePlayer(string xDirection, string yDirection, double speed, double deltaTime, List<VisualObject> visualObjects)
	{
		base.Move(xDirection, yDirection, speed, deltaTime, visualObjects, -1);

		//Debug.WriteLine($"{xDirection} {yDirection}");

		if (xDirection == "e") { base.SwitchAnimation("player_walk_left"); this.faceDirection = "e"; }
		else if (xDirection == "w") { base.SwitchAnimation("player_walk_right"); this.faceDirection = "w"; }
		else if (yDirection == "n") { base.SwitchAnimation("player_walk_up"); this.faceDirection = "n"; }
		else if (yDirection == "s") { base.SwitchAnimation("player_walk_down"); this.faceDirection = "s"; }
		else if (xDirection == "" && yDirection == "")
		{
            if (this.faceDirection == "e") base.SwitchAnimation("player_stand_left");
            else if (this.faceDirection == "w") base.SwitchAnimation("player_stand_right");
            else if (this.faceDirection == "n") base.SwitchAnimation("player_stand_up");
            else if (this.faceDirection == "s") base.SwitchAnimation("player_stand_down");
        }
    }
}
