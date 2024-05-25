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
using System.Xml;
using System.Globalization;
using System.Diagnostics;

namespace coursework
{
    public partial class Form2 : Form
    {
        DataSet ds;
        public Form2()
        {
            InitializeComponent();
           
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
            CalculateTravelTime();
        }
        private void LoadData()
        {
            try
            {
                ds = new DataSet();
                ds.ReadXml("E:\\Курсова робота 2024\\coursework\\coursework\\XMLFile1.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка");
            }
        }

        private void CalculateTravelTime()
        {
            if (ds != null && ds.Tables.Contains("Маршрути"))
            {
                if (!ds.Tables["Маршрути"].Columns.Contains("Тривалість шляху, хв"))
                {
                    ds.Tables["Маршрути"].Columns.Add("Тривалість шляху, хв", typeof(double));
                }

                foreach (DataRow row in ds.Tables["Маршрути"].Rows)
                {
                    string departureTimeStr = row["Час виїзду"].ToString();
                    string arrivalTimeStr = row["Час прибуття"].ToString();

                    if (DateTime.TryParseExact(departureTimeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime departure))
                    {
                        if (DateTime.TryParseExact(arrivalTimeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime arrival))
                        {
                            TimeSpan travelTime = arrival - departure;
                            row["Тривалість шляху, хв"] = travelTime.TotalMinutes;
                        }
                    }
                }

                dataGridView2.DataSource = ds.Tables["Маршрути"];
            }
            else
            {
                MessageBox.Show("Дані не завантажені.", "Помилка");
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (ds == null)
            {                
                LoadData();
            }            
            if (ds != null && ds.Tables.Contains("Маршрути"))
            {                
                string startTimeStr = textBox1.Text;
                string endTimeStr = textBox2.Text;                
                if (DateTime.TryParseExact(startTimeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime) &&
                    DateTime.TryParseExact(endTimeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endTime))
                {                    
                    var filteredRows = ds.Tables["Маршрути"].AsEnumerable()
                        .Where(row =>
                        {
                            DateTime departureTime;
                            bool departureParsed = DateTime.TryParseExact(row.Field<string>("Час виїзду"), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out departureTime);
                            bool isBetweenTimeRange = departureParsed && departureTime.TimeOfDay >= startTime.TimeOfDay && departureTime.TimeOfDay <= endTime.TimeOfDay;
                            bool isMailExchangeUnderLimit = int.TryParse(row.Field<string>("Тривалість обміну пошти, хв"), out int mailExchangeTime) && mailExchangeTime <= 10;
                            return isBetweenTimeRange && isMailExchangeUnderLimit;
                        });                    
                    DataTable filteredTable = filteredRows.Any() ? filteredRows.CopyToDataTable() : ds.Tables["Маршрути"].Clone();                    
                    dataGridView2.DataSource = filteredTable;
                }
                else
                {
                    MessageBox.Show("Будь ласка, введіть коректний час у форматі HH:mm", "Помилка");
                }
            }
            else
            {
                MessageBox.Show("Дані не завантажені.", "Помилка");
            }
        }  

        private void button2_Click(object sender, EventArgs e)
        {
            if (ds == null)
            {                
                LoadData();
            }            
            if (ds != null && ds.Tables.Contains("Маршрути"))
            {                
                var filteredRows = ds.Tables["Маршрути"].AsEnumerable()
                    .Where(row =>
                    {
                        DateTime arrivalTime;
                        bool arrivalParsed = DateTime.TryParseExact(row.Field<string>("Час прибуття"), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalTime);
                        return arrivalParsed && arrivalTime.Hour >= 12;
                    });               
                int count = filteredRows.Count();                
                MessageBox.Show($"Кількість маршрутів з часом прибуття після 12:00: {count}", "Результат");                
                DataTable filteredTable = count > 0 ? filteredRows.CopyToDataTable() : ds.Tables["Маршрути"].Clone();                
                dataGridView2.DataSource = filteredTable;
            }
            else
            {
                MessageBox.Show("Дані не завантажені.", "Помилка");
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (ds == null)
            {               
                LoadData();
                CalculateTravelTime();
            }            
            if (ds != null && ds.Tables.Contains("Маршрути"))
            {                
                double maxDuration = ds.Tables["Маршрути"].AsEnumerable()
                    .Max(row => Convert.ToDouble(row.Field<string>("Тривалість обміну пошти, хв")));                
                var longestRoute = ds.Tables["Маршрути"].AsEnumerable()
                    .Where(row => Convert.ToDouble(row.Field<string>("Тривалість обміну пошти, хв")) == maxDuration)
                    .FirstOrDefault();                
                if (longestRoute != null)
                {                    
                    DataTable longestRouteTable = longestRoute.Table.Clone();
                    longestRouteTable.ImportRow(longestRoute);                    
                    dataGridView2.DataSource = longestRouteTable;
                }
                else
                {
                    MessageBox.Show("Маршрути не знайдено.", "Помилка");
                }
            }
            else
            {
                MessageBox.Show("Дані не завантажені.", "Помилка");
            }
        }
        private void SaveToFile(DataSet dataSet, string fileName)
        {
            try
            {
                dataSet.WriteXml(fileName);
                MessageBox.Show($"Дані успішно збережені у файлі: {fileName}", "Виконано");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження даних: {ex.Message}", "Помилка");
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            DataSet morningRoutesDataSet = ds.Clone();
            DataTable morningRoutesDataTable = morningRoutesDataSet.Tables["Маршрути"];
            morningRoutesDataTable.Clear(); 
            foreach (DataRow row in ds.Tables["Маршрути"].Rows)
            {
                string departureTimeStr = row["Час виїзду"].ToString();
                if (DateTime.TryParseExact(departureTimeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime departure))
                {
                    if (departure.Hour < 12)
                    {
                        morningRoutesDataTable.ImportRow(row);
                    }
                }
            }            
            SaveToFile(morningRoutesDataSet, "E:\\Курсова робота 2024\\coursework\\coursework\\XMLFile2.xml");
        }
              
        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView2.DataSource != null)
            {
                dataGridView2.DataSource = null; 
            }
            else
            {
                dataGridView2.Rows.Clear(); 
            }
        }
    

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();                      
            Form1 mainForm = (Form1)Application.OpenForms["Form1"];
            mainForm.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataSet nightRoutesDataSet = ds.Clone();
            DataTable morningRoutesDataTable = nightRoutesDataSet.Tables["Маршрути"];
            morningRoutesDataTable.Clear(); 

            foreach (DataRow row in ds.Tables["Маршрути"].Rows)
            {
                string departureTimeStr = row["Час виїзду"].ToString();
                if (DateTime.TryParseExact(departureTimeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime departure))
                {
                    if (departure.Hour > 12)
                    {
                        morningRoutesDataTable.ImportRow(row);
                    }
                }
            }           
            SaveToFile(nightRoutesDataSet, "E:\\Курсова робота 2024\\coursework\\coursework\\XMLFile3.xml");
        }
    }
}
