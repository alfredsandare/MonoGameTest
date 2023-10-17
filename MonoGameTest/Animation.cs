using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

public class Animation
{
	public List<string> sprites;
	public List<float> framePoints;
	double sequenceLength;
	double sequenceProgress;
	public string currentSprite;


	public Animation()
	{
		sprites = new List<string>();
		framePoints = new List<float>();
		sequenceLength = 0;
		sequenceProgress = 0;
	}

	public void Add(string sprite, float point)
	{
		sprites.Add(sprite);
		framePoints.Add(point);
		sequenceLength = framePoints.Last();
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
