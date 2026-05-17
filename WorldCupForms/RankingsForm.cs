using DataLayer.Models.Rankings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorldCupForms
{
    public partial class RankingsForm : Form
    {
        private readonly List<PlayerRankingModel> _players;
        private readonly List<MatchRankingModel> _matches;

        // Thread-safe async pagination trackers
        private int _currentPlayerIndex = 0;
        private int _currentMatchIndex = 0;
        private int _currentPageNumber = 1;

        public RankingsForm(List<PlayerRankingModel> players, List<MatchRankingModel> matches)
        {
            InitializeComponent();
            _players = players;
            _matches = matches;

            this.Load += RankingsForm_Load;

            // Strict background event bindings to prevent duplicate UI trigger loops
            printDocument.BeginPrint -= PrintDocument_BeginPrint;
            printDocument.BeginPrint += PrintDocument_BeginPrint;

            printDocument.PrintPage -= PrintDocument_PrintPage;
            printDocument.PrintPage += PrintDocument_PrintPage;

            printDocument.EndPrint -= PrintDocument_EndPrint;
            printDocument.EndPrint += PrintDocument_EndPrint;

            this.FormClosing -= RankingsForm_FormClosing;
            this.FormClosing += RankingsForm_FormClosing;
        }

        private void RankingsForm_Load(object sender, EventArgs e)
        {
            SetupPlayersGridView();
            SetupMatchesGridView();
        }

        // --- FIXED PRINT BUTTON HANDLER ---
        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDialog.Document = printDocument;
            printDialog.UseEXDialog = true;

            // FIX: Passing "this" forces the file dialog to layer strictly above this Form, 
            // preventing the app from minimizing or losing focus when the PDF finishes generating.
            if (printDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    printDocument.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Printing system exception: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            // Reset dataset pointers cleanly before starting pagination
            _currentPlayerIndex = 0;
            _currentMatchIndex = 0;
            _currentPageNumber = 1;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            if (g == null) return;

            // Strict typography mapping using the Inter Font Family
            string fontName = "Inter";
            Font titleFont = new Font(fontName, 16, FontStyle.Bold);
            Font sectionFont = new Font(fontName, 13, FontStyle.Bold);
            Font headerFont = new Font(fontName, 10, FontStyle.Bold);
            Font bodyFont = new Font(fontName, 10, FontStyle.Regular);

            float currentY = e.MarginBounds.Top;
            float leftX = e.MarginBounds.Left;
            float usableWidth = e.MarginBounds.Width;
            float rightX = e.MarginBounds.Right;

            // --- RECALCULATED HORIZONTAL GRID TRACKS ---
            float colPicX = leftX + 15;
            float colNameX = leftX + 90;

            // Exact center anchors and bounding widths for numeric fields
            float colAppsCenter = rightX - 220;
            float colGoalsCenter = rightX - 130;
            float colCardsCenter = rightX - 50;
            float numColumnWidth = 60;

            // Formatting rules to handle different alignments
            StringFormat fullyCenteredFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            StringFormat verticalOnlyCenterFormat = new StringFormat
            {
                Alignment = StringAlignment.Near, // Keeps text starting on the left side
                LineAlignment = StringAlignment.Center // Centers text vertically in the row bounding rectangle
            };

            // 1. MAIN TITLE BANNER (Generated On Timestamp Line Deleted)
            if (_currentPageNumber == 1)
            {
                g.DrawString("National Team Rankings & Statistical Report", titleFont, Brushes.Black, leftX, currentY);
                currentY += titleFont.Height + 25;
            }

            // 2. PLAYER RANKINGS SECTION
            if (_currentPlayerIndex < _players.Count)
            {
                if (_currentPageNumber == 1 || currentY + 80 > e.MarginBounds.Bottom)
                {
                    g.DrawString("Player Rankings (Sorted by Goals & Yellow Cards)", sectionFont, Brushes.DarkGreen, leftX, currentY);
                    currentY += sectionFont.Height + 10;
                }

                // Draw Player Grid Header Blocks (Height 28)
                g.FillRectangle(Brushes.LightGray, leftX, currentY, usableWidth, 28);
                g.DrawRectangle(Pens.Black, leftX, currentY, usableWidth, 28);

                g.DrawString("Picture", headerFont, Brushes.Black, colPicX, currentY + 6);
                g.DrawString("Full Name", headerFont, Brushes.Black, colNameX, currentY + 6);

                // Render headers perfectly centered to match the numbers beneath them
                g.DrawString("Apps", headerFont, Brushes.Black, new RectangleF(colAppsCenter - (numColumnWidth / 2), currentY, numColumnWidth, 28), fullyCenteredFormat);
                g.DrawString("Goals", headerFont, Brushes.Black, new RectangleF(colGoalsCenter - (numColumnWidth / 2), currentY, numColumnWidth, 28), fullyCenteredFormat);
                g.DrawString("Cards", headerFont, Brushes.Black, new RectangleF(colCardsCenter - (numColumnWidth / 2), currentY, numColumnWidth, 28), fullyCenteredFormat);
                currentY += 28;

                // Player Processing Loop
                while (_currentPlayerIndex < _players.Count)
                {
                    float rowHeight = 56; // High clearance budget guarantees square custom portraits never clip margins
                    if (currentY + rowHeight > e.MarginBounds.Bottom)
                    {
                        DrawPageFooter(g, rightX, e.MarginBounds.Bottom);
                        _currentPageNumber++;
                        e.HasMorePages = true;
                        return;
                    }

                    var player = _players[_currentPlayerIndex];

                    // Outer cell container border outlines
                    g.DrawRectangle(Pens.LightGray, leftX, currentY, usableWidth, rowHeight);

                    // Bounding target slot: Safe 44x44 portrait canvas zone
                    float targetBoxW = 44;
                    float targetBoxH = 44;
                    float targetBoxX = colPicX;
                    float targetBoxY = currentY + (rowHeight - targetBoxH) / 2; // Perfectly center vertically

                    if (File.Exists(player.ImagePath))
                    {
                        try
                        {
                            using (var fs = new FileStream(player.ImagePath, FileMode.Open, FileAccess.Read))
                            using (Image img = Image.FromStream(fs))
                            {
                                // Proportional fit calculations (Prevents squishing or stretching completely)
                                float ratioX = targetBoxW / img.Width;
                                float ratioY = targetBoxH / img.Height;
                                float scaleRatio = Math.Min(ratioX, ratioY);

                                float finalW = img.Width * scaleRatio;
                                float finalH = img.Height * scaleRatio;

                                // Center placement vectors inside the 44x44 box
                                float renderX = targetBoxX + (targetBoxW - finalW) / 2;
                                float renderY = targetBoxY + (targetBoxH - finalH) / 2;

                                g.DrawImage(img, renderX, renderY, finalW, finalH);
                            }
                        }
                        catch
                        {
                            g.DrawRectangle(Pens.Gainsboro, targetBoxX, targetBoxY, targetBoxW, targetBoxH);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(Pens.Gainsboro, targetBoxX, targetBoxY, targetBoxW, targetBoxH);
                    }

                    // --- FIX A: LEFT-ALIGNED BUT VERTICALLY CENTERED FOR STRINGS ---
                    // Construct a tracking rectangle mapping the full height of the row starting from colNameX
                    RectangleF nameRect = new RectangleF(colNameX, currentY, colAppsCenter - (numColumnWidth / 2) - colNameX, rowHeight);
                    g.DrawString(player.Name, bodyFont, Brushes.Black, nameRect, verticalOnlyCenterFormat);

                    // --- FIX B: FULLY CENTERED BOTH WAYS FOR NUMBERS ---
                    RectangleF rectApps = new RectangleF(colAppsCenter - (numColumnWidth / 2), currentY, numColumnWidth, rowHeight);
                    RectangleF rectGoals = new RectangleF(colGoalsCenter - (numColumnWidth / 2), currentY, numColumnWidth, rowHeight);
                    RectangleF rectCards = new RectangleF(colCardsCenter - (numColumnWidth / 2), currentY, numColumnWidth, rowHeight);

                    g.DrawString(player.Appearances.ToString(), bodyFont, Brushes.Black, rectApps, fullyCenteredFormat);
                    g.DrawString(player.Goals.ToString(), bodyFont, Brushes.Black, rectGoals, fullyCenteredFormat);
                    g.DrawString(player.YellowCards.ToString(), bodyFont, Brushes.Black, rectCards, fullyCenteredFormat);

                    currentY += rowHeight;
                    _currentPlayerIndex++;
                }
                currentY += 25;
            }

            // 3. MATCH RANKINGS SECTION
            if (_currentPlayerIndex >= _players.Count && _currentMatchIndex < _matches.Count)
            {
                if (currentY + 65 > e.MarginBounds.Bottom)
                {
                    DrawPageFooter(g, rightX, e.MarginBounds.Bottom);
                    _currentPageNumber++;
                    e.HasMorePages = true;
                    return;
                }

                g.DrawString("Match Attendance Rankings (Sorted by Visitors)", sectionFont, Brushes.DarkBlue, leftX, currentY);
                currentY += sectionFont.Height + 10;

                g.FillRectangle(Brushes.LightGray, leftX, currentY, usableWidth, 28);
                g.DrawRectangle(Pens.Black, leftX, currentY, usableWidth, 28);

                float matchColWidth = usableWidth / 4;
                g.DrawString("Home Team (Host)", headerFont, Brushes.Black, leftX + 5, currentY + 6);
                g.DrawString("Away Team (Guest)", headerFont, Brushes.Black, leftX + matchColWidth + 5, currentY + 6);
                g.DrawString("Location / Venue", headerFont, Brushes.Black, leftX + (matchColWidth * 2) + 5, currentY + 6);

                // Fully center the visitor column header
                g.DrawString("Total Visitors", headerFont, Brushes.Black, new RectangleF(leftX + (matchColWidth * 3), currentY, matchColWidth, 28), fullyCenteredFormat);
                currentY += 28;

                while (_currentMatchIndex < _matches.Count)
                {
                    float matchRowHeight = 36;
                    if (currentY + matchRowHeight > e.MarginBounds.Bottom)
                    {
                        DrawPageFooter(g, rightX, e.MarginBounds.Bottom);
                        _currentPageNumber++;
                        e.HasMorePages = true;
                        return;
                    }

                    var match = _matches[_currentMatchIndex];

                    // Draw match row cell boundaries
                    g.DrawRectangle(Pens.LightGray, leftX, currentY, usableWidth, matchRowHeight);

                    // Construct left-aligned, vertically centered rectangles for match details text strings
                    RectangleF rectHome = new RectangleF(leftX + 5, currentY, matchColWidth - 5, matchRowHeight);
                    RectangleF rectAway = new RectangleF(leftX + matchColWidth + 5, currentY, matchColWidth - 5, matchRowHeight);
                    RectangleF rectLoc = new RectangleF(leftX + (matchColWidth * 2) + 5, currentY, matchColWidth - 5, matchRowHeight);

                    g.DrawString(match.HostTeam, bodyFont, Brushes.Black, rectHome, verticalOnlyCenterFormat);
                    g.DrawString(match.GuestTeam, bodyFont, Brushes.Black, rectAway, verticalOnlyCenterFormat);
                    g.DrawString(match.Location, bodyFont, Brushes.Black, rectLoc, verticalOnlyCenterFormat);

                    // Fully center the numeric values (Both horizontally and vertically)
                    RectangleF rectAttendance = new RectangleF(leftX + (matchColWidth * 3), currentY, matchColWidth, matchRowHeight);
                    g.DrawString(match.Attendance.ToString("N0"), bodyFont, Brushes.Black, rectAttendance, fullyCenteredFormat);

                    currentY += matchRowHeight;
                    _currentMatchIndex++;
                }
            }

            DrawPageFooter(g, rightX, e.MarginBounds.Bottom);
            e.HasMorePages = (_currentPlayerIndex < _players.Count || _currentMatchIndex < _matches.Count);
        }

        // Don't forget your footer tracker using Inter!
        private void DrawPageFooter(Graphics g, float rightX, float bottomY)
        {
            Font footerFont = new Font("Inter", 9, FontStyle.Regular);
            g.DrawString($"Page {_currentPageNumber}", footerFont, Brushes.Gray, rightX - 50, bottomY + 10);
        }

        // --- FIXED SUCCESS MESSAGEBOX WINDOW FOCUS ---
        private void PrintDocument_EndPrint(object sender, PrintEventArgs e)
        {
            if (e.PrintAction == PrintAction.PrintToPrinter)
            {
                // Passing "this" anchors this message window natively on top of your app form workspace layer.
                MessageBox.Show(
                    this,
                    "PDF saved successfully",
                    "Print Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void SetupPlayersGridView()
        {
            dgvPlayers.Columns.Clear();

            dgvPlayers.Dock = DockStyle.Fill;

            dgvPlayers.AutoGenerateColumns = false;
            dgvPlayers.AllowUserToAddRows = false;
            dgvPlayers.ReadOnly = true;
            dgvPlayers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvPlayers.RowTemplate.Height = 90;

            dgvPlayers.DefaultCellStyle.Font = new Font("Inter", 10F, FontStyle.Regular);
            dgvPlayers.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10F, FontStyle.Bold);
            dgvPlayers.ColumnHeadersHeight = 45;
            dgvPlayers.EnableHeadersVisualStyles = false;

            // col1
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn
            {
                Name = "Picture",
                HeaderText = "Picture",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 120
            };
            dgvPlayers.Columns.Add(imgCol);

            // col2
            dgvPlayers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Full Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // col3
            dgvPlayers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Appearances",
                HeaderText = "Appearances",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // col4
            dgvPlayers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Goals",
                HeaderText = "Goals",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // col5
            dgvPlayers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "YellowCards",
                HeaderText = "Yellow Cards",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvPlayers.DataSource = _players;

            for (int i = 0; i < _players.Count; i++)
            {
                if (File.Exists(_players[i].ImagePath))
                {
                    try
                    {
                        using (var stream = new FileStream(_players[i].ImagePath, FileMode.Open, FileAccess.Read))
                        {
                            dgvPlayers.Rows[i].Cells["Picture"].Value = Image.FromStream(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Image load issue: {ex.Message}");
                    }
                }
            }
        }
        private void SetupMatchesGridView()
        {
            dgvMatches.Columns.Clear();

            dgvMatches.Dock = DockStyle.Fill;

            dgvMatches.AutoGenerateColumns = false;
            dgvMatches.AllowUserToAddRows = false;
            dgvMatches.ReadOnly = true;
            dgvMatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvMatches.RowTemplate.Height = 50;

            dgvMatches.DefaultCellStyle.Font = new Font("Inter", 10F, FontStyle.Regular);
            dgvMatches.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10F, FontStyle.Bold);
            dgvMatches.ColumnHeadersHeight = 45;
            dgvMatches.EnableHeadersVisualStyles = false;

            // col1
            dgvMatches.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HostTeam",
                HeaderText = "Home Team (Host)",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // col2
            dgvMatches.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GuestTeam",
                HeaderText = "Away Team (Guest)",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // col3
            dgvMatches.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Location",
                HeaderText = "Location / Venue",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // col4
            dgvMatches.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Attendance",
                HeaderText = "Visitors\n(Attendance)",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvMatches.DataSource = _matches;
        }

        private void RankingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ask for exit confirmation
            DialogResult result = MessageBox.Show(
                this,
                "Are you sure you want to close the rankings viewer?",
                "Close Confirmation",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1
            );

            if (result == DialogResult.No || result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                // Clean up tracking indexes completely when leaving the screen scope
                _currentPlayerIndex = 0;
                _currentMatchIndex = 0;
                _currentPageNumber = 1;
            }
        }
    }
}

