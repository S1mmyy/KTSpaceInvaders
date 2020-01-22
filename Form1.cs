using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Data;
using System.IO;

// *********** Space Invaders ************
// Written by Mike Gold
// Copyright 2001
// ****************************************

namespace SpaceInvaders
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
    {
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		private const int kNumberOfRows = 5;
		private const int kNumberOfTries = 3;
		private const int kNumberOfShields = 4;

		private long TimerCounter = 0;
        
		private int TheSpeed = 6;

		private int TheLevel = 0;

		private bool ActiveBullet = false;

		private int NumberOfMen = 3;
		private Man TheMan = null;
		private Saucer CurrentSaucer = null;
		private bool SaucerStart = false;
		private bool GameGoing = true;
		private Bullet TheBullet = new Bullet(20, 30);
		private Score TheScore = null;
		private HighScore TheHighScore = null;
		private InvaderRow[] InvaderRows = new InvaderRow[6];
		private Shield[] Shields = new Shield[4];
		private InvaderRow TheInvaders = null;

		private int kSaucerInterval = 400;

		private string CurrentKeyDown = "";
		private string LastKeyDown = "";
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;

        private Button menuBtn;
        private Panel menuPnl;
        private Button enemyBombMidbtn;
        private Button enemyBombSlowbtn;
        private Button enemyBombFastbtn;
        private Button playerSpeedMidbtn;
        private Button playerSpeedSlowbtn;
        private Button playerSpeedFastbtn;
        private Label playerInputlbl;
        private Label enemyBombSpeedlbl;
        private Label playerBulletSpeedlbl;
        private Label playerMovementSpeedlbl;
        private Button exitBtn;
        private Thread oThread = null;
        private Button controlArrowsbtn;
        private Button controlKeysbtn;

        private Button saveUserbtn;
        private Button saveChangesbtn;
        private TextBox userTextbox;
        private Label makeUserlbl;
        private Button signInbtn;
        private TextBox signIntxtBox;
        private Label signInlbl;
        private Button button1;
        private Button button2;
        private Button button3;
        private Man player = new Man();

        [DllImport("winmm.dll")]
		public static extern long PlaySound(String lpszName, long hModule, long dwFlags);

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			// reduce flicker

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);

			InitializeAllGameObjects(true);

			timer1.Start();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		private void InitializeAllGameObjects(bool bScore)
		{
			InitializeShields();

			InitializeMan();

			if (bScore)
				InitializeScore();

			InitializeInvaderRows(TheLevel);

			CurrentSaucer = new Saucer("saucer0.gif", "saucer1.gif", "saucer2.gif");


			TheScore.GameOver = false;
			GameGoing = true;
			TheSpeed = 6;
            menuItem5.Text = "Lives: " + NumberOfMen;
		}
        

		private void InitializeSaucer()
		{
		    CurrentSaucer.Reset();
		    SaucerStart = true;
		}

		private void InitializeMan()
		{
			TheMan = new Man();
			TheMan.Position.Y = ClientRectangle.Bottom - 50;
			NumberOfMen = 3;
		}

		private void InitializeScore()
		{
			TheScore = new Score(ClientRectangle.Right - 400, 50);
			TheHighScore = new HighScore(ClientRectangle.Left + 25, 50);
			TheHighScore.Read();
		}

		private void InitializeShields()
		{
			for (int i = 0; i < kNumberOfShields; i++)
			{
			    Shields[i] = new Shield();
			    Shields[i].UpdateBounds();
			    Shields[i].Position.X = (Shields[i].GetBounds().Width + 75) * i + 25;
			    Shields[i].Position.Y = ClientRectangle.Bottom - (Shields[i].GetBounds().Height + 75);
			}
		}

		void InitializeInvaderRows(int level)
		{
            InvaderRows[0] = new InvaderRow("invader1.gif", "invader1c.gif", 2 + level);
		    InvaderRows[1] = new InvaderRow("invader2.gif", "invader2c.gif", 3 + level);
		    InvaderRows[2] = new InvaderRow("invader2.gif", "invader2c.gif", 4 + level);
		    InvaderRows[3] = new InvaderRow("invader3.gif", "invader3c.gif", 5 + level);
		    InvaderRows[4] = new InvaderRow("invader3.gif", "invader3c.gif", 6 + level);
		}

		private string m_strCurrentSoundFile = "1.wav";
		public void PlayASound()
		{
			if (m_strCurrentSoundFile.Length > 0)
			{
				PlaySound(Application.StartupPath + "\\" + m_strCurrentSoundFile, 0, 0);
			}
			m_strCurrentSoundFile = "";
			m_nCurrentPriority = 3;
			oThread.Abort();
		}

		int m_nCurrentPriority = 3;
		public void PlaySoundInThread(string wavefile, int priority)
		{
			if (priority <= m_nCurrentPriority)
			{
				m_nCurrentPriority = priority;
				if (oThread != null)
					oThread.Abort();

				m_strCurrentSoundFile = wavefile;
				oThread = new Thread(new ThreadStart(PlayASound));
				oThread.Start();
			}
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();

            this.menuBtn = new System.Windows.Forms.Button();
            this.menuPnl = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.signInbtn = new System.Windows.Forms.Button();
            this.signIntxtBox = new System.Windows.Forms.TextBox();
            this.signInlbl = new System.Windows.Forms.Label();
            this.saveUserbtn = new System.Windows.Forms.Button();
            this.saveChangesbtn = new System.Windows.Forms.Button();
            this.userTextbox = new System.Windows.Forms.TextBox();
            this.makeUserlbl = new System.Windows.Forms.Label();
            this.controlArrowsbtn = new System.Windows.Forms.Button();
            this.controlKeysbtn = new System.Windows.Forms.Button();
            this.exitBtn = new System.Windows.Forms.Button();

            this.enemyBombMidbtn = new System.Windows.Forms.Button();
            this.enemyBombSlowbtn = new System.Windows.Forms.Button();
            this.enemyBombFastbtn = new System.Windows.Forms.Button();

            this.playerSpeedMidbtn = new System.Windows.Forms.Button();
            this.playerSpeedSlowbtn = new System.Windows.Forms.Button();
            this.playerSpeedFastbtn = new System.Windows.Forms.Button();
            this.playerInputlbl = new System.Windows.Forms.Label();
            this.enemyBombSpeedlbl = new System.Windows.Forms.Label();
            this.playerBulletSpeedlbl = new System.Windows.Forms.Label();
            this.playerMovementSpeedlbl = new System.Windows.Forms.Label();
            this.menuPnl.SuspendLayout();
            this.SuspendLayout();
            //
            // timer1
            //
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // mainMenu1
            //
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem5});
            //
            // menuItem1
            //
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.menuItem3,
            this.menuItem4});
            this.menuItem1.Text = "File";
            //
            // menuItem2
            //
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "Restart...";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            //
            // menuItem3
            //
            this.menuItem3.Index = 2;
            this.menuItem3.Text = "Exit";
            this.menuItem3.Click += new System.EventHandler(this.Menu_Exit);
            //
            // menuItem4
            //
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "Pause/Unpause";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            //
            // menuItem5
            //
            this.menuItem5.Index = 1;
            this.menuItem5.Text = "Lives: ";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            //
            // menuBtn
            //
            this.menuBtn.Location = new System.Drawing.Point(585, 12);
            this.menuBtn.Name = "menuBtn";
            this.menuBtn.Size = new System.Drawing.Size(75, 23);
            this.menuBtn.TabIndex = 0;
            this.menuBtn.Text = "Menu";
            this.menuBtn.UseVisualStyleBackColor = true;
            this.menuBtn.Click += new System.EventHandler(this.menuBtn_Click);
            //
            // menuPnl
            //
            this.menuPnl.Controls.Add(this.button1);
            this.menuPnl.Controls.Add(this.button2);
            this.menuPnl.Controls.Add(this.button3);
            this.menuPnl.Controls.Add(this.signInbtn);
            this.menuPnl.Controls.Add(this.signIntxtBox);
            this.menuPnl.Controls.Add(this.signInlbl);
            this.menuPnl.Controls.Add(this.saveUserbtn);
            this.menuPnl.Controls.Add(this.saveChangesbtn);
            this.menuPnl.Controls.Add(this.userTextbox);
            this.menuPnl.Controls.Add(this.makeUserlbl);

            this.menuPnl.Controls.Add(this.controlArrowsbtn);
            this.menuPnl.Controls.Add(this.controlKeysbtn);
            this.menuPnl.Controls.Add(this.exitBtn);
            this.menuPnl.Controls.Add(this.enemyBombMidbtn);
            this.menuPnl.Controls.Add(this.enemyBombSlowbtn);
            this.menuPnl.Controls.Add(this.enemyBombFastbtn);
            this.menuPnl.Controls.Add(this.playerSpeedMidbtn);
            this.menuPnl.Controls.Add(this.playerSpeedSlowbtn);
            this.menuPnl.Controls.Add(this.playerSpeedFastbtn);
            this.menuPnl.Controls.Add(this.playerInputlbl);
            this.menuPnl.Controls.Add(this.enemyBombSpeedlbl);
            this.menuPnl.Controls.Add(this.playerBulletSpeedlbl);
            this.menuPnl.Controls.Add(this.playerMovementSpeedlbl);
            this.menuPnl.Location = new System.Drawing.Point(155, 121);
            this.menuPnl.Name = "menuPnl";
            this.menuPnl.Size = new System.Drawing.Size(412, 429);
            this.menuPnl.TabIndex = 1;
            this.menuPnl.Visible = false;
            //
            // button1
            //
            this.button1.Location = new System.Drawing.Point(126, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "Medium";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            //
            // button2
            //
            this.button2.Location = new System.Drawing.Point(126, 114);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Slow";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            //
            // button3
            //
            this.button3.Location = new System.Drawing.Point(126, 56);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 18;
            this.button3.Text = "Fast";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            //
            // signInbtn
            //
            this.signInbtn.Location = new System.Drawing.Point(203, 389);
            this.signInbtn.Name = "SignInbtn";
            this.signInbtn.Size = new System.Drawing.Size(123, 23);
            this.signInbtn.TabIndex = 16;
            this.signInbtn.Text = "Sign In";
            this.signInbtn.UseVisualStyleBackColor = true;
            this.signInbtn.Click += new System.EventHandler(this.signInbtn_Click);
            //
            // signIntxtBox
            //
            this.signIntxtBox.Location = new System.Drawing.Point(203, 363);
            this.signIntxtBox.Name = "SignIntxtBox";
            this.signIntxtBox.Size = new System.Drawing.Size(123, 20);
            this.signIntxtBox.TabIndex = 15;
            //
            // signInlbl
            //
            this.signInlbl.AutoSize = true;
            this.signInlbl.Location = new System.Drawing.Point(229, 347);
            this.signInlbl.Name = "signInlbl";
            this.signInlbl.Size = new System.Drawing.Size(43, 13);
            this.signInlbl.TabIndex = 14;
            this.signInlbl.Text = "Sign In:";
            //
            // saveUserbtn
            //
            this.saveUserbtn.Location = new System.Drawing.Point(46, 389);
            this.saveUserbtn.Name = "saveUserbtn";
            this.saveUserbtn.Size = new System.Drawing.Size(138, 23);
            this.saveUserbtn.TabIndex = 13;
            this.saveUserbtn.Text = "Save";
            this.saveUserbtn.UseVisualStyleBackColor = true;
            this.saveUserbtn.Click += new System.EventHandler(this.saveUserbtn_Click);
            //
            // saveChangesbtn
            //
            this.saveChangesbtn.Location = new System.Drawing.Point(15, 237);
            this.saveChangesbtn.Name = "saveChangesbtn";
            this.saveChangesbtn.Size = new System.Drawing.Size(311, 86);
            this.saveChangesbtn.TabIndex = 21;
            this.saveChangesbtn.Text = "Save All Changes";
            this.saveChangesbtn.UseVisualStyleBackColor = true;
            this.saveChangesbtn.Click += new System.EventHandler(this.saveChangesbtn_Click);
            //
            // usertxtbox
            //
            this.userTextbox.Location = new System.Drawing.Point(46, 363);
            this.userTextbox.Name = "usertxtbox";
            this.userTextbox.Size = new System.Drawing.Size(138, 20);
            this.userTextbox.TabIndex = 12;
            //
            // makeUserlbl
            //
            this.makeUserlbl.AutoSize = true;
            this.makeUserlbl.Location = new System.Drawing.Point(46, 347);
            this.makeUserlbl.Name = "makeUserlbl";
            this.makeUserlbl.Size = new System.Drawing.Size(85, 13);
            this.makeUserlbl.TabIndex = 11;
            this.makeUserlbl.Text = "Save New User:";
            //
            // playerBulletSpeedlbl
            //
            this.playerBulletSpeedlbl.AutoSize = true;
            this.playerBulletSpeedlbl.Location = new System.Drawing.Point(123, 10);
            this.playerBulletSpeedlbl.Name = "PlayerBulletSpeedlbl";
            this.playerBulletSpeedlbl.Size = new System.Drawing.Size(67, 13);
            this.playerBulletSpeedlbl.TabIndex = 17;
            this.playerBulletSpeedlbl.Text = "Bullet Speed";
            //
            // enemyBombSpeedlbl
            //
            this.enemyBombSpeedlbl.AutoSize = true;
            this.enemyBombSpeedlbl.Location = new System.Drawing.Point(117, 13);
            this.enemyBombSpeedlbl.Name = "enemyBombSpeedlbl";
            this.enemyBombSpeedlbl.Size = new System.Drawing.Size(103, 13);
            this.enemyBombSpeedlbl.TabIndex = 1;
            this.enemyBombSpeedlbl.Text = "Enemy Bomb Speed";
            //
            // playerInputlbl
            //
            this.playerInputlbl.AutoSize = true;
            this.playerInputlbl.Location = new System.Drawing.Point(12, 153);
            this.playerInputlbl.Name = "playerInputlbl";
            this.playerInputlbl.Size = new System.Drawing.Size(119, 13);
            this.playerInputlbl.TabIndex = 2;
            this.playerInputlbl.Text = "Change the player input";
            //
            // playerSpeedFastbtn
            //
            this.playerSpeedFastbtn.Location = new System.Drawing.Point(15, 56);
            this.playerSpeedFastbtn.Name = "playerSpeedFastbtn";
            this.playerSpeedFastbtn.Size = new System.Drawing.Size(75, 23);
            this.playerSpeedFastbtn.TabIndex = 2;
            this.playerSpeedFastbtn.Text = "Fast";
            this.playerSpeedFastbtn.UseVisualStyleBackColor = true;
            this.playerSpeedFastbtn.Click += new System.EventHandler(this.playerSpeedFastbtn_Click);
            //
            // playerSpeedSlowbtn
            //
            this.playerSpeedSlowbtn.Location = new System.Drawing.Point(15, 114);
            this.playerSpeedSlowbtn.Name = "playerSpeedSlowbtn";
            this.playerSpeedSlowbtn.Size = new System.Drawing.Size(75, 23);
            this.playerSpeedSlowbtn.TabIndex = 3;
            this.playerSpeedSlowbtn.Text = "Slow";
            this.playerSpeedSlowbtn.UseVisualStyleBackColor = true;
            this.playerSpeedSlowbtn.Click += new System.EventHandler(this.playerSpeedSlowbtn_Click);
            //
            // playerSpeedMidbtn
            //
            this.playerSpeedMidbtn.Location = new System.Drawing.Point(15, 85);
            this.playerSpeedMidbtn.Name = "playerSpeedMidbtn";
            this.playerSpeedMidbtn.Size = new System.Drawing.Size(75, 23);
            this.playerSpeedMidbtn.TabIndex = 4;
            this.playerSpeedMidbtn.Text = "Medium";
            this.playerSpeedMidbtn.UseVisualStyleBackColor = true;
            this.playerSpeedMidbtn.Click += new System.EventHandler(this.playerSpeedMidbtn_Click);
            //
            // enemyBombMidbtn
            //
            this.enemyBombMidbtn.Location = new System.Drawing.Point(228, 85);
            this.enemyBombMidbtn.Name = "enemyBombMidbtn";
            this.enemyBombMidbtn.Size = new System.Drawing.Size(75, 23);
            this.enemyBombMidbtn.TabIndex = 7;
            this.enemyBombMidbtn.Text = "Medium";
            this.enemyBombMidbtn.UseVisualStyleBackColor = true;
            //
            // enemyBombSlowbtn
            //
            this.enemyBombSlowbtn.Location = new System.Drawing.Point(228, 114);
            this.enemyBombSlowbtn.Name = "enemyBombSlowbtn";
            this.enemyBombSlowbtn.Size = new System.Drawing.Size(75, 23);
            this.enemyBombSlowbtn.TabIndex = 6;
            this.enemyBombSlowbtn.Text = "Slow";
            this.enemyBombSlowbtn.UseVisualStyleBackColor = true;
            //
            // enemyBombFastbtn
            //
            this.enemyBombFastbtn.Location = new System.Drawing.Point(228, 56);
            this.enemyBombFastbtn.Name = "enemyBombFastbtn";
            this.enemyBombFastbtn.Size = new System.Drawing.Size(75, 23);
            this.enemyBombFastbtn.TabIndex = 5;
            this.enemyBombFastbtn.Text = "Fast";
            this.enemyBombFastbtn.UseVisualStyleBackColor = true;
            //
            // playerInputlbl
            //
            this.playerInputlbl.AutoSize = true;
            this.playerInputlbl.Location = new System.Drawing.Point(103, 153);
            this.playerInputlbl.Name = "playerInputlbl";
            this.playerInputlbl.Size = new System.Drawing.Size(119, 13);
            this.playerInputlbl.TabIndex = 2;
            this.playerInputlbl.Text = "Change the player input";
            //
            // enemyBombSpeedlbl
            //
            this.enemyBombSpeedlbl.AutoSize = true;
            this.enemyBombSpeedlbl.Location = new System.Drawing.Point(225, 10);
            this.enemyBombSpeedlbl.Name = "enemyBombSpeedlbl";
            this.enemyBombSpeedlbl.Size = new System.Drawing.Size(103, 13);
            this.enemyBombSpeedlbl.TabIndex = 1;
            this.enemyBombSpeedlbl.Text = "Enemy Bomb Speed";
            //
            // playerMovementSpeedlbl
            //
            this.playerMovementSpeedlbl.AutoSize = true;
            this.playerMovementSpeedlbl.Location = new System.Drawing.Point(3, 10);
            this.playerMovementSpeedlbl.Name = "playerMovementSpeedlbl";
            this.playerMovementSpeedlbl.Size = new System.Drawing.Size(91, 13);
            this.playerMovementSpeedlbl.TabIndex = 0;
            this.playerMovementSpeedlbl.Text = "Movement Speed";
            //
            // exitBtn
            //
            this.exitBtn.Location = new System.Drawing.Point(334, 10);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(75, 313);
            this.exitBtn.TabIndex = 8;
            this.exitBtn.Text = "Exit";
            this.exitBtn.UseVisualStyleBackColor = true;
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            //
            // controlArrowsbtn
            //
            this.controlArrowsbtn.Location = new System.Drawing.Point(15, 169);
            this.controlArrowsbtn.Name = "controlArrowsbtn";
            this.controlArrowsbtn.Size = new System.Drawing.Size(316, 23);
            this.controlArrowsbtn.TabIndex = 10;
            this.controlArrowsbtn.Text = "left arrow, right arrow, spacebar";
            this.controlArrowsbtn.UseVisualStyleBackColor = true;
            //
            // controlKeysbtn
            //
            this.controlKeysbtn.Location = new System.Drawing.Point(10, 198);
            this.controlKeysbtn.Name = "controlKeysbtn";
            this.controlKeysbtn.Size = new System.Drawing.Size(316, 23);
            this.controlKeysbtn.TabIndex = 9;
            this.controlKeysbtn.Text = "\'a\', \'s\', \'w\'";
            this.controlKeysbtn.UseVisualStyleBackColor = true;
            //
            // Form1
            //
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(672, 622);
            this.Controls.Add(this.menuPnl);
            this.Controls.Add(this.menuBtn);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Space Invaders Game";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.menuPnl.ResumeLayout(false);
            this.menuPnl.PerformLayout();
            this.ResumeLayout(false);
        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void HandleKeys()
		{
			switch (CurrentKeyDown)
			{
				case "Space":
					if (ActiveBullet == false)
					{
						TheBullet.Position = TheMan.GetBulletStart();
						ActiveBullet = true;
						PlaySoundInThread("1.wav", 2);
					}
					CurrentKeyDown = LastKeyDown;
					break;
				case "Left":
					TheMan.MoveLeft();
					Invalidate(TheMan.GetBounds());
					if (timer1.Enabled == false)
						timer1.Start();
					break;
				case "Right":
					TheMan.MoveRight(ClientRectangle.Right);
					Invalidate(TheMan.GetBounds());
					if (timer1.Enabled == false)
						timer1.Start();
					break;

                case "W":
                    if (ActiveBullet == false)
                    {
                        TheBullet.Position = TheMan.GetBulletStart();
                        ActiveBullet = true;
                        PlaySoundInThread("1.wav", 2);
                    }
                    CurrentKeyDown = LastKeyDown;
                    break;
                case "A":
                    TheMan.MoveLeft();
                    Invalidate(TheMan.GetBounds());
                    if (timer1.Enabled == false)
                        timer1.Start();
                    break;
                case "D":
                    TheMan.MoveRight(ClientRectangle.Right);
                    Invalidate(TheMan.GetBounds());
                    if (timer1.Enabled == false)
                        timer1.Start();
                    break;
                default:
                    break;
			}
		}

		private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{  
			string result = e.KeyData.ToString();
			CurrentKeyDown = result;
			if (result == "Left"  || result == "Right")
			{
			    LastKeyDown = result;
			}

			//Invalidate(TheMan.GetBounds());
		}

		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			for (int i = 0; i < kNumberOfShields; i++)
			{
				Shields[i].Draw(g);
			}

			//g.FillRectangle(Brushes.Black, 0, 0, ClientRectangle.Width, ClientRectangle.Height);
			TheMan.Draw(g);
			TheScore.Draw(g);
			TheHighScore.Draw(g);
			if (ActiveBullet)
			{
			    TheBullet.Draw(g);
			}

			if (SaucerStart)
			{
			    CurrentSaucer.Draw(g);
			}

			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				TheInvaders.Draw(g);
			}
		}

		private int CalculateLargestLastPosition()
		{
			int max = 0;
			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				int nLastPos = TheInvaders.GetLastInvader().Position.X;
				if (nLastPos > max)
					max = nLastPos;
			}

			return max;
		}

		private int CalculateSmallestFirstPosition()
		{
			int min = 50000;

			try
			{
				for (int i = 0; i < kNumberOfRows; i++)
				{
					TheInvaders = InvaderRows[i];
					int nFirstPos = TheInvaders.GetFirstInvader().Position.X;
					if (nFirstPos < min)
						min = nFirstPos;
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message.ToString());
			}

			return min;
		}

		private void MoveInvaders()
		{
			bool bMoveDown = false;

			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				TheInvaders.Move();
			}

            //if (InvaderSoundCounter % 5)
				//PlaySoundInThread("4.wav", 3);

			if ((CalculateLargestLastPosition()) > ClientRectangle.Width - InvaderRows[4][0].GetWidth())
			{
				TheInvaders.DirectionRight = false;
				SetAllDirections(false);
			}

			if ((CalculateSmallestFirstPosition()) < InvaderRows[4][0].Width/3) 
			{
				TheInvaders.DirectionRight = true;
				SetAllDirections(true);
				for (int i = 0; i < kNumberOfRows; i++)
				{
					bMoveDown = true;
				}
			}

			if (bMoveDown)
			{
				for (int i = 0; i < kNumberOfRows; i++)
				{
					TheInvaders = InvaderRows[i];
					TheInvaders.MoveDown();
				}
			}
		}

		private int TotalNumberOfInvaders()
		{
		  int sum = 0;
			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				sum += TheInvaders.NumberOfLiveInvaders();
			}

			return sum;
		}

		private void MoveInvadersInPlace()
		{
			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				TheInvaders.MoveInPlace();
			}
		}

		private void SetAllDirections(bool bDirection)
		{
			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				TheInvaders.DirectionRight = bDirection;
			}
		}

		public int CalcScoreFromRow(int num)
		{
			int nScore = 10;
			switch (num)
			{
				case 0:
					nScore = 30;
					break;
				case 1:
					nScore = 20;
					break;
				case 2:
					nScore = 20;
					break;
				case 3:
					nScore = 10;
					break;
				case 4:
					nScore = 10;
					break;
			}

			return nScore;
		}

		void TestBulletCollision()
		{
			int rowNum = 0;
			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
			    rowNum = i;
				int collisionIndex = TheInvaders.CollisionTest(TheBullet.GetBounds());
			
				if ((collisionIndex >= 0) && ActiveBullet)
				{
					TheInvaders.Invaders[collisionIndex].BeenHit = true;
					TheScore.AddScore(CalcScoreFromRow(rowNum));
					PlaySoundInThread("0.wav", 1);
					
					ActiveBullet = false;
					TheBullet.Reset();
				}

				if (SaucerStart && ActiveBullet && CurrentSaucer.GetBounds().IntersectsWith(TheBullet.GetBounds()))
				{
				   CurrentSaucer.BeenHit = true;
					if (CurrentSaucer.ScoreCalculated == false)
					{
						TheScore.AddScore(CurrentSaucer.CalculateScore());
						CurrentSaucer.ScoreCalculated = true;
						PlaySoundInThread("3.wav", 1);
					}
				}
			}
		}

		void TestForLanding()
		{
			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				if (TheInvaders.AlienHasLanded(ClientRectangle.Bottom))
				{
					TheMan.BeenHit = true;
					PlaySoundInThread("2.wav", 1);
					TheScore.GameOver = true;
					TheHighScore.Write(TheScore.Count);
					GameGoing = false;
				}
			}
		}

		void ResetAllBombCounters()
		{
			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				TheInvaders.ResetBombCounters();
			}
		}

		void TestBombCollision()
		{
			if (TheMan.Died)
			{
			    NumberOfMen --;
                menuItem5.Text = "Lives = " + NumberOfMen;
                if (NumberOfMen == 0)
				{
					TheHighScore.Write(TheScore.Count);
					TheScore.GameOver = true;
					GameGoing = false;
				}
				else
				{
				    TheMan.Reset();
				    ResetAllBombCounters();
				}
			}

			if (TheMan.BeenHit == true)
				return;

			for (int i = 0; i < kNumberOfRows; i++)
			{
				TheInvaders = InvaderRows[i];
				for (int j = 0; j < TheInvaders.Invaders.Length; j++)
				{
			        for (int k = 0; k < kNumberOfShields; k++)
					{
                        if (Shields[k].TestCollision(TheInvaders.Invaders[j].GetBombBounds(), true, out bool bulletHole))
                        {
                            TheInvaders.Invaders[j].ResetBombPosition();
                            Invalidate(Shields[k].GetBounds());
                        }
                        
                        if (Shields[k].TestCollision(TheBullet.GetBounds(), false, out bulletHole))
						{
							ActiveBullet = false;
							Invalidate(Shields[k].GetBounds());
							TheBullet.Reset();
						}
					}
				
					if (TheInvaders.Invaders[j].IsBombColliding(TheMan.GetBounds()) )
					{
					    TheMan.BeenHit = true;
					    PlaySoundInThread("2.wav", 1);
					}
				}
			}
		}


		private int nTotalInvaders = 0;
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			HandleKeys();

			TimerCounter++;

			if (GameGoing == false)
			{
				if (TimerCounter % 6 == 0)
					MoveInvadersInPlace();
				Invalidate();
				return;
			}


			if (TheBullet.Position.Y < 0)
			{
			    ActiveBullet = false;
			}

			if (TimerCounter % kSaucerInterval == 0)
			{
				InitializeSaucer();
				PlaySoundInThread("8.wav", 1);
				SaucerStart = true;
			}

			if (SaucerStart == true)
			{
				CurrentSaucer.Move();
				if (CurrentSaucer.GetBounds().Left > ClientRectangle.Right)
				{
				    SaucerStart = false;
				}
			}


			if (TimerCounter % TheSpeed == 0)
			{ 
				MoveInvaders();

				nTotalInvaders = TotalNumberOfInvaders();

				if (nTotalInvaders <= 20)
				{
					TheSpeed = 5;
				}

				if (nTotalInvaders <= 10)
				{
					TheSpeed = 4;
				}

				if (nTotalInvaders <= 5)
				{
					TheSpeed = 3;
				}

				if (nTotalInvaders <= 3)
				{
					TheSpeed = 2;
				}

				if (nTotalInvaders <= 1 )
				{
					TheSpeed = 1;
				}

				if (nTotalInvaders == 0)
				{
				    InitializeAllGameObjects(false); // don't initialize score					
				    TheLevel++;
				}
			}

			TestBulletCollision();
			TestBombCollision();

			Invalidate();
			// move invaders

			// move bullets
		}

        private void Form1_Load(object sender, EventArgs e)
        {

        }

		private void Form1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
