﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using System.IO;

namespace Gs_No1
{
    public partial class UiMain : UserControl
    {
        public static readonly int CELL_SIZE = 20;

        /// <summary>
        /// 座標マット。
        /// </summary>
        private CoordMat coordMat;
        private int coordMatRepeatX;
        private int coordMatRepeatY;

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
        /// 画像ボタン。
        /// </summary>
        private Dictionary<string, GraphicButton> buttons;

        /// <summary>
        /// シーンボックス一覧。
        /// </summary>
        private List<SceneBox> sceneBoxList;
        public List<SceneBox> SceneBoxList
        {
            get
            {
                return this.sceneBoxList;
            }
            set
            {
                this.sceneBoxList = value;
            }
        }

        /// <summary>
        /// 接続線一覧
        /// </summary>
        private List<Cable> cableList;
        public List<Cable> CableList
        {
            get
            {
                return this.cableList;
            }
            set
            {
                this.cableList = value;
            }
        }


        /// <summary>
        /// シーン作成回数。0スタート。
        /// </summary>
        private int sceneCreateCounter;

        /// <summary>
        /// マウス・ドラッグの移動量を測ります。
        /// </summary>
        private Point movement;
        public Point Movement
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
        /// ボタン領域の下端。
        /// </summary>
        private int buttonAreaBottom;

        /// <summary>
        /// グリッド行カーソル。
        /// </summary>
        private int rowCursorY;
        private bool isRowCursorYEnable;
        /// <summary>
        /// グリッド列カーソル。
        /// </summary>
        private int rowCursorX;
        private bool isRowCursorXEnable;

        public void Clear()
        {
            this.coordMatRepeatX = 1;
            this.coordMatRepeatY = 1;
            this.rowCursorX = 0;
            this.isRowCursorXEnable = false;
            this.rowCursorY = 0;
            this.isRowCursorYEnable = false;
        }

