﻿namespace Gs_No1
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
            this.SuspendLayout();
            // 
            // UiMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UiMain";
            this.Size = new System.Drawing.Size(360, 362);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UiMain_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UiMain_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UiMain_MouseUp);
            this.Resize += new System.EventHandler(this.UiMain_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
