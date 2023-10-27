using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

public class NPC : Creature
{
	string moveXDirection;
	string moveYDirection;
	double moveTimeLeft;
	Random random;

	public string name;
	public string id;
	private string behavior;
	private int homeX;
	private int homeY;

	public IDictionary<string, List<string>> dialogue;

	public bool dialogueAvailable;
	public string currentDialogue;
	public int dialogueProgress;

	public NPC(IDictionary<string, Animation> animations, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, string defaultSprite, double baseSpeed, string name, string id)
        : base(animations, xPos, yPos, hitBoxWidth, hitboxHeight, layer, true, defaultSprite, baseSpeed)
    {
        random = new Random();
		moveXDirection = "";
		moveYDirection = "";
		this.name = name;
		this.id = id;
		this.behavior = "";

		dialogue = new Dictionary<string, List<string>>();
		dialogueProgress = 0;
    }

	public void SetBehavior(string behavior, int homeX, int homeY)
	{
		//homeX and homeY is where the NPC will always go to.
		this.behavior = behavior;
		this.homeX = homeX;
		this.homeY = homeY;
	}

	public void AddDialogue(string key,  List<string> value)
	{
		dialogue.Add(key, value);
	}

	public string GetDialogueText()
	{
        return dialogue[currentDialogue][dialogueProgress];
	}

	public void Update(double deltaTime, List<VisualObject> visualObjects)
	{
		if (moveTimeLeft < 0 || movementLocked)
		{
			moveXDirection = "";
			moveYDirection = "";
			moveTimeLeft = 0;
		}
		else if (moveXDirection == "" && moveYDirection == "")
		{
			switch (behavior)
			{
				case "random":
					if (random.NextDouble() < 0.006d) //initiate new movement
					{
						InitRandomMovement();
					}
					break;

				case "home":
					if (Util.Distance(xPos, yPos, homeX, homeY) < 100)
					{
						if (random.NextDouble() < 0.006d) InitRandomMovement();
					}
					else
					{
						if (xPos < homeX) moveXDirection = "w";
						else moveXDirection = "e";
						if (yPos < homeY) moveYDirection = "s";
						else moveYDirection = "n";
                        moveTimeLeft = random.NextDouble() * 2 + 1;
                    }
					break;

				default:
					break;
			}
		}
		else
		{
			moveTimeLeft -= deltaTime;
		}
		Move(moveXDirection, moveYDirection, deltaTime, visualObjects, -1);
		UpdateAnimation(deltaTime);
	}

	private void InitRandomMovement()
	{
        double r = random.NextDouble(); if (r < 0.3333) moveXDirection = "w"; else if (r < 0.6666) moveXDirection = ""; else moveXDirection = "e";
        r = random.NextDouble(); if (r < 0.3333) moveYDirection = "n"; else if (r < 0.6666) moveYDirection = ""; else moveYDirection = "s";
        moveTimeLeft = random.NextDouble() * 2 + 1;
    }
}