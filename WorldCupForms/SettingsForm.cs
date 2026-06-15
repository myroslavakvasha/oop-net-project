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

            _selectionService.Load();

            rbMale.Checked = false;
            rbFemale.Checked = false;

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
        }

        private void SetupKeyboardShortcuts()
        {
            this.AcceptButton = btnConfirm;
            this.CancelButton = btnCancel;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(
                "Are you sure you want to save these settings?",
                "Confirm Settings Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string selectedGender = rbFemale.Checked ? "female" : "male";

                string teamToSave = selectedGender == "female"
                    ? _selectionService.FemaleTeam
                    : _selectionService.MaleTeam;

                _selectionService.Save(
                    selectedGender,
                    teamToSave,
                    string.Empty,
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