        public UiMain()
        {
            this.Clear();

            this.buttonAreaBottom = 3 * 50 + 5;
            this.clickedLocation = new Point();
            // 座標マット
            this.coordMat = new CoordMat();
            this.coordMat.IsSelected = true;
            this.coordMat.SourceBounds = new Rectangle(150, 150, this.coordMat.SourceBounds.Width, this.coordMat.SourceBounds.Height);


            this.buttons = new Dictionary<string, GraphicButton>();
            // セーブボタン
            GraphicButton btn = new GraphicButton();
            btn.Id = "save";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Save.png";
            btn.Bounds = new Rectangle(0 * 50, 0, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region セーブ

                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append(Environment.NewLine);
                sb.Append("<scene-tunageru>");
                sb.Append(Environment.NewLine);

                // ──────────
                // UiMain
                // ──────────
                sb.Append("  <ui-main");
                sb.Append(" coord-mat-repeat-x=\"" + this.coordMatRepeatX + "\" coord-mat-repeat-y=\"" + this.coordMatRepeatY + "\"");
                sb.Append(" />");
                sb.Append(Environment.NewLine);

                // ──────────
                // 座標マット
                // ──────────
                this.coordMat.Save(sb);

                // ──────────
                // 全シーン
                // ──────────
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.Save(sb);
                }

                // ──────────
                // 全接続線
                // ──────────
                foreach (Cable cable in this.CableList)
                {
                    cable.Save(sb);
                }

                sb.Append("</scene-tunageru>");
                sb.Append(Environment.NewLine);
                string xml = sb.ToString();

                // テキストファイルとして上書き。
                System.IO.File.WriteAllText("save.xml", xml, Encoding.UTF8);

                // ──────────
                // バックアップファイル
                // ──────────
                //タイムスタンプ
                string timestamp;
                {
                    StringBuilder s = new StringBuilder();
                    DateTime now = System.DateTime.Now;
                    s.Append(String.Format("{0:D4}", now.Year));
                    s.Append(String.Format("{0:D2}", now.Month));
                    s.Append(String.Format("{0:D2}", now.Day));
                    s.Append("_");
                    s.Append(String.Format("{0:D2}", now.Hour));
                    s.Append(String.Format("{0:D2}", now.Minute));
                    s.Append("_");
                    s.Append(String.Format("{0:D2}", now.Second));
                    s.Append(String.Format("{0:D3}", now.Millisecond));
                    timestamp = s.ToString();
                }
                System.IO.File.WriteAllText("BK_"+timestamp+"_save.xml", xml, Encoding.UTF8);

                #endregion
            };
            // ロードボタン
            btn = new GraphicButton();
            btn.Id = "load";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Load.png";
            btn.Bounds = new Rectangle(0 * 50, 1 * 50, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region ロード

                // ──────────
                // クリアー
                // ──────────
                this.Clear();
                this.SceneBoxList.Clear();
                this.CableList.Clear();


                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load("save.xml");
                    //ystem.Console.WriteLine("要素数="+doc.DocumentElement.ChildNodes.Count);

                    foreach (XmlNode xn in doc.DocumentElement.ChildNodes)
                    {
                        if (xn.NodeType == XmlNodeType.Element)
                        {
                            XmlElement xe = (XmlElement)xn;
                            string s;
                            bool b;
                            int x;
                            int y;
                            int w;
                            int h;
                            switch (xe.Name)
                            {
                                // ──────────
                                // UiMain
                                // ──────────
                                case "ui-main":
                                    s = xe.GetAttribute("coord-mat-repeat-x");
                                    int.TryParse(s, out x);
                                    this.coordMatRepeatX = x;
                                    s = xe.GetAttribute("coord-mat-repeat-y");
                                    int.TryParse(s, out y);
                                    this.coordMatRepeatY = y;
                                    break;

                                // ──────────
                                // 座標マット
                                // ──────────
                                case "coord-mat":
                                    this.coordMat.Load(xe);
                                    break;

                                // ──────────
                                // シーン
                                // ──────────
                                case "scene":
                                    SceneBox scene = new SceneBox();
                                    scene.Load(xe);
                                    this.SceneBoxList.Add(scene);
                                    break;

                                // ──────────
                                // 接続線
                                // ──────────
                                case "cable":
                                    Cable cable = new Cable();
                                    cable.Load(xe);

                                    this.CableList.Add(cable);
                                    break;
                            }
                        }
                    }

                    //────────────────────────────────────────
                    // グリッドのずれ修正
                    //────────────────────────────────────────

                    // ──────────
                    // 全シーン
                    // ──────────
                    foreach (SceneBox scene in this.SceneBoxList)
                    {
                        if (
                            0 != (scene.SourceBounds.X - this.coordMat.SourceBounds.X) % UiMain.CELL_SIZE ||
                            0 != (scene.SourceBounds.Y - this.coordMat.SourceBounds.Y) % UiMain.CELL_SIZE
                            )
                        {
                            scene.SourceBounds = new Rectangle(
                                scene.SourceBounds.X - (scene.SourceBounds.X - this.coordMat.SourceBounds.X) % UiMain.CELL_SIZE,
                                scene.SourceBounds.Y - (scene.SourceBounds.Y - this.coordMat.SourceBounds.Y) % UiMain.CELL_SIZE,
                                scene.SourceBounds.Width,
                                scene.SourceBounds.Height
                                );
                        }
                    }

                    // ──────────
                    // 全接続線
                    // ──────────
                    foreach (Cable cable in this.CableList)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (
                                0 != (cable.SourceBounds[i].X - this.coordMat.SourceBounds.X) % UiMain.CELL_SIZE ||
                                0 != (cable.SourceBounds[i].Y - this.coordMat.SourceBounds.Y) % UiMain.CELL_SIZE
                                )
                            {
                                cable.SourceBounds[i] = new Rectangle(
                                    cable.SourceBounds[i].X - (cable.SourceBounds[i].X - this.coordMat.SourceBounds.X) % UiMain.CELL_SIZE,
                                    cable.SourceBounds[i].Y - (cable.SourceBounds[i].Y - this.coordMat.SourceBounds.Y) % UiMain.CELL_SIZE,
                                    cable.SourceBounds[i].Width,
                                    cable.SourceBounds[i].Height
                                    );
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // セーブファイルが無いなど。
                }

                this.Refresh();

                #endregion
            };
            // スクリーンショットボタン
            btn = new GraphicButton();
            btn.Id = "screenShot";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ScreenShot.png";
            btn.Bounds = new Rectangle(0 * 50 + 0, 2*50+0, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region スクリーンショット

                //タイムスタンプ
                string timestamp;
                {
                    StringBuilder s = new StringBuilder();
                    DateTime now = System.DateTime.Now;
                    s.Append(String.Format("{0:D4}",now.Year));
                    s.Append(String.Format("{0:D2}",now.Month));
                    s.Append(String.Format("{0:D2}",now.Day));
                    s.Append("_");
                    s.Append(String.Format("{0:D2}",now.Hour));
                    s.Append(String.Format("{0:D2}",now.Minute));
                    s.Append("_");
                    s.Append(String.Format("{0:D2}",now.Second));
                    s.Append(String.Format("{0:D3}",now.Millisecond));
                    timestamp = s.ToString();
                }

                for (int r = 0; r < this.coordMatRepeatY; r++ )
                {
                    for (int c = 0; c < this.coordMatRepeatX; c++)
                    {
                        // Graphicsオブジェクトを取得
                        Graphics g = null;

                        try
                        {
                            Bitmap bitmap;
                            bitmap = new Bitmap(this.coordMat.Bounds.Width, this.coordMat.Bounds.Height);
                            g = Graphics.FromImage(bitmap);

                            // 背景を白色で塗りつぶします。
                            g.FillRectangle(Brushes.White, 0, 0, this.coordMat.Bounds.Width, this.coordMat.Bounds.Height);

                            // ──────────
                            // 座標マット　描画
                            // ──────────
                            this.coordMat.FileName = r + "_" + c + ".png";
                            Rectangle old2 = new Rectangle(
                                this.coordMat.SourceBounds.X,
                                this.coordMat.SourceBounds.Y,
                                this.coordMat.SourceBounds.Width,
                                this.coordMat.SourceBounds.Height
                                );
                            this.coordMat.SourceBounds = new Rectangle(
                                0,
                                0,
                                this.coordMat.Bounds.Width,
                                this.coordMat.Bounds.Height
                                );
                            bool isSelected2 = this.coordMat.IsSelected;
                            this.coordMat.IsSelected = false;
                            this.coordMat.Paint(g);
                            this.coordMat.SourceBounds = old2;
                            this.coordMat.IsSelected = isSelected2;

                            // ──────────
                            // 全接続線BC　描画
                            // ──────────
                            foreach (Cable cable in this.CableList)
                            {
                                Rectangle[] old = new Rectangle[2];
                                Point[] oldNode = new Point[2];

                                for (int i = 0; i < 2; i++)
                                {
                                    // 退避
                                    old[i] = new Rectangle(
                                        cable.SourceBounds[i].X,
                                        cable.SourceBounds[i].Y,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                    oldNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X,
                                        cable.SourceBoundsNode[i].Y
                                        );

                                    // 入替え
                                    cable.SourceBounds[i] = new Rectangle(
                                        cable.SourceBounds[i].X - c * this.coordMat.SourceBounds.Width - this.coordMat.SourceBounds.X,
                                        cable.SourceBounds[i].Y - r * this.coordMat.SourceBounds.Height - this.coordMat.SourceBounds.Y,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                    cable.SourceBoundsNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X - c * this.coordMat.SourceBounds.Width - this.coordMat.SourceBounds.X,
                                        cable.SourceBoundsNode[i].Y - r * this.coordMat.SourceBounds.Height - this.coordMat.SourceBounds.Y
                                        );
                                }

                                cable.PaintBackCircle(g);

                                // 復元
                                for (int i = 0; i < 2; i++)
                                {
                                    cable.SourceBounds[i] = old[i];
                                }
                                for (int i = 0; i < 2; i++)
                                {
                                    cable.SourceBoundsNode[i] = oldNode[i];
                                }
                            }

                            // ──────────
                            // 全接続線L　描画
                            // ──────────
                            foreach (Cable cable in this.CableList)
                            {
                                Rectangle[] old = new Rectangle[2];
                                Point[] oldNode = new Point[2];

                                for (int i = 0; i < 2; i++)
                                {
                                    // 退避
                                    old[i] = new Rectangle(
                                        cable.SourceBounds[i].X,
                                        cable.SourceBounds[i].Y,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                    oldNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X,
                                        cable.SourceBoundsNode[i].Y
                                        );

                                    // 入替え
                                    cable.SourceBounds[i] = new Rectangle(
                                        cable.SourceBounds[i].X - c * this.coordMat.SourceBounds.Width - this.coordMat.SourceBounds.X,
                                        cable.SourceBounds[i].Y - r * this.coordMat.SourceBounds.Height - this.coordMat.SourceBounds.Y,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                    cable.SourceBoundsNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X - c * this.coordMat.SourceBounds.Width - this.coordMat.SourceBounds.X,
                                        cable.SourceBoundsNode[i].Y - r * this.coordMat.SourceBounds.Height - this.coordMat.SourceBounds.Y
                                        );
                                }

                                cable.PaintLine(g);

                                // 復元
                                for (int i = 0; i < 2; i++)
                                {
                                    cable.SourceBounds[i] = old[i];
                                }
                                for (int i = 0; i < 2; i++)
                                {
                                    cable.SourceBoundsNode[i] = oldNode[i];
                                }
                            }

                            // ──────────
                            // 全シーン　描画
                            // ──────────
                            foreach (SceneBox scene in this.SceneBoxList)
                            {
                                Rectangle old = new Rectangle(
                                    scene.SourceBounds.X,
                                    scene.SourceBounds.Y,
                                    scene.SourceBounds.Width,
                                    scene.SourceBounds.Height
                                    );

                                scene.SourceBounds = new Rectangle(
                                    scene.SourceBounds.X - c * this.coordMat.SourceBounds.Width - this.coordMat.SourceBounds.X,
                                    scene.SourceBounds.Y - r * this.coordMat.SourceBounds.Height - this.coordMat.SourceBounds.Y,
                                    scene.SourceBounds.Width,
                                    scene.SourceBounds.Height
                                    );
                                scene.Paint(g);

                                scene.SourceBounds = old;
                            }

                            // ──────────
                            // 全接続線F　描画
                            // ──────────
                            foreach (Cable cable in this.CableList)
                            {
                                Rectangle[] old = new Rectangle[2];
                                Point[] oldNode = new Point[2];

                                for (int i = 0; i < 2; i++)
                                {
                                    // 退避
                                    old[i] = new Rectangle(
                                        cable.SourceBounds[i].X,
                                        cable.SourceBounds[i].Y,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                    oldNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X,
                                        cable.SourceBoundsNode[i].Y
                                        );

                                    // 入替え
                                    cable.SourceBounds[i] = new Rectangle(
                                        cable.SourceBounds[i].X - c * this.coordMat.SourceBounds.Width - this.coordMat.SourceBounds.X,
                                        cable.SourceBounds[i].Y - r * this.coordMat.SourceBounds.Height - this.coordMat.SourceBounds.Y,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                    cable.SourceBoundsNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X - c * this.coordMat.SourceBounds.Width - this.coordMat.SourceBounds.X,
                                        cable.SourceBoundsNode[i].Y - r * this.coordMat.SourceBounds.Height - this.coordMat.SourceBounds.Y
                                        );
                                }

                                cable.PaintMouseMark(g);

                                // 復元
                                for (int i = 0; i < 2; i++)
                                {
                                    cable.SourceBounds[i] = old[i];
                                }
                                for (int i = 0; i < 2; i++)
                                {
                                    cable.SourceBoundsNode[i] = oldNode[i];
                                }
                            }


                            // 画像ファイルを書き出します。
                            bitmap.Save(Path.Combine(Application.StartupPath, timestamp + "_" + r + "_"+c+".png"), System.Drawing.Imaging.ImageFormat.Png);
                        }
                        finally
                        {
                            if (null != g)
                            {
                                g.Dispose();
                            }
                        }
                    }
                }

                #endregion
            };
            // 座標マットボタン
            btn = new GraphicButton();
            btn.Id = "coordMat";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_CoordMat.png";
            btn.Bounds = new Rectangle(0 * 75 + 100, 0, 75, 75);
            btn.IsSelected = true;
            btn.SwitchOnAction = () => {
                this.coordMat.IsSelected = true;
                // ボタン表示
                this.buttons["moveMat"].IsVisible = true;
                this.buttons["extendT"].IsVisible = true;
                this.buttons["extendR"].IsVisible = true;
                this.buttons["extendB"].IsVisible = true;
                this.buttons["extendL"].IsVisible = true;
                this.buttons["reductT"].IsVisible = true;
                this.buttons["reductR"].IsVisible = true;
                this.buttons["reductB"].IsVisible = true;
                this.buttons["reductL"].IsVisible = true;
                this.buttons["detachesH"].IsVisible = true;
                this.buttons["detachesV"].IsVisible = true;
                this.buttons["shortenH"].IsVisible = true;
                this.buttons["shortenV"].IsVisible = true;
            };
            btn.SwitchOffAction = () => {
                this.coordMat.IsSelected = false;
                // ボタン非表示
                this.buttons["moveMat"].IsVisible = false;
                this.buttons["extendT"].IsVisible = false;
                this.buttons["extendR"].IsVisible = false;
                this.buttons["extendB"].IsVisible = false;
                this.buttons["extendL"].IsVisible = false;
                this.buttons["reductT"].IsVisible = false;
                this.buttons["reductR"].IsVisible = false;
                this.buttons["reductB"].IsVisible = false;
                this.buttons["reductL"].IsVisible = false;
                this.buttons["detachesH"].IsVisible = false;
                this.buttons["detachesV"].IsVisible = false;
                this.buttons["shortenH"].IsVisible = false;
                this.buttons["shortenV"].IsVisible = false;
            };
            // シーンボタン
            btn = new GraphicButton();
            btn.Id = "scene";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Scene.png";
            btn.Bounds = new Rectangle(1 * 75 + 100, 0, 75, 75);
            btn.SwitchOnAction = () =>
            {
                // ボタン表示
                this.buttons["text"].IsVisible = true;
                this.buttons["create"].IsVisible = true;
                this.buttons["copy"].IsVisible = true;
                this.buttons["delete"].IsVisible = true;
                this.buttons["connect"].IsVisible = true;
                this.buttons["cable4"].IsVisible = true;
                this.buttons["shape"].IsVisible = true;
            };
            btn.SwitchOffAction = () =>
            {
                // ボタン非表示
                this.buttons["text"].IsVisible = false;
                this.buttons["create"].IsVisible = false;
                this.buttons["copy"].IsVisible = false;
                this.buttons["delete"].IsVisible = false;
                this.buttons["connect"].IsVisible = false;
                this.buttons["cable4"].IsVisible = false;
                this.buttons["shape"].IsVisible = false;
            };
            // 移動ボタン
            btn = new GraphicButton();
            btn.Id = "move";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Move.png";
            btn.Bounds = new Rectangle(2 * 50, 0*50+75, 50, 50);
            btn.IsSelected = true;
            // マット移動ボタン
            btn = new GraphicButton();
            btn.Id = "moveMat";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_MoveMat.png";
            btn.Bounds = new Rectangle(3 * 50, 0 * 50 + 75, 50, 50);
            // 拡張上ボタン
            btn = new GraphicButton();
            btn.Id = "extendT";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ExtendT.png";
            btn.Bounds = new Rectangle(4 * 50, 0 * 50 + 75, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region 拡張_上

                if (this.coordMatRepeatY < int.MaxValue)
                {
                    this.coordMatRepeatY++;

                    //────────────────────────────────────────
                    // 座標マット１個分下にずらします。
                    //────────────────────────────────────────

                    // ──────────
                    // 全シーン
                    // ──────────
                    foreach (SceneBox scene in this.SceneBoxList)
                    {
                        scene.SourceBounds = new Rectangle(
                            scene.SourceBounds.X,
                            scene.SourceBounds.Y + this.coordMat.SourceBounds.Height,
                            scene.SourceBounds.Width,
                            scene.SourceBounds.Height
                            );
                    }

                    // ──────────
                    // 全接続線
                    // ──────────
                    foreach (Cable cable in this.CableList)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            cable.SourceBounds[i] = new Rectangle(
                                cable.SourceBounds[i].X,
                                cable.SourceBounds[i].Y + this.coordMat.SourceBounds.Height,
                                cable.SourceBounds[i].Width,
                                cable.SourceBounds[i].Height
                                );
                        }
                    }

