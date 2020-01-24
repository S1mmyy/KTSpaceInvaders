using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace SpaceInvaders
{
	/// <summary>
	/// Summary description for HighScore.
	/// </summary>
	public class HighScore : Score
	{
        private UserAttributes person;
        private List<int> topScores;
		public HighScore(int x, int y) : base(x, y)
		{
            //
            // TODO: Add constructor logic here
            //
            person = new UserAttributes();
            topScores = new List<int>();
        }

		public override void Draw(Graphics g)
		{
		    g.DrawString("High Score: " + Count.ToString(), MyFont, Brushes.RoyalBlue, Position.X, Position.Y, new StringFormat());
		}

		public void Write(int theScore)
		{
            DateTime value = DateTime.Today;
            bool go = false;
            Read();
            for(int i = 0; i < topScores.Count; i++)
            {
                if (topScores[i] < theScore)
                {
                    go = true;
                    break;
                }
            }
            if (topScores.Count > 5)
            {
                topScores.RemoveAt(0);
            }
            if (go)
			{
				Count = theScore;
                using (StreamWriter sw = new StreamWriter("highscore.txt", true))
                {
                    sw.WriteLine(Count.ToString() + " " + person.Name + " " + value);
                    sw.Close();
                }
			}
            
            
        }

		public void Read()
		{
		    if (File.Exists("highscore.txt"))
		    {
			    StreamReader sr = new StreamReader("highscore.txt");
			    string score = sr.ReadLine();
			    Count = Convert.ToInt32(score);
			    sr.Close();
                makeList();
            }        
        }

        private void makeList()
        {
            using(StreamReader reader = new StreamReader("highscore.txt")){
                string line = reader.ReadLine();
                string num = "";
                int i = 0;

                while (line != null)
                {
                    
                    for(i = 0; i < line.Length; i++)
                    {
                        if(line[i] == ' ')
                        {
                            break;
                        }
                        else
                        {
                            num += line[i];
                        }
                    }
                    if(topScores.Count == 0)
                    {
                        topScores.Add(Convert.ToInt32(num));
                    }
                    else
                    {
                        for (i = 0; i < topScores.Count; i++)
                        {
                            if(topScores[i] > Convert.ToInt32(num))
                            {
                                topScores.Insert(i, Convert.ToInt32(num));
                            }
                        }
                        if (topScores[i-1] < Convert.ToInt32(num))
                        {
                            topScores.Add(Convert.ToInt32(num));
                        }
                    }
                    line = reader.ReadLine();
                    num = "";
                    //topScores.Add();
                }
                reader.Close();
            }
        }
	}
}
