using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab8_MathProg
{
    public partial class Form1 : Form
    {
        private BalancedTSPAnalyzer solver;
        private List<Point> cityLocations;
        private List<int> optimalPath;
        public Form1()
        {
            InitializeComponent();
            txtOutput.Multiline = true;
            txtOutput.ScrollBars = ScrollBars.Both;
            txtOutput.Dock = DockStyle.Fill;
            txtOutput.ReadOnly = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1_ValueChanged(sender, e);
            //graphPanel.BackColor = Color.White;
            //graphPanel.BorderStyle = BorderStyle.FixedSingle;
            //graphPanel.Paint += GraphPanel_Paint;
        }
        private void btnSolve_Click(object sender, EventArgs e)
        {
            //int[,] matrix = new int[,]
            //{
            //    { 0, 5, 1, 4 },
            //    { 2, 0, 3, 5 },
            //    { 1, 4, 0, 2 },
            //    { 3, 5, 4, 0 }
            //};
            //int[,] matrix = new int[,]
            //{
            //{ 0, 2, 7, 4 ,8},
            //{ 3, 0, 7, 3 ,8},
            //{ 3, 6, 0, 1 ,4},
            //{ 3, 5, 6, 0 ,3},
            //{ 1, 3, 7, 2 ,0},
            //};
            int size = dataGridView1.RowCount;
            int[,] matrix = new int[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[i, j] = Convert.ToInt32(dataGridView1[j,i].Value);

            solver = new BalancedTSPAnalyzer(matrix);
            var result = solver.FindOptimalPath();

            // Выводим результат в TextBox
            solver.PrintAnalysis(txtOutput);
            optimalPath =solver.optimalPath;
            optimalPath.RemoveAt(optimalPath.Count - 1);
            //drawButton.Enabled = true;

            //// Calculate city positions for left-to-right layout
            //CalculateCityPositions();
            //graphPanel.Invalidate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            int size = (int)numericUpDown1.Value;
            for (int i = 0; i < size; i++)
                dataGridView1.Columns.Add(i + "", "");
            for (int i = 0; i < size; i++)
                dataGridView1.Rows.Add();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    dataGridView1[i, j].Value = 0;
            for (int i = 0; i < size; i++)
            {
                dataGridView1[i, i].ReadOnly = true;
                dataGridView1[i, i].Style.BackColor = SystemColors.ControlDark;
            }
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int size = dataGridView1.RowCount;
            Random random = new Random();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if(i!=j) 
                        dataGridView1[i, j].Value = random.Next(10);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        //private void CalculateCityPositions()
        //{
        //    if (solver == null) return;

        //    int cityCount = solver.distances.GetLength(0);
        //    cityLocations = new List<Point>();

        //    int panelWidth = graphPanel.Width - 100;
        //    int panelHeight = graphPanel.Height - 100;

        //    // Start city (leftmost)
        //    cityLocations.Add(new Point(50, panelHeight / 2));

        //    // Intermediate cities (spaced horizontally)
        //    for (int i = 1; i < cityCount; i++)
        //    {
        //        int x = 50 + (i * panelWidth / (cityCount + 1));
        //        int y = 50 + (i * panelHeight / (cityCount + 1));
        //        cityLocations.Add(new Point(x, y));
        //    }

        //    // Adjust last city to be on the right
        //    if (cityCount > 1)
        //    {
        //        cityLocations[cityCount - 1] = new Point(panelWidth, panelHeight / 2);
        //    }
        //}
        //private void GraphPanel_Paint(object sender, PaintEventArgs e)
        //{
        //    if (cityLocations == null || optimalPath == null)
        //        return;

        //    Graphics g = e.Graphics;
        //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        //    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        //    // Draw all possible paths (gray)
        //    for (int from = 0; from < solver.distances.GetLength(0); from++)
        //    {
        //        for (int to = 0; to < solver.distances.GetLength(1); to++)
        //        {
        //            if (from != to && solver.distances[from, to] > 0)
        //            {
        //                DrawArrow(g, cityLocations[from], cityLocations[to],
        //                         Color.LightGray, 1, solver.distances[from, to].ToString(),
        //                         drawLabel: false);
        //            }
        //        }
        //    }

        //    // Draw the optimal path (red)
        //    if (optimalPath.Count > 1)
        //    {
        //        for (int i = 0; i < optimalPath.Count - 1; i++)
        //        {
        //            int from = optimalPath[i];
        //            int to = optimalPath[i + 1];
        //            DrawArrow(g, cityLocations[from], cityLocations[to],
        //                     Color.Red, 3, solver.distances[from, to].ToString());
        //        }
        //    }

        //    // Draw cities
        //    for (int i = 0; i < cityLocations.Count; i++)
        //    {
        //        Color cityColor = i == 0 ? Color.Green : (i == cityLocations.Count - 1 ? Color.Red : Color.Blue);

        //        g.FillEllipse(new SolidBrush(cityColor), cityLocations[i].X - 15, cityLocations[i].Y - 15, 30, 30);
        //        g.DrawString((i + 1).ToString(),
        //                    new Font("Arial", 10, FontStyle.Bold),
        //                    Brushes.White,
        //                    new PointF(cityLocations[i].X - 5, cityLocations[i].Y - 8));
        //    }
        //}

        //private void DrawArrow(Graphics g, Point from, Point to, Color color, int width, string label, bool drawLabel = true)
        //{
        //    using (Pen pen = new Pen(color, width))
        //    {
        //        // Draw line
        //        g.DrawLine(pen, from, to);

        //        // Draw arrow head
        //        float angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        //        PointF arrowPoint1 = new PointF(
        //            to.X - 15 * (float)Math.Cos(angle + Math.PI / 6),
        //            to.Y - 15 * (float)Math.Sin(angle + Math.PI / 6));
        //        PointF arrowPoint2 = new PointF(
        //            to.X - 15 * (float)Math.Cos(angle - Math.PI / 6),
        //            to.Y - 15 * (float)Math.Sin(angle - Math.PI / 6));

        //        g.FillPolygon(new SolidBrush(color), new PointF[] { to, arrowPoint1, arrowPoint2 });

        //        // Draw distance label
        //        if (drawLabel)
        //        {
        //            PointF midPoint = new PointF(
        //                (from.X + to.X) / 2 + 10,
        //                (from.Y + to.Y) / 2 + 10);

        //            g.DrawString(label, this.Font, new SolidBrush(color), midPoint);
        //        }
        //    }
        //}

        //private void drawButton_Click(object sender, EventArgs e)
        //{
        //    graphPanel.Invalidate();
        //}
    }
}
