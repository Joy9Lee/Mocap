using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace waveformClib
{
    delegate void SafeGraphRefresh();
    public partial class waveform : UserControl
    {

        public waveform()
        {
            InitializeComponent();
            //初始化默认坐标值，坐标标定值和坐标标定权值
            _fXBegin = _fXBeginGO = _fXBeginSYS;
            _fYBegin = _fYBeginGO = _fYBeginSYS;
            _fXEnd = _fXEndGO = _fXEndSYS;
            _fYEnd = _fYEndGO = _fYEndSYS;
            _fXQuanBeginGO = _getQuan(_fXBeginGO);
            _fXQuanEndGO = _getQuan(_fXEndGO);
            _fYQuanBeginGO = _getQuan(_fYBeginGO);
            _fYQuanEndGO = _getQuan(_fYEndGO);

            //配色方案初始化

            //波形显示区域背景色
            pictureBoxGraph.BackColor = _GraphBackColor;
            //显示模式
            Auto_move();
        }

        #region ** Paint Function **
        #region **私有函数 画一个点 **
        public void drawpoint(double x1, double y1, double x2, double y2, Color color)
        {
            float gap;
            y1 = y1 * 1000; //V to mV
            y2 = y2 * 1000; //V to mV
            if (x2 > _fXEnd)
            {
                gap = _fXEnd - _fXBegin;
                _fXBegin = _fXEnd;
                _fXEnd = _fXBegin + gap;
                _fXQuanBeginGO = _getQuan(_fXBegin);
                _fXQuanEndGO = _getQuan(_fXEnd);
                _fYQuanBeginGO = _getQuan(_fYBegin);
                _fYQuanEndGO = _getQuan(_fYEnd);
                //f_Refresh();
                GraphRefresh();
            }
            Graphics MyGraphic = pictureBoxGraph.CreateGraphics();
            Pen pe = new Pen(color, 0.01f);
            x1 = (x1 - _fXBegin) * (pictureBoxGraph.Width - 1) / (_fXEnd - _fXBegin);
            y1 = (y1 - _fYBegin) * (pictureBoxGraph.Height - 1) / (_fYEnd - _fYBegin);
            x2 = (x2 - _fXBegin) * (pictureBoxGraph.Width - 1) / (_fXEnd - _fXBegin);
            y2 = (y2 - _fYBegin) * (pictureBoxGraph.Height - 1) / (_fYEnd - _fYBegin);
            MyGraphic.DrawLine(pe, (float)x1, (float)y1, (float)x2, (float)y2);

        }
        private void GraphRefresh()
        {
            if (pictureBoxGraph.InvokeRequired)
            {
                SafeGraphRefresh objSet = new SafeGraphRefresh(GraphRefresh);
                pictureBoxGraph.Invoke(objSet, new object[] { });
            }
            else
            {
                pictureBoxGraph.Refresh();
            }
        }
        #endregion

        #region **私有函数 波形显示区域Paint **
        private void pictureBoxGraph_Paint(object sender, PaintEventArgs e)
        {
            #region **绘图参数初始化**
            int width = pictureBoxGraph.Width;
            int height = pictureBoxGraph.Height;
            Graphics Grap = e.Graphics;

            List<Point> dotPoint = new List<Point>();
            #endregion

            using (Pen pe = new Pen(Color.White, 1f))
            {
                #region **坐标轴零点变换，以左下角为零点，右上方为正绘图**
                Grap.TranslateTransform(0, pictureBoxGraph.Height - 1);
                Grap.ScaleTransform(1, -1);
                #endregion

                #region **根据画图模式和数据调整坐标显示**
                if (_isMoveModeXY || _isShowNumModeXY)
                {
                    //放大缩小模式、移动模式、显示曲线坐标模式下坐标宽度不自动调整，使用当前坐标范围修改标定权值和标定坐标范围
                    #region **更新标定坐标权值和标定坐标范围**
                    _fXQuanBeginGO = _getQuan(_fXBegin);
                    _fXQuanEndGO = _getQuan(_fXEnd);
                    _fYQuanBeginGO = _getQuan(_fYBegin);
                    _fYQuanEndGO = _getQuan(_fYEnd);
                    _changXBegionOrEndGO(_fXBegin, true);
                    _changXBegionOrEndGO(_fXEnd, false);
                    _changYBegionOrEndGO(_fYBegin, true);
                    _changYBegionOrEndGO(_fYEnd, false);
                    #endregion
                }
                else if (_isAutoModeXY)
                {
                    //自动坐标模式
                    #region **获取数据集合的最大坐标范围，并修改标定坐标范围和标定权值，并修改坐标范围**
                    //遍历每条数据集合
                    for (int i = 0; i < _listY.Count; i++)
                    {
                        if (_haveFile[i])//如果判断有文件内容，则先分段读取文件里边的数据
                        {
                            long fileLen;
                            try
                            {
                                FileStream fs = new FileStream(_filePath[i], FileMode.Open, FileAccess.Read);
                                fileLen = fs.Length;
                                fs.Close();
                            }
                            catch
                            {
                                return;
                            }

                            for (int j = 0; j <= fileLen / 0x8000; j++)//分段读取文件里边的数据
                            {
                                dotPoint = _getPoint(i, j);            //读取一段数据
                                //遍历数据集合中的每个点
                                for (int k = 0; k < dotPoint.Count; k++)
                                {
                                    if (dotPoint[k].X < _fXBegin)
                                    {
                                        _changXBegionOrEndGO(dotPoint[k].X, true);
                                        _fXBegin = _fXBeginGO;
                                    }
                                    else if (dotPoint[k].X > _fXEnd)
                                    {
                                        _changXBegionOrEndGO(dotPoint[k].X, false);
                                        _fXEnd = _fXEndGO;
                                    }
                                    if (dotPoint[k].Y < _fYBegin)
                                    {
                                        _changYBegionOrEndGO(dotPoint[k].Y, true);
                                        _fYBegin = _fYBeginGO;
                                    }
                                    else if (dotPoint[k].Y > _fYEnd)
                                    {
                                        _changYBegionOrEndGO(dotPoint[k].Y, false);
                                        _fYEnd = _fYEndGO;
                                    }
                                }

                            }
                        }
                        if (_listX[i] != null)//遍历列表里边的数据
                        {
                            //遍历数据集合中的每个点
                            for (int j = 0; j < _listX[i].Count; j++)
                            {
                                if (_listX[i][j] < _fXBegin)
                                {
                                    _changXBegionOrEndGO(_listX[i][j], true);
                                    _fXBegin = _fXBeginGO;
                                }
                                else if (_listX[i][j] > _fXEnd)
                                {
                                    _changXBegionOrEndGO(_listX[i][j], false);
                                    _fXEnd = _fXEndGO;
                                }
                                if (_listY[i][j] < _fYBegin)
                                {
                                    _changYBegionOrEndGO(_listY[i][j], true);
                                    _fYBegin = _fYBeginGO;
                                }
                                else if (_listY[i][j] > _fYEnd)
                                {
                                    _changYBegionOrEndGO(_listY[i][j], false);
                                    _fYEnd = _fYEndGO;
                                }
                            }

                        }
                    }
                    #endregion
                }
                else if (_isDefaultMoveModeXY && !_isMoveModeXY && !_isAutoModeXY && !_isShowNumModeXY)
                {
                    //默认宽度坐标移动模式 则保证画图的点在图形中显示
                    #region **默认宽度坐标移动模式 保证曲线末尾在图像右边20个像素位置，并且波形一直向左平移**
                    //遍历每条数据集合
                    for (int i = 0; i < _listY.Count; i++)
                    {
                        if (_haveFile[i])
                        {
                            long fileLen;
                            try
                            {
                                FileStream fs = new FileStream(_filePath[i], FileMode.Open, FileAccess.Read);
                                fileLen = fs.Length;
                                fs.Close();
                            }
                            catch
                            {
                                return;
                            }

                            for (int j = 0; j <= fileLen / 0x8000; j++)
                            {
                                dotPoint = _getPoint(i, j);
                                for (int k = 0; k < dotPoint.Count; k++)
                                {
                                    float distanceX = 20 * (_fXEnd - _fXBegin) / pictureBoxGraph.Width;
                                    if (dotPoint[k].X > _fXEnd - distanceX)
                                    {
                                        _fXBeginGO = _fXBeginGO + dotPoint[k].X - _fXEndGO + distanceX;
                                        _fXEndGO = dotPoint[k].X + distanceX;
                                        _fXEnd = _fXEndGO;
                                        _fXBegin = _fXBeginGO;
                                    }
                                    if (dotPoint[k].X < _fXBegin)
                                    {
                                        _fXBeginGO = _fXBeginGO + dotPoint[k].X - _fXEndGO + distanceX;
                                        _fXEndGO = dotPoint[k].X + distanceX;
                                        _fXEnd = _fXEndGO;
                                        _fXBegin = _fXBeginGO;
                                    }
                                    /* Y 轴自动调整
                                    if (dotPoint[k].Y < _fYBegin)
                                    {
                                        _changYBegionOrEndGO(dotPoint[k].Y, true);
                                        _fYBegin = _fYBeginGO;
                                    }
                                    else if (dotPoint[k].Y > _fYEnd)
                                    {

                                        _changYBegionOrEndGO(dotPoint[k].Y, false);
                                        _fYEnd = _fYEndGO;
                                    }
                                     */
                                }
                            }
                        }
                        if (_listX[i] != null)
                        {
                            //遍历数据集合中的每个点
                            for (int j = 0; j < _listX[i].Count; j++)
                            {
                                float distanceX = 20 * (_fXEnd - _fXBegin) / pictureBoxGraph.Width;
                                if (_listX[i][j] > _fXEnd - distanceX)
                                {
                                    _fXBeginGO = _fXBeginGO + _listX[i][j] - _fXEndGO + distanceX;
                                    _fXEndGO = _listX[i][j] + distanceX;
                                    _fXEnd = _fXEndGO;
                                    _fXBegin = _fXBeginGO;
                                }
                                if (_listX[i][j] < _fXBegin)
                                {
                                    _fXBeginGO = _fXBeginGO + _listX[i][j] - _fXEndGO + distanceX;
                                    _fXEndGO = _listX[i][j] + distanceX;
                                    _fXEnd = _fXEndGO;
                                    _fXBegin = _fXBeginGO;
                                }
                                /* Y轴自动调整
                                if (_listY[i][j] < _fYBegin)
                                {
                                    _changYBegionOrEndGO(_listY[i][j], true);
                                    _fYBegin = _fYBeginGO;
                                }
                                else if (_listY[i][j] > _fYEnd)
                                {

                                    _changYBegionOrEndGO(_listY[i][j], false);
                                    _fYEnd = _fYEndGO;
                                }
                                 */
                            }
                        }
                    }
                    #endregion
                }
                //更新坐标显示
                pictureBoxLeft.Refresh();
                pictureBoxBottom.Refresh();
                #endregion

                #region **网格显示设置**
                if (_isLinesShowXY)
                {
                    #region **画网格线**
                    #region **参数初始化**
                    Grap.SmoothingMode = SmoothingMode.None;
                    pe.Width = 1;
                    float i = _fXpxGO;  //临时 计数
                    #endregion
                    #region **画垂直网格**
                    #region **第三段**
                    pe.Color = Color.FromArgb(_iLineShowColorAlpha / 3, _iLineShowColor);   //画笔颜色
                    if (_fXLinesShowThird != 0)
                    {
                        if (_bXLinesLBegin)
                        {
                            while (i < width)
                            {
                                Grap.DrawLine(pe, i, 0, i, height);
                                i = i + _fXLinesShowThird;
                            }
                        }
                        else
                        {
                            while (i > 0)
                            {
                                Grap.DrawLine(pe, i, 0, i, height);
                                i = i - _fXLinesShowThird;
                            }
                        }
                    }
                    #endregion
                    #region **第二段**
                    pe.Color = Color.FromArgb(_iLineShowColorAlpha / 2, _iLineShowColor);   //画笔颜色
                    i = _fXpxGO;
                    if (_fXLinesShowSecond != 0)
                    {
                        if (_bXLinesLBegin)
                        {
                            while (i < width)
                            {
                                Grap.DrawLine(pe, i, 0, i, height);
                                i = i + _fXLinesShowSecond;
                            }
                        }
                        else
                        {
                            while (i > 0)
                            {
                                Grap.DrawLine(pe, i, 0, i, height);
                                i = i - _fXLinesShowSecond;
                            }
                        }
                    }
                    #endregion
                    #region **第一段**
                    pe.Color = Color.FromArgb(_iLineShowColorAlpha, _iLineShowColor);
                    i = _fXpxGO;
                    if (_bXLinesLBegin)
                    {
                        while (i < width)
                        {
                            Grap.DrawLine(pe, i, 0, i, height);
                            i = i + _fXLinesShowFirst;
                        }
                    }
                    else
                    {
                        while (i > 0)
                        {
                            Grap.DrawLine(pe, i, 0, i, height);
                            i = i - _fXLinesShowFirst;
                        }
                    }
                    #endregion
                    #endregion
                    #region **画水平网格**
                    #region **第三段**
                    pe.Color = Color.FromArgb(_iLineShowColorAlpha / 3, _iLineShowColor);   //画笔颜色
                    if (_fYLinesShowThird != 0)
                    {
                        if (_bYLinesLBegin)
                        {
                            i = height + 29 - _fYpxGO;
                            while (i < height)
                            {
                                Grap.DrawLine(pe, 0, i, width, i);
                                i = i + _fYLinesShowThird;
                            }
                        }
                        else
                        {
                            i = 29 - _fYpxGO + height;
                            while (i > 0)
                            {
                                Grap.DrawLine(pe, 0, i, width, i);
                                i = i - _fYLinesShowThird;
                            }
                        }
                    }
                    #endregion
                    #region **第二段**
                    pe.Color = Color.FromArgb(_iLineShowColorAlpha / 2, _iLineShowColor);   //画笔颜色
                    if (_fYLinesShowSecond != 0)
                    {
                        if (_bYLinesLBegin)
                        {
                            i = height + 29 - _fYpxGO;
                            while (i < height)
                            {
                                Grap.DrawLine(pe, 0, i, width, i);
                                i = i + _fYLinesShowSecond;
                            }
                        }
                        else
                        {
                            i = 29 - _fYpxGO + height;
                            while (i > 0)
                            {
                                Grap.DrawLine(pe, 0, i, width, i);
                                i = i - _fYLinesShowSecond;
                            }
                        }
                    }
                    #endregion
                    #region **第一段**
                    pe.Color = Color.FromArgb(_iLineShowColorAlpha, _iLineShowColor);   //画笔颜色
                    if (_bYLinesLBegin)
                    {
                        i = height + 29 - _fYpxGO;
                        while (i < height)
                        {
                            Grap.DrawLine(pe, 0, i, width, i);
                            i = i + _fYLinesShowFirst;
                        }
                    }
                    else
                    {
                        i = 29 - _fYpxGO + height;
                        while (i > 0)
                        {
                            Grap.DrawLine(pe, 0, i, width, i);
                            i = i - _fYLinesShowFirst;
                        }
                    }
                    #endregion

                    #endregion
                    #endregion
                }
                #endregion

                #region **画波形线条**
                #region **参数初始化**
                Grap.SmoothingMode = SmoothingMode.AntiAlias;
                #endregion

                #region **遍历每条线并画出**
                for (int i = 0; i < _listY.Count; i++)
                {
                    _listDrawPoints.Clear();

                    //装载颜色
                    pe.Color = _listColor[i];
                    //装载宽度
                    pe.Width = _listWidth[i];
                    //装载连接点
                    pe.LineJoin = _listLineJoin[i];
                    //装载起始线帽
                    pe.StartCap = _listLineCap[i];

                    //装载坐标
                    if (!_changeToDrawPoints(i, _listDrawPoints, width, height))
                    {
                        continue;
                    }

                    try
                    {
                        if (_listDrawStyle[i] == DrawStyle.Line)
                        {
                            //绘制线
                            if (_listDrawPoints.Count == 1)
                                continue;
                            pe.LineJoin = LineJoin.Bevel;   //设置线条连接点样式，防止线宽在大于1的情况下导致的转折点不精确的问题
                            Grap.DrawLines(pe, _listDrawPoints.ToArray());
                        }
                        else if (_listDrawStyle[i] == DrawStyle.dot)
                        {
                            //绘制方形点
                            foreach (PointF points in _listDrawPoints)
                            {
                                Grap.DrawRectangle(pe, points.X, points.Y, 1, 1);
                            }
                        }
                        else
                        {
                            //绘制条形线
                            foreach (PointF points in _listDrawPoints)
                            {
                                Grap.DrawLine(pe, points.X, points.Y, points.X, 0);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //发生数据溢出错误
                    }
                }
                #endregion
                #endregion
            }
        }
        #endregion

        #region **私有函数 X轴区域Paint **
        private void pictureBoxBottom_Paint(object sender, PaintEventArgs e)
        {
            //左右留空50px，高45px
            #region **参数初始化**
            int width = pictureBoxBottom.Width;
            int height = pictureBoxBottom.Height;
            float linesQuan;                            //获得两权的大值
            float linesNum;                             //可以分成的线段
            float pxLine;                               //每段坐标线间隔
            float pxwidth;                              //所要画坐标的像素范围
            float pxGO;                                 //所要画坐标的起点像素位置
            int currentI;                               //临时，循环用变量
            float currentDraw;                          //临时，循环用变量，画坐标线和坐标值
            decimal showTextT;                          //要显示的坐标值，decimal能够精确显示坐标值
            float width50 = width - 50;                 //临时变量，为提高计算效率
            Graphics Grap = e.Graphics;
            #endregion

            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, width, height),
                _backColorL, _backColorH, 90, false))
            using (Pen pe = new Pen(_coordinateLineColor, 2f))
            using (SolidBrush brushString = new SolidBrush(_coordinateStringColor))
            using (StringFormat format = new StringFormat())
            using (Font fo = new Font("宋体", 9))
            {
                #region **画背景色**
                Grap.FillRectangle(brush, e.ClipRectangle);
                #endregion

                #region **画坐标**
                //根据标定坐标权值判断画坐标的方向，从权值大的方向开始画
                if (_fXQuanBeginGO <= _fXQuanEndGO)
                {
                    //从右往左画

                    #region **参数初始化**
                    format.LineAlignment = StringAlignment.Near;    //文字垂直上对齐
                    format.Alignment = StringAlignment.Center;      //文字水平居中对齐
                    _bXLinesLBegin = false;                                                 //标记【X轴网格线从右往左画】
                    linesQuan = _fXQuanEndGO;                                                   //画图权值为X轴坐标标定结束权值
                    linesNum = (_fXEndGO - _fXBeginGO) / linesQuan;                             //当前权值下可分为多少段坐标
                    pxwidth = (float)(width - 100) / (_fXBegin - _fXEnd) * (_fXBeginGO - _fXEndGO);         //所要画坐标的像素范围
                    pxLine = pxwidth / linesNum;                                                //每段坐标线间隔
                    _fXLinesShowFirst = pxLine;                                             //标记【X轴第一层网格线间隔】
                    pxGO = (_fXEndGO - _fXBegin) / (_fXEnd - _fXBegin) * (width - 100) + 50;    //所要画坐标的起点像素位置
                    _fXpxGO = pxGO - 51;                                                    //标记【所要画X轴坐标的起点像素位置】
                    #endregion

                    #region **第一层坐标**
                    if (pxLine <= 250)  //若间距够画第二层坐标并显示坐标值的话，就不需要话第一层坐标
                    {
                        //开始画第一层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fXEndGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 6);                                  //画坐标线
                                Grap.DrawString(showTextT.ToString(), fo, brushString, currentDraw, 6, format);     //画坐标值
                            }
                            showTextT -= (decimal)linesQuan;    //更新要画的坐标值
                            currentDraw -= pxLine;              //更新坐标线和坐标值的位置
                        }
                    }
                    #endregion

                    #region **第二层坐标**
                    //第二层坐标,第一层坐标的基础上5等分，前提是间距大于10px，若间距大于50px则显示坐标值
                    if (pxLine > 250)
                    {
                        #region **参数初始化**
                        pe.Width = 2f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fXLinesShowSecond = pxLine;    //标记【X轴第二层网格线间隔】
                        #endregion
                        //开始画第二层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fXEndGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 6);  //画坐标线
                                Grap.DrawString(showTextT.ToString(), fo, brushString, currentDraw, 6, format); //画坐标值
                            }
                            showTextT -= (decimal)linesQuan;
                            currentDraw -= pxLine;
                        }
                    }
                    else if (pxLine > 50)
                    {
                        #region **参数初始化**
                        pe.Width = 2f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fXLinesShowSecond = pxLine;    //标记【X轴第二层网格线间隔】
                        #endregion
                        //开始画第二层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fXEndGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 6);  //画坐标线
                            }
                            currentDraw -= pxLine;
                        }
                    }
                    else
                    {
                        //标记第二层和第三层间隔
                        _fXLinesShowSecond = 0;
                        _fXLinesShowThird = 0;
                    }
                    #endregion

                    #region **第三层坐标**
                    //第三层坐标,第二层坐标的基础上5等分，前提是间距大于10px
                    if (pxLine > 50)
                    {
                        #region **参数初始化**
                        pe.Width = 1f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fXLinesShowThird = pxLine; //标记【X轴第三层网格线间隔】
                        #endregion
                        //开始画第三层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fXEndGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 5);  //画坐标线
                            }
                            currentDraw -= pxLine;
                        }
                    }
                    else if (pxLine > 20)   //不满足则2等分，前提是间距大于10px
                    {
                        #region **参数初始化**
                        pe.Width = 1f;
                        linesQuan = linesQuan / 2f;
                        linesNum = linesNum * 2f;
                        pxLine = pxLine / 2f;
                        _fXLinesShowThird = pxLine;     //标记【X轴第三层网格线间隔】
                        #endregion
                        //开始画第三层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fXEndGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 5);  //画坐标线
                            }
                            currentDraw -= pxLine;
                        }
                    }
                    else
                    {
                        //标记第三层间隔
                        _fXLinesShowThird = 0;
                    }
                    #endregion
                }
                else
                {
                    //从左往右画

                    #region **参数初始化**
                    format.LineAlignment = StringAlignment.Near;    //文字垂直上对齐
                    format.Alignment = StringAlignment.Center;      //文字水平居中对齐
                    _bXLinesLBegin = true;                                                  //标记【X轴网格线从左往右画】
                    linesQuan = _fXQuanBeginGO;                                                     //画图权值为X轴坐标标定起始权值
                    linesNum = (_fXEndGO - _fXBeginGO) / linesQuan;                                 //当前权值下可为为多少段坐标
                    pxwidth = (float)(width - 100) / (_fXBegin - _fXEnd) * (_fXBeginGO - _fXEndGO); //所要画坐标的像素范围
                    pxLine = pxwidth / linesNum;                                                    //每段坐标线间隔
                    _fXLinesShowFirst = pxLine;                                             //标记【X轴第一层网格线间隔】
                    pxGO = 50 - (_fXBegin - _fXBeginGO) / (_fXEnd - _fXBegin) * (width - 100);      //所要画坐标的起点像素位置
                    _fXpxGO = pxGO - 51;                                                    //标记【所要画X轴坐标的起点像素位置】
                    #endregion

                    #region **第一层坐标**
                    if (pxLine <= 250)  //若间距够画第二层坐标并显示坐标值的话，就不需要话第一层坐标
                    {
                        //开始画第一层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fXBeginGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 6);                                  //画坐标线
                                Grap.DrawString(showTextT.ToString(), fo, brushString, currentDraw, 6, format);     //画坐标值
                            }
                            showTextT += (decimal)linesQuan;    //更新要画的坐标值
                            currentDraw += pxLine;              //更新坐标线和坐标值的位置
                        }
                    }
                    #endregion

                    #region **第二层坐标**
                    //第二层坐标,第一层坐标的基础上5等分，前提是间距大于10px，若间距大于50px则显示坐标值
                    if (pxLine > 250)
                    {
                        #region **参数初始化**
                        pe.Width = 2f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fXLinesShowSecond = pxLine;    //标记【X轴第二层网格线间隔】
                        #endregion
                        //开始画第二层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fXBeginGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 6);  //画坐标线
                                Grap.DrawString(showTextT.ToString(), fo, brushString, currentDraw, 6, format); //画坐标值
                            }
                            showTextT += (decimal)linesQuan;
                            currentDraw += pxLine;
                        }
                    }
                    else if (pxLine > 50)
                    {
                        #region **参数初始化**
                        pe.Width = 2f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fXLinesShowSecond = pxLine;    //标记【X轴第二层网格线间隔】
                        #endregion
                        //开始画第二层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 6);  //画坐标线
                            }
                            currentDraw += pxLine;
                        }
                    }
                    else
                    {
                        //标记第二层和第三层间隔
                        _fXLinesShowSecond = 0;
                        _fXLinesShowThird = 0;
                    }
                    #endregion

                    #region **第三层坐标**
                    //第三层坐标,第二层坐标的基础上5等分，前提是间距大于10px
                    if (pxLine > 50)
                    {
                        #region **参数初始化**
                        pe.Width = 1f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fXLinesShowThird = pxLine;     //标记【X轴第三层网格线间隔】
                        #endregion
                        //开始画第三层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 5);  //画坐标线
                            }
                            currentDraw += pxLine;
                        }
                    }
                    else if (pxLine > 20)   //不满足则2等分，前提是间距大于10px
                    {
                        #region **参数初始化**
                        pe.Width = 1f;
                        linesQuan = linesQuan / 2f;
                        linesNum = linesNum * 2f;
                        pxLine = pxLine / 2f;
                        _fXLinesShowThird = pxLine;     //标记【X轴第三层网格线间隔】
                        #endregion
                        //开始画第三层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 50 && currentDraw < width50)
                            {
                                Grap.DrawLine(pe, currentDraw, 0, currentDraw, 5);  //画坐标线
                            }
                            currentDraw += pxLine;
                        }
                    }
                    else
                    {
                        //标记第三层间隔
                        _fXLinesShowThird = 0;
                    }
                    #endregion
                }
                #endregion

                #region **画两端基本坐标**
                pe.Width = 2;               //画笔线宽
                Grap.FillRectangle(brush, 28, 8, 46, 16);           //坐标背景色，用于遮盖
                Grap.FillRectangle(brush, width - 63, 8, 50, 16);   //坐标背景色，用于遮盖
                Grap.DrawLine(pe, 50, 0, 50, 10);                                               //X轴起始坐标线条
                Grap.DrawString(string.Format("{0}", Math.Round(_fXBegin, _iAccuracy)), fo, brushString, 45, 6, format);                      //X轴起始坐标值
                format.Alignment = StringAlignment.Near;      //文字水平居左对齐
                Grap.DrawLine(pe, width - 50, 0, width - 50, 10);                               //X轴结束坐标线条
                Grap.DrawString(string.Format("{0}", Math.Round(_fXEnd, _iAccuracy)).ToString(), fo, brushString, width - 50, 6, format);     //X轴结束坐标值
                #endregion


            }
        }
        #endregion

        #region **私有函数 Y轴区域Paint **
        private void pictureBoxLeft_Paint(object sender, PaintEventArgs e)
        {
            #region **参数初始化**
            int width = pictureBoxLeft.Width;
            int height = pictureBoxLeft.Height;
            float linesQuan;                            //获得两权的大值
            float linesNum;                             //可以分成的线段
            float pxLine;                               //每段坐标线间隔
            float pxwidth;                              //所要画坐标的像素范围
            float pxGO;                                 //所要画坐标的起点像素位置
            int currentI;                               //临时，循环用变量
            float currentDraw;                          //临时，循环用变量，画坐标线和坐标值
            decimal showTextT;                          //要显示的坐标值，decimal能够精确显示坐标值
            float topMarge = 20;                            //上方留白
            Graphics Grap = e.Graphics;
            #endregion

            using (SolidBrush brush = new SolidBrush(_backColorL))
            using (Pen pe = new Pen(_coordinateLineColor, 2f))
            using (SolidBrush brushString = new SolidBrush(_coordinateStringColor))
            using (StringFormat format = new StringFormat())
            using (Font fo = new Font("宋体", 9))
            {
                #region **画背景色**
                Grap.FillRectangle(brush, e.ClipRectangle);
                #endregion

                #region **画坐标**
                //根据标定坐标权值判断画坐标的方向，从权大的方向开始画
                if (_fYQuanBeginGO <= _fYQuanEndGO)
                {
                    //从上往下画

                    #region **参数初始化**
                    format.Alignment = StringAlignment.Far;         //文字右对齐
                    format.LineAlignment = StringAlignment.Center;  //文字垂直居中对齐
                    _bYLinesLBegin = false;                                                         //标记【Y轴网格线从上往下画】
                    linesQuan = _fYQuanEndGO;                                                               //画图权值为Y轴坐标标定结束权值
                    linesNum = (_fYEndGO - _fYBeginGO) / linesQuan;                                         //当前权值下可为为多少段坐标
                    pxwidth = (float)(height - topMarge) / (_fYBegin - _fYEnd) * (_fYBeginGO - _fYEndGO);         //所要画坐标的像素范围
                    pxLine = pxwidth / linesNum;                                                            //每段坐标线间隔
                    _fYLinesShowFirst = pxLine;                                                     //标记【Y轴第一层网格线间隔】
                    pxGO = topMarge - (_fYEnd - _fYEndGO) / (_fYBegin - _fYEnd) * (height - topMarge);                  //所要画坐标的起点像素位置
                    _fYpxGO = pxGO;                                                                 //标记【所要画Y轴坐标的起点像素位置】
                    #endregion

                    #region **第一层坐标**
                    if (pxLine <= 150)  //若间距够画第二层坐标并显示坐标值的话，就不需要话第一层坐标
                    {
                        //开始画第一层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fYEndGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > topMarge && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);     //画坐标线
                                Grap.DrawString(showTextT.ToString(), fo, brushString, 44, currentDraw, format);    //画坐标值
                            }
                            showTextT -= (decimal)linesQuan;    //更新要画的坐标值
                            currentDraw += pxLine;              //更新坐标线和坐标值的位置
                        }
                    }
                    #endregion

                    #region **第二层坐标**
                    //第二层坐标,第一层坐标的基础上5等分，前提是间距大于10px，若间距大于30px则显示坐标值
                    if (pxLine > 150)
                    {
                        #region **参数初始化**
                        pe.Width = 2f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fYLinesShowSecond = pxLine;    //标记【Y轴第二层网格线间隔】
                        #endregion
                        //开始画第二层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fYEndGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 30 && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                                Grap.DrawString(showTextT.ToString(), fo, brushString, 44, currentDraw, format);    //画坐标值
                            }
                            showTextT -= (decimal)linesQuan;
                            currentDraw += pxLine;
                        }
                    }
                    else if (pxLine > 50)
                    {
                        #region **参数初始化**
                        pe.Width = 2f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fYLinesShowSecond = pxLine;    //标记【Y轴第二层网格线间隔】
                        #endregion
                        //开始画第二层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 30 && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                            }
                            currentDraw += pxLine;
                        }
                    }
                    else
                    {
                        //标记第二层和第三层间隔
                        _fYLinesShowSecond = 0;
                        _fYLinesShowThird = 0;
                    }
                    #endregion

                    #region **第三层坐标**
                    //第三层坐标,第二层坐标的基础上5等分，前提是间距大于10px
                    if (pxLine > 50)
                    {
                        #region **参数初始化**
                        pe.Width = 1f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fYLinesShowThird = pxLine; //标记【Y轴第三层网格线间隔】
                        #endregion
                        //开始画第三层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 30 && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                            }
                            currentDraw += pxLine;
                        }
                    }
                    else if (pxLine > 20)   //不满足则2等分，前提是间距大于10px
                    {
                        #region **参数初始化**
                        pe.Width = 1f;
                        linesQuan = linesQuan / 2f;
                        linesNum = linesNum * 2f;
                        pxLine = pxLine / 2f;
                        _fYLinesShowThird = pxLine; //标记【Y轴第三层网格线间隔】
                        #endregion
                        //开始画第三层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > 30 && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                            }
                            currentDraw += pxLine;
                        }
                    }
                    else
                    {
                        //标记第三层间隔
                        _fYLinesShowThird = 0;
                    }
                    #endregion
                }
                else
                {
                    //从下往上画

                    #region **参数初始化**
                    format.Alignment = StringAlignment.Far;         //文字右对齐
                    format.LineAlignment = StringAlignment.Center;  //文字垂直居中对齐
                    _bYLinesLBegin = true;                                                          //标记【Y轴网格线从下往上画】
                    linesQuan = _fYQuanBeginGO;                                                             //画图权值为Y轴坐标标定起始权值
                    linesNum = (_fYEndGO - _fYBeginGO) / linesQuan;                                         //当前权值下可为为多少段坐标
                    pxwidth = (float)(height - 30) / (_fYBegin - _fYEnd) * (_fYBeginGO - _fYEndGO);         //所要画坐标的像素范围
                    pxLine = pxwidth / linesNum;                                                            //每段坐标线间隔
                    _fYLinesShowFirst = pxLine;                                                     //标记【Y轴第一层网格线间隔】
                    pxGO = (_fYBeginGO - _fYEnd) / (_fYBegin - _fYEnd) * (height - 30) + 30;                //所要画坐标的起点像素位置
                    _fYpxGO = pxGO;                                                                 //标记【所要画Y轴坐标的起点像素位置】
                    #endregion

                    #region **第一层坐标**
                    if (pxLine <= 150)  //若间距够画第二层坐标并显示坐标值的话，就不需要话第一层坐标
                    {
                        //开始画第一层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fYBeginGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > topMarge && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                                Grap.DrawString(showTextT.ToString(), fo, brushString, 44, currentDraw, format);    //画坐标值
                            }
                            showTextT += (decimal)linesQuan;    //更新要画的坐标值
                            currentDraw -= pxLine;              //更新坐标线和坐标值的位置
                        }
                    }
                    #endregion

                    #region **第二层坐标**
                    //第二层坐标,第一层坐标的基础上5等分，前提是间距大于10px，若间距大于30px则显示坐标值
                    if (pxLine > 150)
                    {
                        #region **参数初始化**
                        pe.Width = 2f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fYLinesShowSecond = pxLine;    //标记【Y轴第二层网格线间隔】
                        #endregion
                        //开始画第二层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        showTextT = (decimal)_fYBeginGO;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > topMarge && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                                Grap.DrawString(showTextT.ToString(), fo, brushString, 44, currentDraw, format);    //画坐标值
                            }
                            showTextT += (decimal)linesQuan;
                            currentDraw -= pxLine;
                        }
                    }
                    else if (pxLine > 50)
                    {
                        #region **参数初始化**
                        pe.Width = 2f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fYLinesShowSecond = pxLine;    //标记【Y轴第二层网格线间隔】
                        #endregion
                        //开始画第二层坐标，显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > topMarge && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                            }
                            currentDraw -= pxLine;
                        }
                    }
                    else
                    {
                        //标记第二层和第三层间隔
                        _fYLinesShowSecond = 0;
                        _fYLinesShowThird = 0;
                    }
                    #endregion

                    #region **第三层坐标**
                    //第三层坐标,第二层坐标的基础上5等分，前提是间距大于10px
                    if (pxLine > 50)
                    {
                        #region **参数初始化**
                        pe.Width = 1f;
                        linesQuan = linesQuan / 5f;
                        linesNum = linesNum * 5f;
                        pxLine = pxLine / 5f;
                        _fYLinesShowThird = pxLine; //标记【Y轴第三层网格线间隔】
                        #endregion
                        //开始画第三层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > topMarge && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                            }
                            currentDraw -= pxLine;
                        }
                    }
                    else if (pxLine > 20)   //不满足则2等分，前提是间距大于10px
                    {
                        #region **参数初始化**
                        pe.Width = 1f;
                        linesQuan = linesQuan / 2f;
                        linesNum = linesNum * 2f;
                        pxLine = pxLine / 2f;
                        _fYLinesShowThird = pxLine; //标记【Y轴第三层网格线间隔】
                        #endregion
                        //开始画第三层坐标，不显示坐标值
                        currentI = (int)linesNum + 1;
                        currentDraw = pxGO;
                        for (int i = 0; i < currentI; i++)
                        {
                            if (currentDraw > topMarge && currentDraw < height)
                            {
                                Grap.DrawLine(pe, 44, currentDraw, 50, currentDraw);    //画坐标线
                            }
                            currentDraw -= pxLine;
                        }
                    }
                    else
                    {
                        //标记第三层间隔
                        _fYLinesShowThird = 0;
                    }
                    #endregion
                }
                #endregion

                #region **画两端基本坐标**
                format.Alignment = StringAlignment.Center;      //文字右对齐
                format.LineAlignment = StringAlignment.Far;     //文字下对齐
                pe.Width = 2;                   //画笔线宽
                Grap.FillRectangle(brush, 5, height - 17, 45, 16);  //坐标背景色，用于遮盖
                Grap.FillRectangle(brush, 5, 15, 45, 16);          //坐标背景色，用于遮盖
                Grap.DrawLine(pe, 26, height - 1, 50, height - 1);                                  //Y轴起始坐标线条
                Grap.DrawString(string.Format("{0}", Math.Round(_fYBegin, _iAccuracy)), fo, brushString, 26, height - 2, format);    //Y轴起始坐标值
                Grap.DrawLine(pe, 26, topMarge + 1, 50, topMarge + 1);                                                  //Y轴结束坐标线条
                Grap.DrawString(string.Format("{0}", Math.Round(_fYEnd, _iAccuracy)), fo, brushString, 26, topMarge - 1, format);              //Y轴结束坐标值
                #endregion

            }
        }
        #endregion

        #region **私有函数 TOP区域Paint **
        private void pictureBoxTop_Paint(object sender, PaintEventArgs e)
        {
            #region **参数初始化**
            int width = pictureBoxTop.Width;
            int height = pictureBoxTop.Height;
            Graphics Grap = e.Graphics;
            #endregion

            using (SolidBrush brush = new SolidBrush(_backColorL))
            using (StringFormat format = new StringFormat())
            using (Font fo = new Font("宋体", 9))
            using (Font foTitle = new Font("宋体", _titleSize, FontStyle.Bold))
            {
                #region **画背景色**
                Grap.FillRectangle(brush, e.ClipRectangle);
                #endregion

                #region **画Y轴标签**
                format.LineAlignment = StringAlignment.Far;     //文字垂直下对齐
                format.Alignment = StringAlignment.Near;        //文字水平左对齐
                brush.Color = _coordinateStringTitleColor;
                Grap.DrawString(_SySnameY, fo, brush, 0, height - 3, format);   //画Y轴标签
                #endregion

                #region **画标题**
                brush.Color = _titleBorderColor;
                Grap.DrawString(_SyStitle, foTitle, brush, pictureBoxTop.Width * _titlePosition + 1, height + 1, format);
                Grap.DrawString(_SyStitle, foTitle, brush, pictureBoxTop.Width * _titlePosition - 1, height - 1, format);
                brush.Color = _titleColor;
                Grap.DrawString(_SyStitle, foTitle, brush, pictureBoxTop.Width * _titlePosition, height, format);
                #endregion
            }
        }
        #endregion

        #region **私有函数 Right区域Paint **
        private void pictureBoxRight_Paint(object sender, PaintEventArgs e)
        {
            #region **参数初始化**
            int width = pictureBoxRight.Width;
            int height = pictureBoxRight.Height;
            Graphics Grap = e.Graphics;
            #endregion

            using (SolidBrush brushString = new SolidBrush(_coordinateStringColor))
            using (StringFormat format = new StringFormat(StringFormatFlags.DirectionVertical))
            using (Font fo = new Font("宋体", 9))
            using (SolidBrush brush = new SolidBrush(_backColorL))
            {
                #region **画背景色**
                Grap.FillRectangle(brush, e.ClipRectangle);
                #endregion

                #region **画X轴标签**
                brushString.Color = _coordinateStringTitleColor;
                Grap.DrawString(_SySnameX, fo, brushString, 0, height - 50, format);
                #endregion
            }
        }
        #endregion

        #region **私有函数 波形显示控件大小改变时候触发**
        private void ZGraph_Resize(object sender, EventArgs e)
        {
            #region **放大框隐藏 更新相关联的标记和状态**
            pictureBoxGraph.Cursor = Cursors.Arrow;         //箭头光标
            #endregion
            //刷新界面 
            pictureBoxGraph.Refresh();
            pictureBoxTop.Refresh();
        }
        #endregion
        #endregion

        #region ** FuncPublic **
        #region ** 更新波形 **
        public void waveDraw(float x, float y)
        {
            xData.Add(x);
            yData.Add(y);
            f_Refresh();
        }
        #endregion

        #region ** 初始化显示 **
        public void waveInit(Color col, int weight)
        {
            //数据初始化

            xData.Clear();
            yData.Clear();
            f_ClearAllPix();
            f_LoadOnePix(xData, yData, col, weight);
        }
        #endregion
        #region ** 设置视窗大小 **
        public void VisualSet(float x, float y)
        {
            _fXEndSYS = _fXBeginSYS + x;
            _fYEndSYS = _fYBeginSYS + y;
            _fXEnd = _fXEndGO = _fXBeginGO + x;
            _fYBegin = _fYBeginGO = -y / 2;
            _fYEnd = _fYEndGO = y / 2;
            _fXQuanEndGO = _getQuan(_fXEndGO);
            _fYQuanBeginGO = _getQuan(_fYBeginGO);
            _fYQuanEndGO = _getQuan(_fYEndGO);

            pictureBoxGraph.Refresh();                      //刷新界面
        }
        #endregion

        #region ** 获得视窗大小 **
        public float VisualGetX()
        {
            float x = 0;
            x = _fXEndSYS - _fXBeginSYS;
            return x;
        }

        public float VisualGetY()
        {
            float y = 0;
            y = _fYEndSYS - _fYBeginSYS;
            return y;
        }
        #endregion

        #region ** 平移窗口 **
        public void Auto_move()
        {

            _isDefaultMoveModeXY = true;                          //标记，取消自动调整大小模式
            //初始化坐标值和坐标标定值
            _fXBegin = _fXBeginGO = _fXBeginSYS;
            _fYBegin = _fYBeginGO = _fYBeginSYS;
            _fXEnd = _fXEndGO = _fXEndSYS;
            _fYEnd = _fYEndGO = _fYEndSYS;
            _fXQuanBeginGO = _getQuan(_fXBeginGO);
            _fXQuanEndGO = _getQuan(_fXEndGO);
            _fYQuanBeginGO = _getQuan(_fYBeginGO);
            _fYQuanEndGO = _getQuan(_fYEndGO);

            pictureBoxGraph.Refresh();                      //刷新界面



        }
        #endregion

        #region **清空所有加载的波形数据 f_ClearAllPix():void **
        /// <summary>
        /// 清空所有加载的波形数据并清空显示
        /// </summary>
        public void f_ClearAllPix()
        {
            //重新初始化
            _listX.Clear();
            _listY.Clear();
            _haveFile.Clear();
            _filePath.Clear();
            _listColor.Clear();
            _listWidth.Clear();
            _listLineJoin.Clear();
            _listLineCap.Clear();
            _listDrawStyle.Clear();
            //更新
            pictureBoxGraph.Refresh();
        }
        #endregion


        #region **更新显示 f_Refresh():void **
        /// <summary>
        /// 更新显示
        /// </summary>
        public void f_Refresh()
        {
            pictureBoxGraph.Refresh();
        }
        #endregion

        #region **设置波形控件初始化状态**
        /// <summary>
        /// 波形控件初始化状态
        /// </summary>
        public enum GraphStyle
        {
            /// <summary>
            /// 自动调整坐标模式
            /// </summary>
            AutoMode,
            /// <summary>
            /// 按默认坐标范围平移
            /// </summary>
            DefaultMoveMode,
            /// <summary>
            /// 正常显示模式
            /// </summary>
            None
        }
        /// <summary>
        /// 初始化波形控件
        /// </summary>
        /// <param name="mode">初始化模式</param>
        public void f_InitMode(GraphStyle mode)
        {
            switch (mode)
            {
                case GraphStyle.AutoMode:
                    _isAutoModeXY = true;
                    _isDefaultMoveModeXY = false;
                    break;
                case GraphStyle.DefaultMoveMode:
                    _isAutoModeXY = false;
                    _isDefaultMoveModeXY = true;
                    break;
                case GraphStyle.None:
                    _isAutoModeXY = false;
                    _isDefaultMoveModeXY = false;
                    break;
                default:
                    break;

            }
            Refresh();
        }
        #endregion

        #region **清空原有数据并加载一条波形数据**
        /// <summary>
        /// 清空原有数据并加载一条波形数据
        /// </summary>
        /// <param name="listX">X轴</param>
        /// <param name="listY">Y轴</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        /// <param name="listLineJoin">线条连接点</param>
        /// <param name="listLineCap">线条起始线帽</param>
        /// <param name="listDrawStyle">线条样式</param>
        public void f_LoadOnePix(List<float> listX, List<float> listY, Color listColor, int listWidth, LineJoin listLineJoin, LineCap listLineCap, DrawStyle listDrawStyle)
        {
            //重新初始化
            f_ClearAllPix();
            //装载
            _listX.Add(listX);
            _listY.Add(listY);
            _filePath.Add(null);
            _haveFile.Add(false);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(listLineJoin);
            _listLineCap.Add(listLineCap);
            _listDrawStyle.Add(listDrawStyle);

        }
        /// <summary>
        /// 清空原有数据并加载一条波形数据，显示样式为线条
        /// </summary>
        /// <param name="listX">X轴</param>
        /// <param name="listY">Y轴</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        public void f_LoadOnePix(List<float> listX, List<float> listY, Color listColor, int listWidth)
        {
            //重新初始化
            f_ClearAllPix();
            //装载
            _listX.Add(listX);
            _listY.Add(listY);
            _filePath.Add(null);
            _haveFile.Add(false);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(LineJoin.Bevel);
            _listLineCap.Add(LineCap.NoAnchor);
            _listDrawStyle.Add(DrawStyle.Line);

        }
        /// <summary>
        /// 清空原有数据并加载一条波形数据,文件和链表混排
        /// </summary>
        /// <param name="wavePath">波形文件位置</param>
        /// <param name="listX">X轴</param>
        /// <param name="listY">Y轴</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        /// <param name="listLineJoin">线条连接点</param>
        /// <param name="listLineCap">线条起始线帽</param>
        /// <param name="listDrawStyle">线条样式</param>
        public void f_LoadOnePix(string wavePath, List<float> listX, List<float> listY, Color listColor, int listWidth, LineJoin listLineJoin, LineCap listLineCap, DrawStyle listDrawStyle)
        {
            //重新初始化
            f_ClearAllPix();
            //装载
            _listX.Add(listX);
            _listY.Add(listY);
            _filePath.Add(wavePath);
            _haveFile.Add(true);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(listLineJoin);
            _listLineCap.Add(listLineCap);
            _listDrawStyle.Add(listDrawStyle);
        }

        /// <summary>
        /// 清空原有数据并加载一条波形数据，显示样式为线条，文件和链表混排
        /// </summary>
        /// <param name="wavePath">波形文件位置</param>
        /// <param name="listX">X轴</param>
        /// <param name="listY">Y轴</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        public void f_LoadOnePix(string wavePath, List<float> listX, List<float> listY, Color listColor, int listWidth)
        {
            //重新初始化
            f_ClearAllPix();
            //装载
            _listX.Add(listX);
            _listY.Add(listY);
            _filePath.Add(wavePath);
            _haveFile.Add(true);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(LineJoin.Bevel);
            _listLineCap.Add(LineCap.NoAnchor);
            _listDrawStyle.Add(DrawStyle.Line);

        }

        /// <summary>
        /// 清空原有数据并加载一条波形数据，只读取文件数据
        /// </summary>
        /// <param name="wavePath">波形文件位置</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        /// <param name="listLineJoin">线条连接点</param>
        /// <param name="listLineCap">线条起始线帽</param>
        /// <param name="listDrawStyle">线条样式</param>
        public void f_LoadOnePix(string wavePath, Color listColor, int listWidth, LineJoin listLineJoin, LineCap listLineCap, DrawStyle listDrawStyle)
        {
            //重新初始化
            f_ClearAllPix();
            //装载
            _listX.Add(null);
            _listY.Add(null);
            _filePath.Add(wavePath);
            _haveFile.Add(true);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(listLineJoin);
            _listLineCap.Add(listLineCap);
            _listDrawStyle.Add(listDrawStyle);
        }

        /// <summary>
        /// 清空原有数据并加载一条波形数据，显示样式为线条，只读取文件数据
        /// </summary>
        /// <param name="wavePath">波形文件位置</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        public void f_LoadOnePix(string wavePath, Color listColor, int listWidth)
        {
            //重新初始化
            f_ClearAllPix();
            //装载
            _listX.Add(null);
            _listY.Add(null);
            _filePath.Add(wavePath);
            _haveFile.Add(true);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(LineJoin.Bevel);
            _listLineCap.Add(LineCap.NoAnchor);
            _listDrawStyle.Add(DrawStyle.Line);
        }
        #endregion

        #region **在原有波形上添加一条波形数据 **
        /// <summary>
        /// 在原有波形上添加一条线
        /// 不可动态循环
        /// </summary>
        /// <param name="listX">X轴</param>
        /// <param name="listY">Y轴</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        /// <param name="listLineJoin">线条连接点</param>
        /// <param name="listLineCap">线条起始线帽</param>
        /// <param name="listDrawStyle">线条样式</param>
        public void f_AddPix(List<float> listX, List<float> listY, Color listColor, int listWidth, LineJoin listLineJoin, LineCap listLineCap, DrawStyle listDrawStyle)
        {
            //装载
            _listX.Add(listX);
            _listY.Add(listY);
            _filePath.Add(null);
            _haveFile.Add(false);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(listLineJoin);
            _listLineCap.Add(listLineCap);
            _listDrawStyle.Add(listDrawStyle);
        }
        /// <summary>
        /// 在原有波形上添加一条线，显示样式为线条
        /// 不可动态循环
        /// </summary>
        /// <param name="listX">X轴</param>
        /// <param name="listY">Y轴</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>        
        public void f_AddPix(List<float> listX, List<float> listY, Color listColor, int listWidth)
        {
            //装载
            _listX.Add(listX);
            _listY.Add(listY);
            _filePath.Add(null);
            _haveFile.Add(false);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(LineJoin.Bevel);
            _listLineCap.Add(LineCap.NoAnchor);
            _listDrawStyle.Add(DrawStyle.Line);
        }
        /// <summary>
        /// 在原有波形上添加一条线,文件和链表混排
        /// </summary>
        /// <param name="wavePath">波形文件位置</param>
        /// <param name="listX">X轴</param>
        /// <param name="listY">Y轴</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        /// <param name="listLineJoin">线条连接点</param>
        /// <param name="listLineCap">线条起始线帽</param>
        /// <param name="listDrawStyle">线条样式</param>
        public void f_AddPix(string wavePath, List<float> listX, List<float> listY, Color listColor, int listWidth, LineJoin listLineJoin, LineCap listLineCap, DrawStyle listDrawStyle)
        {
            //装载
            _listX.Add(listX);
            _listY.Add(listY);
            _filePath.Add(wavePath);
            _haveFile.Add(true);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(listLineJoin);
            _listLineCap.Add(listLineCap);
            _listDrawStyle.Add(listDrawStyle);
        }
        /// <summary>
        /// 在原有波形上添加一条线，显示样式为线条，文件和链表混排
        /// </summary>
        /// <param name="wavePath">波形文件位置</param>
        /// <param name="listX">X轴</param>
        /// <param name="listY">Y轴</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        public void f_AddPix(string wavePath, List<float> listX, List<float> listY, Color listColor, int listWidth)
        {
            //装载
            _listX.Add(listX);
            _listY.Add(listY);
            _filePath.Add(wavePath);
            _haveFile.Add(true);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(LineJoin.Bevel);
            _listLineCap.Add(LineCap.NoAnchor);
            _listDrawStyle.Add(DrawStyle.Line);
        }

        /// <summary>
        /// 在原有波形上添加一条线,只显示文件数据
        /// </summary>
        /// <param name="wavePath">波形文件位置</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        /// <param name="listLineJoin">线条连接点</param>
        /// <param name="listLineCap">线条起始线帽</param>
        /// <param name="listDrawStyle">线条样式</param>
        public void f_AddPix(string wavePath, Color listColor, int listWidth, LineJoin listLineJoin, LineCap listLineCap, DrawStyle listDrawStyle)
        {
            //装载
            _listX.Add(null);
            _listY.Add(null);
            _filePath.Add(wavePath);
            _haveFile.Add(true);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(listLineJoin);
            _listLineCap.Add(listLineCap);
            _listDrawStyle.Add(listDrawStyle);
        }
        /// <summary>
        /// 在原有波形上添加一条线，显示样式为线条，只显示文件数据
        /// </summary>
        /// <param name="wavePath">波形文件位置</param>
        /// <param name="listColor">线条颜色</param>
        /// <param name="listWidth">线条宽度</param>
        public void f_AddPix(string wavePath, Color listColor, int listWidth)
        {
            //装载
            _listX.Add(null);
            _listY.Add(null);
            _filePath.Add(wavePath);
            _haveFile.Add(true);
            _listColor.Add(listColor);
            _listWidth.Add(listWidth);
            _listLineJoin.Add(LineJoin.Bevel);
            _listLineCap.Add(LineCap.NoAnchor);
            _listDrawStyle.Add(DrawStyle.Line);
        }
        #endregion

        #region ** 清除其中一个波形 **

        /// <summary>
        /// 清楚一个波形
        /// </summary>
        /// <param name="index">波形的索引号</param>
        public void f_ClearAWave(int index)
        {
            _listX.RemoveAt(index);
            _listY.RemoveAt(index);
            _haveFile.RemoveAt(index);
            _filePath.RemoveAt(index);
            _listColor.RemoveAt(index);
            _listWidth.RemoveAt(index);
            _listLineJoin.RemoveAt(index);
            _listLineCap.RemoveAt(index);
            _listDrawStyle.RemoveAt(index);
        }
        #endregion
        #endregion

        #region ** FuncPrivate **
        #region ** 私有方法 在文件里边读取一组数据_getPoint(list：int,count：int):List<Point> **

        /// <summary>
        /// 在文件里边读取一组数据
        /// </summary>
        /// <param name="list">波形序号</param>
        /// <param name="count">读取第几块内容</param>
        /// <returns></returns>
        private List<Point> _getPoint(int list, int count)
        {
            FileStream fs = new FileStream(_filePath[list], FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            List<Point> point = new List<Point>();

            long len = fs.Length;                                      //获取点长度
            long fileSeek = (long)(count * 0x8000);                    //分段读取数据，每一段0x8000个数据

            fs.Seek(fileSeek, SeekOrigin.Begin);                       //设置偏移量
            int lenEnd = (int)((len - fileSeek) / 8);                  //获取从偏移量位置到文件结束可以表示多少个点，每一个坐标点占用八个字节
            if (lenEnd > 0x1000)                                       //每一块最多处理0x1000个点
            {
                lenEnd = 0x1000;
            }
            for (int i = 0; i < lenEnd; i++)
            {
                Point pot = new Point(br.ReadInt32(), br.ReadInt32()); //把读取的数据压入列表里边返回出去
                point.Add(pot);
            }
            br.Close();
            fs.Close();
            return point;
        }
        #endregion

        #region **私有方法 计算一个浮点数的权值 _getQuan(m:float):float **
        /// <summary>
        /// 计算一个浮点数的权值
        /// 如234.53返回100
        /// </summary>
        /// <param name="m">要计算权值的浮点数</param>
        /// <returns>权值</returns>
        private float _getQuan(float m)
        {
            float quan = 1f;        //临时，权值
            m = (m < 0) ? -m : m;   //取绝对值
            if (m == 0)
            {
                return 1f;          //默认0的权值为1
            }
            else if (m < 1)
            {
                do { quan /= 10f; }
                while ((m = m * 10f) < 1);
                return quan;
            }
            else
            {
                while ((m /= 10f) >= 1) { quan *= 10f; }
                return quan;
            }
        }
        #endregion

        #region **私有方法 根据溢出坐标范围的浮点数，改变X轴的坐标标定权值和坐标标定值 _changXBegionOrEndGO(m:float,isL:bool):void **
        /// <summary>
        /// 根据溢出坐标范围的浮点数，改变X轴的坐标标定权值和坐标标定值
        /// </summary>
        /// <param name="m">溢出坐标范围的浮点数</param>
        /// <param name="isL">是否从左边溢出</param>
        private void _changXBegionOrEndGO(float m, bool isL)
        {
            float quan = _getQuan(m);   //获得该溢出数的权值
            if (isL)
            {
                //如果值是从左边溢出
                #region **修改权值存入标定权值,控制权差在10倍以内**
                if (quan < _fXQuanEndGO)
                {
                    _fXQuanBeginGO = _fXQuanEndGO / 10f;
                }
                else if (quan > _fXQuanEndGO)
                {
                    _fXQuanBeginGO = quan;
                    _fXQuanEndGO = _fXQuanBeginGO / 10f;
                }
                else
                {
                    _fXQuanBeginGO = _fXQuanEndGO;
                }
                #endregion
                #region **根据新的权值修改坐标标定值**
                if (m <= _fXQuanBeginGO && m >= -_fXQuanBeginGO)
                {
                    _fXBeginGO = -_fXQuanBeginGO;
                }
                else
                {
                    _fXBeginGO = ((int)(m / _fXQuanBeginGO) - 1) * _fXQuanBeginGO;
                }
                #endregion
            }
            else
            {
                //如果值是从右边溢出
                #region **修改权值存入标定权值，控制权差在10倍以内**
                if (quan < _fXQuanBeginGO)
                {
                    _fXQuanEndGO = _fXQuanBeginGO / 10f;
                }
                else if (quan > _fXQuanBeginGO)
                {
                    _fXQuanEndGO = quan;
                    _fXQuanBeginGO = _fXQuanEndGO / 10f;
                }
                else
                {
                    _fXQuanEndGO = _fXQuanBeginGO;
                }
                #endregion
                #region **根据新的权值修改坐标标定值**
                if (m <= _fXQuanEndGO && m >= _fXQuanBeginGO)
                {
                    _fXEndGO = _fXQuanEndGO;
                }
                else
                {
                    _fXEndGO = ((int)(m / _fXQuanEndGO) + 1) * _fXQuanEndGO;
                }
                #endregion
            }
        }
        #endregion

        #region **私有方法 根据溢出坐标范围的浮点数，改变Y轴的坐标标定权值和坐标标定值 _changYBegionOrEndOGO(m:float, isL:bool):void **
        /// <summary>
        /// 根据溢出坐标范围的浮点数，改变Y轴的坐标标定权值和坐标标定值
        /// </summary>
        /// <param name="m">溢出坐标范围的浮点数</param>
        /// <param name="isL">是否从下边溢出</param>
        private void _changYBegionOrEndGO(float m, bool isL)
        {
            float quan = _getQuan(m);   //获得该溢出数的权值
            if (isL)
            {
                //如果值是从左边溢出
                #region **修改权值存入标定权值,控制权差在10倍以内**
                if (quan < _fYQuanEndGO)
                {
                    _fYQuanBeginGO = _fYQuanEndGO / 10f;
                }
                else if (quan > _fYQuanEndGO)
                {
                    _fYQuanBeginGO = quan;
                    _fYQuanEndGO = _fYQuanBeginGO / 10f;
                }
                else
                {
                    _fYQuanBeginGO = _fYQuanEndGO;
                }
                #endregion
                #region **根据新的权值修改坐标标定值**
                if (m <= _fYQuanBeginGO && m >= -_fYQuanBeginGO)
                {
                    _fYBeginGO = -_fYQuanBeginGO;
                }
                else
                {
                    _fYBeginGO = ((int)(m / _fYQuanBeginGO) - 1) * _fYQuanBeginGO;
                }
                #endregion
            }
            else
            {
                //如果值是从右边溢出
                #region **修改权值存入标定权值，控制权差在10倍以内**
                if (quan < _fYQuanBeginGO)
                {
                    _fYQuanEndGO = _fYQuanBeginGO / 10f;
                }
                else if (quan > _fYQuanBeginGO)
                {
                    _fYQuanEndGO = quan;
                    _fYQuanBeginGO = _fYQuanEndGO / 10f;
                }
                else
                {
                    _fYQuanEndGO = _fYQuanBeginGO;
                }
                #endregion
                #region **根据新的权值修改坐标标定值**
                if (m <= _fYQuanEndGO && m >= _fYQuanBeginGO)
                {
                    _fYEndGO = _fYQuanEndGO;
                }
                else
                {
                    _fYEndGO = ((int)(m / _fYQuanEndGO) + 1) * _fYQuanEndGO;
                }
                #endregion
            }
        }
        #endregion

        #region **私有方法 遍历要画的数据集合，并转换为坐标值 _changeToDrawPoints(index:int, listDrawPoints:ref List<PointF>, width:int, height:int):bool **
        /// <summary>
        /// 遍历要画的数据集合，并转换为坐标值
        /// </summary>
        /// <param name="index">要遍历的数据集合的编号</param>
        /// <param name="listDrawPoints">转后后的坐标集合</param>
        /// <param name="width">画布像素宽度</param>
        /// <param name="height">画布像素高度</param>
        /// <returns></returns>
        private bool _changeToDrawPoints(int index, List<PointF> listDrawPoints, int width, int height)
        {
            PointF currentPointF = new PointF(0, 0);
            List<Point> dotPoint = new List<Point>();
            //坐标起始和结束值之差小于精度范围则返回false
            if ((_fXEnd - _fXBegin) < _fAccuracy || (_fYEnd - _fYBegin) < _fAccuracy)
            {
                return false;
            }

            if (_haveFile[index])
            {
                FileStream fs = new FileStream(_filePath[index], FileMode.Open, FileAccess.Read);
                long fileLen = fs.Length;
                fs.Close();

                for (int i = 0; i <= fileLen / 0x8000; i++)
                {
                    dotPoint = _getPoint(index, i);
                    for (int j = 0; j < dotPoint.Count; j++)
                    {
                        currentPointF.X = (dotPoint[j].X - _fXBegin) * (width - 1) / (_fXEnd - _fXBegin);
                        currentPointF.Y = (dotPoint[j].Y - _fYBegin) * (height - 1) / (_fYEnd - _fYBegin);

                        listDrawPoints.Add(currentPointF);
                    }
                }
            }
            if (_listX[index] != null)
            {
                int length = _listX[index].Count;
                //遍历并转换为坐标值
                for (int i = 0; i < length; i++)
                {
                    //非数字则跳过
                    if (float.IsNaN(_listX[index][i]) || float.IsNaN(_listY[index][i]))
                    {
                        continue;
                    }
                    //转换为像素坐标
                    currentPointF.X = (_listX[index][i] - _fXBegin) * (width - 1) / (_fXEnd - _fXBegin);
                    currentPointF.Y = (_listY[index][i] - _fYBegin) * (height - 1) / (_fYEnd - _fYBegin);
                    //装载坐标
                    listDrawPoints.Add(currentPointF);
                }
            }

            //无数据则返回false
            if (listDrawPoints.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region **私有方法 波形显示中矩形区域的坐标转换为数据值 _changeXYPointsToNum(xB:float, xE:float, yB:float, yE:float, outxB:ref float, outxE:ref float, outyB:ref float, outyE:ref float):void **
        /// <summary>
        /// 波形显示中矩形区域的坐标转换为数据值
        /// </summary>
        /// <param name="xB">矩形区域左上角X轴坐标</param>
        /// <param name="xE">矩形区域右下角X轴坐标</param>
        /// <param name="yB">矩形区域左上角Y轴坐标</param>
        /// <param name="yE">矩形区域右下角Y轴坐标</param>
        /// <param name="outxB">转换后的左上角X轴数据值</param>
        /// <param name="outxE">转换后的右下角X轴数据值</param>
        /// <param name="outyB">转换后的左上角Y轴数据值</param>
        /// <param name="outyE">转换后的右下角Y轴数据值</param>
        private void _changeXYPointsToNum(float xB, float xE, float yB, float yE,
            ref float outxB, ref float outxE, ref float outyB, ref float outyE)
        {
            float currentB, currentE;
            currentB = xB / (pictureBoxGraph.Width - 1) * (_fXEnd - _fXBegin) + _fXBegin;
            currentE = xE / (pictureBoxGraph.Width - 1) * (_fXEnd - _fXBegin) + _fXBegin;
            outxB = currentB;
            outxE = currentE;
            currentB = _fYEnd - yB / (pictureBoxGraph.Height - 1) * (_fYEnd - _fYBegin);
            currentE = _fYEnd - yE / (pictureBoxGraph.Height - 1) * (_fYEnd - _fYBegin);
            outyE = currentB;
            outyB = currentE;
        }
        /// <summary>
        /// 波形显示中一个点的坐标转换为数据值
        /// </summary>
        /// <param name="x">要转换的点的X轴坐标</param>
        /// <param name="y">要转换的点的Y轴坐标</param>
        /// <param name="outX">转换后的X轴坐标</param>
        /// <param name="outY">转换后的Y轴坐标</param>
        private void _changeXYPointsToNum(float x, float y, ref float outX, ref float outY)
        {
            outX = x / (pictureBoxGraph.Width - 1) * (_fXEnd - _fXBegin) + _fXBegin;
            outY = _fYEnd - y / (pictureBoxGraph.Height - 1) * (_fYEnd - _fYBegin);
        }
        #endregion

        #region **读取文件里边的数据并显示鼠标所在位置曲线的是极坐标 ShowCoordinate(e:MouseEventArgs):void**
        /// <summary>
        /// 读取文件里边的数据并显示鼠标所在位置曲线的坐标
        /// </summary>
        /// <param name="e">鼠标事件相关的参数</param>
        private void ShowCoordinate(MouseEventArgs e)
        {
            bool haveDot = false;
            float mouseX = 0, mouseY = 0;                                            //储存鼠标的转换坐标值
            float fileX = 0, fileY = 0;
            float fileX1 = 0, fileY1 = 0;
            _changeXYPointsToNum(e.Location.X, e.Location.Y, ref mouseX, ref mouseY);
            double minXY = Math.Sqrt(mouseX * mouseX + mouseY * mouseY);             //储存鼠标点与曲线坐标最近的距离
            for (int k = 0; k < _listX.Count; k++)                                   //遍历所有的波形曲线
            {
                if (_haveFile[k])                                                    //先读取文件里边的数值然后再读取列表里边的数据
                {
                    FileStream fs = new FileStream(_filePath[k], FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    long len = (long)(fs.Length / 8);                                //获取点长度
                    for (long i = 0; i < len - 1; i++)
                    {
                        fileX = br.ReadInt32();                                      //读取文件里边的数据
                        fileY = br.ReadInt32();
                        if (fileX > mouseX)                                          //如果判断其中x坐标比鼠标的转换坐标还大，说明已经找到相近的点
                        {
                            fileX1 = br.ReadInt32();
                            fileY1 = br.ReadInt32();
                            br.Close();
                            fs.Close();
                            haveDot = true;                                          //表示已经找到符合条件的点，可以进行相关的处理了
                            break;
                        }
                    }
                    if (haveDot)
                    {
                        float cuvX = (fileX - _fXBegin) * (pictureBoxGraph.Width - 1) / (_fXEnd - _fXBegin); //把得到的两个点左边转换为图形的实际坐标
                        float cuvY = pictureBoxGraph.Height - (fileY - _fYBegin) * (pictureBoxGraph.Height - 1) / (_fYEnd - _fYBegin);
                        float cuvX1 = (fileX1 - _fXBegin) * (pictureBoxGraph.Width - 1) / (_fXEnd - _fXBegin);
                        float cuvY1 = pictureBoxGraph.Height - (fileY1 - _fYBegin) * (pictureBoxGraph.Height - 1) / (_fYEnd - _fYBegin);
                        double diatance = Math.Sqrt((e.Location.X - cuvX) * (e.Location.X - cuvX) + (e.Location.Y - cuvY) * (e.Location.Y - cuvY));//判断这个点与鼠标点是否相差的太远
                        double diatance1 = Math.Sqrt((e.Location.X - cuvX) * (e.Location.X - cuvX) + (e.Location.Y - cuvY) * (e.Location.Y - cuvY));
                        if ((diatance <= 10) || (diatance1 <= 10))                  //判断鼠标点击区域是否落在曲线上
                        {

                            haveDot = false;
                            Refresh();
                            return;                                                  //跳出函数
                        }
                    }
                }
                if (_listX[k] != null)                                               //获取列表里边的数据
                {
                    int count;
                    try
                    {
                        count = _listX[0].Count - 1;
                    }
                    catch
                    {
                        return;
                    }
                    int minI = count;
                    //遍历坐标，确定鼠标点击的区域横坐标在曲线的某两个点之间
                    for (int i = 0; i < _listX[k].Count - 1; i++)
                    {
                        if (mouseX - _listX[k][i] >= 0 && mouseX - _listX[k][i + 1] <= 0)//鼠标位置在某两个值之间
                        {
                            minI = i;
                            break;
                        }
                        else
                            minI = count;
                    }

                    if (minI != count)                                                   //如果标号有变化，则说明鼠标有点在曲线的横坐标范围内
                    {
                        float cuvX = (_listX[k][minI] - _fXBegin) * (pictureBoxGraph.Width - 1) / (_fXEnd - _fXBegin);
                        float cuvY = pictureBoxGraph.Height - (_listY[k][minI] - _fYBegin) * (pictureBoxGraph.Height - 1) / (_fYEnd - _fYBegin);
                        float cuvX1 = (_listX[k][minI] - _fXBegin) * (pictureBoxGraph.Width - 1) / (_fXEnd - _fXBegin);
                        float cuvY1 = pictureBoxGraph.Height - (_listY[k][minI] - _fYBegin) * (pictureBoxGraph.Height - 1) / (_fYEnd - _fYBegin);
                        double diatance = Math.Sqrt((e.Location.X - cuvX) * (e.Location.X - cuvX) + (e.Location.Y - cuvY) * (e.Location.Y - cuvY));
                        double diatance1 = Math.Sqrt((e.Location.X - cuvX1) * (e.Location.X - cuvX1) + (e.Location.Y - cuvY1) * (e.Location.Y - cuvY1));

                        if ((diatance <= 10) || (diatance1 <= 10))                        //判断鼠标点击区域是否落在曲线上
                        {

                            Refresh();
                            return;
                        }
                    }
                }

            }
        }
        #endregion
        #endregion

        #region ** Parameters Private **


        #region ** 私有成员 波形显示控件基本成员 **
        /**************************************************************
         * 
         * 波形显示控件基本成员
         * 
         * *************************************************************/
        /// <summary>
        /// 波形显示控件标题
        /// </summary>
        private string _SyStitle = "sEMG 通道n";  //公开
        /// <summary>
        /// X轴名称
        /// </summary>
        private string _SySnameX = "时间";   //公开
        /// <summary>
        /// Y轴名称
        /// </summary>
        private string _SySnameY = "电压";   //公开
        /// <summary>
        /// 当前坐标是否自动调整以适合窗口大小
        /// </summary>
        private bool _isAutoModeXY = false;
        /// <summary>
        /// 当前是否显示网格
        /// </summary>
        private bool _isLinesShowXY = true;
        /// <summary>
        /// 坐标精确度
        /// </summary>
        private float _fAccuracy = 0.05f;
        /// <summary>
        /// 坐标显示最多小数位数
        /// </summary>
        private int _iAccuracy = 2;
        #endregion


        #region ** 私有成员 波形显示控件数据相关 **
        //数据点
        private List<float> xData = new List<float>();
        private List<float> yData = new List<float>();
        /************************************************************
         * 
         * 波形显示控件数据相关
         * 
         * *********************************************************/
        /// <summary>
        /// 要显示的数据集合的X轴方向值集合的引用
        /// 若显示多条数据则依次添加X轴方向值集合的引用
        /// </summary>
        private List<List<float>> _listX = new List<List<float>>();
        /// <summary>
        /// 要显示的数据集合的Y轴方向值集合的引用
        /// 若显示多条数据则依次添加Y轴方向值集合的引用
        /// </summary>
        private List<List<float>> _listY = new List<List<float>>();
        /// <summary>
        /// 储存波形信号文件的位置
        /// </summary>
        private List<string> _filePath = new List<string>();
        /// <summary>
        /// 是否启用文件采集波形的标志位
        /// </summary>
        private List<bool> _haveFile = new List<bool>();
        /// <summary>
        /// 要显示数据集合的线条颜色
        /// </summary>
        private List<Color> _listColor = new List<Color>();
        /// <summary>
        /// 要显示数据集合的线条宽度
        /// </summary>
        private List<int> _listWidth = new List<int>();
        /// <summary>
        /// 要显示数据集合的连接点
        /// </summary>
        private List<LineJoin> _listLineJoin = new List<LineJoin>();
        /// <summary>
        /// 要显示数据集合的起始线帽
        /// </summary>
        private List<LineCap> _listLineCap = new List<LineCap>();
        /// <summary>
        /// 要显示数据集合的样式
        /// </summary>
        private List<DrawStyle> _listDrawStyle = new List<DrawStyle>();
        /// <summary>
        /// 默认可移动的坐标范围，在接收数据时波形向左平移
        /// </summary>
        private bool _isDefaultMoveModeXY = false;
        /// <summary>
        /// 当前是否处于可移动模式
        /// </summary>
        private bool _isMoveModeXY = false;
        /// <summary>
        /// 当前是否处于显示坐标模式
        /// </summary>
        private bool _isShowNumModeXY = false;
        #endregion


        #region ** 私有成员 波形显示控件坐标相关 **
        /******************************************************************
         * 
         * 波形显示控件坐标相关
         * 
         * ****************************************************************/
        // 初始坐标
        private float _fXBeginSYS = 0f;     //公开
        private float _fXEndSYS = 8;      //公开
        private float _fYBeginSYS = -8;     //公开
        private float _fYEndSYS = 8;       //公开

        // 当前要画的线的像素坐标
        private List<PointF> _listDrawPoints = new List<PointF>();

        // 当前显示波形的X轴起始坐标值
        private float _fXBegin;
        // 当前显示波形的X轴结束坐标值
        private float _fXEnd;
        // 当前显示波形的Y轴起始坐标值
        private float _fYBegin;
        // 当前显示波形的Y轴结束坐标值
        private float _fYEnd;

        // 当前显示波形的X轴坐标标定起始值
        private float _fXBeginGO;
        // 当前显示波形的X轴坐标标定结束值
        private float _fXEndGO;
        // 当前显示波形的Y轴坐标标定起始值
        private float _fYBeginGO;
        // 当前显示波形的Y轴坐标标定结束值
        private float _fYEndGO;

        // 当前显示波形的X轴坐标标定起始权值
        private float _fXQuanBeginGO;
        // 当前显示波形的X轴坐标标定结束权值
        private float _fXQuanEndGO;
        // 当前显示波形的Y轴坐标标定起始权值
        private float _fYQuanBeginGO;
        // 当前显示波形的Y轴坐标标定结束权值
        private float _fYQuanEndGO;
        #endregion


        #region ** 私有成员 波形显示控件网格相关 **
        /*********************************************************
         * 
         * 波形显示控件网格相关
         * 
         * *******************************************************/
        // X轴网格线是否从左开始画
        private bool _bXLinesLBegin;
        // Y轴网格线是否从下开始画
        private bool _bYLinesLBegin;
        // 所要画X轴坐标的起点像素位置
        private float _fXpxGO;
        // 所要画Y轴坐标的起点像素位置
        private float _fYpxGO;
        // X轴第一层网格线间隔
        private float _fXLinesShowFirst = 0;
        // X轴第二层网格线间隔
        private float _fXLinesShowSecond = 0;
        // X轴第三层网格线间隔
        private float _fXLinesShowThird = 0;
        // Y轴第一层网格线间隔
        private float _fYLinesShowFirst = 0;
        // Y轴第二层网格线间隔
        private float _fYLinesShowSecond = 0;
        // Y轴第三层网格线间隔
        private float _fYLinesShowThird = 0;
        #endregion


        #region ** 私有成员 波形显示控件外观样式方案 **
        /*********************************************************
         * 
         * 波形显示控件外观样式方案
         * 
         * *******************************************************/
        //控件标题字体大小
        private int _titleSize = 14;
        //控件标题位置
        private float _titlePosition = 0.4f;
        //控件标题颜色
        private Color _titleColor = Color.FromArgb(0, 0, 0);
        //控件标题描边颜色
        private Color _titleBorderColor = Color.FromArgb(250, 250, 250);

        //背景色渐进起始颜色
        private Color _backColorL = Color.FromArgb(255, 255, 255);
        //背景色渐进终止颜色
        private Color _backColorH = Color.FromArgb(200, 200, 200);

        //坐标线颜色
        private Color _coordinateLineColor = Color.FromArgb(0, 0, 0);
        //坐标值颜色
        private Color _coordinateStringColor = Color.FromArgb(0, 0, 0);
        //坐标标题颜色
        private Color _coordinateStringTitleColor = Color.FromArgb(0, 0, 0);

        //网格线的透明度
        private int _iLineShowColorAlpha = 100;
        //网格线的颜色
        private Color _iLineShowColor = Color.FromArgb(255, 255, 255);

        //波形显示区域背景色
        private Color _GraphBackColor = Color.FromArgb(0, 0, 0);

        //工具栏背景色
        private Color ControlItemBackColor = Color.FromArgb(0, 0, 0);
        //工具栏按钮背景颜色
        private Color ControlButtonBackColor = Color.FromArgb(0, 0, 0);
        //工具栏按钮前景选中颜色
        private Color ControlButtonForeColorL = Color.FromArgb(250, 250, 250);
        //工具栏按钮前景未选中颜色
        private Color ControlButtonForeColorH = Color.FromArgb(100, 100, 100);

        //标签说明框背景颜色
        private Color DirectionBackColor = Color.FromArgb(32, 32, 32);
        //标签说明框文字颜色
        private Color DirectionForeColor = Color.FromArgb(255, 255, 255);

        //放大选取框背景颜色
        private Color BigXYBackColor = Color.FromArgb(255, 255, 255);
        //放大选取框按钮背景颜色
        private Color BigXYButtonBackColor = Color.FromArgb(200, 255, 255, 255);
        //放大选取框按钮文字颜色
        private Color BigXYButtonForeColor = Color.FromArgb(0, 0, 0);
        #endregion
        #endregion

        #region ** Parameters Public **
        #region **公有属性**
        /// <summary>
        /// 波形显示控件标题
        /// </summary>
        public string m_SyStitle
        {
            set { _SyStitle = value; }
            get { return _SyStitle; }
        }
        /// <summary>
        /// X轴名称
        /// </summary>
        public string m_SySnameX
        {
            set { _SySnameX = value; }
            get { return _SySnameX; }
        }
        /// <summary>
        /// Y轴名称
        /// </summary>
        public string m_SySnameY
        {
            set { _SySnameY = value; }
            get { return _SySnameY; }
        }
        /// <summary>
        /// 初始X轴起始坐标
        /// </summary>
        public float m_fXBeginSYS
        {
            set { _fXBeginSYS = value; }
            get { return _fXBeginSYS; }
        }
        /// <summary>
        /// 初始X轴结束坐标
        /// </summary>
        public float m_fXEndSYS
        {
            set { _fXEndSYS = value; }
            get { return _fXEndSYS; }
        }
        /// <summary>
        /// 初始Y轴起始坐标
        /// </summary>
        public float m_fYBeginSYS
        {
            set { _fYBeginSYS = value; }
            get { return _fYBeginSYS; }
        }
        /// <summary>
        /// 初始Y轴结束坐标
        /// </summary>
        public float m_fYEndSYS
        {
            set { _fYEndSYS = value; }
            get { return _fYEndSYS; }
        }
        #endregion

        #region **公有属性 控件外观样式**
        /*********************************************************
         * 
         * 波形显示控件外观样式方案
         * 
         * *******************************************************/
        /// <summary>
        /// 控件标题字体大小
        /// </summary>
        public int m_titleSize
        {
            set { _titleSize = value; }
            get { return _titleSize; }
        }
        /// <summary>
        /// 控件标题位置
        /// </summary>
        public float m_titlePosition
        {
            set { _titlePosition = value; }
            get { return _titlePosition; }
        }
        /// <summary>
        /// 控件标题颜色
        /// </summary>
        public Color m_titleColor
        {
            set { _titleColor = value; }
            get { return _titleColor; }
        }
        /// <summary>
        /// 控件标题描边颜色
        /// </summary>
        public Color m_titleBorderColor
        {
            set { _titleBorderColor = value; }
            get { return _titleBorderColor; }
        }
        /// <summary>
        /// 背景色渐进起始颜色
        /// </summary>
        public Color m_backColorL
        {
            set { _backColorL = value; }
            get { return _backColorL; }
        }
        /// <summary>
        /// 背景色渐进终止颜色
        /// </summary>
        public Color m_backColorH
        {
            set { _backColorH = value; }
            get { return _backColorH; }
        }
        /// <summary>
        /// 坐标线颜色
        /// </summary>
        public Color m_coordinateLineColor
        {
            set { _coordinateLineColor = value; }
            get { return _coordinateLineColor; }
        }
        /// <summary>
        /// 坐标值颜色
        /// </summary>
        public Color m_coordinateStringColor
        {
            set { _coordinateStringColor = value; }
            get { return _coordinateStringColor; }
        }
        /// <summary>
        /// 坐标标题颜色
        /// </summary>
        public Color m_coordinateStringTitleColor
        {
            set { _coordinateStringTitleColor = value; }
            get { return _coordinateStringTitleColor; }
        }
        /// <summary>
        /// 网格线的透明度
        /// </summary>
        public int m_iLineShowColorAlpha
        {
            set { _iLineShowColorAlpha = value; }
            get { return _iLineShowColorAlpha; }
        }
        /// <summary>
        /// 网格线的颜色
        /// </summary>
        public Color m_iLineShowColor
        {
            set { _iLineShowColor = value; }
            get { return _iLineShowColor; }
        }
        /// <summary>
        /// 波形显示区域背景色
        /// </summary>
        public Color m_GraphBackColor
        {
            set
            {
                _GraphBackColor = value;
                pictureBoxGraph.BackColor = _GraphBackColor;
            }
            get { return _GraphBackColor; }
        }

        #endregion


        /// <summary>
        /// 画图样式
        /// </summary>
        public enum DrawStyle
        {
            /// <summary>
            /// 线条
            /// </summary>
            Line,
            /// <summary>
            /// 点
            /// </summary>
            dot,
            /// <summary>
            /// 条形
            /// </summary>
            bar
        }
        #endregion

    }
}
