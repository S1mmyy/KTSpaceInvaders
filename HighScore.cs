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

        public HighScore(int x, int y, UserAttributes user) : base(x, y)
        {
            person = user;
            topScores = new List<int>();
        }

        public override void Draw(Graphics g)
        {
            g.DrawString("High Score: " + Count.ToString(), MyFont, Brushes.RoyalBlue, Position.X, Position.Y, new StringFormat());
        }

        public void Write(int theScore)
        {
            DateTime value = DateTime.Now;
            bool go = false;
            MakeList(theScore);
            string temp = "";

            if (topScores.Count < 5)
            {
                go = true;
            }
            else
            {
                temp = topScores[topScores.Count - 1] + "";
                topScores.RemoveAt(topScores.Count - 1);
                Delete(temp);
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

        private void Delete(string remove)
        {
            string tempFile = "tempFile.txt";
            bool once = true;

            using (var sr = new StreamReader("highscore.txt"))
            using (var sw = new StreamWriter(tempFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (once && line.Substring(0, remove.Length).Equals(remove))
                    {
                        once = false;
                    }
                    else
                    {
                        sw.WriteLine(line);
                    }

                }
            }

            File.Delete("highscore.txt");
            File.Move(tempFile, "highscore.txt");
        }

        public void Read()
        {
            if (File.Exists("highscore.txt"))
            {
                MakeList();
                StreamReader sr = new StreamReader("highscore.txt");
                string score = sr.ReadLine();
                Count = Convert.ToInt32(topScores[0]);
                sr.Close();
            }
        }

        private void MakeList(int num)
        {
            topScores.Add(num);
            MakeList();
        }

        private void MakeList()
        {
            using (StreamReader reader = new StreamReader("highscore.txt"))
            {
                string line = reader.ReadLine();
                string num = "";
                int i = 0;

                while (line != null)
                {

                    for (i = 0; i < line.Length; i++)
                    {
                        if (line[i] == ' ')
                        {
                            break;
                        }
                        else
                        {
                            num += line[i];
                        }
                    }

                    if (topScores.Count == 0)
                    {
                        topScores.Add(Convert.ToInt32(num));
                    }
                    else
                    {
                        SortList(topScores.Count);
                    }
                    line = reader.ReadLine();
                    num = "";
                }
                reader.Close();
            }
        }

        private void SortList(int x)
        {
            int temp = 0;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    if (topScores[i] > topScores[j])
                    {
                        temp = topScores[i];
                        topScores[i] = topScores[j];
                        topScores[j] = temp;
                    }
                }
            }
        }
    }
}