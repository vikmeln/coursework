using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace coursework
{
    public partial class Form1 : Form
    {
        Form2 fr2;
        public Form1()
        {
            InitializeComponent();
            fr2 = new Form2();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Заполните все поля.", "Ошибка.");
            }
            else
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = textBox1.Text;
                dataGridView1.Rows[n].Cells[1].Value = textBox4.Text;
                dataGridView1.Rows[n].Cells[2].Value = textBox2.Text;
                dataGridView1.Rows[n].Cells[3].Value = textBox3.Text;
                dataGridView1.Rows[n].Cells[4].Value = textBox5.Text;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.", "Ошибка.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.TableName = "Маршрути";
                dt.Columns.Add("Номер маршрута");
                dt.Columns.Add("Пункт призначення");
                dt.Columns.Add("Час виїзду");
                dt.Columns.Add("Час прибуття");
                dt.Columns.Add("Тривалість обміну пошти, хв");
                ds.Tables.Add(dt);
                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    DataRow row = ds.Tables["Маршрути"].NewRow();
                    row["Номер маршрута"] = r.Cells[0].Value;
                    row["Пункт призначення"] = r.Cells[1].Value;
                    row["Час виїзду"] = r.Cells[2].Value;
                    row["Час прибуття"] = r.Cells[3].Value;
                    row["Тривалість обміну пошти, хв"] = r.Cells[4].Value;
                    ds.Tables["Маршрути"].Rows.Add(row);
                }
                ds.WriteXml("E:\\Курсова робота 2024\\coursework\\coursework\\XMLFile1.xml");
                MessageBox.Show("XML файл успешно сохранен.", "Выполнено.");
            }
            catch
            {
                MessageBox.Show("Невозможно сохранить XML файл.", "Ошибка.");
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            fr2.Show();
        }
    }
}
