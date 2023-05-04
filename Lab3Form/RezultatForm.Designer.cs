using Lab3;
using System.Globalization;
namespace Lab3Form
{
    partial class RezultatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public RezultatForm(Anova a)
        {
            InitializeComponent();
            label11.Text = "SSA=" + (a.SSA1).ToString("F5");
            label12.Text = "SSE=" + (a.SSE1).ToString("F5");
            label13.Text = "SSR=" + (a.SST1).ToString("F5");
            label14.Text = "k-1=" + a.DfAlt;
            label15.Text = "k(n-1)=" + a.DfErr;
            int x = (a.K * a.N) - 1;
            label16.Text = "kn-1=" + x;
            label17.Text = "Sa^2=" + (a.VarSa).ToString("F5");
            label18.Text = "Se^2=" + (a.VarSe).ToString("F5");
            label19.Text = (a.F_Test).ToString("F5") + "";
            label20.Text = a.F_Table.ToString("F5") + "";
            label21.Text = "[" + (a.Interval.getItem1()).ToString("F5") + " ; " + (a.Interval.getItem2()).ToString("F5") + "]";
        }
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            label16 = new Label();
            label17 = new Label();
            label18 = new Label();
            label19 = new Label();
            label20 = new Label();
            label21 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(53, 57);
            label1.Name = "label1";
            label1.Size = new Size(108, 20);
            label1.TabIndex = 0;
            label1.Text = "Izvor varijacije ";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(227, 57);
            label2.Name = "label2";
            label2.Size = new Size(81, 20);
            label2.TabIndex = 1;
            label2.Text = "Alternative";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(417, 57);
            label3.Name = "label3";
            label3.Size = new Size(53, 20);
            label3.TabIndex = 2;
            label3.Text = "Greska";
            label3.Click += label3_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(607, 57);
            label4.Name = "label4";
            label4.Size = new Size(60, 20);
            label4.TabIndex = 3;
            label4.Text = "Ukupno";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(53, 119);
            label5.Name = "label5";
            label5.Size = new Size(107, 20);
            label5.TabIndex = 4;
            label5.Text = "Suma kvadrata";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(53, 188);
            label6.Name = "label6";
            label6.Size = new Size(146, 20);
            label6.TabIndex = 5;
            label6.Text = "Broj stepeni slobode";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(53, 246);
            label7.Name = "label7";
            label7.Size = new Size(68, 20);
            label7.TabIndex = 6;
            label7.Text = "Varijanse";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(53, 310);
            label8.Name = "label8";
            label8.Size = new Size(153, 20);
            label8.TabIndex = 7;
            label8.Text = "Tabelarna F vrijednost";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(53, 359);
            label9.Name = "label9";
            label9.Size = new Size(157, 20);
            label9.TabIndex = 8;
            label9.Text = "Izracunata F vrijednost";
            label9.Click += label9_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(53, 403);
            label10.Name = "label10";
            label10.Size = new Size(132, 20);
            label10.TabIndex = 9;
            label10.Text = "Interval povjerenja";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(227, 119);
            label11.Name = "label11";
            label11.Size = new Size(58, 20);
            label11.TabIndex = 21;
            label11.Text = "label11";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(417, 119);
            label12.Name = "label12";
            label12.Size = new Size(58, 20);
            label12.TabIndex = 22;
            label12.Text = "label12";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(607, 119);
            label13.Name = "label13";
            label13.Size = new Size(58, 20);
            label13.TabIndex = 23;
            label13.Text = "label13";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(227, 188);
            label14.Name = "label14";
            label14.Size = new Size(58, 20);
            label14.TabIndex = 24;
            label14.Text = "label14";
            label14.Click += label14_Click;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(417, 188);
            label15.Name = "label15";
            label15.Size = new Size(58, 20);
            label15.TabIndex = 25;
            label15.Text = "label15";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(609, 188);
            label16.Name = "label16";
            label16.Size = new Size(58, 20);
            label16.TabIndex = 26;
            label16.Text = "label16";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(227, 246);
            label17.Name = "label17";
            label17.Size = new Size(58, 20);
            label17.TabIndex = 27;
            label17.Text = "label17";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(417, 246);
            label18.Name = "label18";
            label18.Size = new Size(58, 20);
            label18.TabIndex = 28;
            label18.Text = "label18";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(227, 310);
            label19.Name = "label19";
            label19.Size = new Size(58, 20);
            label19.TabIndex = 29;
            label19.Text = "label19";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(227, 359);
            label20.Name = "label20";
            label20.Size = new Size(58, 20);
            label20.TabIndex = 30;
            label20.Text = "label20";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(227, 403);
            label21.Name = "label21";
            label21.Size = new Size(58, 20);
            label21.TabIndex = 31;
            label21.Text = "label21";
            // 
            // RezultatForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label21);
            Controls.Add(label20);
            Controls.Add(label19);
            Controls.Add(label18);
            Controls.Add(label17);
            Controls.Add(label16);
            Controls.Add(label15);
            Controls.Add(label14);
            Controls.Add(label13);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "RezultatForm";
            Text = "RezultatForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label20;
        private Label label21;
    }
}