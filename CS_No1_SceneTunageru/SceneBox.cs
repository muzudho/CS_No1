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
                System.Console.WriteLine("シーンボックス movement(" + movement.X + "," + movement.Y + "," + movement.Width + "," + movement.Height + ")");
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
        }


        public void Paint(Graphics g)
        {
            //────────────────────────────────────────
            // 移動前の残像
            //────────────────────────────────────────

            Rectangle bounds2 = new Rectangle(
                this.sourceBounds.X,
                this.sourceBounds.Y,
                this.sourceBounds.Width - 2,
                this.sourceBounds.Height - 2
                );

            Pen pen = new Pen(Color.FromArgb(128,0,0,0), 2.0f);

            // 枠線
            g.DrawRectangle(pen, bounds2);

            // タイトル
            g.DrawString(
                this.title,
                new Font("ＭＳ ゴシック", 20.0f),
                new SolidBrush(Color.FromArgb(128,0,0,0)),
                new Point(
                    bounds2.X,
                    bounds2.Y
                    )
                );

            //────────────────────────────────────────
            // 移動後
            //────────────────────────────────────────

            bounds2 = new Rectangle(
                this.sourceBounds.X + this.Movement.X,
                this.sourceBounds.Y + this.Movement.Y,
                this.sourceBounds.Width - 2 + this.Movement.Width,
                this.sourceBounds.Height - 2 + this.Movement.Height
                );
            pen = new Pen(Color.Black, 2.0f);
            
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
                new Font("ＭＳ ゴシック",20.0f),
                Brushes.Black,
                new Point(
                    bounds2.X,
                    bounds2.Y
                    )
                );

        }

    }
}