                    this.Refresh();
                }

                #endregion
            };
            // 拡張右ボタン
            btn = new GraphicButton();
            btn.Id = "extendR";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ExtendR.png";
            btn.Bounds = new Rectangle(5 * 50, 0 * 50 + 75, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region 拡張_右

                if (this.coordMatRepeatX < int.MaxValue)
                {
                    this.coordMatRepeatX++;
                    this.Refresh();
                }

                #endregion
            };
            // 拡張下ボタン
            btn = new GraphicButton();
            btn.Id = "extendB";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ExtendB.png";
            btn.Bounds = new Rectangle(6 * 50, 0 * 50 + 75, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region 拡張_下

                if (this.coordMatRepeatY < int.MaxValue)
                {
                    this.coordMatRepeatY++;
                    this.Refresh();
                }

                #endregion
            };
            // 拡張左ボタン
            btn = new GraphicButton();
            btn.Id = "extendL";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ExtendL.png";
            btn.Bounds = new Rectangle(7 * 50, 0 * 50 + 75, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region 拡張_左

                if (this.coordMatRepeatX < int.MaxValue)
                {
                    this.coordMatRepeatX++;

                    //────────────────────────────────────────
                    // 座標マット１個分右にずらします。
                    //────────────────────────────────────────

                    // ──────────
                    // 全シーン
                    // ──────────
                    foreach (SceneBox scene in this.SceneBoxList)
                    {
                        scene.SourceBounds = new Rectangle(
                            scene.SourceBounds.X + this.coordMat.SourceBounds.Width,
                            scene.SourceBounds.Y,
                            scene.SourceBounds.Width,
                            scene.SourceBounds.Height
                            );
                    }

                    // ──────────
                    // 全接続線
                    // ──────────
                    foreach (Cable cable in this.CableList)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            cable.SourceBounds[i] = new Rectangle(
                                cable.SourceBounds[i].X + this.coordMat.SourceBounds.Width,
                                cable.SourceBounds[i].Y,
                                cable.SourceBounds[i].Width,
                                cable.SourceBounds[i].Height
                                );
                        }
                    }

                    this.Refresh();
                }

                #endregion
            };
            // 縮小上ボタン
            btn = new GraphicButton();
            btn.Id = "reductT";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ReductT.png";
            btn.Bounds = new Rectangle(8 * 50, 0 * 50 + 75, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region 縮小_上

                if (1 < this.coordMatRepeatY)
                {
                    this.coordMatRepeatY--;

                    //────────────────────────────────────────
                    // 座標マット１個分上にずらします。
                    //────────────────────────────────────────

                    // ──────────
                    // 全シーン
                    // ──────────
                    foreach (SceneBox scene in this.SceneBoxList)
                    {
                        scene.SourceBounds = new Rectangle(
                            scene.SourceBounds.X,
                            scene.SourceBounds.Y - this.coordMat.SourceBounds.Height,
                            scene.SourceBounds.Width,
                            scene.SourceBounds.Height
                            );
                    }

                    // ──────────
                    // 全接続線
                    // ──────────
                    foreach (Cable cable in this.CableList)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            cable.SourceBounds[i] = new Rectangle(
                                cable.SourceBounds[i].X,
                                cable.SourceBounds[i].Y - this.coordMat.SourceBounds.Height,
                                cable.SourceBounds[i].Width,
                                cable.SourceBounds[i].Height
                                );
                        }
                    }

                    this.Refresh();
                }
                #endregion
            };
            // 縮小右ボタン
            btn = new GraphicButton();
            btn.Id = "reductR";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ReductR.png";
            btn.Bounds = new Rectangle(9 * 50, 0 * 50 + 75, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region 縮小_右
                if (1<this.coordMatRepeatX)
                {
                    this.coordMatRepeatX--;
                    this.Refresh();
                }
                #endregion
            };
            // 縮小下ボタン
            btn = new GraphicButton();
            btn.Id = "reductB";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ReductB.png";
            btn.Bounds = new Rectangle(10 * 50, 0 * 50 + 75, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region 縮小_下
                if (1 < this.coordMatRepeatY)
                {
                    this.coordMatRepeatY--;
                    this.Refresh();
                }
                #endregion
            };
            // 縮小左ボタン
            btn = new GraphicButton();
            btn.Id = "reductL";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ReductL.png";
            btn.Bounds = new Rectangle(11 * 50, 0 * 50 + 75, 50, 50);
            btn.SwitchOnAction = () =>
            {
                #region 縮小_左
                if (1<this.coordMatRepeatX)
                {
                    this.coordMatRepeatX--;

                    //────────────────────────────────────────
                    // 座標マット１個分左にずらします。
                    //────────────────────────────────────────

                    // ──────────
                    // 全シーン
                    // ──────────
                    foreach (SceneBox scene in this.SceneBoxList)
                    {
                        scene.SourceBounds = new Rectangle(
                            scene.SourceBounds.X - this.coordMat.SourceBounds.Width,
                            scene.SourceBounds.Y,
                            scene.SourceBounds.Width,
                            scene.SourceBounds.Height
                            );
                    }

                    // ──────────
                    // 全接続線
                    // ──────────
                    foreach (Cable cable in this.CableList)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            cable.SourceBounds[i] = new Rectangle(
                                cable.SourceBounds[i].X - this.coordMat.SourceBounds.Width,
                                cable.SourceBounds[i].Y,
                                cable.SourceBounds[i].Width,
                                cable.SourceBounds[i].Height
                                );
                        }
                    }

                    this.Refresh();
                }
                #endregion
            };
            // 離す横
            btn = new GraphicButton();
            btn.Id = "detachesH";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_DetachesH.png";
            btn.Bounds = new Rectangle(12 * 50, 0 * 50 + 75, 50, 50);
            // 離す縦
            btn = new GraphicButton();
            btn.Id = "detachesV";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_DetachesV.png";
            btn.Bounds = new Rectangle(13 * 50, 0 * 50 + 75, 50, 50);
            // 縮める横
            btn = new GraphicButton();
            btn.Id = "shortenH";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ShortenH.png";
            btn.Bounds = new Rectangle(14 * 50, 0 * 50 + 75, 50, 50);
            // 縮める縦
            btn = new GraphicButton();
            btn.Id = "shortenV";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_ShortenV.png";
            btn.Bounds = new Rectangle(15 * 50, 0 * 50 + 75, 50, 50);
            // テキストボタン
            btn = new GraphicButton();
            btn.Id = "text";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Text.png";
            btn.Bounds = new Rectangle(3 * 50, 0 * 50 + 75, 50, 50);
            btn.IsVisible = false;
            // 作成ボタン
            btn = new GraphicButton();
            btn.Id = "create";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Create.png";
            btn.Bounds = new Rectangle(4 * 50, 0 * 50 + 75, 50, 50);
            btn.IsVisible = false;
            btn.SwitchOnAction = () =>
                {
                    //既存シーンボックスの選択解除
                    foreach (SceneBox scene2 in this.SceneBoxList)
                    {
                        scene2.IsSelected = false;
                    }

                    //シーンボックス作成
                    SceneBox scene = new SceneBox();
                    scene.Title = "シーン"+this.sceneCreateCounter;
                    scene.SourceBounds = new Rectangle(
                        // 画面の 200,200 あたりに作成。
                        200 + this.coordMat.SourceBounds.X % UiMain.CELL_SIZE,
                        200 + this.coordMat.SourceBounds.Y % UiMain.CELL_SIZE,
                        6 * UiMain.CELL_SIZE,
                        3*UiMain.CELL_SIZE
                        );
                    scene.FontSize = 12.0f;
                    scene.IsSelected = true;
                    this.SceneBoxList.Add(scene);

                    this.sceneCreateCounter++;
                };
            // コピーボタン
            btn = new GraphicButton();
            btn.Id = "copy";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Copy.png";
            btn.Bounds = new Rectangle(5 * 50, 0 * 50 + 75, 50, 50);
            btn.IsVisible = false;
            // 削除ボタン
            btn = new GraphicButton();
            btn.Id = "delete";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Delete.png";
            btn.Bounds = new Rectangle(6 * 50, 0 * 50 + 75, 50, 50);
            btn.IsVisible = false;
            // 接続ボタン
            btn = new GraphicButton();
            btn.Id = "connect";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Connect.png";
            btn.Bounds = new Rectangle(7 * 50, 0 * 50 + 75, 50, 50);
            btn.IsVisible = false;
            btn.SwitchOnAction = () =>
            {
                // 全シーン選択解除
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.IsSelected = false;
                }
            };
            // 4節接続ボタン
            btn = new GraphicButton();
            btn.Id = "cable4";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Cable4.png";
            btn.Bounds = new Rectangle(8 * 50, 0 * 50 + 75, 50, 50);
            btn.IsVisible = false;
            btn.SwitchOnAction = () =>
            {
                // 全シーン選択解除
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.IsSelected = false;
                }
            };
            // 形状切替ボタン
            btn = new GraphicButton();
            btn.Id = "shape";
            this.buttons[btn.Id] = btn;
            btn.FilePath = "img/btn_Shape.png";
            btn.Bounds = new Rectangle(9 * 50, 0 * 50 + 75, 50, 50);
            btn.IsVisible = false;
            btn.SwitchOnAction = () =>
            {
                // TODO:
            };

            this.sceneBoxList = new List<SceneBox>();
            this.cableList = new List<Cable>();
            this.Movement = new Point();
            InitializeComponent();
            this.textBox1.Font = new Font("ＭＳ ゴシック", 12.0f);
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
            int x = this.coordMat.SourceBounds.X;
            int y = this.coordMat.SourceBounds.Y;
            int width = this.coordMat.SourceBounds.Width;
            int height = this.coordMat.SourceBounds.Height;
            for (int row = 0; row < this.coordMatRepeatY; row++)
            {
                for (int column = 0; column < this.coordMatRepeatX; column++)
                {
                    this.coordMat.FileName = row + "_" + column + ".png";
                    this.coordMat.SourceBounds = new Rectangle(
                        column * width + x,
                        row * height + y,
                        width,
                        height
                        );
                    this.coordMat.Paint(g);
                }
            }
            this.coordMat.SourceBounds = new Rectangle(x,y,width,height);

            // ──────────
            // 全接続線BC
            // ──────────
            foreach (Cable cable in this.CableList)
            {
                cable.PaintBackCircle(g);
            }

            // ──────────
            // 全接続線L
            // ──────────
            foreach (Cable cable in this.CableList)
            {
                cable.PaintLine(g);
            }

            // ──────────
            // シーンボックス
            // ──────────
            foreach (SceneBox scene in this.SceneBoxList)
            {
                scene.Paint(g);
            }

            // ──────────
            // 全接続線F
            // ──────────
            foreach (Cable cable in this.CableList)
            {
                cable.PaintMouseMark(g);
            }


            // ──────────
            // 列カーソル、行カーソル
            // ──────────
            if (this.isRowCursorXEnable)
            {
                g.FillRectangle(
                    new SolidBrush(Color.FromArgb(128,0,255,0)),//半透明の緑色
                    this.rowCursorX,
                    0,
                    UiMain.CELL_SIZE,
                    this.Height
                    );
            }

            if (this.isRowCursorYEnable)
            {
                g.FillRectangle(
                    new SolidBrush(Color.FromArgb(128, 0, 255, 0)),//半透明の緑色
                    0,
                    this.rowCursorY,
                    this.Width,
                    UiMain.CELL_SIZE
                    );
            }


            // ──────────
            // ボタン背景
            // ──────────
            g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), new Rectangle(
                0,
                0,
                this.Width,
                this.buttonAreaBottom
                ));

            // 各種ボタン
            this.buttons["save"].Paint(g);
            this.buttons["load"].Paint(g);
            this.buttons["screenShot"].Paint(g);
            this.buttons["coordMat"].Paint(g);
            this.buttons["scene"].Paint(g);
            this.buttons["move"].Paint(g);
            this.buttons["moveMat"].Paint(g);
            this.buttons["extendT"].Paint(g);
            this.buttons["extendR"].Paint(g);
            this.buttons["extendB"].Paint(g);
            this.buttons["extendL"].Paint(g);
            this.buttons["reductT"].Paint(g);
            this.buttons["reductR"].Paint(g);
            this.buttons["reductB"].Paint(g);
            this.buttons["reductL"].Paint(g);
            this.buttons["detachesH"].Paint(g);
            this.buttons["detachesV"].Paint(g);
            this.buttons["shortenH"].Paint(g);
            this.buttons["shortenV"].Paint(g);
            this.buttons["text"].Paint(g);
            this.buttons["create"].Paint(g);
            this.buttons["copy"].Paint(g);
            this.buttons["delete"].Paint(g);
            this.buttons["connect"].Paint(g);
            this.buttons["cable4"].Paint(g);
            this.buttons["shape"].Paint(g);


            // クリック地点
            if(this.isClickedLocationVisible)
            {
                g.FillEllipse(
                    new SolidBrush( Color.FromArgb(128,0,0,255)),
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

                // 構成部品の移動量をクリアーします。
                this.coordMat.Movement = new Rectangle();
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.Movement = new Rectangle();
                }
            }
        }

        private void UiMain_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void UiMain_MouseUp(object sender, MouseEventArgs e)
        {
            //────────────────────────────────────────
            // 何もないところでマウスボタンを放したかどうか
            //────────────────────────────────────────
            #region 何もないところでマウスボタンを放したかどうか

            bool actorReleased = false;//ボタン、シーン、接続線のいずれかの上で放したら真
            bool buttonReleased = false;//ボタンの上で放したら真

            // セーブ、ロード、スクリーンショット
            if (this.buttons["save"].IsHit(e.Location))
            {
                this.buttons["save"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["load"].IsHit(e.Location))
            {
                this.buttons["load"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["screenShot"].IsHit(e.Location))
            {
                this.buttons["screenShot"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // 座標マット
            if (this.buttons["coordMat"].IsHit(e.Location))
            {
                this.buttons["coordMat"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // シーン
            if (this.buttons["scene"].IsHit(e.Location))
            {
                this.buttons["scene"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // 移動、マット移動
            if (this.buttons["move"].IsHit(e.Location))
            {
                this.buttons["move"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["moveMat"].IsHit(e.Location))
            {
                this.buttons["moveMat"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // 拡張　上、右、下、左
            if (this.buttons["extendT"].IsHit(e.Location))
            {
                this.buttons["extendT"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["extendR"].IsHit(e.Location))
            {
                this.buttons["extendR"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["extendB"].IsHit(e.Location))
            {
                this.buttons["extendB"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["extendL"].IsHit(e.Location))
            {
                this.buttons["extendL"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // 縮小　上、右、下、左
            if (this.buttons["reductT"].IsHit(e.Location))
            {
                this.buttons["reductT"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["reductR"].IsHit(e.Location))
            {
                this.buttons["reductR"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["reductB"].IsHit(e.Location))
            {
                this.buttons["reductB"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["reductL"].IsHit(e.Location))
            {
                this.buttons["reductL"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // 離す、詰める
            if (this.buttons["detachesH"].IsHit(e.Location))
            {
                this.buttons["detachesH"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["detachesV"].IsHit(e.Location))
            {
                this.buttons["detachesV"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["shortenH"].IsHit(e.Location))
            {
                this.buttons["shortenH"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["shortenV"].IsHit(e.Location))
            {
                this.buttons["shortenV"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // テキスト
            if (this.buttons["text"].IsHit(e.Location))
            {
                this.buttons["text"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // 作成、コピー、削除
            if (this.buttons["create"].IsHit(e.Location))
            {
                this.buttons["create"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["copy"].IsHit(e.Location))
            {
                this.buttons["copy"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["delete"].IsHit(e.Location))
            {
                this.buttons["delete"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // 接続、4節接続
            if (this.buttons["connect"].IsHit(e.Location))
            {
                this.buttons["connect"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            if (this.buttons["cable4"].IsHit(e.Location))
            {
                this.buttons["cable4"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            // 形状切替
            if (this.buttons["shape"].IsHit(e.Location))
            {
                this.buttons["shape"].PerformSwitchOn(sender, e);
                actorReleased = true;
                buttonReleased = true;
            }

            foreach (SceneBox scene in this.SceneBoxList)
            {
                if (scene.Bounds.Contains(e.Location))
                {
                    actorReleased = true;
                }
            }

            // 何もないところの上で放したとき
            if (!actorReleased)
            {
            }

            #endregion



            // マウスボタンを放した地点を設定。
            this.releaseMouseButtonLocation = e.Location;

            bool isMoveCoordMat = false;
            int moveSceneFlg = 0;// 1:全シーン移動。2:選択シーンのみ移動。
            bool moveCableFlg = false;
            // 座標マットモードの場合
            if (this.buttons["coordMat"].IsSelected)
            {
                isMoveCoordMat = true;

                if (this.buttons["moveMat"].IsSelected)
                {
                    // マット移動モードの場合

                    // シーン、接続線は動かしません。
                }
                else
                {
                    moveSceneFlg = 1;
                    moveCableFlg = true;
                }
            }
            // シーンモードの場合
            if (this.buttons["scene"].IsSelected)
            {
                moveSceneFlg = 2;
            }


            // 選択されている構成部品があれば、移動量分だけ移動します。
            if (isMoveCoordMat)
            {
                this.coordMat.SourceBounds = new Rectangle(
                    this.coordMat.SourceBounds.X + this.Movement.X,
                    this.coordMat.SourceBounds.Y + this.Movement.Y,
                    this.coordMat.SourceBounds.Width,
                    this.coordMat.SourceBounds.Height
                    );
                this.coordMat.Movement = new Rectangle();
            }

            if (moveSceneFlg == 1)
            {
                // ──────────
                // 全シーン　移動量セット
                // ──────────
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    int x;
                    int y;
                    if (isMoveCoordMat)
                    {
                        x = scene.SourceBounds.X + this.Movement.X;
                        y = scene.SourceBounds.Y + this.Movement.Y;
                    }
                    else
                    {
                        x = scene.SourceBounds.X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                        y = scene.SourceBounds.Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                    }
                    scene.SourceBounds = new Rectangle(
                        x,
                        y,
                        scene.SourceBounds.Width,
                        scene.SourceBounds.Height
                        );
                    scene.Movement = new Rectangle();
                }
            }
            else if (moveSceneFlg == 2)
            {
                // ──────────
                // 選択シーン　移動量セット
                // ──────────
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    if (scene.IsSelected)
                    {
                        scene.SourceBounds = new Rectangle(
                            scene.SourceBounds.X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE,
                            scene.SourceBounds.Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE,
                            scene.SourceBounds.Width,
                            scene.SourceBounds.Height
                            );
                        scene.Movement = new Rectangle();
                    }
                }

                // ──────────
                // 選択接続線　移動量セット
                // ──────────
                foreach (Cable cable in this.CableList)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (cable.IsSelected[i])
                        {
                            cable.SourceBounds[i] = new Rectangle(
                                cable.SourceBounds[i].X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE,
                                cable.SourceBounds[i].Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE,
                                cable.SourceBounds[i].Width,
                                cable.SourceBounds[i].Height
                                );
                            cable.Movement[i] = new Rectangle();
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        if (cable.IsSelectedNode[i])
                        {
                            cable.SourceBoundsNode[i] = new Point(
                                cable.SourceBoundsNode[i].X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE,
                                cable.SourceBoundsNode[i].Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE
                                );
                            cable.MovementNode[i] = new Point();
                        }
                    }
                }
            }

            if (moveCableFlg)
            {
                // 全ての接続線を、移動量分だけ移動します。
                foreach (Cable cable in this.CableList)
                {
                    // ──────────
                    // [0]起点　[1]終点
                    // ──────────
                    for (int i = 0; i < 2; i++)
                    {
                        int x;
                        int y;
                        if (isMoveCoordMat)
                        {
                            x = cable.SourceBounds[i].X + this.Movement.X;
                            y = cable.SourceBounds[i].Y + this.Movement.Y;
                        }
                        else
                        {
                            x = cable.SourceBounds[i].X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                            y = cable.SourceBounds[i].Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                        }
                        cable.SourceBounds[i] = new Rectangle(
                            x,
                            y,
                            cable.SourceBounds[i].Width,
                            cable.SourceBounds[i].Height
                            );
                        cable.Movement[i] = new Rectangle();
                    }

                    // ──────────
                    // 中間点[0～1]
                    // ──────────
                    for (int i = 0; i < 2; i++)
                    {
                        int x;
                        int y;
                        if (isMoveCoordMat)
                        {
                            x = cable.SourceBoundsNode[i].X + this.Movement.X;
                            y = cable.SourceBoundsNode[i].Y + this.Movement.Y;
                        }
                        else
                        {
                            x = cable.SourceBoundsNode[i].X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                            y = cable.SourceBoundsNode[i].Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                        }
                        cable.SourceBoundsNode[i] = new Point(
                            x,
                            y
                            );
                        cable.MovementNode[i] = new Point();
                    }
                }
            }

            this.isReleaseMouseButtonLocationVisible = true;


            this.Refresh();
        }

        private void UiMain_MouseDown(object sender, MouseEventArgs e)
        {

            //────────────────────────────────────────
            // テキストボックス→シーン名
            //────────────────────────────────────────
            if (this.textBox1.Visible)
            {
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    if (scene.IsSelected)
                    {
                        scene.Title = this.textBox1.Text;
                    }
                }

                // フォーカスを外します。
                this.ActiveControl = null;
            }

            //────────────────────────────────────────
            // 何もないところでマウスボタン押下したかどうか
            //────────────────────────────────────────
            #region 何もないところでマウスボタン押下したかどうか

            bool actorPressed = false;//ボタン、シーン、接続線のいずれかの上で放したら真
            bool buttonReleased = false;//ボタンの上で放したら真

            // セーブ、ロード、スクリーンショット
            if (this.buttons["save"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["load"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["screenShot"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // 座標マット
            if (this.buttons["coordMat"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // シーン
            if (this.buttons["scene"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // 移動、マット移動
            if (this.buttons["move"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["moveMat"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // 拡張　上、右、下、左
            if (this.buttons["extendT"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["extendR"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["extendB"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["extendL"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // 縮小　上、右、下、左
            if (this.buttons["reductT"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["reductR"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["reductB"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["reductL"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // 離す、縮める
            if (this.buttons["detachesH"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["detachesV"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["shortenH"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["shortenV"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // テキスト
            if (this.buttons["text"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // 作成、コピー、削除
            if (this.buttons["create"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["copy"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["delete"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // 接続、4節接続、切断
            if (this.buttons["connect"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            if (this.buttons["cable4"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // 形状切替
            if (this.buttons["shape"].IsHit(e.Location))
            {
                actorPressed = true;
                buttonReleased = true;
            }

            // ──────────
            // 全シーン
            // ──────────
            foreach (SceneBox scene in this.SceneBoxList)
            {
                if (scene.IsHit(e.Location))
                {
                    actorPressed = true;
                }
            }

            // ──────────
            // 全接続線
            // ──────────
            foreach (Cable cable in this.CableList)
            {
                if (cable.IsHit(0,e.Location))
                {
                    actorPressed = true;
                }

                if (cable.IsHit(1,e.Location))
                {
                    actorPressed = true;
                }

                if (cable.IsHitNode(0, e.Location))
                {
                    actorPressed = true;
                }

                if (cable.IsHitNode(1, e.Location))
                {
                    actorPressed = true;
                }
            }

            // 何もないところでマウスボタン押下したとき
            if (!actorPressed)
            {
                // ──────────
                // 全シーン　選択解除
                // ──────────
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.IsSelected = false;
                }

                // ──────────
                // 全接続線　選択解除
                // ──────────
                foreach (Cable cable in this.CableList)
                {
                    cable.IsSelected[0] = false;
                    cable.IsSelected[1] = false;
                    cable.IsSelectedNode[0] = false;
                    cable.IsSelectedNode[1] = false;
                }
            }

            #endregion


            // マウスボタンを押した地点を設定。
            this.clickedLocation = e.Location;
            this.isClickedLocationVisible = true;

            // マウスボタンを放した地点を消す。
            this.isReleaseMouseButtonLocationVisible = false;


            // ラジオボタンのように。
            #region ラジオボタンのように

            // 今回、マウスボタンで押されたボタン
            string pressedBtn="";
            if (this.buttons["coordMat"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["coordMat"].Id;
            }
            else if (this.buttons["scene"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["scene"].Id;
            }

            if ("" != pressedBtn)
            {
                // 選択されていればオン、選択されていなければオフ。
                if (this.buttons["coordMat"].Id == pressedBtn)
                {
                    this.buttons["coordMat"].IsSelected = true;
                }
                else
                {
                    this.buttons["coordMat"].IsSelected = false;
                    this.buttons["coordMat"].SwitchOffAction();
                }

                if (this.buttons["scene"].Id == pressedBtn)
                {
                    this.buttons["scene"].IsSelected = true;
                }
                else
                {
                    this.buttons["scene"].IsSelected = false;
                    this.buttons["scene"].SwitchOffAction();
                }
            }

            // ラジオボタンのように。
            // 今回、マウスボタンで押されたボタン
            pressedBtn = "";
            if (this.buttons["move"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["move"].Id;
            }
            else if (this.buttons["moveMat"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["moveMat"].Id;
            }
            else if (this.buttons["extendT"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["extendT"].Id;
            }
            else if (this.buttons["extendR"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["extendR"].Id;
            }
            else if (this.buttons["extendB"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["extendB"].Id;
            }
            else if (this.buttons["extendL"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["extendL"].Id;
            }
            else if (this.buttons["reductT"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["reductT"].Id;
            }
            else if (this.buttons["reductR"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["reductR"].Id;
            }
            else if (this.buttons["reductB"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["reductB"].Id;
            }
            else if (this.buttons["reductL"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["reductL"].Id;
            }
            else if (this.buttons["detachesH"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["detachesH"].Id;
            }
            else if (this.buttons["detachesV"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["detachesV"].Id;
            }
            else if (this.buttons["shortenH"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["shortenH"].Id;
            }
            else if (this.buttons["shortenV"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["shortenV"].Id;
            }
            else if (this.buttons["text"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["text"].Id;
            }
            else if (this.buttons["create"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["create"].Id;
            }
            else if (this.buttons["copy"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["copy"].Id;
            }
            else if (this.buttons["delete"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["delete"].Id;
            }
            else if (this.buttons["connect"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["connect"].Id;
            }
            else if (this.buttons["cable4"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["cable4"].Id;
            }
            else if (this.buttons["shape"].IsHit(e.Location))
            {
                pressedBtn = this.buttons["shape"].Id;
            }

            // 選択されていればオン、選択されていなければオフ。
            if ("" != pressedBtn)
            {
                // 移動、マット移動
                if (this.buttons["move"].Id == pressedBtn)
                {
                    this.buttons["move"].IsSelected = true;
                }
                else
                {
                    this.buttons["move"].IsSelected = false;
                    this.buttons["move"].SwitchOffAction();
                }

                if (this.buttons["moveMat"].Id == pressedBtn)
                {
                    this.buttons["moveMat"].IsSelected = true;
                }
                else
                {
                    this.buttons["moveMat"].IsSelected = false;
                    this.buttons["moveMat"].SwitchOffAction();
                }

                // 拡張　上、右、下、左
                if (this.buttons["extendT"].Id == pressedBtn)
                {
                    this.buttons["extendT"].IsSelected = true;
                }
                else
                {
                    this.buttons["extendT"].IsSelected = false;
                    this.buttons["extendT"].SwitchOffAction();
                }

                if (this.buttons["extendR"].Id == pressedBtn)
                {
                    this.buttons["extendR"].IsSelected = true;
                }
                else
                {
                    this.buttons["extendR"].IsSelected = false;
                    this.buttons["extendR"].SwitchOffAction();
                }

                if (this.buttons["extendB"].Id == pressedBtn)
                {
                    this.buttons["extendB"].IsSelected = true;
                }
                else
                {
                    this.buttons["extendB"].IsSelected = false;
                    this.buttons["extendB"].SwitchOffAction();
                }

                if (this.buttons["extendL"].Id == pressedBtn)
                {
                    this.buttons["extendL"].IsSelected = true;
                }
                else
                {
                    this.buttons["extendL"].IsSelected = false;
                    this.buttons["extendL"].SwitchOffAction();
                }

                // 縮小　上、右、下、左
                if (this.buttons["reductT"].Id == pressedBtn)
                {
                    this.buttons["reductT"].IsSelected = true;
                }
                else
                {
                    this.buttons["reductT"].IsSelected = false;
                    this.buttons["reductT"].SwitchOffAction();
                }

                if (this.buttons["reductR"].Id == pressedBtn)
                {
                    this.buttons["reductR"].IsSelected = true;
                }
                else
                {
                    this.buttons["reductR"].IsSelected = false;
                    this.buttons["reductR"].SwitchOffAction();
                }

                if (this.buttons["reductB"].Id == pressedBtn)
                {
                    this.buttons["reductB"].IsSelected = true;
                }
                else
                {
                    this.buttons["reductB"].IsSelected = false;
                    this.buttons["reductB"].SwitchOffAction();
                }

                if (this.buttons["reductL"].Id == pressedBtn)
                {
                    this.buttons["reductL"].IsSelected = true;
                }
                else
                {
                    this.buttons["reductL"].IsSelected = false;
                    this.buttons["reductL"].SwitchOffAction();
                }

                // 離す、縮める
                if (this.buttons["detachesH"].Id == pressedBtn)
                {
                    this.buttons["detachesH"].IsSelected = true;
                }
                else
                {
                    this.buttons["detachesH"].IsSelected = false;
                    this.buttons["detachesH"].SwitchOffAction();
                }

                if (this.buttons["detachesV"].Id == pressedBtn)
                {
                    this.buttons["detachesV"].IsSelected = true;
                }
                else
                {
                    this.buttons["detachesV"].IsSelected = false;
                    this.buttons["detachesV"].SwitchOffAction();
                }

                if (this.buttons["shortenH"].Id == pressedBtn)
                {
                    this.buttons["shortenH"].IsSelected = true;
                }
                else
                {
                    this.buttons["shortenH"].IsSelected = false;
                    this.buttons["shortenH"].SwitchOffAction();
                }

                if (this.buttons["shortenV"].Id == pressedBtn)
                {
                    this.buttons["shortenV"].IsSelected = true;
                }
                else
                {
                    this.buttons["shortenV"].IsSelected = false;
                    this.buttons["shortenV"].SwitchOffAction();
                }

                // テキスト
                if (this.buttons["text"].Id == pressedBtn)
                {
                    this.buttons["text"].IsSelected = true;
                }
                else
                {
                    this.buttons["text"].IsSelected = false;
                    this.buttons["text"].SwitchOffAction();
                }

                // 作成、コピー、削除
                if (this.buttons["create"].Id == pressedBtn)
                {
                    this.buttons["create"].IsSelected = true;
                }
                else
                {
                    this.buttons["create"].IsSelected = false;
                    this.buttons["create"].SwitchOffAction();
                }

                if (this.buttons["copy"].Id == pressedBtn)
                {
                    this.buttons["copy"].IsSelected = true;
                }
                else
                {
                    this.buttons["copy"].IsSelected = false;
                    this.buttons["copy"].SwitchOffAction();
                }

                if (this.buttons["delete"].Id == pressedBtn)
                {
                    this.buttons["delete"].IsSelected = true;
                }
                else
                {
                    this.buttons["delete"].IsSelected = false;
                    this.buttons["delete"].SwitchOffAction();
                }

                // 接続、4節接続、切断
                if (this.buttons["connect"].Id == pressedBtn)
                {
                    this.buttons["connect"].IsSelected = true;
                }
                else
                {
                    this.buttons["connect"].IsSelected = false;
                    this.buttons["connect"].SwitchOffAction();
                }

                if (this.buttons["cable4"].Id == pressedBtn)
                {
                    this.buttons["cable4"].IsSelected = true;
                }
                else
                {
                    this.buttons["cable4"].IsSelected = false;
                    this.buttons["cable4"].SwitchOffAction();
                }

                // 形状切替
                if (this.buttons["shape"].Id == pressedBtn)
                {
                    this.buttons["shape"].IsSelected = true;
                }
                else
                {
                    this.buttons["shape"].IsSelected = false;
                    this.buttons["shape"].SwitchOffAction();
                }
            }

            #endregion


            // ボタンエリアと被っていない領域でマウスダウンをした場合。
            if (this.buttonAreaBottom < e.Location.Y)
            {
                if (this.buttons["scene"].IsSelected)
                {
                    if (this.buttons["connect"].IsSelected)
                    {
                        //────────────────────────────────────────
                        // シーンモード　2節接続モード
                        //────────────────────────────────────────
                        #region 2節接続モード

                        if (!buttonReleased)
                        {
                            // ボタンの上で放した場合以外に限る。

                            bool isHit = false;
                            // 座標マット
                            int x = this.coordMat.SourceBounds.X;
                            int y = this.coordMat.SourceBounds.Y;
                            int width = this.coordMat.SourceBounds.Width;
                            int height = this.coordMat.SourceBounds.Height;
                            for (int row = 0; row < this.coordMatRepeatY; row++)
                            {
                                for (int column = 0; column < this.coordMatRepeatX; column++)
                                {
                                    this.coordMat.FileName = row + "_" + column + ".png";
                                    this.coordMat.SourceBounds = new Rectangle(
                                        column * width + x,
                                        row * height + y,
                                        width,
                                        height
                                        );
                                    if (this.coordMat.IsHit(e.Location))
                                    {
                                        isHit = true;
                                        goto BREAK_FOR1;
                                    }
                                }
                            }
                        BREAK_FOR1:
                            this.coordMat.SourceBounds = new Rectangle(x, y, width, height);

                            if (isHit)
                            {
                                Cable cable = null;
                                if (0 < this.CableList.Count)
                                {
                                    // 既存ケーブルの最終要素
                                    cable = this.CableList[this.CableList.Count - 1];

                                    // [0]起点　かつ　[1]終点　の両方が表示されている場合、新しく追加。
                                    if (cable.IsVisible[0] && cable.IsVisible[1])
                                    {
                                        cable = new Cable();
                                        this.CableList.Add(cable);
                                    }
                                }
                                else
                                {
                                    cable = new Cable();
                                    this.CableList.Add(cable);
                                }

                                // ──────────
                                // [0]起点　[1]終点
                                // ──────────
                                int i;
                                if (!cable.IsVisible[0])
                                {
                                    i = 0;
                                }
                                else
                                {
                                    i = 1;
                                }

                                cable.IsVisible[i] = true;
                                cable.SourceBounds[i] = new Rectangle(
                                    (e.Location.X) - (e.Location.X - this.coordMat.Bounds.X) % UiMain.CELL_SIZE,
                                    (e.Location.Y) - (e.Location.Y - this.coordMat.Bounds.Y) % UiMain.CELL_SIZE,
                                    UiMain.CELL_SIZE,
                                    UiMain.CELL_SIZE
                                    );
                            }
                        }

                        #endregion
                    }
                    else if (this.buttons["cable4"].IsSelected)
                    {
                        //────────────────────────────────────────
                        // シーンモード　4節接続モード
                        //────────────────────────────────────────
                        #region 4節接続モード

                        if (!buttonReleased)
                        {
                            // ボタンの上で放した場合以外に限る。

                            Cable cable = new Cable();

                            // 始点
                            cable.IsVisible[0] = true;
                            cable.SourceBounds[0] = new Rectangle(
                                (e.Location.X) - (e.Location.X - this.coordMat.Bounds.X) % UiMain.CELL_SIZE,
                                (e.Location.Y) - (e.Location.Y - this.coordMat.Bounds.Y) % UiMain.CELL_SIZE,
                                UiMain.CELL_SIZE,
                                UiMain.CELL_SIZE
                                );

                            // 中間点0
                            cable.IsVisibleNode[0] = true;
                            cable.SourceBoundsNode[0] = new Point(
                                (e.Location.X) - (e.Location.X - this.coordMat.Bounds.X) % UiMain.CELL_SIZE + 2*UiMain.CELL_SIZE,
                                (e.Location.Y) - (e.Location.Y - this.coordMat.Bounds.Y) % UiMain.CELL_SIZE + 2 * UiMain.CELL_SIZE
                                );

                            // 中間点1
                            cable.IsVisibleNode[1] = true;
                            cable.SourceBoundsNode[1] = new Point(
                                (e.Location.X) - (e.Location.X - this.coordMat.Bounds.X) % UiMain.CELL_SIZE - 4 * UiMain.CELL_SIZE,
                                (e.Location.Y) - (e.Location.Y - this.coordMat.Bounds.Y) % UiMain.CELL_SIZE - 2 * UiMain.CELL_SIZE
                                );

                            // 終点
                            cable.IsVisible[1] = true;
                            cable.SourceBounds[1] = new Rectangle(
                                (e.Location.X) - (e.Location.X - this.coordMat.Bounds.X) % UiMain.CELL_SIZE - 2 * UiMain.CELL_SIZE,
                                (e.Location.Y) - (e.Location.Y - this.coordMat.Bounds.Y) % UiMain.CELL_SIZE - 2 * UiMain.CELL_SIZE,
                                UiMain.CELL_SIZE,
                                UiMain.CELL_SIZE
                                );

                            this.CableList.Add(cable);
                        }

                        #endregion
                    }
                    else
                    {
                        //────────────────────────────────────────
                        // シーンモード　その他
                        //────────────────────────────────────────

                        // ──────────
                        // (1)接続線 (2)シーン　排他選択
                        // ──────────
                        #region 接続線_選択
                        // 始端終端
                        SceneBox scene2 = null;
                        Cable cable2 = null;
                        int i = 0;
                        foreach (Cable cable in this.CableList)
                        {
                            if (cable.IsHit(1,e.Location))
                            {
                                cable2 = cable;
                                i = 1;
                                // 最初の１件のみ
                                break;
                            }
                            else if (cable.IsHit(0,e.Location))
                            {
                                cable2 = cable;
                                i = 0;
                                // 最初の１件のみ
                                break;
                            }
                        }

                        if (null != cable2)
                        {
                            cable2.IsSelected[i] = true;
                            goto END_CABLE_SELECT;
                        }

                        // 中間点2つ
                        cable2 = null;
                        i = 0;
                        foreach (Cable cable in this.CableList)
                        {
                            if (cable.IsHitNode(1,e.Location))
                            {
                                cable2 = cable;
                                i = 1;
                                // 最初の１件のみ
                                break;
                            }
                            else if (cable.IsHitNode(0,e.Location))
                            {
                                cable2 = cable;
                                i = 0;
                                // 最初の１件のみ
                                break;
                            }
                        }

                        if (null != cable2)
                        {
                            cable2.IsSelectedNode[i] = true;
                            goto END_CABLE_SELECT;
                        }

                        // ──────────
                        // 選択シーンボックス
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            if (scene.IsHit(e.Location))
                            {
                                scene2 = scene;
                                // 最初の１件のみ
                                break;
                            }
                        }

                    END_CABLE_SELECT:
                        ;
                        #endregion


                        if (this.buttons["copy"].IsSelected)
                        {
                            if (null != scene2)
                            {
                                //────────────────────────────────────────
                                // コピー
                                //────────────────────────────────────────
                                #region コピー

                                SceneBox scene3 = scene2.Clone();
                                scene3.SourceBounds = new Rectangle(
                                    scene3.SourceBounds.X + UiMain.CELL_SIZE,
                                    scene3.SourceBounds.Y + UiMain.CELL_SIZE,
                                    scene3.SourceBounds.Width,
                                    scene3.SourceBounds.Height
                                    );
                                this.SceneBoxList.Add(scene3);

                                #endregion
                            }

                        }
                        else if (this.buttons["shape"].IsSelected)
                        {
                            if (null != scene2)
                            {
                                //────────────────────────────────────────
                                // 形状切替
                                //────────────────────────────────────────
                                #region 形状切替

                                switch (scene2.Shape)
                                {
                                    case 0:
                                        scene2.Shape = 1;
                                        break;
                                    case 1:
                                        scene2.Shape = 2;
                                        break;
                                    default:
                                        scene2.Shape = 0;
                                        break;
                                }

                                #endregion
                            }

                        }
                        else if (this.buttons["text"].IsSelected)
                        {
                            if (null != scene2)
                            {
                                //────────────────────────────────────────
                                // テキストモード
                                //────────────────────────────────────────
                                #region テキストモード

                                //他のシーンを選択解除
                                foreach (SceneBox scene in this.SceneBoxList)
                                {
                                    scene.IsSelected = false;
                                }

                                scene2.IsSelected = true;
                                this.textBox1.Visible = true;
                                this.textBox1.Bounds = new Rectangle(
                                    scene2.Bounds.X,
                                    scene2.Bounds.Y,
                                    scene2.Bounds.Width,
                                    scene2.Bounds.Height
                                    );
                                this.textBox1.Text = scene2.Title;
                                this.textBox1.Font = new Font(scene2.FontName, scene2.FontSize);

                                this.textBox1.Focus();
                                this.textBox1.SelectAll();

                                #endregion
                            }

                        }
                        else if (this.buttons["delete"].IsSelected)
                        {
                            //────────────────────────────────────────
                            // 削除モード
                            //────────────────────────────────────────
                            #region 削除モード

                            this.CableList.Remove(cable2);
                            this.SceneBoxList.Remove(scene2);

                            #endregion
                        }
                        else
                        {
                            if (null != scene2)
                            {
                                scene2.IsSelected = true;
                            }
                        }

                    }
                }
                else if (this.buttons["coordMat"].IsSelected)
                {
                    if (this.buttons["detachesH"].IsSelected)
                    {
                        //────────────────────────────────────────
                        // 座標マットモード　離す横
                        //────────────────────────────────────────
                        #region 座標マットモード　離す横

                        // マウスクリック位置のグリッド列以右のものを、グリッド１つ分右にずらします。

                        // ──────────
                        // 該当シーンボックス
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            if (e.Location.X - e.Location.X % UiMain.CELL_SIZE <= scene.Bounds.X)
                            {
                                scene.SourceBounds = new Rectangle(
                                    scene.SourceBounds.X + UiMain.CELL_SIZE,
                                    scene.SourceBounds.Y,
                                    scene.SourceBounds.Width,
                                    scene.SourceBounds.Height
                                    );
                            }
                        }

                        // ──────────
                        // 該当接続線
                        // ──────────
                        foreach (Cable cable in this.CableList)
                        {
                            // 始端、終端
                            for (int i = 0; i < 2; i++)
                            {
                                if (e.Location.X - e.Location.X % UiMain.CELL_SIZE <= cable.Bounds[i].X)
                                {
                                    cable.SourceBounds[i] = new Rectangle(
                                        cable.SourceBounds[i].X + UiMain.CELL_SIZE,
                                        cable.SourceBounds[i].Y,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                }
                            }

                            // 中間点1～2
                            for (int i = 0; i < 2; i++)
                            {
                                if (e.Location.X - e.Location.X % UiMain.CELL_SIZE <= cable.BoundsNode[i].X)
                                {
                                    cable.SourceBoundsNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X + UiMain.CELL_SIZE,
                                        cable.SourceBoundsNode[i].Y
                                        );
                                }
                            }
                        }

                        #endregion
                    }
                    else if (this.buttons["detachesV"].IsSelected)
                    {
                        //────────────────────────────────────────
                        // 座標マットモード　離す縦
                        //────────────────────────────────────────
                        #region 座標マットモード　離す縦

                        // マウスクリック位置のグリッド行以下のものを、グリッド１つ分下げます。

                        // ──────────
                        // 該当シーンボックス
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            if (e.Location.Y - e.Location.Y % UiMain.CELL_SIZE <= scene.Bounds.Y)
                            {
                                scene.SourceBounds = new Rectangle(
                                    scene.SourceBounds.X,
                                    scene.SourceBounds.Y + UiMain.CELL_SIZE,
                                    scene.SourceBounds.Width,
                                    scene.SourceBounds.Height
                                    );
                            }
                        }

                        // ──────────
                        // 該当接続線
                        // ──────────
                        foreach (Cable cable in this.CableList)
                        {
                            // 始端、終端
                            for (int i = 0; i < 2; i++)
                            {
                                if (e.Location.Y - e.Location.Y % UiMain.CELL_SIZE <= cable.Bounds[i].Y)
                                {
                                    cable.SourceBounds[i] = new Rectangle(
                                        cable.SourceBounds[i].X,
                                        cable.SourceBounds[i].Y + UiMain.CELL_SIZE,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                }
                            }

                            // 中間点1～2
                            for (int i = 0; i < 2; i++)
                            {
                                if (e.Location.Y - e.Location.Y % UiMain.CELL_SIZE <= cable.BoundsNode[i].Y)
                                {
                                    cable.SourceBoundsNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X,
                                        cable.SourceBoundsNode[i].Y + UiMain.CELL_SIZE
                                        );
                                }
                            }
                        }

                        #endregion
                    }
                    else if (this.buttons["shortenH"].IsSelected)
                    {
                        //────────────────────────────────────────
                        // 座標マットモード　縮める横
                        //────────────────────────────────────────
                        #region 座標マットモード　縮める横

                        // マウスクリック位置のグリッド列以右のものを、グリッド１つ分左にずらします。

                        // ──────────
                        // 該当シーンボックス
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            if (e.Location.X - e.Location.X % UiMain.CELL_SIZE <= scene.Bounds.X)
                            {
                                scene.SourceBounds = new Rectangle(
                                    scene.SourceBounds.X - UiMain.CELL_SIZE,
                                    scene.SourceBounds.Y,
                                    scene.SourceBounds.Width,
                                    scene.SourceBounds.Height
                                    );
                            }
                        }

                        // ──────────
                        // 該当接続線
                        // ──────────
                        foreach (Cable cable in this.CableList)
                        {
                            // 始点、終点
                            for (int i = 0; i < 2; i++)
                            {
                                if (e.Location.X - e.Location.X % UiMain.CELL_SIZE <= cable.Bounds[i].X)
                                {
                                    cable.SourceBounds[i] = new Rectangle(
                                        cable.SourceBounds[i].X - UiMain.CELL_SIZE,
                                        cable.SourceBounds[i].Y,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                }
                            }

                            // 中間点1～2
                            for (int i = 0; i < 2; i++)
                            {
                                if (e.Location.X - e.Location.X % UiMain.CELL_SIZE <= cable.BoundsNode[i].X)
                                {
                                    cable.SourceBoundsNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X - UiMain.CELL_SIZE,
                                        cable.SourceBoundsNode[i].Y
                                        );
                                }
                            }
                        }

                        #endregion
                    }
                    else if (this.buttons["shortenV"].IsSelected)
                    {
                        //────────────────────────────────────────
                        // 座標マットモード　縮める縦
                        //────────────────────────────────────────
                        #region 座標マットモード　縮める縦

                        // マウスクリック位置のグリッド行以下のものを、グリッド１つ分下げます。

                        // ──────────
                        // 該当シーンボックス
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            if (e.Location.Y - e.Location.Y % UiMain.CELL_SIZE <= scene.Bounds.Y)
                            {
                                scene.SourceBounds = new Rectangle(
                                    scene.SourceBounds.X,
                                    scene.SourceBounds.Y - UiMain.CELL_SIZE,
                                    scene.SourceBounds.Width,
                                    scene.SourceBounds.Height
                                    );
                            }
                        }

                        // ──────────
                        // 該当接続線
                        // ──────────
                        foreach (Cable cable in this.CableList)
                        {
                            // 始点、終点
                            for (int i = 0; i < 2; i++)
                            {
                                if (e.Location.Y - e.Location.Y % UiMain.CELL_SIZE <= cable.Bounds[i].Y)
                                {
                                    cable.SourceBounds[i] = new Rectangle(
                                        cable.SourceBounds[i].X,
                                        cable.SourceBounds[i].Y - UiMain.CELL_SIZE,
                                        cable.SourceBounds[i].Width,
                                        cable.SourceBounds[i].Height
                                        );
                                }
                            }

                            // 中間点1～2
                            for (int i = 0; i < 2; i++)
                            {
                                if (e.Location.Y - e.Location.Y % UiMain.CELL_SIZE <= cable.BoundsNode[i].Y)
                                {
                                    cable.SourceBoundsNode[i] = new Point(
                                        cable.SourceBoundsNode[i].X,
                                        cable.SourceBoundsNode[i].Y - UiMain.CELL_SIZE
                                        );
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        this.isRowCursorXEnable = false;
                        this.isRowCursorYEnable = false;
                    }
                }
            }


            this.Refresh();
        }

        private void UiMain_Load(object sender, EventArgs e)
        {
            this.buttons["save"].Load();
            this.buttons["load"].Load();
            this.buttons["screenShot"].Load();
            this.buttons["coordMat"].Load();
            this.buttons["scene"].Load();
            this.buttons["move"].Load();
            this.buttons["moveMat"].Load();
            this.buttons["extendT"].Load();
            this.buttons["extendR"].Load();
            this.buttons["extendB"].Load();
            this.buttons["extendL"].Load();
            this.buttons["reductT"].Load();
            this.buttons["reductR"].Load();
            this.buttons["reductB"].Load();
            this.buttons["reductL"].Load();
            this.buttons["detachesH"].Load();
            this.buttons["detachesV"].Load();
            this.buttons["shortenH"].Load();
            this.buttons["shortenV"].Load();
            this.buttons["text"].Load();
            this.buttons["create"].Load();
            this.buttons["copy"].Load();
            this.buttons["delete"].Load();
            this.buttons["connect"].Load();
            this.buttons["cable4"].Load();
            this.buttons["shape"].Load();
        }

        private void UiMain_MouseMove(object sender, MouseEventArgs e)
        {
            // マウスオーバー
            #region マウスオーバー

            bool forcedOff = false;
            if (this.buttons["coordMat"].IsSelected)
            {
                //────────────────────────────────────────
                // 座標マットモード
                //────────────────────────────────────────
            }
            else if (this.buttons["scene"].IsSelected)
            {
                //────────────────────────────────────────
                // シーンモード
                //────────────────────────────────────────

                // ・接続線
                // ・シーン
                //にマウスカーソルが合わさったとき、枠線を緑色にします。

                // ──────────
                // 優先順1　全接続線
                // ──────────
                foreach (Cable cable in this.CableList)
                {
                    cable.CheckMouseOver(0,e.Location, ref forcedOff);
                    cable.CheckMouseOver(1,e.Location, ref forcedOff);
                    cable.CheckMouseOverNode(0, e.Location, ref forcedOff);
                    cable.CheckMouseOverNode(1, e.Location, ref forcedOff);
                }

                // ──────────
                // 優先順2　全シーン
                // ──────────
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.CheckMouseOver(e.Location, ref forcedOff);
                }
            }

            //────────────────────────────────────────
            // 優先順3　ボタン
            //────────────────────────────────────────

            // セーブ、ロード、スクリーンショット
            this.buttons["save"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["load"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["screenShot"].CheckMouseOver(e.Location, ref forcedOff);

            // 座標マット
            this.buttons["coordMat"].CheckMouseOver(e.Location, ref forcedOff);

            // シーン
            this.buttons["scene"].CheckMouseOver(e.Location, ref forcedOff);

            // 移動
            this.buttons["move"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["moveMat"].CheckMouseOver(e.Location, ref forcedOff);

            // 拡張　上、右、下、左
            this.buttons["extendT"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["extendR"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["extendB"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["extendL"].CheckMouseOver(e.Location, ref forcedOff);

            // 縮小　上、右、下、左
            this.buttons["reductT"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["reductR"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["reductB"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["reductL"].CheckMouseOver(e.Location, ref forcedOff);

            // 離す、縮める
            this.buttons["detachesH"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["detachesV"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["shortenH"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["shortenV"].CheckMouseOver(e.Location, ref forcedOff);

            // テキスト
            this.buttons["text"].CheckMouseOver(e.Location, ref forcedOff);

            // 作成、コピー、削除
            this.buttons["create"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["copy"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["delete"].CheckMouseOver(e.Location, ref forcedOff);

            // 接続、4節接続
            this.buttons["connect"].CheckMouseOver(e.Location, ref forcedOff);
            this.buttons["cable4"].CheckMouseOver(e.Location, ref forcedOff);

            // 形状切替
            this.buttons["shape"].CheckMouseOver(e.Location, ref forcedOff);

            #endregion


            if (this.buttons["text"].IsSelected)
            {
                //────────────────────────────────────────
                // テキストモード
                //────────────────────────────────────────

                // 移動モード禁止。
            }
            else
            {
                if (this.isClickedLocationVisible && !this.isReleaseMouseButtonLocationVisible)
                {
                    // マウス・ドラッグの移動量を測ります。
                    if (this.buttons["moveMat"].IsSelected)
                    {
                        // マット移動モードの場合、グリッド単位で移動します。
                        this.Movement = new Point(
                            e.Location.X - this.clickedLocation.X - (e.Location.X - this.clickedLocation.X)%UiMain.CELL_SIZE,
                            e.Location.Y - this.clickedLocation.Y - (e.Location.Y - this.clickedLocation.Y) % UiMain.CELL_SIZE
                            );
                    }
                    else
                    {
                        this.Movement = new Point(
                            e.Location.X - this.clickedLocation.X,
                            e.Location.Y - this.clickedLocation.Y
                            );
                    }
                    //ystem.Console.WriteLine("ドラッグ中 e(" + e.Location.X + "," + e.Location.Y + ") click(" + this.clickedLocation.X + "," + this.clickedLocation.Y + ") move(" + this.Movement.X + "," + this.Movement.Y + ")");

                    bool isMoveCoordMat = false;
                    int moveSceneFlg = 0;// 1:全シーン移動。2:選択シーンのみ移動。
                    bool moveCableFlg = false;
                    // 座標マットモードの場合
                    if (this.buttons["coordMat"].IsSelected)
                    {
                        isMoveCoordMat = true;

                        if (this.buttons["moveMat"].IsSelected)
                        {
                            // マット移動モードの場合

                            // シーン、接続線は動かしません。
                        }
                        else
                        {
                            moveSceneFlg = 1;
                            moveCableFlg = true;
                        }
                    }
                    // シーンモードの場合
                    if (this.buttons["scene"].IsSelected)
                    {
                        moveSceneFlg = 2;
                    }

                    // 選択している構成部品に、移動量をセットします。
                    if (isMoveCoordMat)
                    {
                        this.coordMat.Movement = new Rectangle(this.Movement.X, this.Movement.Y, 0, 0);
                    }

                    if (moveSceneFlg == 1)
                    {
                        // ──────────
                        // 全シーン　移動量セット
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            int x;
                            int y;
                            if (isMoveCoordMat)
                            {
                                x = this.Movement.X;
                                y = this.Movement.Y;
                            }
                            else
                            {
                                // ずれ補正
                                //
                                //　　移動量は、グリッドサイズの倍数となる。
                                //
                                x = this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                                y = this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                            }

                            scene.Movement = new Rectangle(
                                x,
                                y,
                                0, 0);
                        }
                    }
                    else if (moveSceneFlg == 2)
                    {
                        // ──────────
                        // 選択シーン　移動量セット
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            if (scene.IsSelected)
                            {
                                // ずれ補正
                                //
                                //　　移動量は、グリッドサイズの倍数となる。
                                //
                                scene.Movement = new Rectangle(
                                    this.Movement.X - this.Movement.X % UiMain.CELL_SIZE,
                                    this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE,
                                    0, 0);
                            }
                        }

                        // ──────────
                        // 選択接続線　移動量セット
                        // ──────────
                        foreach (Cable cable in this.CableList)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (cable.IsSelected[i])
                                {
                                    cable.Movement[i] = new Rectangle(
                                        this.Movement.X - this.Movement.X % UiMain.CELL_SIZE,
                                        this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE,
                                        0, 0);
                                }
                            }

                            for (int i = 0; i < 2; i++)
                            {
                                if (cable.IsSelectedNode[i])
                                {
                                    cable.MovementNode[i] = new Point(
                                        this.Movement.X - this.Movement.X % UiMain.CELL_SIZE,
                                        this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE
                                        );
                                }
                            }
                        }
                    }

                    if (moveCableFlg)
                    {
                        // ──────────
                        // 全接続線　移動量セット
                        // ──────────
                        foreach (Cable cable in this.CableList)
                        {
                            // ──────────
                            // [0]起点　[1]終点
                            // ──────────
                            for (int i = 0; i < 2; i++)
                            {
                                int x;
                                int y;
                                if (isMoveCoordMat)
                                {
                                    x = this.Movement.X;
                                    y = this.Movement.Y;
                                }
                                else
                                {
                                    x = this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                                    y = this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                                }

                                cable.Movement[i] = new Rectangle(
                                    x,
                                    y,
                                    0, 0);
                            }

                            for (int i = 0; i < 2; i++)
                            {
                                int x;
                                int y;
                                if (isMoveCoordMat)
                                {
                                    x = this.Movement.X;
                                    y = this.Movement.Y;
                                }
                                else
                                {
                                    x = this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                                    y = this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                                }

                                cable.MovementNode[i] = new Point(
                                    x,
                                    y);
                            }
                        }
                    }
                }
                else
                {
                    this.Movement = new Point();
                }
            }


            if (this.buttons["coordMat"].IsSelected)
            {
                if (this.buttons["detachesH"].IsSelected)
                {
                    //────────────────────────────────────────
                    // 座標マットモード　離す横
                    //────────────────────────────────────────
                    #region 座標マットモード　離す横

                    // ──────────
                    // マウスオーバー列
                    // ──────────
                    this.rowCursorX = e.Location.X - e.Location.X % UiMain.CELL_SIZE + this.coordMat.Bounds.X % UiMain.CELL_SIZE;
                    this.isRowCursorXEnable = true;
                    this.isRowCursorYEnable = false;

                    #endregion
                }
                else if (this.buttons["detachesV"].IsSelected)
                {
                    //────────────────────────────────────────
                    // 座標マットモード　離す縦
                    //────────────────────────────────────────
                    #region 座標マットモード　離す縦

                    // ──────────
                    // マウスオーバー行
                    // ──────────
                    this.rowCursorY = e.Location.Y - e.Location.Y % UiMain.CELL_SIZE + this.coordMat.Bounds.Y % UiMain.CELL_SIZE;
                    this.isRowCursorYEnable = true;
                    this.isRowCursorXEnable = false;

                    #endregion
                }
                else if (this.buttons["shortenH"].IsSelected)
                {
                    //────────────────────────────────────────
                    // 座標マットモード　縮める横
                    //────────────────────────────────────────
                    #region 座標マットモード　縮める横

                    // ──────────
                    // マウスオーバー列
                    // ──────────
                    this.rowCursorX = e.Location.X - e.Location.X % UiMain.CELL_SIZE + this.coordMat.Bounds.X % UiMain.CELL_SIZE;
                    this.isRowCursorXEnable = true;
                    this.isRowCursorYEnable = false;

                    #endregion
                }
                else if (this.buttons["shortenV"].IsSelected)
                {
                    //────────────────────────────────────────
                    // 座標マットモード　縮める縦
                    //────────────────────────────────────────
                    #region 座標マットモード　縮める縦

                    // ──────────
                    // マウスオーバー行
                    // ──────────
                    this.rowCursorY = e.Location.Y - e.Location.Y % UiMain.CELL_SIZE + this.coordMat.Bounds.Y % UiMain.CELL_SIZE;
                    this.isRowCursorYEnable = true;
                    this.isRowCursorXEnable = false;

                    #endregion
                }
                else
                {
                    this.isRowCursorXEnable = false;
                    this.isRowCursorYEnable = false;
                }
            }

            this.Refresh();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            this.textBox1.Visible = false;
        }
    }
}
