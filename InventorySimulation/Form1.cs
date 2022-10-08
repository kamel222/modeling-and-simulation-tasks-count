using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryTesting;
using InventoryModels;
namespace InventorySimulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
         }

        private void Form1_Load(object sender, EventArgs e)
        {
            SimulationSystem d = new SimulationSystem();
            d.readfile("TestCase1.txt");
            d.sim();
            dataGridView1.DataSource = d.SimulationCases;
            string test = TestingManager.Test(d, Constants.FileNames.TestCase1);
            MessageBox.Show(test);
            textBox1.Text = d.PerformanceMeasures.EndingInventoryAverage.ToString();
            textBox2.Text = d.PerformanceMeasures.ShortageQuantityAverage.ToString();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
