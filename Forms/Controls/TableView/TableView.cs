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
    internal class TableView : TabPage
    {
        private readonly ContextMenu _contextMenu;
        private readonly DataGridView _dataGridView;
        private readonly SplitContainer _splitContainer;
        private readonly SideMenu _sideMenu;

        private readonly Label _label;
        private readonly ITable _table;
        private readonly string _verboseName;

        public TableView(string verboseName, ITable table)
        {
            _verboseName = verboseName;
            _table = table;

            _splitContainer = new SplitContainer();
            _sideMenu = new SideMenu(_table.ColumnsConfiguration);
            _dataGridView = new DataGridView();
            _contextMenu = new ContextMenu();
            _label = new Label();

            Initialize();
        }

        private async Task DeleteEntryAsync(Dictionary<string, object> fieldsToFilter)
        {
            var query = _table
                .Delete();

            foreach (var f in fieldsToFilter.Keys)
                query.Where(f, fieldsToFilter[f]);

            try
            {
                await query.ExecuteAsync()
                    .ConfigureAwait(false);
            }
            catch
            {
                throw new ApplicationException("Не удалось удалить запись");
            }
        }

        private async Task InsertEntryAsync(DataEventArgs e)
        {
            try
            {
                await _table
                    .Insert(e.Data)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
            catch
            {
                throw new ApplicationException("Не удалось добавить запись");
            }
        }

        private async Task UpdateEntryAsync(DataEventArgs e, Dictionary<string, object> fieldsToFilter)
        {
            var query = _table
                .Update(e.Data);

            foreach (var f in fieldsToFilter.Keys)
                query.Where(f, fieldsToFilter[f]);

            try {
                await query.ExecuteAsync()
                    .ConfigureAwait(false);
            }
            catch
            {
                throw new ApplicationException("Не удалось обновить запись");
            }
        }

        private async void OnDelete()
        {
            if (_dataGridView.SelectedRows.Count == 0) return;

            var row = _dataGridView.SelectedRows[0];

            var fieldsToFilter = new Dictionary<string, object>(row.Cells.Count);

            foreach (DataGridViewCell cell in row.Cells)
            {
                var columnName = _dataGridView.Columns[cell.ColumnIndex].Name;
                var c = _table.ColumnsConfiguration[columnName];

                if (c.IsPrimaryKey)
                    fieldsToFilter[columnName] = cell.Value;
            }

            try
            {
                await DeleteEntryAsync(fieldsToFilter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            _dataGridView.Rows.RemoveAt(row.Index);
        }

        private void OnInsert()
        {
            var data = new List<CellData>(_table.ColumnsConfiguration.Count);

            foreach (var column in _table.ColumnsConfiguration.Keys)
            {
                var c = _table.ColumnsConfiguration[column];

                if (c.IsAutoField) continue;

                data.Add(new CellData(column, c, null));
            }

            var form = ShowEntityForm(data.ToArray(), InsertEntryAsync);
        }

        private void HandleSelectedRow()
        {
            if (_dataGridView.SelectedRows.Count == 0) return;

            var row = _dataGridView.SelectedRows[0];

            var data = new List<CellData>(row.Cells.Count);

            var fieldsToFilter = new Dictionary<string, object>(row.Cells.Count);

            foreach (DataGridViewCell cell in row.Cells)
            {
                var columnName = _dataGridView.Columns[cell.ColumnIndex].Name;
                var c = _table.ColumnsConfiguration[columnName];

                if (c.IsPrimaryKey)
                    fieldsToFilter[columnName] = cell.Value;
                if (c.IsAutoField)
                    continue;

                data.Add(new CellData(columnName, c , cell.Value));
            }

            var form = ShowEntityForm(data.ToArray(), e => UpdateEntryAsync(e, fieldsToFilter));
        }

        private EntityForm ShowEntityForm(CellData[] data, Func<DataEventArgs, Task> onSaveAction)
        {
            var form = new EntityForm(data);
            form.OnSave += async (s, e) =>
            {
                form.Close();

                try
                {
                    await onSaveAction.Invoke(e);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                await UpdateDgv();
            };

            form.ShowDialog();
            return form;
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
                rowData = await GetRowDataAsync(sortedColumn?.Name, sortOrder, _sideMenu.GetFilters());
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

            foreach(var column in _table.ColumnsConfiguration.Keys)
                _dataGridView.Columns.Add(column, _table.ColumnsConfiguration[column].ColumnVerboseName);

            if (sortedColumn != null)
                _dataGridView.Columns[sortedColumn.Index].HeaderCell.SortGlyphDirection = sortOrder;

            foreach (var data in rowData)
                _dataGridView.Rows.Add(data);

            if (selectedRowIndex != -1 && _dataGridView.Rows.Count > selectedRowIndex)
                _dataGridView.Rows[selectedRowIndex].Selected = true;

            _dataGridView.Visible = true;
        }

        private async Task<object[][]> GetRowDataAsync(string sortedColumn, SortOrder sortOrder, ConditionsComponent cmp)
        {
            var rows = new List<object[]>();

            var selectQuery = _table
                .Select(_table.ColumnsConfiguration.Keys.ToArray());

            foreach(var c in cmp.RangeConditions.Keys)
            {
                var range = cmp.RangeConditions[c];
                if (range[0] != null)
                    selectQuery.Greater(c, range[0]);
                if (range[1] != null)
                    selectQuery.Less(c, range[1]);
            }

            foreach (var c in cmp.ContainsConditions.Keys)
            {
                var substring = cmp.ContainsConditions[c];
                selectQuery.Contains(c, substring);
            }

            foreach (var c in cmp.EqualsConditions.Keys)
            {
                var value = cmp.ContainsConditions[c];
                selectQuery.Where(c, value);
            }

            if (!string.IsNullOrEmpty(sortedColumn))
            {
                selectQuery.OrderBy(
                    sortedColumn,
                    sortOrder == SortOrder.Ascending);
            }

            var table = await selectQuery.ExecuteAsync()
                .ConfigureAwait(false);

            foreach (DataRow row in table.Rows)
                rows.Add(row.ItemArray);

            return rows.ToArray();
        }

        private void OnDataGridViewSortCompare(object sender, DataGridViewSortCompareEventArgs args)
        {
            string value1 = args.CellValue1 != null ? args.CellValue1.ToString() : string.Empty;
            string value2 = args.CellValue2 != null ? args.CellValue2.ToString() : string.Empty;

            args.SortResult = string.Compare(value1, value2);
            args.Handled = true;
        }

        private void OnCellClick(object sender, DataGridViewCellEventArgs args)
        {
            if (args.RowIndex != -1)
                HandleSelectedRow();
        }

        private void OnKeyPress(object sender, KeyPressEventArgs args)
        {
            if (args.KeyChar != (char)Keys.Enter)
                return;

            args.Handled = true;
            HandleSelectedRow();
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

            _sideMenu.OnFilter += OnFilter;
            _splitContainer.Panel2.Controls.Add(_sideMenu);

            InitializeDataGridView();
            _splitContainer.Panel1.Controls.Add(_dataGridView);
            _splitContainer.Panel1.Controls.Add(_label);

            InitializeContextMenu();
            _splitContainer.Panel1.ContextMenu = _contextMenu;
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

            _dataGridView.CellDoubleClick += OnCellClick;
            _dataGridView.KeyPress += OnKeyPress;
            _dataGridView.SortCompare += OnDataGridViewSortCompare;
            _dataGridView.Sorted += OnSorted;
        }

        private void InitializeContextMenu()
        {
            var insertMenuItem = new MenuItem() { Text = "Добавить запись" };
            var deleteMenuItem = new MenuItem() { Text = "Удалить запись" };

            insertMenuItem.Click += (o, e) => OnInsert();
            deleteMenuItem.Click += (o, e) => OnDelete();

            var items = new MenuItem[]
            {
                insertMenuItem,
                deleteMenuItem
            };

            _contextMenu.MenuItems.AddRange(items);
        }
    }
}