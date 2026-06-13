namespace WorldCupForms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            pHeader = new Panel();
            btnSettings = new Button();
            cbTeams = new ComboBox();
            lbChooseTeam = new Label();
            pbSpinner = new PictureBox();
            flpFavorites = new FlowLayoutPanel();
            flpAllPlayers = new FlowLayoutPanel();
            lbFavouritePlayers = new Label();
            lbAllPlayers = new Label();
            btnViewRankings = new Button();
            pHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbSpinner).BeginInit();
            SuspendLayout();
            // 
            // pHeader
            // 
            pHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pHeader.BackColor = Color.WhiteSmoke;
            pHeader.BorderStyle = BorderStyle.FixedSingle;
            pHeader.Controls.Add(btnSettings);
            pHeader.Controls.Add(cbTeams);
            pHeader.Controls.Add(lbChooseTeam);
            pHeader.Location = new Point(0, 0);
            pHeader.Name = "pHeader";
            pHeader.Size = new Size(2534, 132);
            pHeader.TabIndex = 0;
            // 
            // btnSettings
            // 
            btnSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSettings.Font = new Font("Inter", 9F);
            btnSettings.Location = new Point(2185, 42);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(300, 60);
            btnSettings.TabIndex = 2;
            btnSettings.Text = "⚙ Settings";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // cbTeams
            // 
            cbTeams.Font = new Font("Inter", 9F);
            cbTeams.FormattingEnabled = true;
            cbTeams.Location = new Point(318, 52);
            cbTeams.Name = "cbTeams";
            cbTeams.Size = new Size(378, 43);
            cbTeams.TabIndex = 1;
            // 
            // lbChooseTeam
            // 
            lbChooseTeam.AutoSize = true;
            lbChooseTeam.Font = new Font("Inter", 9F);
            lbChooseTeam.Location = new Point(48, 55);
            lbChooseTeam.Name = "lbChooseTeam";
            lbChooseTeam.Size = new Size(229, 35);
            lbChooseTeam.TabIndex = 0;
            lbChooseTeam.Text = "Choose your team:";
            // 
            // pbSpinner
            // 
            pbSpinner.BackColor = Color.Transparent;
            pbSpinner.Image = (Image)resources.GetObject("pbSpinner.Image");
            pbSpinner.Location = new Point(1230, 750);
            pbSpinner.Name = "pbSpinner";
            pbSpinner.Size = new Size(100, 100);
            pbSpinner.SizeMode = PictureBoxSizeMode.Zoom;
            pbSpinner.TabIndex = 3;
            pbSpinner.TabStop = false;
            pbSpinner.Visible = false;
            // 
            // flpFavorites
            // 
            flpFavorites.BackColor = Color.White;
            flpFavorites.Location = new Point(0, 259);
            flpFavorites.Name = "flpFavorites";
            flpFavorites.Size = new Size(1024, 1050);
            flpFavorites.TabIndex = 4;
            // 
            // flpAllPlayers
            // 
            flpAllPlayers.AutoScroll = true;
            flpAllPlayers.BackColor = Color.White;
            flpAllPlayers.Location = new Point(1030, 259);
            flpAllPlayers.Name = "flpAllPlayers";
            flpAllPlayers.Size = new Size(1504, 1050);
            flpAllPlayers.TabIndex = 5;
            // 
            // lbFavouritePlayers
            // 
            lbFavouritePlayers.AutoSize = true;
            lbFavouritePlayers.Font = new Font("Inter", 14F, FontStyle.Bold);
            lbFavouritePlayers.Location = new Point(327, 172);
            lbFavouritePlayers.Name = "lbFavouritePlayers";
            lbFavouritePlayers.Size = new Size(362, 54);
            lbFavouritePlayers.TabIndex = 3;
            lbFavouritePlayers.Text = "Favourive players:";
            // 
            // lbAllPlayers
            // 
            lbAllPlayers.AutoSize = true;
            lbAllPlayers.Font = new Font("Inter", 14F, FontStyle.Bold);
            lbAllPlayers.Location = new Point(1694, 172);
            lbAllPlayers.Name = "lbAllPlayers";
            lbAllPlayers.Size = new Size(229, 54);
            lbAllPlayers.TabIndex = 6;
            lbAllPlayers.Text = "All players:";
            // 
            // btnViewRankings
            // 
            btnViewRankings.Font = new Font("Inter", 9F);
            btnViewRankings.Location = new Point(49, 1376);
            btnViewRankings.Name = "btnViewRankings";
            btnViewRankings.Size = new Size(499, 97);
            btnViewRankings.TabIndex = 7;
            btnViewRankings.Text = "View Rankings and Reports";
            btnViewRankings.UseVisualStyleBackColor = true;
            btnViewRankings.Click += btnViewRankings_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(2534, 1529);
            Controls.Add(btnViewRankings);
            Controls.Add(lbAllPlayers);
            Controls.Add(lbFavouritePlayers);
            Controls.Add(pbSpinner);
            Controls.Add(flpAllPlayers);
            Controls.Add(flpFavorites);
            Controls.Add(pHeader);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1024, 768);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "World Cup";
            FormClosing += Form_FormClosing;
            Load += MainForm_Load;
            pHeader.ResumeLayout(false);
            pHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbSpinner).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pHeader;
        private Button btnSettings;
        private ComboBox cbTeams;
        private Label lbChooseTeam;
        private PictureBox pbSpinner;
        private FlowLayoutPanel flpFavorites;
        private FlowLayoutPanel flpAllPlayers;
        private Label lbFavouritePlayers;
        private Label lbAllPlayers;
        private Button btnViewRankings;
    }
}