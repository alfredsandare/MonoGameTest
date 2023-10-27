using System;

public class Util
{
	public Util()
	{
        
	}

    public static double Distance(int x1, int y1, int x2, int y2)
    {
        return Math.Pow(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2), 0.5);
    }
}
