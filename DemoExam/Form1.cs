using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DemoExam
{
    public partial class LoginForm: Form
    {
        private string connectionString = @"Data source =WINHOME-KUACF9H; Initial Catalog= Shoe; Integrated Security= True;";
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string query = @"
                            SELECT U.full_name, U.role_name
                            FROM Users U
                            WHERE U.email = @Login AND U.Password_hash = @Password";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using(SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fullName = reader["full_name"].ToString();
                                string roleName = reader["role_name"].ToString();
                                MessageBox.Show($"Добро пожаловать {fullName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Form2 form2 = new Form2(fullName, roleName);
                                form2.Show();
                                this.Hide();

                            }
                            else MessageBox.Show("Неверно введены данные", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                        }
                    }catch(Exception ex)
                    {
                        MessageBox.Show($"Ошибка подключение к бд:{ex.Message}","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnGuest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Добро пожаловать гость", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Form2 form2 = new Form2("Гость", "Гость");
            form2.Show();
            this.Hide();
        }
    }
}
