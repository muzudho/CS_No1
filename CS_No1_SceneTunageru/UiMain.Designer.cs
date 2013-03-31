namespace Gs_No1
{
    partial class UiMain
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(26, 18);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 19);
            this.textBox1.TabIndex = 0;
            this.textBox1.Visible = false;
            this.textBox1.WordWrap = false;
            this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
            // 
            // UiMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox1);
            this.DoubleBuffered = true;
            this.Name = "UiMain";
            this.Size = new System.Drawing.Size(360, 362);
            this.Load += new System.EventHandler(this.UiMain_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UiMain_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UiMain_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UiMain_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UiMain_MouseUp);
            this.Resize += new System.EventHandler(this.UiMain_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
    }
}
