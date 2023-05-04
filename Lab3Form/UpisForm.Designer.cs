using Lab3;
using MathNet.Numerics.Statistics;
using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Lab3Form
{
    partial class UpisForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        private Anova anova;
        private TextBox alt1;
        private TextBox alt2;
        private TextBox vjerovatnocaKontrast;
        private TextBox anovaVjerovatnoca;
        private Button button1;
        private double[][] matrix;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            anovaVjerovatnoca = new TextBox();
            alt1 = new TextBox();
            alt2 = new TextBox();
            vjerovatnocaKontrast = new TextBox();
            button1 = new Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            matrixContainerPanel = new Panel();
            SuspendLayout();
            // 
            // anovaVjerovatnoca
            // 
            anovaVjerovatnoca.Location = new Point(43, 379);
            anovaVjerovatnoca.Name = "anovaVjerovatnoca";
            anovaVjerovatnoca.Size = new Size(167, 27);
            anovaVjerovatnoca.TabIndex = 1;
            anovaVjerovatnoca.TextAlign = HorizontalAlignment.Center;
            anovaVjerovatnoca.TextChanged += anovaVjerovatnoca_TextChanged;
            // 
            // alt1
            // 
            alt1.Location = new Point(672, 361);
            alt1.Name = "alt1";
            alt1.Size = new Size(167, 27);
            alt1.TabIndex = 2;
            alt1.TextAlign = HorizontalAlignment.Center;
            alt1.TextChanged += alt1_TextChanged;
            // 
            // alt2
            // 
            alt2.Location = new Point(672, 454);
            alt2.Name = "alt2";
            alt2.Size = new Size(167, 27);
            alt2.TabIndex = 3;
            alt2.TextAlign = HorizontalAlignment.Center;
            alt2.TextChanged += alt2_TextChanged;
            // 
            // vjerovatnocaKontrast
            // 
            vjerovatnocaKontrast.Location = new Point(43, 454);
            vjerovatnocaKontrast.Name = "vjerovatnocaKontrast";
            vjerovatnocaKontrast.Size = new Size(167, 27);
            vjerovatnocaKontrast.TabIndex = 4;
            vjerovatnocaKontrast.TextAlign = HorizontalAlignment.Center;
            vjerovatnocaKontrast.TextChanged += vjerovatnocaKontrast_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(394, 419);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 5;
            button1.Text = "Izracunaj";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(53, 338);
            label1.Name = "label1";
            label1.Size = new Size(148, 20);
            label1.TabIndex = 7;
            label1.Text = "ANOVA vjerovatnoca";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(67, 419);
            label2.Name = "label2";
            label2.Size = new Size(96, 20);
            label2.TabIndex = 8;
            label2.Text = "Vjerovatnoca";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(705, 419);
            label3.Name = "label3";
            label3.Size = new Size(93, 20);
            label3.TabIndex = 9;
            label3.Text = "Alternativa 2";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(705, 338);
            label4.Name = "label4";
            label4.Size = new Size(93, 20);
            label4.TabIndex = 10;
            label4.Text = "Alternativa 1";
            label4.Click += label4_Click;
            // 
            // matrixContainerPanel
            // 
            matrixContainerPanel.Location = new Point(308, 87);
            matrixContainerPanel.Name = "matrixContainerPanel";
            matrixContainerPanel.Size = new Size(250, 125);
            matrixContainerPanel.TabIndex = 11;
            // 
            // UpisForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(883, 509);
            Controls.Add(matrixContainerPanel);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(vjerovatnocaKontrast);
            Controls.Add(alt2);
            Controls.Add(alt1);
            Controls.Add(anovaVjerovatnoca);
            Name = "UpisForm";
            Text = "UpisForm";
            Load += UpisForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public UpisForm(Anova a)
        {
            InitializeComponent();
            anova = a;
            GenerateMatrixFields();
        }

        private void GenerateMatrixFields()
        {
            int numRows = anova.N;
            int numCols = anova.K;

            // Clear any existing controls from the matrix container panel
            matrixContainerPanel.Controls.Clear();

            // Loop through each row and create a new panel containing the required number of TextBox controls
            for (int i = 0; i < numRows; i++)
            {
                Panel rowPanel = new Panel();
                rowPanel.Dock = DockStyle.Top;
                rowPanel.Height = matrixContainerPanel.Height / numRows;

                for (int j = 0; j < numCols; j++)
                {
                    TextBox textBox = new TextBox();
                    textBox.Width = rowPanel.Width / numCols;
                    textBox.Location = new Point(j * textBox.Width, 0);
                    rowPanel.Controls.Add(textBox);
                }

                matrixContainerPanel.Controls.Add(rowPanel);
            }
        }
        private double[][] GetMatrixFromPanel()
        {
            // Get the number of rows and columns in the matrix
            int numRows = matrixContainerPanel.Controls.Count;
            int numCols = matrixContainerPanel.Controls[0].Controls.Count;

            // Initialize the matrix array
            double[][] matrix = new double[numRows][];
            for (int i = 0; i < numRows; i++)
            {
                matrix[i] = new double[numCols];
            }

            // Loop through the rows and columns of the panel, and add the values to the matrix
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    double.TryParse(matrixContainerPanel.Controls[i].Controls[j].Text, out matrix[i][j]);
                }
            }

            return matrix;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(alt1.Text) && !string.IsNullOrEmpty(alt2.Text) &&
               !string.IsNullOrEmpty(vjerovatnocaKontrast.Text) &&
               !string.IsNullOrEmpty(anovaVjerovatnoca.Text))
            {
                matrix = GetMatrixFromPanel();
                DataGridView myDataGridView = new DataGridView();

                anova.IzracunajSrednjeVrijednostiKolona(matrix);
                anova.IzracunajEfekat(matrix);
                anova.IzracunajVarijacijeIVarijanse(matrix);
                anova.IzracunajFTabelarno(int.Parse(anovaVjerovatnoca.Text));

                double[] flatMatrix = matrix.SelectMany(row => row).ToArray();
                double location = flatMatrix.Mean();
                double scale = flatMatrix.StandardDeviation();

                anova.IzracunajStudentovuRaspodjelu(int.Parse(vjerovatnocaKontrast.Text), location, scale);
                anova.IzracunajKontrast(int.Parse(alt1.Text), int.Parse(alt2.Text));
                anova.formirajInterval();
                RezultatForm rezultatForm = new RezultatForm(anova);
                this.Hide();
                rezultatForm.Show();
            }
        }
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Panel matrixContainerPanel;
    }
}


