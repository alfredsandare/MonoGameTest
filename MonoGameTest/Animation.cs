using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Animation
{
	List<string> sprites;
	List<float> framePoints;
	double sequenceLength;
	double sequenceProgress;
	public string currentSprite;


	public Animation(List<string> sprites, List<float> framePoints)
	{
		this.sprites = sprites;
		this.framePoints = framePoints;
		this.sequenceLength = framePoints.Last();
		this.sequenceProgress = 0;
	}

	public void Update(double deltaTime)
	{
		this.sequenceProgress += deltaTime;
		if (this.sequenceProgress > this.sequenceLength) this.sequenceProgress -= this.sequenceLength;
		for (int i=0; i<this.framePoints.Count; i++)
		{
			if (this.sequenceProgress < this.framePoints[i])
			{
				this.currentSprite = this.sprites[i];
				break;
			}
		}
	}

	public void Reset()
	{
		this.sequenceProgress = 0;
	}
}
