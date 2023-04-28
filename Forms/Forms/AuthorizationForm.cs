using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading.Tasks;
using Dmon.Model;

namespace Dmon
{
    public partial class AuthorizationForm : Form
    {
        private readonly ITable _table;
        private readonly SqlConnection _connection;

        public AuthorizationForm(SqlConnection connection)
        {
            _connection = connection;
            _table = new TableBuilder(connection).BuildUsersTable();

            InitializeComponent();
            button1.Click += (s, e) => Autorize();
        }

        private async void Autorize()
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            label1.Enabled = false;
            label2.Enabled = false;
            button1.Enabled = false;
            label3.Visible = true;

            try
            {
                var username = textBox1.Text;
                var password = textBox2.Text;

                if (!await ValidateUser(username, password))
                {
                    MessageBox.Show("Неверное имя пользователя или пароль");
                }
                else
                {
                    var form = new MainForm(_connection, username);
                    form.Show();

                    Visible = false;
                    form.FormClosed += (o, s) =>
                    {
                        _connection.Close();
                        Close();
                    };
                }
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");
            }
            finally
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                label1.Enabled = true;
                label2.Enabled = true;
                button1.Enabled = true;
                label3.Visible = false;
            }
        }

        private async Task<bool> ValidateUser(string username, string password)
        {
            return await _table
                .Exists()
                .Where("username", username)
                .Where("password", password)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }
    }
}