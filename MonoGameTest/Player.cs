using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Player : Creature
{
	string faceDirection;
	public Player(List<SpriteComponent> spriteComponents, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, bool isSolid) : base(spriteComponents, xPos, yPos, hitBoxWidth, hitboxHeight, layer, isSolid)
	{
		this.faceDirection = "e";
	}

	public void MovePlayer(string xDirection, string yDirection, double speed, double deltaTime, List<VisualObject> visualObjects)
	{
		base.Move(xDirection, yDirection, speed, deltaTime, visualObjects, -1);

		//Debug.WriteLine($"{xDirection} {yDirection}");

		if (xDirection == "e") { base.spriteComponents[0].SwitchAnimation("player_walk_left"); this.faceDirection = "e"; }
		else if (xDirection == "w") { base.spriteComponents[0].SwitchAnimation("player_walk_right"); this.faceDirection = "w"; }
		else if (yDirection == "n") { base.spriteComponents[0].SwitchAnimation("player_walk_up"); this.faceDirection = "n"; }
		else if (yDirection == "s") { base.spriteComponents[0].SwitchAnimation("player_walk_down"); this.faceDirection = "s"; }
		else if (xDirection == "" && yDirection == "")
		{
            if (this.faceDirection == "e") base.spriteComponents[0].SwitchAnimation("player_stand_left");
            else if (this.faceDirection == "w") base.spriteComponents[0].SwitchAnimation("player_stand_right");
            else if (this.faceDirection == "n") base.spriteComponents[0].SwitchAnimation("player_stand_up");
            else if (this.faceDirection == "s") base.spriteComponents[0].SwitchAnimation("player_stand_down");
        }
    }
}
