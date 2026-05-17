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
    public partial class StartupForm : Form
    {
        private readonly SelectionService _selectionService;
        public StartupForm(SelectionService selectionService)
        {
            InitializeComponent();
            _selectionService = selectionService;

            this.AcceptButton = btnConfirm;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string gender = rbFemale.Checked ? "female" : "male";

            _selectionService.Save(gender, String.Empty, new Dictionary<string, List<string>>());

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
