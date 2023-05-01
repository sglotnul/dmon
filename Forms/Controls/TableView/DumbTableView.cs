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
        private static IDictionary<string, string> _columns => new Dictionary<string, string>
        {
            { "id", "Id" },
            { "name", "Название" },
            { "price", "Цена" },
            { "categoryName", "Категория" }
        };

        private readonly ContextMenu _contextMenu;
        private readonly DataGridView _dataGridView;
        private readonly SplitContainer _splitContainer;
        private readonly GroupedControlWithValue _groupedControl;
        private readonly Label _label;
        private readonly RolesRepository _rolesRepository;
        private readonly string _verboseName;
        private readonly DumbRepository _dumbRepository;

        private bool canWrite = true;

        public DumbTableView(string verboseName, DumbRepository dumbRepository, RolesRepository rolesRepository)
        {
            _verboseName = verboseName;
            _dumbRepository = dumbRepository;
            _rolesRepository = rolesRepository;

            _splitContainer = new SplitContainer();
            _dataGridView = new DataGridView();
            _contextMenu = new ContextMenu();
            _label = new Label();
            
            _groupedControl = new GroupedControlWithValue(GetTextBox(), tb => tb.Text)
            { 
                Text = "Местоположение",
                Dock = DockStyle.Top
            };

            Initialize();
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

            object[][] rowData;

            try
            {
                rowData = await GetRowDataAsync(sortedColumn?.Name, sortOrder);
            }
            catch (UnauthorizedAccessException)
            {
                _label.Text = "Недотаточно прав";
                return;
            }
            catch (ApplicationException ex)
            {

                MessageBox.Show(ex.Message);
                return;
            }
            catch (System.Data.SqlClient.SqlException)
            {

                _label.Text = "Что-то пошло не так";
                return;
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");
                return;
            }

            _dataGridView.Columns.Clear();
            _dataGridView.Rows.Clear();

            foreach (var keypair in _columns)
                _dataGridView.Columns.Add(keypair.Key, keypair.Value);

            if (sortedColumn != null)
                _dataGridView.Columns[sortedColumn.Index].HeaderCell.SortGlyphDirection = sortOrder;

            foreach (var data in rowData)
                _dataGridView.Rows.Add(data);

            if (selectedRowIndex != -1 && _dataGridView.Rows.Count > selectedRowIndex)
                _dataGridView.Rows[selectedRowIndex].Selected = true;

            _dataGridView.Visible = true;
        }

        private async Task<object[][]> GetRowDataAsync(string sortedColumn, SortOrder sortOrder)
        {
            var permissions = await _rolesRepository.GetUserPermissionsAsync("Products");
            canWrite = (permissions & UserPermissions.Write) != 0;
            if ((permissions & UserPermissions.Read) == 0)
                throw new UnauthorizedAccessException();

            DataTable dataTable;
            var rows = new List<object[]>();

            var location = _groupedControl.Controls[0].Text;

            if (!string.IsNullOrEmpty(sortedColumn))
            {
                dataTable = await _dumbRepository.GetProductsFromLocation(
                    location,
                    sortedColumn,
                    sortOrder == SortOrder.Ascending);
            }
            else
            {
                dataTable = await _dumbRepository.GetProductsFromLocation(
                    location,
                    "id",
                    sortOrder == SortOrder.Ascending);
            }
            
            foreach (DataRow row in dataTable.Rows)
                rows.Add(row.ItemArray);

            return rows.ToArray();
        }

        private void OnDataGridViewSortCompare(object sender, DataGridViewSortCompareEventArgs args)
        {
            var value1 = args.CellValue1 != null ? args.CellValue1.ToString() : string.Empty;
            var value2 = args.CellValue2 != null ? args.CellValue2.ToString() : string.Empty;

            args.SortResult = string.Compare(value1, value2);
            args.Handled = true;
        }

        private void OnSorted(object sender, EventArgs args)
        {
            foreach (DataGridViewColumn c in _dataGridView.Columns)
            {
                if (c != _dataGridView.SortedColumn)
                    c.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        private async void OnEnter(object sender, EventArgs args)
        {
            if (_dataGridView.Columns.Count != 0) return;

            await UpdateDgv();
        }

        private async void OnFilter(object sender, EventArgs args)
        {
            await UpdateDgv();
        }

        private void Initialize()
        {
            BackgroundImageLayout = ImageLayout.None;
            Cursor = Cursors.Arrow;
            Text = _verboseName;

            InitializeSplitContainer();
            Controls.Add(_splitContainer);

            Enter += OnEnter;
        }

        private void InitializeSplitContainer()
        {
            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.SplitterDistance = 800;

            _label.Text = "Подождите...";
            _label.AutoSize = true;
            _label.TextAlign = ContentAlignment.MiddleCenter;
            _label.Location = new Point(60, 38);
            _label.Anchor = AnchorStyles.None;
            _label.Font = new Font("Microsoft Sans Serif", 14F);

            InitializeDataGridView();
            _splitContainer.Panel1.Controls.Add(_dataGridView);
            _splitContainer.Panel1.Controls.Add(_label);
            
            _splitContainer.Panel1.ContextMenu = _contextMenu;
            
            _splitContainer.Panel2.Controls.Add(_groupedControl);
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
            
            _dataGridView.SortCompare += OnDataGridViewSortCompare;
            _dataGridView.Sorted += OnSorted;
        }

        private TextBox GetTextBox()
        {
            var tb = new TextBox();
            tb.TextChanged += OnFilter;
            return tb;
        }
    }
}