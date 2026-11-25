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
using System.IO;

namespace DemoExam
{
    public partial class Form2: Form
    {
        private string connectionString = @"Data source =WINHOME-KUACF9H; Initial Catalog= Shoe; Integrated Security= True;";
        private string currentRole;
        private string currentName;
        public Form2(string fullName,string role)
        {
            InitializeComponent();
            currentRole = role;
            currentName = fullName;
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (currentRole == "Администратор") lblName.Text = currentName;
            else
            {
                lblName.Text = currentName;
                btnAdmin.Visible = false;
            }
            LoadTovarData();
        }

        private void LoadTovarData()
        {
            string query = @"
                           SELECT *
                           FROM Товары T
                           WHERE 1=1";
            string SearchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(SearchText))
            {
                query += "AND (T.Наименование_товара LIKE @Search OR T.Поставщик LIKE @Search)";
            }
            if (cmbFilter.SelectedIndex == 1) query += "AND T.Категория_товара = N'Мужская обувь'";
            if (cmbFilter.SelectedIndex == 2) query += "AND T.Категория_товара = N'Женская обувь'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Search", "%" + SearchText + "%");
                    try
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dgvTovar.DataSource = dataTable;
                    }catch(Exception ex)
                    {
                        MessageBox.Show($"Ошибка таблицы Товар {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private void dgvTovar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            DataGridViewRow row = dgvTovar.Rows[e.RowIndex];

            lblCT.Text = row.Cells[6].Value?.ToString();
            label1.Text = row.Cells[9].Value?.ToString();
            label2.Text = row.Cells[5].Value?.ToString();
            label3.Text = row.Cells[4].Value?.ToString();
            double discount = Convert.ToDouble(row.Cells["Действующая_скидка"].Value ?? 0);
            label4.Text = row.Cells[3].Value?.ToString();
            double price = Convert.ToDouble(label4.Text);
            if (discount > 1)
            {
                label4.Font = new Font(label4.Font, FontStyle.Strikeout);
                label4.ForeColor = Color.Red;
                label8.Text = (price * (1 - discount / 100)).ToString();
            }
            label5.Text = row.Cells[2].Value?.ToString();
            label6.Text = row.Cells[8].Value?.ToString();
            label7.Text = $"Скидка \n{ row.Cells[7].Value?.ToString()}%";
            string image = row.Cells[10].Value?.ToString();
            

            string imagePath = Path.Combine(Application.StartupPath, "Images", image);
            if (File.Exists(imagePath))
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = Image.FromFile(imagePath);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBox1.Image = null;
                if (pictureBox1.Image == null)
                {
                    pictureBox1.Image?.Dispose();
                    imagePath = Path.Combine(Application.StartupPath, "Images", "picture.png");
                    pictureBox1.Image = Image.FromFile(imagePath);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void dgvTovar_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvTovar.Rows[e.RowIndex];
            int count = Convert.ToInt32(row.Cells["КолВо_на_складе"].Value ?? 0);
            if (count <= 0) { row.DefaultCellStyle.BackColor = Color.LightBlue; return; }
            double discount = Convert.ToDouble(row.Cells["Действующая_скидка"].Value ?? 0);
            row.DefaultCellStyle.BackColor = discount > 15 ? Color.SeaGreen : dgvTovar.DefaultCellStyle.BackColor;
            row.DefaultCellStyle.ForeColor = discount > 15 ? Color.White : dgvTovar.DefaultCellStyle.ForeColor;
            

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadTovarData();
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTovarData();
        }
    }
}
