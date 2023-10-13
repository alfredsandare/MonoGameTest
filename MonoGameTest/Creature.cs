using System;
using System.Collections.Generic;

public class Creature : VisualObject
{
	public Creature(IDictionary<string, Animation> animations, int xPos, int yPos, int hitBoxWidth, int hitboxHeight, int layer, bool isSolid, string defaultSprite) 
		: base(animations, xPos, yPos, hitBoxWidth, hitboxHeight, layer, isSolid, defaultSprite)
    {

	}

    public void Move(string xDirection, string yDirection, double speed, double deltaTime, List<VisualObject> visualObjects, int thisObjectIndex)
    {
        if (xDirection != "" && yDirection != "")
        {
            speed *= 1 / (Math.Pow(2, 0.5));
        }

        int xMultiplier = 0; if (xDirection == "w") xMultiplier = 1; else if (xDirection == "e") xMultiplier = -1;
        int yMultiplier = 0; if (yDirection == "s") yMultiplier = 1; else if (yDirection == "n") yMultiplier = -1;

        base.xPos += (int)(speed * deltaTime * xMultiplier);
        base.yPos += (int)(speed * deltaTime * yMultiplier);

        for (int i = 0; i < visualObjects.Count; i++)
        {
            if (i != thisObjectIndex && EntitiesOverlap(visualObjects[i].xPos, visualObjects[i].yPos, visualObjects[i].hitboxWidth, visualObjects[i].hitboxHeight))
            {
                //Debug.WriteLine("{0} {1} {2} {3}", visualObjects[i].xPos, visualObjects[i].yPos, visualObjects[i].hitboxWidth, visualObjects[i].hitboxHeight);
                if (this.isSolid && visualObjects[i].isSolid)
                {
                    int[] distances =
                    {
                        Math.Abs(visualObjects[i].xPos - (base.xPos + base.hitboxXOffset + base.hitboxWidth)), //snap to the left
						Math.Abs(visualObjects[i].yPos - (base.yPos + base.hitboxYOffset + base.hitboxHeight)), //snap to the top
						Math.Abs(visualObjects[i].xPos + visualObjects[i].hitboxWidth - base.xPos - base.hitboxXOffset), //snap to the right
						Math.Abs(visualObjects[i].yPos + visualObjects[i].hitboxHeight - base.yPos - base.hitboxYOffset), //snap to the bottom
					};

                    int smallestDistance = 999999999;
                    int direction = 0;
                    for (int j = 0; j < 4; j++)
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
                            base.xPos -= smallestDistance;
                            break;
                        case 1:
                            base.yPos -= smallestDistance;
                            break;
                        case 2:
                            base.xPos += smallestDistance;
                            break;
                        case 3:
                            base.yPos += smallestDistance;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
