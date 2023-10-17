using System;
using System.Collections.Generic;
using System.Diagnostics;

public class NPC : Creature
{
	string moveXDirection;
	string moveYDirection;
	double moveTimeLeft;
	Random random;

	public string name;
	public string id;

	public NPC(IDictionary<string, Animation> animations, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, string defaultSprite, double baseSpeed, string name, string id)
        : base(animations, xPos, yPos, hitBoxWidth, hitboxHeight, layer, true, defaultSprite, baseSpeed)
    {
        random = new Random();
		moveXDirection = "";
		moveYDirection = "";
		this.name = name;
		this.id = id;
    }

	public void Update(double deltaTime, List<VisualObject> visualObjects)
	{
		if (moveTimeLeft < 0)
		{
			moveXDirection = "";
			moveYDirection = "";
			moveTimeLeft = 0;
		}
		if (moveXDirection == "" && moveYDirection == "")
		{
			Debug.WriteLine("hello");
			if (random.NextDouble() < 0.006d) //initiate new movement
			{
				double r = random.NextDouble(); if (r < 0.3333) moveXDirection = "w"; else if (r < 0.6666) moveXDirection = ""; else moveXDirection = "e";
				r = random.NextDouble(); if (r < 0.3333) moveYDirection = "n"; else if (r < 0.6666) moveYDirection = ""; else moveYDirection = "s";
				moveTimeLeft = random.NextDouble() * 2 + 1;
			}
		}
		else
		{
			moveTimeLeft -= deltaTime;
		}
		Debug.WriteLine($"x: {moveXDirection} y: {moveYDirection}");
		Move(moveXDirection, moveYDirection, deltaTime, visualObjects, -1);
		UpdateAnimation(deltaTime);
	}
}