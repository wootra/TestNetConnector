using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace WindowsApplication4
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private WindowsApplication4.CustomColorCheckBox customColorCheckBox1;
		private WindowsApplication4.CustomColorRadioButton customColorRadioButton1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.customColorCheckBox1 = new WindowsApplication4.CustomColorCheckBox();
			this.customColorRadioButton1 = new WindowsApplication4.CustomColorRadioButton();
			this.SuspendLayout();
			// 
			// customColorCheckBox1
			// 
			this.customColorCheckBox1.CheckColor = System.Drawing.Color.SpringGreen;
			this.customColorCheckBox1.Checked = true;
			this.customColorCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.customColorCheckBox1.Location = new System.Drawing.Point(96, 168);
			this.customColorCheckBox1.Name = "customColorCheckBox1";
			this.customColorCheckBox1.Size = new System.Drawing.Size(104, 64);
			this.customColorCheckBox1.TabIndex = 1;
			this.customColorCheckBox1.Text = "customColorCheckBox1";
			// 
			// customColorRadioButton1
			// 
			this.customColorRadioButton1.CheckColor = System.Drawing.Color.Coral;
			this.customColorRadioButton1.Checked = true;
			this.customColorRadioButton1.Location = new System.Drawing.Point(104, 80);
			this.customColorRadioButton1.Name = "customColorRadioButton1";
			this.customColorRadioButton1.TabIndex = 2;
			this.customColorRadioButton1.TabStop = true;
			this.customColorRadioButton1.Text = "customColorRadioButton1";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 278);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.customColorRadioButton1,
																		  this.customColorCheckBox1});
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
	}
}
