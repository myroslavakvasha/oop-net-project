namespace WorldCupForms
{
    partial class StartupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupForm));
            lbChampionship = new Label();
            lbWelcome = new Label();
            lbLang = new Label();
            rbFemale = new RadioButton();
            rbMale = new RadioButton();
            lbEnglish = new Label();
            btnConfirm = new Button();
            SuspendLayout();
            // 
            // lbChampionship
            // 
            lbChampionship.AutoSize = true;
            lbChampionship.Font = new Font("Inter", 9F);
            lbChampionship.Location = new Point(183, 288);
            lbChampionship.Name = "lbChampionship";
            lbChampionship.Size = new Size(259, 35);
            lbChampionship.TabIndex = 0;
            lbChampionship.Text = "Select championship:";
            lbChampionship.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbWelcome
            // 
            lbWelcome.Anchor = AnchorStyles.Top;
            lbWelcome.AutoSize = true;
            lbWelcome.Font = new Font("Inter", 18F, FontStyle.Bold);
            lbWelcome.Location = new Point(336, 102);
            lbWelcome.Name = "lbWelcome";
            lbWelcome.Size = new Size(664, 68);
            lbWelcome.TabIndex = 1;
            lbWelcome.Text = "WELCOME TO WORLD CUP";
            // 
            // lbLang
            // 
            lbLang.AutoSize = true;
            lbLang.Font = new Font("Inter", 9F);
            lbLang.Location = new Point(183, 435);
            lbLang.Name = "lbLang";
            lbLang.Size = new Size(261, 35);
            lbLang.TabIndex = 2;
            lbLang.Text = "Application language:";
            lbLang.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // rbFemale
            // 
            rbFemale.AutoSize = true;
            rbFemale.Checked = true;
            rbFemale.Font = new Font("Inter", 9F);
            rbFemale.Location = new Point(517, 288);
            rbFemale.Name = "rbFemale";
            rbFemale.Size = new Size(335, 39);
            rbFemale.TabIndex = 3;
            rbFemale.TabStop = true;
            rbFemale.Text = "Female (World Cup 2019)";
            rbFemale.UseVisualStyleBackColor = true;
            // 
            // rbMale
            // 
            rbMale.AutoSize = true;
            rbMale.Font = new Font("Inter", 9F);
            rbMale.Location = new Point(517, 343);
            rbMale.Name = "rbMale";
            rbMale.Size = new Size(309, 39);
            rbMale.TabIndex = 4;
            rbMale.Text = "Male (World Cup 2018)";
            rbMale.UseVisualStyleBackColor = true;
            // 
            // lbEnglish
            // 
            lbEnglish.AutoSize = true;
            lbEnglish.Font = new Font("Inter", 9F);
            lbEnglish.Location = new Point(517, 435);
            lbEnglish.Name = "lbEnglish";
            lbEnglish.Size = new Size(97, 35);
            lbEnglish.TabIndex = 5;
            lbEnglish.Text = "English";
            lbEnglish.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnConfirm
            // 
            btnConfirm.BackColor = Color.White;
            btnConfirm.Font = new Font("Inter", 9F, FontStyle.Bold);
            btnConfirm.ForeColor = SystemColors.ActiveCaptionText;
            btnConfirm.Location = new Point(426, 535);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(485, 60);
            btnConfirm.TabIndex = 6;
            btnConfirm.Text = "Confirm";
            btnConfirm.UseVisualStyleBackColor = false;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // StartupForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1340, 697);
            Controls.Add(btnConfirm);
            Controls.Add(lbEnglish);
            Controls.Add(rbMale);
            Controls.Add(rbFemale);
            Controls.Add(lbLang);
            Controls.Add(lbWelcome);
            Controls.Add(lbChampionship);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "StartupForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbChampionship;
        private Label lbWelcome;
        private Label lbLang;
        private RadioButton rbFemale;
        private RadioButton rbMale;
        private Label lbEnglish;
        private Button btnConfirm;
    }
}