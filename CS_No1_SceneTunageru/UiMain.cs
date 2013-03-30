using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gs_No1
{
    public partial class UiMain : UserControl
    {

        /// <summary>
        /// 座標マット。
        /// </summary>
        private CoordinateMat coordinateMat;

        /// <summary>
        /// クリックした始点。
        /// </summary>
        private Point clickedLocation;
        private bool isClickedLocationVisible;

        /// <summary>
        /// マウスボタンを離した地点。
        /// </summary>
        private Point releaseMouseButtonLocation;
        private bool isReleaseMouseButtonLocationVisible;

        public UiMain()
        {
            this.coordinateMat = new CoordinateMat();
            this.clickedLocation = new Point();
            InitializeComponent();
        }

        /// <summary>
        /// クリックした地点と、マウスボタンを放した地点に丸を描き、直線で結びます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UiMain_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 枠線
            g.DrawRectangle(Pens.Red, 0, 0, this.Width-1, this.Height-1);

            // 座標マット
            this.coordinateMat.Paint(g);

            // クリック地点
            if(this.isClickedLocationVisible)
            {
                g.FillEllipse(
                    Brushes.Blue,
                    this.clickedLocation.X-5,
                    this.clickedLocation.Y-5,
                    10,
                    10
                    );
            }

            // マウスボタンを放した地点
            if(this.isReleaseMouseButtonLocationVisible)
            {
                g.FillEllipse(
                    Brushes.Blue,
                    this.releaseMouseButtonLocation.X - 5,
                    this.releaseMouseButtonLocation.Y - 5,
                    10,
                    10
                    );

                // 線を引きます。
                g.DrawLine(
                    Pens.Blue,
                    this.clickedLocation,
                    this.releaseMouseButtonLocation
                    );
            }
        }

        private void UiMain_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void UiMain_MouseUp(object sender, MouseEventArgs e)
        {
            // マウスボタンを放した地点を設定。
            this.releaseMouseButtonLocation = e.Location;
            this.isReleaseMouseButtonLocationVisible = true;

            // マウス・ドラッグの移動量を測ります。
            Point move = new Point(
                this.releaseMouseButtonLocation.X - this.clickedLocation.X,
                this.releaseMouseButtonLocation.Y - this.clickedLocation.Y
                );
            System.Console.WriteLine( "move("+move.X+","+move.Y+")");

            // 選択されている構成部品があれば、移動量分だけ移動します。
            if(this.coordinateMat.IsSelected)
            {
                this.coordinateMat.Bounds = new Rectangle(
                    this.coordinateMat.Bounds.X + move.X,
                    this.coordinateMat.Bounds.Y + move.Y,
                    this.coordinateMat.Bounds.Width,
                    this.coordinateMat.Bounds.Height
                    );
            }

            this.Refresh();
        }

        private void UiMain_MouseDown(object sender, MouseEventArgs e)
        {
            // マウスボタンを押した地点を設定。
            this.clickedLocation = e.Location;
            this.isClickedLocationVisible = true;

            // マウスボタンを放した地点を消す。
            this.isReleaseMouseButtonLocationVisible = false;

            // 構成部品にもマウスクリックを伝達。
            this.coordinateMat.MouseClick(sender, e);
            this.Refresh();
        }
    }
}
