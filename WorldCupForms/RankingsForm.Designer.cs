namespace WorldCupForms
{
    partial class RankingsForm
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
            tcRankings = new TabControl();
            tpPlayers = new TabPage();
            dgvPlayers = new DataGridView();
            tpMatches = new TabPage();
            dgvMatches = new DataGridView();
            btnPrint = new Button();
            printDocument = new System.Drawing.Printing.PrintDocument();
            printDialog = new PrintDialog();
            tcRankings.SuspendLayout();
            tpPlayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPlayers).BeginInit();
            tpMatches.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMatches).BeginInit();
            SuspendLayout();
            // 
            // tcRankings
            // 
            tcRankings.Controls.Add(tpPlayers);
            tcRankings.Controls.Add(tpMatches);
            tcRankings.Location = new Point(0, 0);
            tcRankings.Name = "tcRankings";
            tcRankings.SelectedIndex = 0;
            tcRankings.Size = new Size(2540, 1380);
            tcRankings.TabIndex = 0;
            // 
            // tpPlayers
            // 
            tpPlayers.Controls.Add(dgvPlayers);
            tpPlayers.Font = new Font("Inter", 9F);
            tpPlayers.Location = new Point(8, 46);
            tpPlayers.Name = "tpPlayers";
            tpPlayers.Padding = new Padding(3);
            tpPlayers.Size = new Size(2524, 1326);
            tpPlayers.TabIndex = 0;
            tpPlayers.Text = "Player rankings";
            tpPlayers.UseVisualStyleBackColor = true;
            // 
            // dgvPlayers
            // 
            dgvPlayers.AllowUserToAddRows = false;
            dgvPlayers.BackgroundColor = Color.WhiteSmoke;
            dgvPlayers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPlayers.GridColor = Color.Teal;
            dgvPlayers.Location = new Point(0, 116);
            dgvPlayers.Name = "dgvPlayers";
            dgvPlayers.ReadOnly = true;
            dgvPlayers.RowHeadersWidth = 82;
            dgvPlayers.RowTemplate.Height = 60;
            dgvPlayers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPlayers.Size = new Size(2514, 1174);
            dgvPlayers.TabIndex = 0;
            // 
            // tpMatches
            // 
            tpMatches.Controls.Add(dgvMatches);
            tpMatches.Font = new Font("Inter", 9F);
            tpMatches.Location = new Point(8, 46);
            tpMatches.Name = "tpMatches";
            tpMatches.Padding = new Padding(3);
            tpMatches.Size = new Size(2524, 1326);
            tpMatches.TabIndex = 1;
            tpMatches.Text = "Match rankings";
            tpMatches.UseVisualStyleBackColor = true;
            // 
            // dgvMatches
            // 
            dgvMatches.AllowUserToAddRows = false;
            dgvMatches.BackgroundColor = Color.WhiteSmoke;
            dgvMatches.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMatches.GridColor = Color.Teal;
            dgvMatches.Location = new Point(5, 155);
            dgvMatches.Name = "dgvMatches";
            dgvMatches.ReadOnly = true;
            dgvMatches.RowHeadersWidth = 82;
            dgvMatches.RowTemplate.Height = 60;
            dgvMatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMatches.Size = new Size(2514, 1174);
            dgvMatches.TabIndex = 1;
            // 
            // btnPrint
            // 
            btnPrint.Font = new Font("Inter", 9F);
            btnPrint.Location = new Point(58, 1404);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(499, 97);
            btnPrint.TabIndex = 1;
            btnPrint.Text = "Print rankings (pdf)";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += btnPrint_Click;
            // 
            // printDocument
            // 
            printDocument.BeginPrint += PrintDocument_BeginPrint;
            printDocument.PrintPage += PrintDocument_PrintPage;
            // 
            // printDialog
            // 
            printDialog.UseEXDialog = true;
            // 
            // RankingsForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2534, 1529);
            Controls.Add(btnPrint);
            Controls.Add(tcRankings);
            Name = "RankingsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Rankings";
            FormClosing += RankingsForm_FormClosing;
            tcRankings.ResumeLayout(false);
            tpPlayers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPlayers).EndInit();
            tpMatches.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMatches).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tcRankings;
        private TabPage tpPlayers;
        private TabPage tpMatches;
        private DataGridView dgvPlayers;
        private DataGridView dgvMatches;
        private Button btnPrint;
        private System.Drawing.Printing.PrintDocument printDocument;
        private PrintDialog printDialog;
    }
}