//			string result = e.KeyChar.ToString();
//			Invalidate(TheMan.GetBounds());
		}

		private void Form1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			string result = e.KeyData.ToString();
			if (result == "Left"  || result == "Right")
			{
				LastKeyDown = "";
			}
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			this.InitializeAllGameObjects(true);
			TheLevel = 0;
		}

        //pause and unpause
        private void menuItem4_Click(object sender, EventArgs e)
        {
            if (GameGoing == false && kNumberOfTries > 0)
            {
                GameGoing = true;
            }
            else if (GameGoing == true && kNumberOfTries > 0)
            {
                GameGoing = false;
            }
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {

        }

        private void Menu_Exit(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

        private void menuBtn_Click(object sender, EventArgs e)
        {
            menuPnl.Visible = true;
        }

        private void playerSpeedFastbtn_Click(object sender, EventArgs e)
        {
            player.setInterval(15);
        }

        private void playerSpeedMidbtn_Click(object sender, EventArgs e)
        {
            player.setInterval(10);
        }

        private void playerSpeedSlowbtn_Click(object sender, EventArgs e)
        {
            player.setInterval(5);
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            menuPnl.Visible = false;
        }

        private void saveUserbtn_Click(object sender, EventArgs e)
        {
            /*
            The data in this order:
            1) Player Info:
                player speed
                player bullet speed
            2) enemy Info:
                enemy speed
                enemy bomb speed
            3) User Info:
                players input
                players live
                players level
             */
            using (StreamWriter writer = new StreamWriter(userTextbox.Text))
            {
                writer.WriteLine();
            }
            userTextbox.Text = "";
        }

        private void saveChangesbtn_Click(object sender, EventArgs e)
        {

        }

        private void signInbtn_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
