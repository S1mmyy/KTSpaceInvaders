using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpaceInvaders
{
	/// <summary>
	/// Summary description for Bomb.
	/// </summary>
	public class Bomb : GameObject
	{
		public const int kBombInterval = 5;
		public int TheBombInterval = kBombInterval;

		public Bomb(int x, int y)
		{
			ImageBounds.Width = 5;
			ImageBounds.Height = 15;
			Position.X = x;
			Position.Y = y;
		}


		public override void Draw(Graphics g)
		{
			UpdateBounds();
			g.FillRectangle(Brushes.White , MovingBounds);
			Position.Y += TheBombInterval;
		}

		public void ResetBomb(int yPos)
		{
		  Position.Y = yPos;
		  TheBombInterval = kBombInterval;
		  UpdateBounds();
		}


	}
}
