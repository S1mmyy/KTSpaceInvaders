using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml.Serialization;

// *********** Space Invaders ************
// Written by Mike Gold
// Copyright 2001
// ****************************************

namespace SpaceInvaders
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : Form
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
        private bool GameGoing = false;
        private Bullet TheBullet = new Bullet(20, 30);
        private Bomb TheBomb = new Bomb(20, 30);
        private Score TheScore = null;
        private HighScore TheHighScore = null;
        private InvaderRow[] InvaderRows = new InvaderRow[6];
        private Shield[] Shields = new Shield[4];
        private InvaderRow TheInvaders = null;
        private bool useArrows = true;

        XmlSerializer serializer = new XmlSerializer(typeof(List<UserAttributes>));
        //sets the settings to the default values
        private UserAttributes currentSettings = new UserAttributes { Name = "", BombSpeed = 5, BulletSpeed = 20, MovementSpeed = 5, UseArrows = true };
        //a list that can have the current profiles added to it
        private List<UserAttributes> savedProfiles = new List<UserAttributes>();

		private int kSaucerInterval = 400;

		private string CurrentKeyDown = "";
		private string LastKeyDown = "";
        private MainMenu mainMenu1;
        private MenuItem menuItem1;
        private MenuItem menuItem2;
        private MenuItem menuItem3;
        private MenuItem menuItem4;
        private MenuItem menuItem5;

        private Button menuBtn;
        private Panel menuPnl;
        private Button enemyBombSlowbtn;
        private Button enemyBombFastbtn;
        private Button playerSpeedMidbtn;
        private Button playerSpeedSlowbtn;
        private Button playerSpeedFastbtn;
        private Button bulletSpeedMidbtn;
        private Button bulletSpeedSlowbtn;
        private Button bulletSpeedFastbtn;
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
        private TextBox signInTextBox;
        private Label signInlbl;
        private Label userMsglbl;

        [DllImport("winmm.dll")]
		public static extern long PlaySound(String lpszName, long hModule, long dwFlags);

		public Form1()
		{
            //gets all currently saved profiles
            using (StreamReader reader = new StreamReader("userprofiles.xml"))
            {
                savedProfiles = serializer.Deserialize(reader) as List<UserAttributes>;
            }
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
            TheMan.Interval = 5;
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
            InvaderRows[0] = new InvaderRow("invader1.gif", "invader1c.gif", 2 + level, level);
		    InvaderRows[1] = new InvaderRow("invader2.gif", "invader2c.gif", 3 + level, level);
		    InvaderRows[2] = new InvaderRow("invader2.gif", "invader2c.gif", 4 + level, level);
		    InvaderRows[3] = new InvaderRow("invader3.gif", "invader3c.gif", 5 + level, level);
		    InvaderRows[4] = new InvaderRow("invader3.gif", "invader3c.gif", 6 + level, level);
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
            this.userMsglbl = new System.Windows.Forms.Label();
            this.signInbtn = new System.Windows.Forms.Button();
            this.signInTextBox = new System.Windows.Forms.TextBox();
            this.signInlbl = new System.Windows.Forms.Label();
            this.saveUserbtn = new System.Windows.Forms.Button();
            this.saveChangesbtn = new System.Windows.Forms.Button();
            this.userTextbox = new System.Windows.Forms.TextBox();
            this.makeUserlbl = new System.Windows.Forms.Label();
            this.controlArrowsbtn = new System.Windows.Forms.Button();
            this.controlKeysbtn = new System.Windows.Forms.Button();
            this.exitBtn = new System.Windows.Forms.Button();
            this.bulletSpeedMidbtn = new System.Windows.Forms.Button();
            this.bulletSpeedSlowbtn = new System.Windows.Forms.Button();
            this.bulletSpeedFastbtn = new System.Windows.Forms.Button();
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
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "Restart...";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "Exit";
            this.menuItem3.Click += new System.EventHandler(this.Menu_Exit);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "Pause/Unpause";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.Text = "Lives: ";
            // 
            // menuBtn
            // 
            this.menuBtn.Location = new System.Drawing.Point(585, 12);
            this.menuBtn.Name = "menuBtn";
            this.menuBtn.Size = new System.Drawing.Size(75, 23);
            this.menuBtn.TabIndex = 0;
            this.menuBtn.TabStop = false;
            this.menuBtn.Text = "Menu";
            this.menuBtn.UseVisualStyleBackColor = true;
            this.menuBtn.Visible = false;
            this.menuBtn.Click += new System.EventHandler(this.menuBtn_Click);
            // 
            // menuPnl
            // 
            this.menuPnl.Controls.Add(this.userMsglbl);
            this.menuPnl.Controls.Add(this.signInbtn);
            this.menuPnl.Controls.Add(this.signInTextBox);
            this.menuPnl.Controls.Add(this.signInlbl);
            this.menuPnl.Controls.Add(this.saveUserbtn);
            this.menuPnl.Controls.Add(this.saveChangesbtn);
            this.menuPnl.Controls.Add(this.userTextbox);
            this.menuPnl.Controls.Add(this.makeUserlbl);
            this.menuPnl.Controls.Add(this.controlArrowsbtn);
            this.menuPnl.Controls.Add(this.controlKeysbtn);
            this.menuPnl.Controls.Add(this.exitBtn);
            this.menuPnl.Controls.Add(this.bulletSpeedMidbtn);
            this.menuPnl.Controls.Add(this.bulletSpeedSlowbtn);
            this.menuPnl.Controls.Add(this.bulletSpeedFastbtn);
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
            // 
            // userMsglbl
            // 
            this.userMsglbl.AutoSize = true;
            this.userMsglbl.Location = new System.Drawing.Point(332, 379);
            this.userMsglbl.Name = "userMsglbl";
            this.userMsglbl.Size = new System.Drawing.Size(0, 13);
            this.userMsglbl.TabIndex = 22;
            // 
            // signInbtn
            // 
            this.signInbtn.Location = new System.Drawing.Point(203, 389);
            this.signInbtn.Name = "signInbtn";
            this.signInbtn.Size = new System.Drawing.Size(123, 23);
            this.signInbtn.TabIndex = 16;
            this.signInbtn.TabStop = false;
            this.signInbtn.Text = "Sign In";
            this.signInbtn.UseVisualStyleBackColor = true;
            this.signInbtn.Click += new System.EventHandler(this.signInbtn_Click);
            // 
            // signInTextBox
            // 
            this.signInTextBox.Location = new System.Drawing.Point(203, 363);
            this.signInTextBox.Name = "signInTextBox";
            this.signInTextBox.Size = new System.Drawing.Size(123, 20);
            this.signInTextBox.TabIndex = 15;
            this.signInTextBox.TabStop = false;
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
            this.saveUserbtn.TabStop = false;
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
            this.saveChangesbtn.TabStop = false;
            this.saveChangesbtn.Text = "Save All Changes";
            this.saveChangesbtn.UseVisualStyleBackColor = true;
            this.saveChangesbtn.Click += new System.EventHandler(this.saveChangesbtn_Click);
            // 
            // userTextbox
            // 
            this.userTextbox.Location = new System.Drawing.Point(46, 363);
            this.userTextbox.Name = "userTextbox";
            this.userTextbox.Size = new System.Drawing.Size(138, 20);
            this.userTextbox.TabIndex = 12;
            this.userTextbox.TabStop = false;
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
            // controlArrowsbtn
            // 
            this.controlArrowsbtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.controlArrowsbtn.Location = new System.Drawing.Point(10, 169);
            this.controlArrowsbtn.Name = "controlArrowsbtn";
            this.controlArrowsbtn.Size = new System.Drawing.Size(316, 23);
            this.controlArrowsbtn.TabIndex = 10;
            this.controlArrowsbtn.TabStop = false;
            this.controlArrowsbtn.Text = "left arrow, right arrow, spacebar";
            this.controlArrowsbtn.UseVisualStyleBackColor = false;
            this.controlArrowsbtn.Click += new System.EventHandler(this.controlArrowsbtn_Click);
            // 
            // controlKeysbtn
            // 
            this.controlKeysbtn.Location = new System.Drawing.Point(10, 198);
            this.controlKeysbtn.Name = "controlKeysbtn";
            this.controlKeysbtn.Size = new System.Drawing.Size(316, 23);
            this.controlKeysbtn.TabIndex = 9;
            this.controlKeysbtn.TabStop = false;
            this.controlKeysbtn.Text = "\'a\', \'s\', \'w\'";
            this.controlKeysbtn.UseVisualStyleBackColor = true;
            this.controlKeysbtn.Click += new System.EventHandler(this.controlKeysbtn_Click);
            // 
            // exitBtn
            // 
            this.exitBtn.Location = new System.Drawing.Point(334, 10);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(75, 313);
            this.exitBtn.TabIndex = 8;
            this.exitBtn.TabStop = false;
            this.exitBtn.Text = "Continue";
            this.exitBtn.UseVisualStyleBackColor = true;
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            // 
            // bulletSpeedMidbtn
            // 
            this.bulletSpeedMidbtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.bulletSpeedMidbtn.Location = new System.Drawing.Point(126, 85);
            this.bulletSpeedMidbtn.Name = "bulletSpeedMidbtn";
            this.bulletSpeedMidbtn.Size = new System.Drawing.Size(75, 23);
            this.bulletSpeedMidbtn.TabIndex = 20;
            this.bulletSpeedMidbtn.TabStop = false;
            this.bulletSpeedMidbtn.Text = "Medium";
            this.bulletSpeedMidbtn.UseVisualStyleBackColor = false;
            this.bulletSpeedMidbtn.Click += new System.EventHandler(this.bulletSpeedMidbtn_Click);
            // 
            // bulletSpeedSlowbtn
            // 
            this.bulletSpeedSlowbtn.Location = new System.Drawing.Point(126, 114);
            this.bulletSpeedSlowbtn.Name = "bulletSpeedSlowbtn";
            this.bulletSpeedSlowbtn.Size = new System.Drawing.Size(75, 23);
            this.bulletSpeedSlowbtn.TabIndex = 19;
            this.bulletSpeedSlowbtn.TabStop = false;
            this.bulletSpeedSlowbtn.Text = "Slow";
            this.bulletSpeedSlowbtn.UseVisualStyleBackColor = true;
            this.bulletSpeedSlowbtn.Click += new System.EventHandler(this.bulletSpeedSlowbtn_Click);
            // 
            // bulletSpeedFastbtn
            // 
            this.bulletSpeedFastbtn.Location = new System.Drawing.Point(126, 56);
            this.bulletSpeedFastbtn.Name = "bulletSpeedFastbtn";
            this.bulletSpeedFastbtn.Size = new System.Drawing.Size(75, 23);
            this.bulletSpeedFastbtn.TabIndex = 18;
            this.bulletSpeedFastbtn.TabStop = false;
            this.bulletSpeedFastbtn.Text = "Fast";
            this.bulletSpeedFastbtn.UseVisualStyleBackColor = true;
            this.bulletSpeedFastbtn.Click += new System.EventHandler(this.bulletSpeedFastbtn_Click);
            // 
            // enemyBombSlowbtn
            // 
            this.enemyBombSlowbtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.enemyBombSlowbtn.Location = new System.Drawing.Point(228, 114);
            this.enemyBombSlowbtn.Name = "enemyBombSlowbtn";
            this.enemyBombSlowbtn.Size = new System.Drawing.Size(75, 23);
            this.enemyBombSlowbtn.TabIndex = 6;
            this.enemyBombSlowbtn.TabStop = false;
            this.enemyBombSlowbtn.Text = "Slow";
            this.enemyBombSlowbtn.UseVisualStyleBackColor = false;
            this.enemyBombSlowbtn.Click += new System.EventHandler(this.enemyBombSlowbtn_Click);
            // 
            // enemyBombFastbtn
            // 
            this.enemyBombFastbtn.Location = new System.Drawing.Point(228, 56);
            this.enemyBombFastbtn.Name = "enemyBombFastbtn";
            this.enemyBombFastbtn.Size = new System.Drawing.Size(75, 23);
            this.enemyBombFastbtn.TabIndex = 5;
            this.enemyBombFastbtn.TabStop = false;
            this.enemyBombFastbtn.Text = "Fast";
            this.enemyBombFastbtn.UseVisualStyleBackColor = true;
            this.enemyBombFastbtn.Click += new System.EventHandler(this.enemyBombFastbtn_Click);
            // 
            // playerSpeedMidbtn
            // 
            this.playerSpeedMidbtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.playerSpeedMidbtn.Location = new System.Drawing.Point(15, 85);
            this.playerSpeedMidbtn.Name = "playerSpeedMidbtn";
            this.playerSpeedMidbtn.Size = new System.Drawing.Size(75, 23);
            this.playerSpeedMidbtn.TabIndex = 4;
            this.playerSpeedMidbtn.TabStop = false;
            this.playerSpeedMidbtn.Text = "Medium";
            this.playerSpeedMidbtn.UseVisualStyleBackColor = false;
            this.playerSpeedMidbtn.Click += new System.EventHandler(this.playerSpeedMidbtn_Click);
            // 
            // playerSpeedSlowbtn
            // 
            this.playerSpeedSlowbtn.Location = new System.Drawing.Point(15, 114);
            this.playerSpeedSlowbtn.Name = "playerSpeedSlowbtn";
            this.playerSpeedSlowbtn.Size = new System.Drawing.Size(75, 23);
            this.playerSpeedSlowbtn.TabIndex = 3;
            this.playerSpeedSlowbtn.TabStop = false;
            this.playerSpeedSlowbtn.Text = "Slow";
            this.playerSpeedSlowbtn.UseVisualStyleBackColor = true;
            this.playerSpeedSlowbtn.Click += new System.EventHandler(this.playerSpeedSlowbtn_Click);
            // 
            // playerSpeedFastbtn
            // 
            this.playerSpeedFastbtn.Location = new System.Drawing.Point(15, 56);
            this.playerSpeedFastbtn.Name = "playerSpeedFastbtn";
            this.playerSpeedFastbtn.Size = new System.Drawing.Size(75, 23);
            this.playerSpeedFastbtn.TabIndex = 2;
            this.playerSpeedFastbtn.TabStop = false;
            this.playerSpeedFastbtn.Text = "Fast";
            this.playerSpeedFastbtn.UseVisualStyleBackColor = true;
            this.playerSpeedFastbtn.Click += new System.EventHandler(this.playerSpeedFastbtn_Click);
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
            // playerBulletSpeedlbl
            // 
            this.playerBulletSpeedlbl.AutoSize = true;
            this.playerBulletSpeedlbl.Location = new System.Drawing.Point(123, 10);
            this.playerBulletSpeedlbl.Name = "playerBulletSpeedlbl";
            this.playerBulletSpeedlbl.Size = new System.Drawing.Size(67, 13);
            this.playerBulletSpeedlbl.TabIndex = 17;
            this.playerBulletSpeedlbl.Text = "Bullet Speed";
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
            if (useArrows)
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
                    default:
                        break;
                }
            }
            else if (!useArrows)
            {
                switch (CurrentKeyDown)
                {
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
                    if (++TheLevel % 5 == 0)
                    {
                        NumberOfMen++;
                    }
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
            timer1.Enabled = false;
        }

		private void Form1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			//string result = e.KeyChar.ToString();
			//Invalidate(TheMan.GetBounds());
		}

		private void Form1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			string result = e.KeyData.ToString();
			if (result == "Left"  || result == "Right")
			{
				LastKeyDown = "";
			}
		}

        //restarts game
		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			this.InitializeAllGameObjects(true);
			TheLevel = 0;
		}

        //pause and unpause game
        private void menuItem4_Click(object sender, EventArgs e)
        {
            if (GameGoing == false && kNumberOfTries > 0)
            {
                GameGoing = true;
                timer1.Enabled = true;
            }
            else if (GameGoing == true && kNumberOfTries > 0)
            {
                GameGoing = false;
                timer1.Enabled = false;
            }
        }

        private void Menu_Exit(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

        //open the menu
        private void menuBtn_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            menuPnl.Visible = true;
            menuBtn.Visible = false;
            GameGoing = false;
        }

        //close the menu
        private void exitBtn_Click(object sender, EventArgs e)
        {
            menuPnl.Visible = false;
            menuBtn.Visible = true;
            GameGoing = true;
            timer1.Enabled = true;
        }

        //save a new user profile
        private void saveUserbtn_Click(object sender, EventArgs e)
        {
            if (userTextbox.Text != "")
            {
                //makes sure there isnt an existing profile with the same name
                foreach (UserAttributes currUser in savedProfiles)
                {
                    if (currUser.Name == userTextbox.Text)
                    {
                        currentSettings = currUser;
                        LoadSettings(currentSettings, sender, e);
                        userMsglbl.Text = "Loaded profile: \n" + currentSettings.Name;
                        userTextbox.Text = "";
                        return;
                    }
                }
                savedProfiles.Add(currentSettings);
                currentSettings.Name = userTextbox.Text;
                this.saveChangesbtn_Click(sender, e);
                userTextbox.Text = "";
            }
        }

        //save the current set of options to the current profile, and saves to their profile
        private void saveChangesbtn_Click(object sender, EventArgs e)
        {
            currentSettings.BombSpeed = TheBomb.TheBombInterval;
            currentSettings.BulletSpeed = TheBullet.BulletInterval;
            currentSettings.MovementSpeed = TheMan.Interval;
            currentSettings.UseArrows = useArrows;
            
            using (StreamWriter writer = new StreamWriter("userprofiles.xml"))
            {
                serializer.Serialize(writer, savedProfiles);
            }

            userMsglbl.Text = "Made new profile: \n" + currentSettings.Name;
        }

        //looks for a profile with the name that's input, and will load settings if find that profile
        private void signInbtn_Click(object sender, EventArgs e)
        {
            if (signInTextBox.Text != "")
            {
                using (StreamReader reader = new StreamReader("userprofiles.xml"))
                {
                    savedProfiles = serializer.Deserialize(reader) as List<UserAttributes>;
                }

                foreach (UserAttributes currUser in savedProfiles)
                {
                    if (currUser.Name == signInTextBox.Text)
                    {
                        currentSettings = currUser;
                        LoadSettings(currentSettings, sender, e);
                        userMsglbl.Text = "Loaded profile: \n" + currentSettings.Name;
                        signInTextBox.Text = "";
                        return;
                    }
                }

                userMsglbl.Text = "User not found.";
            }
        }

        //applies the settings from a saved profile that signs in
        private void LoadSettings(UserAttributes applySettings, object senderPass, EventArgs ePass)
        {
            switch (applySettings.BulletSpeed)
            {
                case 30:
                    bulletSpeedFastbtn_Click(senderPass, ePass);
                    break;
                case 20:
                    bulletSpeedMidbtn_Click(senderPass, ePass);
                    break;
                case 10:
                    bulletSpeedSlowbtn_Click(senderPass, ePass);
                    break;
            }

            switch (applySettings.BombSpeed)
            {
                case 15:
                    enemyBombFastbtn_Click(senderPass, ePass);
                    break;
                case 5:
                    enemyBombSlowbtn_Click(senderPass, ePass);
                    break;
            }

            switch (applySettings.MovementSpeed)
            {
                case 15:
                    playerSpeedFastbtn_Click(senderPass, ePass);
                    break;
                case 10:
                    playerSpeedMidbtn_Click(senderPass, ePass);
                    break;
                case 5:
                    playerSpeedSlowbtn_Click(senderPass, ePass);
                    break;
            }

            if (applySettings.UseArrows)
            {
                controlArrowsbtn_Click(senderPass, ePass);
            }
            else
            {
                controlKeysbtn_Click(senderPass, ePass);
            }
        }

        private void bulletSpeedFastbtn_Click(object sender, EventArgs e)
        {
            TheBullet.BulletInterval = 30;
            bulletSpeedFastbtn.BackColor = SystemColors.Highlight;
            bulletSpeedMidbtn.BackColor = SystemColors.Control;
            bulletSpeedSlowbtn.BackColor = SystemColors.Control;
        }

        private void bulletSpeedMidbtn_Click(object sender, EventArgs e)
        {
            TheBullet.BulletInterval = 20;
            bulletSpeedFastbtn.BackColor = SystemColors.Control;
            bulletSpeedMidbtn.BackColor = SystemColors.Highlight;
            bulletSpeedSlowbtn.BackColor = SystemColors.Control;
        }

        private void bulletSpeedSlowbtn_Click(object sender, EventArgs e)
        {
            TheBullet.BulletInterval = 10;
            bulletSpeedFastbtn.BackColor = SystemColors.Control;
            bulletSpeedMidbtn.BackColor = SystemColors.Control;
            bulletSpeedSlowbtn.BackColor = SystemColors.Highlight;
        }

        private void playerSpeedFastbtn_Click(object sender, EventArgs e)
        {
            TheMan.Interval = 15;
            playerSpeedFastbtn.BackColor = SystemColors.Highlight;
            playerSpeedMidbtn.BackColor = SystemColors.Control;
            playerSpeedSlowbtn.BackColor = SystemColors.Control;
        }

        private void playerSpeedMidbtn_Click(object sender, EventArgs e)
        {
            TheMan.Interval = 10;
            playerSpeedFastbtn.BackColor = SystemColors.Control;
            playerSpeedMidbtn.BackColor = SystemColors.Highlight;
            playerSpeedSlowbtn.BackColor = SystemColors.Control;
        }

        private void playerSpeedSlowbtn_Click(object sender, EventArgs e)
        {
            TheMan.Interval = 5;
            playerSpeedFastbtn.BackColor = SystemColors.Control;
            playerSpeedMidbtn.BackColor = SystemColors.Control;
            playerSpeedSlowbtn.BackColor = SystemColors.Highlight;
        }

        private void enemyBombFastbtn_Click(object sender, EventArgs e)
        {
            TheBomb.TheBombInterval = 15;
            enemyBombFastbtn.BackColor = SystemColors.Highlight;
            enemyBombSlowbtn.BackColor = SystemColors.Control;
        }

        private void enemyBombSlowbtn_Click(object sender, EventArgs e)
        {
            TheBomb.TheBombInterval = 5;
            enemyBombFastbtn.BackColor = SystemColors.Control;
            enemyBombSlowbtn.BackColor = SystemColors.Highlight;
        }

        private void controlArrowsbtn_Click(object sender, EventArgs e)
        {
            useArrows = true;
            controlArrowsbtn.BackColor = SystemColors.Highlight;
            controlKeysbtn.BackColor = SystemColors.Control;
        }

        private void controlKeysbtn_Click(object sender, EventArgs e)
        {
            useArrows = false;
            controlArrowsbtn.BackColor = SystemColors.Control;
            controlKeysbtn.BackColor = SystemColors.Highlight;
        }
    }
}
