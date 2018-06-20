using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace EvalCSCode
{
	/// <summary>
	/// Zusammenfassung f? Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Erforderlich f? die Windows Form-Designerunterst?zung
			//
			InitializeComponent();
			//textBox5.Text = sToEval;

			//
			// TODO: F?en Sie den Konstruktorcode nach dem Aufruf von InitializeComponent hinzu
			//
		}

		/// <summary>
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Vom Windows Form-Designer generierter Code
		/// <summary>
		/// Erforderliche Methode f? die Designerunterst?zung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor ge?dert werden.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(557, 69);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 25);
            this.button1.TabIndex = 0;
            this.button1.Text = "Eval code";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(10, 60);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(528, 35);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "MessageBox.Show(\"Hello Says Eval\", \"HyFromEval\", MessageBoxButtons.OK, MessageBox" +
    "Icon.Information);";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Simple Eval";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(134, 164);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(404, 34);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "MessageBox.Show(\"Hello Says Eval\");";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(10, 164);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(120, 21);
            this.textBox3.TabIndex = 4;
            this.textBox3.Text = "Param1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(557, 172);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 25);
            this.button2.TabIndex = 5;
            this.button2.Text = "Eval code";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(10, 267);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(528, 26);
            this.textBox4.TabIndex = 6;
            this.textBox4.Text = "EvalCSCode.EvalCSCode.callAFunc();";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(557, 267);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 25);
            this.button3.TabIndex = 7;
            this.button3.Text = "Eval code";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(10, 370);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(528, 78);
            this.textBox5.TabIndex = 8;
            this.textBox5.Text = "EvalCSCode.EvalCSCode elc = new EvalCSCode.EvalCSCode();\nelc.callBFunc();";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(557, 370);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(90, 25);
            this.button4.TabIndex = 9;
            this.button4.Text = "Eval code";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Eval with param";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 215);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(230, 18);
            this.label4.TabIndex = 12;
            this.label4.Text = "Eval external reference (static)";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(10, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(528, 34);
            this.label5.TabIndex = 13;
            this.label5.Text = "In this example you can test a simple c# command or execute short code snippets. " +
    "Most of the standard system includes are made but none to the parent assembly";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(10, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(528, 35);
            this.label6.TabIndex = 14;
            this.label6.Text = "This example first displays what you submit as param in a MessageBox an then exec" +
    "utes  the  code. Includes are the same as in simple eval";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(10, 233);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(528, 34);
            this.label7.TabIndex = 15;
            this.label7.Text = "This example shows how to call a static function in a external namespace (from th" +
    "e evaluated code sight). Here we include the parent assembly for use by eval cod" +
    "e";
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(10, 310);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(230, 17);
            this.label8.TabIndex = 16;
            this.label8.Text = "Eval external reference (non-static)";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(10, 327);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(528, 43);
            this.label9.TabIndex = 17;
            this.label9.Text = resources.GetString("label9.Text");
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(662, 469);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Der Haupteinstiegspunkt f? die Anwendung.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			//EvalCSCode elp = new EvalCSCode();
			EvalCSCode.Eval(textBox1.Text);
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			EvalCSCode.EvalWithParam(textBox2.Text,((object)textBox3.Text));
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			EvalCSCode.EvalWithRef(textBox4.Text);
		}

		string sToEval = "EvalCSCode.EvalCSCode elc = new EvalCSCode.EvalCSCode();\nelc.callBFunc();";
		private void button4_Click(object sender, System.EventArgs e)
		{
			//string sTest = textBox5.Text;
			EvalCSCode.EvalWithRef(textBox5.Text);
		}
	}
}
