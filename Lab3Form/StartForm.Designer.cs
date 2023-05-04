using Lab3Form;
using Lab3;
namespace Lab3Form
{
    partial class StartForm : Form
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

        //   #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            anovaVjerovatnoca = new TextBox();
            alt1 = new TextBox();
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // anovaVjerovatnoca
            // 
            anovaVjerovatnoca.Location = new Point(79, 103);
            anovaVjerovatnoca.Name = "anovaVjerovatnoca";
            anovaVjerovatnoca.Size = new Size(125, 27);
            anovaVjerovatnoca.TabIndex = 0;
            anovaVjerovatnoca.TextAlign = HorizontalAlignment.Center;
            anovaVjerovatnoca.TextChanged += anovaVjerovatnoca_TextChanged;
            // 
            // alt1
            // 
            alt1.Location = new Point(79, 197);
            alt1.Name = "alt1";
            alt1.Size = new Size(125, 27);
            alt1.TabIndex = 1;
            alt1.TextAlign = HorizontalAlignment.Center;
            alt1.TextChanged += alt1_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(89, 325);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 2;
            button1.Text = "Unos";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(89, 80);
            label1.Name = "label1";
            label1.Size = new Size(98, 20);
            label1.TabIndex = 3;
            label1.Text = "Broj mjerenja";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(89, 174);
            label2.Name = "label2";
            label2.Size = new Size(110, 20);
            label2.TabIndex = 4;
            label2.Text = "Broj alternative";
            label2.Click += label2_Click;
            // 
            // StartForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(286, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(alt1);
            Controls.Add(anovaVjerovatnoca);
            Name = "StartForm";
            Text = "StartForm";
            Load += StartForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        //#

        private void button1_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(anovaVjerovatnoca.Text) && !string.IsNullOrEmpty(alt1.Text))
            {

                UpisForm upis = new UpisForm(new Anova(Convert.ToInt32(anovaVjerovatnoca.Text), Convert.ToInt32(alt1.Text)));
                this.Hide();
                upis.Show();
            }

        }

        private TextBox anovaVjerovatnoca;
        private TextBox alt1;


        private Button button1;
        private Label label1;
        private Label label2;
    }
}