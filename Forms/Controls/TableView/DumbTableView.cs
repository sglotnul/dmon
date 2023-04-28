using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Dmon.Model;

namespace Dmon
{
    class DumbTableView : TabPage
    {
        private readonly string _verboseName;
        private readonly DataGridView _dataGridView;

        public DumbTableView(string verboseName, RolesRepository rolesRepository)
        {
            _verboseName = verboseName;

            _dataGridView = new DataGridView();

            Initialize();
            UpdateDgv().Wait();
        }

        private async Task UpdateDgv()
        {
            DataGridViewColumn sortedColumn = null;
            SortOrder sortOrder = SortOrder.None;
            int selectedRowIndex = _dataGridView.SelectedRows.Count > 0 ? _dataGridView.SelectedRows[0].Index : -1;

            foreach (DataGridViewColumn col in _dataGridView.Columns)
            {
                if (col.HeaderCell.SortGlyphDirection == SortOrder.None) continue;

                sortedColumn = col;
                sortOrder = col.HeaderCell.SortGlyphDirection;

                break;
            }

            _dataGridView.Visible = false;

            _dataGridView.Columns.Clear();
            _dataGridView.Rows.Clear();

            _dataGridView.Columns.Add("c1", "c1");
            _dataGridView.Columns.Add("c2", "c2");
            _dataGridView.Columns.Add("c3", "c3");

            if (sortedColumn != null)
                _dataGridView.Columns[sortedColumn.Index].HeaderCell.SortGlyphDirection = sortOrder;

            if (selectedRowIndex != -1 && _dataGridView.Rows.Count > selectedRowIndex)
                _dataGridView.Rows[selectedRowIndex].Selected = true;

            _dataGridView.Visible = true;
        }

        private void Initialize()
        {
            BackgroundImageLayout = ImageLayout.None;
            Cursor = Cursors.Arrow;
            Text = _verboseName;

            InitializeDataGridView();
            Controls.Add(_dataGridView);
        }

        private void InitializeDataGridView()
        {
            _dataGridView.BorderStyle = BorderStyle.None;
            _dataGridView.Cursor = Cursors.Hand;
            _dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            _dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dataGridView.Dock = DockStyle.Fill;
            _dataGridView.BackgroundColor = SystemColors.Control;
            _dataGridView.RowsDefaultCellStyle.SelectionBackColor = Color.LightGray;
            _dataGridView.RowsDefaultCellStyle.SelectionForeColor = Color.DarkSlateGray;
            _dataGridView.AllowUserToAddRows = false;
            _dataGridView.AllowUserToDeleteRows = false;
            _dataGridView.RowHeadersVisible = false;
            _dataGridView.MultiSelect = false;
            _dataGridView.ReadOnly = true;
        }
    }
}
