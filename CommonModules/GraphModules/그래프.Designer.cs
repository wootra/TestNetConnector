namespace GraphModules
{
    partial class 그래프
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.multiLineGraph1 = new GraphModules.MultiLineGraph();
            this.SuspendLayout();
            // 
            // multiLineGraph1
            // 
            this.multiLineGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiLineGraph1.Location = new System.Drawing.Point(0, 0);
            this.multiLineGraph1.Name = "multiLineGraph1";
            this.multiLineGraph1.Size = new System.Drawing.Size(559, 327);
            this.multiLineGraph1.TabIndex = 0;
            this.multiLineGraph1.U_BufferSize = 100;
            this.multiLineGraph1.U_LineColors = new System.Drawing.Color[] {
        System.Drawing.Color.Red,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Green,
        System.Drawing.Color.Yellow};
            this.multiLineGraph1.U_RefreshCount = 20;
            // 
            // 그래프
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 327);
            this.Controls.Add(this.multiLineGraph1);
            this.Name = "그래프";
            this.Text = "그래프";
            this.ResumeLayout(false);

        }

        #endregion

        private MultiLineGraph multiLineGraph1;

    }
}