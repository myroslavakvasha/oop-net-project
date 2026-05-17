using DataLayer.Services;
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
    public partial class SettingsForm : Form
    {
        private readonly SelectionService _selectionService;

        public SettingsForm()
        {
            InitializeComponent();
            _selectionService = new SelectionService();

            // 1. Force a strict reload from disk
            _selectionService.Load();

            // 2. Clear both out explicitly 
            rbMale.Checked = false;
            rbFemale.Checked = false;

            // 3. Set the checks safely inside the constructor before WinForms sets autofocus
            string activeGender = _selectionService.Gender?.Trim().ToLower() ?? "male";
            if (activeGender == "female")
            {
                rbFemale.Checked = true;
            }
            else
            {
                rbMale.Checked = true;
            }

            SetupKeyboardShortcuts();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // Leave this completely blank! Do not put any checkbox logic here.
        }

        private void SetupKeyboardShortcuts()
        {
            // Bind Enter key to Confirm and Esc key to Cancel as requested by the assignment
            this.AcceptButton = btnConfirm;
            this.CancelButton = btnCancel;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // Request confirmation from the user to set the priority
            var dialogResult = MessageBox.Show(
                "Are you sure you want to save these settings?",
                "Confirm Settings Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string selectedGender = rbFemale.Checked ? "female" : "male";

                // Save the configuration back to the file system using your SelectionService structure
                _selectionService.Save(
                    selectedGender,
                    _selectionService.Team,
                    _selectionService.FavoritesByTeam
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
