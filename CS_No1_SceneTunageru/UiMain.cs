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
        private CoordinateMat coordMat;

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

        /// <summary>
        /// 座標マット・ボタン
        /// </summary>
        private GraphicButton coordMatGbtn;

        /// <summary>
        /// シーン編集・ボタン
        /// </summary>
        private GraphicButton sceneGbtn;

        /// <summary>
        /// ムーブ・ボタン
        /// </summary>
        private GraphicButton moveGbtn;

        /// <summary>
        /// テキスト・ボタン
        /// </summary>
        private GraphicButton textGbtn;

        public UiMain()
        {
            this.coordMat = new CoordinateMat();
            this.coordMat.IsSelected = true;
            this.clickedLocation = new Point();
            this.coordMatGbtn = new GraphicButton();
            this.coordMatGbtn.Id = "coordMat";
            this.coordMatGbtn.FilePath = "img/btn_CoordMat.png";
            this.coordMatGbtn.Bounds = new Rectangle(50, 0, 50, 50);
            this.coordMatGbtn.IsSelected = true;
            this.coordMatGbtn.SwitchOnAction = () => { this.coordMat.IsSelected = true; };
            this.coordMatGbtn.SwitchOffAction = () => { this.coordMat.IsSelected = false; };
            this.sceneGbtn = new GraphicButton();
            this.sceneGbtn.Id = "scene";
            this.sceneGbtn.FilePath = "img/btn_Scene.png";
            this.sceneGbtn.Bounds = new Rectangle(100, 0, 50, 50);
            this.moveGbtn = new GraphicButton();
            this.moveGbtn.Id = "move";
            this.moveGbtn.FilePath = "img/btn_Move.png";
            this.moveGbtn.Bounds = new Rectangle( 50, 50, 50, 50);
            this.moveGbtn.IsSelected = true;
            this.textGbtn = new GraphicButton();
            this.textGbtn.Id = "text";
            this.textGbtn.FilePath = "img/btn_Text.png";
            this.textGbtn.Bounds = new Rectangle(100, 50, 50, 50);
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
            this.coordMat.Paint(g);

            // 各種ボタン
            this.coordMatGbtn.Paint(g);
            this.sceneGbtn.Paint(g);
            this.moveGbtn.Paint(g);
            this.textGbtn.Paint(g);

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
            if(this.coordMat.IsSelected)
            {
                this.coordMat.Bounds = new Rectangle(
                    this.coordMat.Bounds.X + move.X,
                    this.coordMat.Bounds.Y + move.Y,
                    this.coordMat.Bounds.Width,
                    this.coordMat.Bounds.Height
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
            this.coordMat.MouseDown(sender, e);

            // ラジオボタンのように。
            // 今回、マウスボタンで押されたボタン
            string pressedBtn="";
            if(this.coordMatGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.coordMatGbtn.Id;
            }
            else if(this.sceneGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.sceneGbtn.Id;
            }

            if ("" != pressedBtn)
            {
                // 選択されていればオン、選択されていなければオフ。
                if (this.coordMatGbtn.Id == pressedBtn)
                {
                    this.coordMatGbtn.IsSelected = true;
                }
                else
                {
                    this.coordMatGbtn.IsSelected = false;
                }

                if (this.sceneGbtn.Id == pressedBtn)
                {
                    this.sceneGbtn.IsSelected = true;
                }
                else
                {
                    this.sceneGbtn.IsSelected = false;
                }
            }

            // ラジオボタンのように。
            // 今回、マウスボタンで押されたボタン
            pressedBtn = "";
            if (this.moveGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.moveGbtn.Id;
            }
            else if (this.textGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.textGbtn.Id;
            }

            if ("" != pressedBtn)
            {
                // 選択されていればオン、選択されていなければオフ。
                if (this.moveGbtn.Id == pressedBtn)
                {
                    this.moveGbtn.IsSelected = true;
                }
                else
                {
                    this.moveGbtn.IsSelected = false;
                }

                if (this.textGbtn.Id == pressedBtn)
                {
                    this.textGbtn.IsSelected = true;
                }
                else
                {
                    this.textGbtn.IsSelected = false;
                }
            }

            this.Refresh();
        }

        private void UiMain_Load(object sender, EventArgs e)
        {
            this.coordMatGbtn.Load();
            this.sceneGbtn.Load();
            this.moveGbtn.Load();
            this.textGbtn.Load();
        }

        private void UiMain_MouseClick(object sender, MouseEventArgs e)
        {
            //座標マットボタンを押したとき
            if(this.coordMatGbtn.Bounds.Contains(e.Location))
            {
                this.coordMatGbtn.PerformSwitchOn(sender, e);
            }

            //シーンボタンを押したとき
            if (this.sceneGbtn.Bounds.Contains(e.Location))
            {
                this.sceneGbtn.PerformSwitchOn(sender, e);
            }

            //移動ボタンを押したとき
            if (this.moveGbtn.Bounds.Contains(e.Location))
            {
                this.moveGbtn.PerformSwitchOn(sender, e);
            }

            //テキストボタンを押したとき
            if (this.textGbtn.Bounds.Contains(e.Location))
            {
                this.textGbtn.PerformSwitchOn(sender, e);
            }
        }
    }
}
