using Microsoft.Xna.Framework.Input;
using System;
using System.Data;

public class InputHandler
{
	KeyboardState currentState;
	KeyboardState prevState;
	MouseState mouseState;
	MouseState prevMouseState;
	public InputHandler()
	{
		
	}

	public void Update()
	{
		prevState = currentState;
		currentState = Keyboard.GetState();
		prevMouseState = mouseState;
		mouseState = Mouse.GetState();
	}

	public bool KeyDown(Keys key)
	{
		/* Returns true if the key was just pressed */
		return (currentState.IsKeyDown(key) && !prevState.IsKeyDown(key));
	}

	public bool KeyUp(Keys key)
	{
		/* Returns true if the key was just released */
		return (currentState.IsKeyUp(key) && !prevState.IsKeyUp(key));
	}

	public bool IsPressed(Keys key)
	{
		/* Returns true if the key is pressed down currently */
		return currentState.IsKeyDown(key);
	}

	public bool MouseButtonState(string button)
	{
		switch (button)
		{
			case "LMB":
				return mouseState.LeftButton == ButtonState.Pressed;

			case "RMB":
				return mouseState.RightButton == ButtonState.Pressed;

			default:
				throw new Exception($"Invalid mouse button: {button}");
		}
	}

	public bool MouseButtonDown(string button)
	{
		switch (button)
		{
			case "LMB":
				return mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released;

			case "RMB":
				return mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released;

			default:
				throw new Exception($"Invalid mouse button: {button}");
		}
	}

    public bool MouseButtonUp(string button)
    {
        switch (button)
        {
            case "LMB":
                return mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed;

            case "RMB":
                return mouseState.RightButton == ButtonState.Released && prevMouseState.RightButton == ButtonState.Pressed;

            default:
                throw new Exception($"Invalid mouse button: {button}");
        }
    }
}
