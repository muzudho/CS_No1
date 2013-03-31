using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Gs_No1
{
    /// <summary>
    /// シーン・ボックス。
    /// </summary>
    public class SceneBox
    {

        /// <summary>
        /// 移動前の境界線。
        /// </summary>
        private Rectangle sourceBounds;
        public Rectangle SourceBounds
        {
            get
            {
                return this.sourceBounds;
            }
            set
            {
                this.sourceBounds = value;
            }
        }

        /// <summary>
        /// 動かした量。
        /// </summary>
        private Rectangle movement;
        public Rectangle Movement
        {
            get
            {
                return movement;
            }
            set
            {
                movement = value;
            }
        }

        /// <summary>
        /// 移動後の境界線。
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    this.SourceBounds.X + this.Movement.X,
                    this.SourceBounds.Y + this.Movement.Y,
                    this.SourceBounds.Width + this.Movement.Width,
                    this.SourceBounds.Height + this.Movement.Height
                    );
            }
        }

        /// <summary>
        /// フォントサイズ。
        /// </summary>
        private float fontSize;
        public float FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
            }
        }




        /// <summary>
        /// 表示文字列。
        /// </summary>
        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        /// <summary>
        /// 選択中。
        /// </summary>
        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
            }
        }





        /// <summary>
        /// 
        /// </summary>
        public SceneBox()
        {
            this.title = "名無し";
            this.sourceBounds = new Rectangle(0,0,100,100);
            this.Movement = new Rectangle();
            this.fontSize = 20.0f;
        }


        public void Paint(Graphics g)
        {
            // 線の太さ
            float weight = 2.0f;

            //────────────────────────────────────────
            // 移動前の残像
            //────────────────────────────────────────

            Rectangle bounds2 = new Rectangle(
                (int)(this.sourceBounds.X + weight/2f),
                (int)(this.sourceBounds.Y + weight/2f),
                (int)(this.sourceBounds.Width - weight/2f),
                (int)(this.sourceBounds.Height - weight/2f)
                );

            Pen pen = new Pen(Color.FromArgb(128, 0, 0, 0), weight);

            // 枠線
            g.DrawRectangle(pen, bounds2);

            // タイトル
            g.DrawString(
                this.title,
                new Font("ＭＳ ゴシック", this.FontSize),
                new SolidBrush(Color.FromArgb(128,0,0,0)),
                new Point(
                    (int)(bounds2.X + weight),
                    (int)(bounds2.Y + weight)
                    )
                );

            //────────────────────────────────────────
            // 移動後
            //────────────────────────────────────────

            bounds2 = new Rectangle(
                (int)(this.sourceBounds.X + this.Movement.X + weight/2),
                (int)(this.sourceBounds.Y + this.Movement.Y + weight / 2),
                (int)(this.sourceBounds.Width - weight/2f + this.Movement.Width),
                (int)(this.sourceBounds.Height - weight/2f + this.Movement.Height)
                );
            pen = new Pen(Color.Black, weight);
            
            // 背景色
            Brush backBrush;
            if (this.IsSelected)
            {
                backBrush = Brushes.Lime;
            }
            else
            {
                backBrush = Brushes.White;
            }
            g.FillRectangle(backBrush, bounds2);

            // 枠線
            g.DrawRectangle(pen, bounds2);

            // タイトル
            g.DrawString(
                this.title,
                new Font("ＭＳ ゴシック", this.FontSize),
                Brushes.Black,
                new Point(
                    (int)(bounds2.X + weight),
                    (int)(bounds2.Y + weight)
                    )
                );

        }

        public void Save(StringBuilder sb)
        {
            sb.Append("  <scene title=\"" + this.Title + "\" x=\"" + this.Bounds.X + "\" y=\"" + this.Bounds.Y + "\" width=\"" + this.Bounds.Width + "\" height=\"" + this.Bounds.Height + "\" />");
            sb.Append(Environment.NewLine);

            System.Console.WriteLine("シーン：　座標（" + this.Bounds.X + "," + this.Bounds.Y + "）　タイトル（" + this.Title + "）");
        }

    }
}
