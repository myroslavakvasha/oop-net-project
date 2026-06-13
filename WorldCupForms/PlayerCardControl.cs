using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorldCupForms
{
    public partial class PlayerCardControl : UserControl
    {
        private readonly string _teamFifaCode;
        private readonly string _gender;
        public Player Player { get; private set; }
        public bool IsFavorite { get; private set; }

        public event EventHandler<Player> FavoriteToggled;
        public bool IsSelected { get; private set; }

        public event EventHandler SelectionChanged;

        private readonly string _sharedImgFolder = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "DataLayer", "Resources", "img", "players")
        );

        public PlayerCardControl(Player player, bool isFavorite, string teamFifaCode, string gender)
        {
            InitializeComponent();
            Player = player;
            IsFavorite = isFavorite;
            _teamFifaCode = teamFifaCode;
            _gender = gender.Trim().ToLower();

            LoadData();
            SetupContextMenu();
        }

        private void LoadData()
        {
            lbName.Text = Player.Name;
            lbNumber.Text = $"#{Player.ShirtNumber}";
            lbPosition.Text = Player.Position.ToString();
            lbCaptain.Visible = Player.Captain;
            lbStar.Visible = IsFavorite;

            LoadImage(_teamFifaCode);
        }

        private void LoadImage(string teamFifaCode)
        {
            try
            {
                var imagePath = Path.Combine(_sharedImgFolder, $"{_gender}_{teamFifaCode}_{Player.ShirtNumber}.png");
                var defaultPath = Path.Combine(_sharedImgFolder, "default.png");

                string pathToLoad = File.Exists(imagePath) ? imagePath : defaultPath;

                if (File.Exists(pathToLoad))
                {
                    using (var stream = new FileStream(pathToLoad, FileMode.Open, FileAccess.Read))
                    {
                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = Image.FromStream(stream);
                    }
                }
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        private void SetupContextMenu()
        {
            var menu = new ContextMenuStrip();

            // toggle favs
            var toggleItem = new ToolStripMenuItem(IsFavorite ? "Remove from favorites" : "Add to favorites");
            toggleItem.Click += (s, e) => FavoriteToggled?.Invoke(this, Player);
            menu.Items.Add(toggleItem);

            // upload pic
            var uploadItem = new ToolStripMenuItem("Change player picture...");
            uploadItem.Click += (s, e) => ChangePlayerPicture();
            menu.Items.Add(uploadItem);

            // delete pic
            var removeItem = new ToolStripMenuItem("Remove player picture");
            removeItem.Click += (s, e) => RemovePlayerPicture();
            menu.Items.Add(removeItem);

            this.ContextMenuStrip = menu;
            pictureBox1.ContextMenuStrip = menu;
            lbName.ContextMenuStrip = menu;
        }

        private void RemovePlayerPicture()
        {
            try
            {
                var imagePath = Path.Combine(_sharedImgFolder, $"{_gender}_{_teamFifaCode}_{Player.ShirtNumber}.png");

                if (File.Exists(imagePath))
                {
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = null;

                    File.Delete(imagePath);

                    LoadImage(_teamFifaCode);

                    MessageBox.Show("Player picture removed successfully. Reverted to default image.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("This player is already using the default image.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove player picture: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadImage(_teamFifaCode);
            }
        }

        private void ChangePlayerPicture()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.PNG;*.JPG;*.JPEG)|*.PNG;*.JPG;*.JPEG";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var targetPath = Path.Combine(_sharedImgFolder, $"{_gender}_{_teamFifaCode}_{Player.ShirtNumber}.png");

                        Directory.CreateDirectory(_sharedImgFolder);

                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = null;

                        File.Copy(ofd.FileName, targetPath, overwrite: true);

                        LoadImage(_teamFifaCode);
                        MessageBox.Show("Player image saved successfully to shared resources!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        public void SetFavorite(bool isFavorite)
        {
            IsFavorite = isFavorite;
            lbStar.Visible = isFavorite;

            SetupContextMenu();
        }

        private void PlayerCardControl_Load(object sender, EventArgs e) { }
        private Point _dragStartPoint;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _dragStartPoint = e.Location;
            }
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            BackColor = selected
                ? Color.LightBlue
                : Color.White;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            SetSelected(!IsSelected);
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                if (Math.Abs(e.X - _dragStartPoint.X) > SystemInformation.DragSize.Width ||
                    Math.Abs(e.Y - _dragStartPoint.Y) > SystemInformation.DragSize.Height)
                {
                    DoDragDrop(this, DragDropEffects.Move);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (Control c in Controls)
            {
                c.Click += (s, ev) =>
                {
                    SetSelected(!IsSelected);
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                };

                c.MouseDown += (s, ev) =>
                {
                    if (ev.Button == MouseButtons.Left)
                        _dragStartPoint = ev.Location;
                };

                c.MouseMove += (s, ev) =>
                {
                    if (ev.Button == MouseButtons.Left)
                    {
                        if (Math.Abs(ev.X - _dragStartPoint.X) > SystemInformation.DragSize.Width ||
                            Math.Abs(ev.Y - _dragStartPoint.Y) > SystemInformation.DragSize.Height)
                        {
                            DoDragDrop(this, DragDropEffects.Move);
                        }
                    }
                };
            }
        }
    }

}
