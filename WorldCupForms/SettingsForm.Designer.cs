namespace WorldCupForms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            lbTitle = new Label();
            rbFemale = new RadioButton();
            rbMale = new RadioButton();
            btnConfirm = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lbTitle
            // 
            lbTitle.AutoSize = true;
            lbTitle.Font = new Font("Inter", 12F, FontStyle.Bold);
            lbTitle.Location = new Point(292, 94);
            lbTitle.Name = "lbTitle";
            lbTitle.Size = new Size(443, 45);
            lbTitle.TabIndex = 0;
            lbTitle.Text = "Change the championship:";
            // 
            // rbFemale
            // 
            rbFemale.AutoSize = true;
            rbFemale.Font = new Font("Inter", 9F);
            rbFemale.Location = new Point(138, 253);
            rbFemale.Name = "rbFemale";
            rbFemale.Size = new Size(127, 39);
            rbFemale.TabIndex = 1;
            rbFemale.TabStop = true;
            rbFemale.Text = "Female";
            rbFemale.UseVisualStyleBackColor = true;
            // 
            // rbMale
            // 
            rbMale.AutoSize = true;
            rbMale.Font = new Font("Inter", 9F);
            rbMale.Location = new Point(138, 325);
            rbMale.Name = "rbMale";
            rbMale.Size = new Size(101, 39);
            rbMale.TabIndex = 2;
            rbMale.TabStop = true;
            rbMale.Text = "Male";
            rbMale.UseVisualStyleBackColor = true;
            // 
            // btnConfirm
            // 
            btnConfirm.BackColor = Color.OliveDrab;
            btnConfirm.Font = new Font("Inter", 9F, FontStyle.Bold);
            btnConfirm.ForeColor = SystemColors.ButtonHighlight;
            btnConfirm.Location = new Point(138, 465);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(377, 69);
            btnConfirm.TabIndex = 3;
            btnConfirm.Text = "Confirm";
            btnConfirm.UseVisualStyleBackColor = false;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Brown;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Font = new Font("Inter", 9F, FontStyle.Bold);
            btnCancel.ForeColor = SystemColors.ButtonHighlight;
            btnCancel.Location = new Point(530, 465);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(377, 69);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1049, 602);
            Controls.Add(btnCancel);
            Controls.Add(btnConfirm);
            Controls.Add(rbMale);
            Controls.Add(rbFemale);
            Controls.Add(lbTitle);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbTitle;
        private RadioButton rbFemale;
        private RadioButton rbMale;
        private Button btnConfirm;
        private Button btnCancel;
    }
}