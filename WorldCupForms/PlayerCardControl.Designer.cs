namespace WorldCupForms
{
    partial class PlayerCardControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerCardControl));
            pictureBox1 = new PictureBox();
            lbName = new Label();
            lbNumber = new Label();
            lbPosition = new Label();
            lbCaptain = new Label();
            lbStar = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(75, 24);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(198, 247);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lbName
            // 
            lbName.Font = new Font("Inter", 9F);
            lbName.Location = new Point(31, 288);
            lbName.Name = "lbName";
            lbName.Size = new Size(291, 35);
            lbName.TabIndex = 1;
            lbName.Text = "Name";
            lbName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbNumber
            // 
            lbNumber.AutoSize = true;
            lbNumber.Font = new Font("Inter", 9F);
            lbNumber.Location = new Point(31, 323);
            lbNumber.Name = "lbNumber";
            lbNumber.Size = new Size(106, 35);
            lbNumber.TabIndex = 2;
            lbNumber.Text = "Number";
            // 
            // lbPosition
            // 
            lbPosition.AutoSize = true;
            lbPosition.Font = new Font("Inter", 9F);
            lbPosition.Location = new Point(31, 358);
            lbPosition.Name = "lbPosition";
            lbPosition.Size = new Size(105, 35);
            lbPosition.TabIndex = 3;
            lbPosition.Text = "Position";
            // 
            // lbCaptain
            // 
            lbCaptain.AutoSize = true;
            lbCaptain.Font = new Font("Inter", 9F, FontStyle.Bold);
            lbCaptain.Location = new Point(31, 393);
            lbCaptain.Name = "lbCaptain";
            lbCaptain.Size = new Size(107, 35);
            lbCaptain.TabIndex = 4;
            lbCaptain.Text = "Captain";
            // 
            // lbStar
            // 
            lbStar.AutoSize = true;
            lbStar.Font = new Font("Inter", 12F);
            lbStar.Location = new Point(269, 385);
            lbStar.Name = "lbStar";
            lbStar.Size = new Size(53, 45);
            lbStar.TabIndex = 5;
            lbStar.Text = "★";
            // 
            // PlayerCardControl
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(lbStar);
            Controls.Add(lbCaptain);
            Controls.Add(lbPosition);
            Controls.Add(lbNumber);
            Controls.Add(lbName);
            Controls.Add(pictureBox1);
            Name = "PlayerCardControl";
            Size = new Size(348, 448);
            Load += PlayerCardControl_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lbName;
        private Label lbNumber;
        private Label lbPosition;
        private Label lbCaptain;
        private Label lbStar;
    }
}
