using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace Conductor.GUI
{

    public class CartesianGrid : UserControl
    {


        public void RandomMoves()
        {
            bool Abort = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Escape);
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += RandomMovesWorker;
            bgw.RunWorkerAsync();
        }

        private void RandomMovesWorker(object sender, DoWorkEventArgs e)
        {

            bool Abort = false;

            while (!Abort)
            {
             //   Abort = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Escape);
                while (_AnimationInProgress)
                    System.Threading.Thread.Sleep(100);
                RandomMove();

            }


        }

        Point GetCellAddressPositionInWindowCoordinates(CartesianGridCell cell)
        {
            if (this.InvokeRequired)
            {
                return (Point)this.Invoke((Func<Point>)delegate
                {
                    return this.PointToScreen(Point.Round(cell.AddressPosition));
                });
            }
            else
                return this.PointToScreen(Point.Round(cell.AddressPosition));
        }

        public void RandomMove()
        {
            _AnimationInProgress = true;
            Point initialPosition = Cursor.Position;
            CartesianGridCell startCell = GetRandomFullCell();
            CartesianGridCell finishCell = GetRandomEmptyCell();
            if (startCell == null || finishCell == null)
                return;


            Point start = GetCellAddressPositionInWindowCoordinates(startCell);
            Point finish = GetCellAddressPositionInWindowCoordinates(finishCell);


            int duration = _Random.Next(200, 1200);
            MouseMoveArgs args = new MouseMoveArgs() { duration = duration, start = start, finish = finish, initial = initialPosition };
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += MoveCellInternal;
            bgw.RunWorkerAsync(args);
        }

        Random _Random = new Random();
        bool _AnimationInProgress = false;


        CartesianGridCell GetRandomFullCell()
        {
            if (_CellMapByObject.Keys.Count == 0)
                return null;
            return _CellMapByObject.ElementAt(_Random.Next(0, _CellMapByObject.Count)).Value;
        }

        CartesianGridCell GetRandomEmptyCell()
        {
            if (_CellMapByObject.Keys.Count == this.Capacity)
                return null;
            while (true)
            {
                int index = _Random.Next(0, this.Capacity - 1);
                if (_CellMapByOrdinal[index].Item == null)
                    return _CellMapByOrdinal[index];
            }
        }

        class MouseMoveArgs
        {

            public int duration { get; set; }
            public Point start { get; set; }
            public Point finish { get; set; }
            public Point initial { get; set; }
        }


        void MoveCellInternal(object sender, DoWorkEventArgs e)
        {
            MouseMoveArgs args = e.Argument as MouseMoveArgs;

            Point start = args.start;
            Cursor.Position = start;
            Point newPosition = args.finish;
            int duration_milliseconds = args.duration;
            WinAPI.MouseDown();

            // Find the vector between start and newPosition
            float deltaX = newPosition.X - start.X;
            float deltaY = newPosition.Y - start.Y;

            // start a timer
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            float timeFraction = 0.0F;

            do
            {
                timeFraction = (float)stopwatch.ElapsedMilliseconds / duration_milliseconds;
                if (timeFraction > 1.0F)
                    timeFraction = 1.0F;

                PointF curPoint = new PointF(start.X + timeFraction * deltaX,
                                             start.Y + timeFraction * deltaY);
                Cursor.Position = Point.Round(curPoint);
                Thread.Sleep(20);
                
       



            } while (timeFraction < 1.0);

            WinAPI.MouseUp();

            Cursor.Position = args.initial;
            _AnimationInProgress = false;

        }


        public enum SelectionMode { None, SingleCell, MultipleCells }

        public CartesianGrid()
        {
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            LayoutGrid(true);
            this.MouseMove += CartesianGrid_MouseMove;
            this.MouseDown += CartesianGrid_MouseDown;
            this.MouseUp += CartesianGrid_MouseUp;
            this.Resize += new System.EventHandler(this.Control_Resize);
            this._HoverTimer.Elapsed += _HoverTimer_Elapsed;
        }

        #region Mouse Events

        void CartesianGrid_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseCurrentPosition = e.Location;
            bool CheckForMovement = (!_inDragPhase && !_inSelectRectanglePhase && MouseButtons == MouseButtons.Left);
            if (CheckForMovement)
            {
                if
               (Math.Abs(_mouseDownStartPosition.X - _mouseCurrentPosition.X) >= _dragTheshold.X ||
                Math.Abs(_mouseDownStartPosition.Y - _mouseCurrentPosition.Y) >= _dragTheshold.Y)
                {
                    if (_inPreDragPhase && AllowReordering)
                    {
                        _inPreDragPhase = false;


                        Debug.WriteLine("Entering drag phase");
                        Rectangle r = GetSelectedSpanningRectangle();
                        _DragSelectedCellBounds = r;
                        _DragCellLeftUpper = _Cells[r.X, r.Y];
                        Point upperLeft = _Cells[r.X, r.Y].Rectangle.Location;
                        Rectangle bottomRightRect = _Cells[r.X + r.Width, r.Y + r.Height].Rectangle;
                        Point bottomRight = new Point(bottomRightRect.Left + bottomRightRect.Width, bottomRightRect.Top + bottomRightRect.Height);
                        int borderPadding = this.SelectedCellBorderWidth / 4;
                        Rectangle targetBounds = new Rectangle(Math.Max(upperLeft.X - borderPadding, 0), Math.Max(upperLeft.Y - borderPadding, 0), bottomRight.X - upperLeft.X + 2 * borderPadding, bottomRight.Y - upperLeft.Y + 2 * borderPadding);
                        System.Drawing.Bitmap bmpWhole = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
                        this.DrawToBitmap(bmpWhole, this.ClientRectangle);
                        List<Rectangle> clipRects = new List<Rectangle>();
                        for (int colIndex = 0; colIndex < _Columns; colIndex++)
                            for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                            {
                                if (colIndex >= r.X && colIndex <= (r.X + r.Width) &&
                                    rowIndex >= r.Y && rowIndex <= (r.Y + r.Height))
                                {
                                    CartesianGridCell cell = _Cells[colIndex, rowIndex];
                                    if (!cell.Selected)
                                    {
                                        Rectangle clipRect = cell.Rectangle;
                                        bool leftIsBorderedByCell = (colIndex > r.X && _Cells[colIndex - 1, rowIndex].Selected);
                                        bool topIsBorderedByCell = (rowIndex > r.Y && _Cells[colIndex, rowIndex - 1].Selected);
                                        bool rightIsBorderedByCell = (colIndex < r.X + r.Width && _Cells[colIndex + 1, rowIndex].Selected);
                                        bool bottomIsBorderedByCell = (rowIndex < r.Y + r.Height && _Cells[colIndex, rowIndex + 1].Selected);
                                        bool upperLeftIsBorderedByCell = (colIndex > r.X && rowIndex > r.Y && _Cells[colIndex - 1, rowIndex - 1].Selected);
                                        bool lowerLeftIsBorderedByCell = (colIndex > r.X && rowIndex < r.Y + r.Height && _Cells[colIndex - 1, rowIndex + 1].Selected);
                                        bool upperRightIsBorderedByCell = (colIndex < r.X + r.Width && rowIndex > r.Y && _Cells[colIndex + 1, rowIndex - 1].Selected);
                                        bool lowerRightIsBorderedByCell = (colIndex < r.X + r.Width && rowIndex < r.Y + r.Height && _Cells[colIndex + 1, rowIndex + 1].Selected);
                                        Rectangle centerClipRect = System.Drawing.Rectangle.Inflate(clipRect, -borderPadding, -borderPadding);
                                        clipRects.Add(centerClipRect);
                                        if (!topIsBorderedByCell)
                                            clipRects.Add(new Rectangle(centerClipRect.Left, centerClipRect.Top - 2 * borderPadding, centerClipRect.Width, 2 * borderPadding));
                                        if (!bottomIsBorderedByCell || rowIndex == r.Y + r.Height)
                                            clipRects.Add(new Rectangle(centerClipRect.Left, centerClipRect.Top + centerClipRect.Height, centerClipRect.Width, 2 * borderPadding));
                                        if (!leftIsBorderedByCell)
                                            clipRects.Add(new Rectangle(centerClipRect.Left - 2 * borderPadding, centerClipRect.Top, 2 * borderPadding, centerClipRect.Height));
                                        if (!rightIsBorderedByCell)
                                            clipRects.Add(new Rectangle(centerClipRect.Left + centerClipRect.Width, centerClipRect.Top, 2 * borderPadding, centerClipRect.Height));
                                        if (!bottomIsBorderedByCell && !leftIsBorderedByCell && !lowerLeftIsBorderedByCell)
                                            clipRects.Add(new Rectangle(centerClipRect.Left - 2 * borderPadding, centerClipRect.Top + centerClipRect.Height, 2 * borderPadding, 2 * borderPadding));
                                        if (!topIsBorderedByCell && !leftIsBorderedByCell && !upperLeftIsBorderedByCell)
                                            clipRects.Add(new Rectangle(centerClipRect.Left - 2 * borderPadding, centerClipRect.Top - 2 * borderPadding, 2 * borderPadding, 2 * borderPadding));
                                        if (!topIsBorderedByCell && !rightIsBorderedByCell && !upperRightIsBorderedByCell)
                                            clipRects.Add(new Rectangle(centerClipRect.Left + centerClipRect.Width, centerClipRect.Top - 2 * borderPadding, 2 * borderPadding, 2 * borderPadding));
                                        if (!bottomIsBorderedByCell && !rightIsBorderedByCell && !lowerRightIsBorderedByCell)
                                            clipRects.Add(new Rectangle(centerClipRect.Left + centerClipRect.Width, centerClipRect.Top + centerClipRect.Height, 2 * borderPadding, 2 * borderPadding));
                                    }
                                }
                            }

                        Graphics g = Graphics.FromImage(bmpWhole);
                        foreach (Rectangle clipRect in clipRects)
                        {
                            g.SetClip(clipRect);
                            g.Clear(Color.Transparent);
                        }
                        g.ResetClip();
                        Bitmap cursorBitmap = WinAPI.CaptureCursor();
                        IntPtr Hicon = cursorBitmap.GetHicon();
                        Icon cursorIcon = Icon.FromHandle(Hicon);
                        g.DrawIcon(cursorIcon, _mouseDownStartPosition.X, _mouseDownStartPosition.Y);
                        g.Dispose();
                        if (bmpWhole.Height < targetBounds.Height) { targetBounds.Height = bmpWhole.Height; }
                        if (bmpWhole.Width < targetBounds.Width) { targetBounds.Width = bmpWhole.Width; }
                        if (bmpWhole.Height < targetBounds.Bottom) targetBounds.Height = targetBounds.Height - targetBounds.Bottom + bmpWhole.Height;
                        if (bmpWhole.Width < targetBounds.Right) targetBounds.Width = targetBounds.Width - targetBounds.Right + bmpWhole.Width;
                        Bitmap bmp = bmpWhole.Clone(targetBounds, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                        //   CheckDPIs();
                        float scaling = WinAPI.getScalingFactor();
                        int cursorWidth = (int)(bmp.Width * scaling);
                        int cursorHeight = (int)(bmp.Height * scaling);
                        Bitmap bmp2 = new Bitmap(cursorWidth, cursorHeight);
                        using (Graphics gfx = Graphics.FromImage(bmp2))
                        {
                            ColorMatrix matrix = new ColorMatrix();
                            matrix.Matrix33 = 0.55f;
                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            gfx.DrawImage(bmp, new Rectangle(0, 0, cursorWidth, cursorHeight), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            int xHotspot = _mouseDownStartPosition.X - upperLeft.X;
                            int yHotspot = _mouseDownStartPosition.Y - upperLeft.Y;
                            _DragOffset = new Point(xHotspot, yHotspot);
                            this.Cursor = WinAPI.CreateCursor(bmp2, xHotspot, yHotspot);
                        }
                        bmpWhole.Dispose();
                        bmp.Dispose();
                        bmp2.Dispose();
                        _inDragPhase = true;
                        this.Invalidate();
                    }
                    else

                    {
                        if (CellSelectionMode != SelectionMode.MultipleCells)
                            return;
                        _inSelectRectanglePhase = true;

                        if (ModifierKeys != Keys.Shift)
                            ClearSelectionInternal(true);
                        else
                        {
                            var current = GetCellFromLocation(e.Location);
                            if (current != null && current.Selected)
                                current.Selected = false;
                            this.Invalidate();

                        }


                    }
                }
                return;
            }

            if (_inDragPhase)
            {
                CartesianGridCell currentCell = this.GetCellFromLocation(e.Location);
                if (currentCell != _lastDragPhaseCell)
                {
                    bool TargetOK = IsDropTargetValidHighlight(e.Location);
                    _lastDragPhaseCell = currentCell;
                    this.Invalidate();
                }
            }

            if (_inSelectRectanglePhase)
                this.Invalidate();


            if (MouseButtons != MouseButtons.Left && _Cells != null)
            {
                CartesianGridCell leftRectangle = null;
                CartesianGridCell enteredRectangle = null;
                Point? current = GetGridCoordinatesFromLocation(e.Location, false);
                bool changeDetected = false;
                bool offGrid = (current == null);

                if (offGrid)
                    if (_lastMouseOverCell == null)
                        return;
                    else
                    {
                        changeDetected = true;
                        leftRectangle = _lastMouseOverCell;
                        enteredRectangle = null;
                    }
                else //onGrid
                 if (_lastMouseOverCell == null)
                {
                    changeDetected = true;
                    enteredRectangle = _Cells[current.Value.X, current.Value.Y];
                    leftRectangle = null;
                }
                else //need to see if there is an actual change
                    if (_lastMouseOverCell.ColIndex == current.Value.X && _lastMouseOverCell.RowIndex == current.Value.Y)
                    return;
                else
                {
                    changeDetected = true;
                    leftRectangle = _lastMouseOverCell;
                    enteredRectangle = _Cells[current.Value.X, current.Value.Y];
                }

                if (changeDetected)
                {
                    StartHoverTimer();
                    _lastMouseOverCell = enteredRectangle;
                    if (leftRectangle != null)
                        CellLeft?.Invoke(leftRectangle);
                    if (enteredRectangle != null)
                        CellEntered?.Invoke(enteredRectangle);
                }
            }
        }

        void CartesianGrid_MouseDown(object sender, MouseEventArgs e)
        {
            StopHoverTimer();
            var currentCellCoordinates = this.GetGridCoordinatesFromLocation(e.Location, false);
            if (currentCellCoordinates == null)
                return;

            CartesianGridCell currentCell = _Cells[currentCellCoordinates.Value.X, currentCellCoordinates.Value.Y];

            if (e.Button == MouseButtons.Right && currentCell.Item != null)
            {
                ShowPropertyViewer(currentCell.Item);
                return;
            }

            if (CellSelectionMode == SelectionMode.None || e.Button != MouseButtons.Left)
                return;
            _defaultCursor = this.Cursor;
            _mouseCurrentPosition = _mouseDownStartPosition = e.Location;
            _dragTheshold = WinAPI.GetDragThreshold();

            bool currentlySelected = currentCell.Selected;
            bool shiftKeyIsDown = (ModifierKeys == Keys.Shift);
            bool NeedsRedraw = false;

            if (currentCell.Selected && shiftKeyIsDown)
            {
                currentCell.Selected = false;
                NeedsRedraw = true;
            }
            else if (!currentCell.Selected && shiftKeyIsDown && currentCell.Item != null)
            {
                if (CellSelectionMode == SelectionMode.SingleCell)
                    this.ClearSelectionInternal(false);
                currentCell.Selected = true;
                NeedsRedraw = true;
            }

            else if (!currentCell.Selected && currentCell.Item != null)
            {
                this.ClearSelectionInternal(false);
                currentCell.Selected = true;
                NeedsRedraw = true;
            }

            //If shift key is not down, and we are clicking on an already selected cell, that is the cue to consider initiating a drag/drop
            _inPreDragPhase = currentCell.Selected && ModifierKeys != Keys.Shift;

            if (_inPreDragPhase)
                _cellMouseClicked = currentCell;

            if (NeedsRedraw)
                this.Invalidate();
        }

        void CartesianGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (_inDragPhase)
            {
                _inDragPhase = false;
                this.Cursor = _defaultCursor;
                Debug.WriteLine("Drag phase ended");
                Debug.WriteLine("Mouse hot spot:" + e.Location.ToString());
                Debug.WriteLine("offset: " + _DragOffset.ToString());
                Debug.WriteLine("Selected cell that contains cursor:" + _cellMouseClicked.Address.ToString());
                Point? current = this.GetGridCoordinatesFromLocation(e.Location, false, false);
                if (current == null) { this.Invalidate(); return; }
                CartesianGridCell targetCell = _Cells[current.Value.X, current.Value.Y];
                bool DoMove = IsDropTargetValid(e.Location);
                ClearDropTargetHighlights();
                if (DoMove && targetCell != _cellMouseClicked)
                {
                    MoveSelectedCellsTo(e.Location);
                    ClearSelection();
                }
                this.Invalidate();
                return;
            }


            if (_inSelectRectanglePhase)
            {
                _inSelectRectanglePhase = false;
                var rc = getMouseMovedRectangle();
                if (rc.Width > 0 && rc.Height > 0)
                    SelectSpannedCells(rc);
                this.Invalidate();
                return;
            }
        }

        #endregion

        #region Helper Classes

        public class CartesianGridCell
        {

            private CartesianGridCell() { }
            public CartesianGridCell(int ColIndex, int RowIndex)
            {
                this.ColIndex = ColIndex;
                this.RowIndex = RowIndex;
                this.Coordinates = new Point(ColIndex, RowIndex);
            }
            public bool BorderHighlighted { get; set; }
            const float BORDER_PROPORTION = 0.05f;
            public int Ordinal { get; internal set; }
            public Rectangle Rectangle { get; set; }
            public string Address { get; set; }
            public int ColIndex { get; private set; }
            public int RowIndex { get; private set; }
            public Point Coordinates { get; private set; }
            public PointF AddressPosition
            {
                get
                {
                    float left = (this.Rectangle.Left + this.Rectangle.Width * BORDER_PROPORTION);
                    float top = (this.Rectangle.Top + this.Rectangle.Height * BORDER_PROPORTION);
                    return new PointF(left, top);
                }
            }
            public Rectangle LabelRectangle
            {
                get
                {
                    Rectangle r = this.Rectangle;
                    int offset = (int)(r.Height * BORDER_PROPORTION * 4);
                    r.Y += offset;
                    r.Height -= offset;
                    return r;
                }
            }
            public Color OverlayColor { get; set; }
            public string OverlayCaption { get; set; }
            public Color BackColor { get; set; }
            public string Text { get; set; }
            public object Item { get; set; }
            public bool Selected { get; set; }
            public bool DropTargetHighlighted { get; set; }
        }

        #endregion

        #region Private variables

        protected int _Rows = 8;
        protected int _Columns = 12;
        protected int _GridPadding = 5;
        protected int _CellPadding = 2;
        protected Mapping.FillOrder _Order = Mapping.FillOrder.LeftRightTopBottom;
        protected CartesianGridCell[,] _Cells = null;
        protected Dictionary<int, CartesianGridCell> _CellMapByOrdinal = null;
        protected Dictionary<string, CartesianGridCell> _CellMapByAddress = null;
        protected Dictionary<object, CartesianGridCell> _CellMapByObject = new Dictionary<object, CartesianGridCell>();

        Cursor _defaultCursor;
        Point _mouseDownStartPosition;
        Point _mouseCurrentPosition;
        Point _dragTheshold;
        bool _inSelectRectanglePhase;
        bool _inDragPhase;
        bool _inPreDragPhase;
        CartesianGridCell _DragCellLeftUpper = null;
        Rectangle _DragSelectedCellBounds = Rectangle.Empty;
        Point _DragOffset = Point.Empty;
        object _DrawLock = new object();
        CartesianGridCell _cellMouseClicked = null;
        CartesianGridCell _lastDragPhaseCell = null;
        CartesianGridCell _lastMouseOverCell = null;
        System.Timers.Timer _HoverTimer = new System.Timers.Timer();
        object _DataSource = null;
        Type _DataBoundItemTypeRestriction = null;
        int _CaptionHeight = 35;
        int _ErrorCaptionHeight = 35;
        ICartesianMapper _Mapper = new CartesianMapper();
        int _SelectedCellBorderWidth = 4;
        int _SelectedCellBorderHalfWidth;
        int _SelectedCellBorderMiterWidth;
        int _SelectedCellBorderMiterHalfWidth;
        Color _SelectedCellBorderColor = Color.BlueViolet;
        SelectionMode _CellSelectionMode;
        int _GridLineWidth = 1;
        string _Text = null;
        GridDimensions _Dimensions = new GridDimensions();
        object _BindingLock = new object();
        PopupPropertyViewer _PPV = new PopupPropertyViewer();
        private bool _ThrowExceptionsOnErrors = true;

        #endregion

        #region Internal logic

        System.Collections.IEnumerable CurrentEnumerator { get { return (_DataSource as System.Collections.IEnumerable); } }

        IBindingList CurrentIBindingList { get { return (_DataSource as IBindingList); } }

        public List<string> GetAddressesInFillOrder()
        {
            List<string> results = new List<string>();
            for (int i = 0; i < this.Capacity; i++)
                results.Add(this.Mapper.GetAddressFromOrdinal(i));
            return results;
        }

        void LayoutGrid(bool FillOrderChanged)
        {
            if (_Rows == 0 || _Columns == 0) { _Cells = null; return; }
            bool DimensionsChanged = false;
            if (_Cells == null || _Cells.GetLength(0) != _Columns || _Cells.GetLength(1) != _Rows)
            {
                DimensionsChanged = true;
                _Cells = new CartesianGridCell[_Columns, _Rows];
                _CellMapByAddress = new Dictionary<string, CartesianGridCell>();
                _CellMapByOrdinal = new Dictionary<int, CartesianGridCell>();
            }
            _Mapper.RowCount = _Rows;
            _Mapper.ColCount = _Columns;
            _Mapper.Order = this.ItemFillOrder;
            int ControlWidth = this.Width - 2 * _GridPadding;
            int TotalCaptionHeight = ((!String.IsNullOrEmpty(_Text)) ? this.CaptionHeight : 0)
               + ((!String.IsNullOrEmpty(ErrorText) && !HideErrors) ? this.ErrorCaptionHeight : 0);
            int ControlHeight = this.Height
                   - 2 * _GridPadding
                   - TotalCaptionHeight;
            int ElementWidth = ControlWidth / _Columns;
            int ElementHeight = ControlHeight / _Rows;
            int DrawingWidth = ElementWidth * _Columns;
            int DrawingHeight = ElementHeight * _Rows;
            int DrawingLeft = _GridPadding + (ControlWidth - DrawingWidth) / 2;
            int DrawingTop = TotalCaptionHeight + _GridPadding + (ControlHeight - DrawingHeight) / 2;

            for (int colIndex = 0; colIndex < _Columns; colIndex++)
                for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                {
                    CartesianGridCell rec = null;
                    if (DimensionsChanged)
                    {
                        rec = new CartesianGridCell(colIndex, rowIndex);
                        rec.Address = _Mapper.GetAddressFromCoordinates(colIndex, rowIndex);
                        rec.Ordinal = _Mapper.GetOrdinalFromCoordinates(colIndex, rowIndex);
                        _Cells[colIndex, rowIndex] = rec;
                        _CellMapByAddress[rec.Address] = rec;
                        _CellMapByOrdinal[rec.Ordinal] = rec;
                    }
                    else
                    {
                        rec = _Cells[colIndex, rowIndex];
                        if (FillOrderChanged)
                        {
                            //Ordinals change
                            rec.Ordinal = _Mapper.GetOrdinalFromCoordinates(colIndex, rowIndex);
                            _CellMapByOrdinal[rec.Ordinal] = rec;
                        }
                    }

                    Rectangle re = new Rectangle();
                    re.Y = DrawingTop + ElementHeight * rowIndex;
                    re.X = DrawingLeft + ElementWidth * colIndex;
                    re.Height = ElementHeight;
                    re.Width = ElementWidth;
                    rec.Rectangle = re;
                }

            this.Invalidate();
        }

        void Redraw()
        {
            this.LayoutGrid(false);
            this.Invalidate();
        }

        Rectangle GetAddressRect(Rectangle cellRectangle)
        {
            int height = cellRectangle.Height / 4;
            int width = cellRectangle.Width / 4;
            return new Rectangle(cellRectangle.X, cellRectangle.Y, width, height);
        }

        Rectangle GetLabelRect(Rectangle cellRectangle)
        {
            int height = Math.Max(cellRectangle.Height - 2 * _CellPadding, 1);
            int width = Math.Max(cellRectangle.Width - 2 * _CellPadding, 1);
            return new Rectangle(cellRectangle.X, cellRectangle.Y, width, height);
        }

        //Font GetFittingFont(Rectangle testRect, string testLabel, StringFormat format = null, string fontFamilyName = "Arial")
        //{
        //    int size = 2;
        //    int size_step = 2;
        //    using (Graphics g = this.CreateGraphics())
        //    {
        //        do
        //        {
        //            Font font = new Font(fontFamilyName, size, FontStyle.Regular, GraphicsUnit.Pixel);
        //            SizeF stringSize;
        //            if (format != null)
        //                stringSize = g.MeasureString(testLabel, font, testRect.Width, format);
        //            else
        //                stringSize = g.MeasureString(testLabel, font);
        //            font.Dispose();
        //            if (stringSize.Height > testRect.Height || stringSize.Width > testRect.Width) break;
        //            size += size_step;
        //        } while (true);
        //        g.Dispose();
        //        int final_size = size - size_step;
        //        if (final_size < 1) final_size = 1;
        //        return new Font(fontFamilyName, final_size, FontStyle.Regular, GraphicsUnit.Pixel);
        //    }
        //}



        protected List<string> CaptionOverlayOverrides { set; get; }

        void ClearItems(bool ClearObjectMap)
        {
            //  Debug.WriteLine("Clear items called:" + ClearObjectMap.ToString());
            for (int colIndex = 0; colIndex < _Columns; colIndex++)
                for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                    _Cells[colIndex, rowIndex].Item = null;
            if (ClearObjectMap)
                _CellMapByObject.Clear();

        }

        void LayoutItems()
        {
            lock (_DrawLock)
            {
                ClearItems(false);
                if (CurrentEnumerator == null) return;
                foreach (object boundItem in CurrentEnumerator)
                {
                    Point coordinates = Point.Empty;
                    if (_CellMapByObject.ContainsKey(boundItem))
                        _CellMapByObject[boundItem].Item = boundItem;
                    else
                    {
                        CartesianGridCell availableCell = this.GetFirstEmptyCell();
                        if (availableCell == null)
                            HandleError("No room to fit item " + boundItem.ToString());
                        else
                        {
                            availableCell.Item = boundItem;
                            _CellMapByObject[boundItem] = availableCell;
                            IGridCellInformation info = boundItem as IGridCellInformation;
                            if (info != null)
                                info.CurrentCartesianAddress = availableCell.Address;

                        }
                    }
                }
            }
            Redraw();
        }

        CartesianGridCell GetCellFromLocation(Point point)
        {
            Point? coordinates = GetGridCoordinatesFromLocation(point, false, false);
            if (coordinates == null) return null;
            return _Cells[coordinates.Value.X, coordinates.Value.Y];
        }

        Point? GetGridCoordinatesFromLocation(Point point, bool NeedsClientConversion = true, bool AllowExtensionPastGrid = false)
        {
            Point localPoint;

            if (NeedsClientConversion)
                localPoint = this.PointToClient(point);
            else
                localPoint = point;
            point = new Point(-1, -1);
            float height = _Cells[0, 0].Rectangle.Height;
            float width = _Cells[0, 0].Rectangle.Width;


            if (AllowExtensionPastGrid)
            {
                if (localPoint.Y <= _Cells[0, 0].Rectangle.Y && localPoint.X <= _Cells[0, 0].Rectangle.X)
                    return new Point(0, 0);

                if (localPoint.Y > _Cells[_Columns - 1, _Rows - 1].Rectangle.Y && localPoint.X > _Cells[_Columns - 1, _Rows - 1].Rectangle.X)
                    return new Point(_Columns - 1, _Rows - 1);
            }


            if ((!AllowExtensionPastGrid && localPoint.Y <= _Cells[0, 0].Rectangle.Y) || height == 0 || width == 0)
                return null;
            int left = _Cells[0, 0].Rectangle.Left;
            int top = _Cells[0, 0].Rectangle.Top;
            int row = (int)Math.Truncate((localPoint.Y - top) / height);
            int col = (int)Math.Truncate((localPoint.X - left) / width);

            if (AllowExtensionPastGrid)
            {
                if (row > _Rows - 1)
                    row = _Rows - 1;
                if (col > _Columns - 1)
                    col = _Columns - 1;
                if (row < 0)
                    row = 0;
                if (col < 0)
                    col = 0;
            }


            if (row > _Rows - 1 || col > _Columns - 1 || row < 0 || col < 0)
                return null;
            point.X = col;
            point.Y = row;
            return point;
        }

        Rectangle getMouseMovedRectangle()
        {
            return new Rectangle(
                Math.Min(_mouseDownStartPosition.X, _mouseCurrentPosition.X),
                Math.Min(_mouseDownStartPosition.Y, _mouseCurrentPosition.Y),
                Math.Abs(_mouseDownStartPosition.X - _mouseCurrentPosition.X),
                Math.Abs(_mouseDownStartPosition.Y - _mouseCurrentPosition.Y));
        }

        void ClearSelectionInternal(bool Invalidate)
        {
            for (int colIndex = 0; colIndex < _Columns; colIndex++)
                for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                    _Cells[colIndex, rowIndex].Selected = false;
            if (Invalidate)
                this.Invalidate();
        }

        Rectangle GetSelectedSpanningRectangle()

        {

            int top = -1;
            int left = -1;
            int right = -1;
            int bottom = -1;
            for (int colIndex = 0; colIndex < _Columns; colIndex++)
                for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                    if (_Cells[colIndex, rowIndex].Selected)
                    {
                        if (colIndex < left || left == -1)
                            left = colIndex;
                        if (rowIndex < top || top == -1)
                            top = rowIndex;
                        if (right == -1 || colIndex > right)
                            right = colIndex;
                        if (bottom == -1 || rowIndex > bottom)
                            bottom = rowIndex;
                    }
            return new Rectangle(left, top, right - left, bottom - top);

        }

        void SelectSpannedCells(Rectangle r)

        {
            bool Extended = (ModifierKeys == Keys.Shift);
            var startCell = this.GetGridCoordinatesFromLocation(new Point(r.Left, r.Top), false, true);
            var endCell = this.GetGridCoordinatesFromLocation(new Point(r.Right, r.Bottom), false, true);
            if (startCell == null || endCell == null) return;
            for (int colIndex = startCell.Value.X; colIndex <= endCell.Value.X; colIndex++)
                for (int rowIndex = startCell.Value.Y; rowIndex <= endCell.Value.Y; rowIndex++)
                {
                    if (_Cells[colIndex, rowIndex].Item != null)
                        _Cells[colIndex, rowIndex].Selected = (Extended) ? !_Cells[colIndex, rowIndex].Selected : true;
                }
        }

        bool IsDropTargetValid(Point currentPositionLocal)
        {
            Point? destinationPoint = this.GetGridCoordinatesFromLocation(currentPositionLocal, false, false);
            if (destinationPoint == null) return false;
            Point destination = destinationPoint.Value;
            Point source = _cellMouseClicked.Coordinates;
            int xOffset = destination.X - source.X;
            int yOffset = destination.Y - source.Y;

            for (int colIndex = _DragSelectedCellBounds.X; colIndex <= _DragSelectedCellBounds.X + _DragSelectedCellBounds.Width; colIndex++)
                for (int rowIndex = _DragSelectedCellBounds.Y; rowIndex <= _DragSelectedCellBounds.Y + _DragSelectedCellBounds.Height; rowIndex++)
                    if (_Cells[colIndex, rowIndex].Selected && _Cells[colIndex, rowIndex].Item != null)
                    {
                        int xTarget = colIndex + xOffset;
                        int yTarget = rowIndex + yOffset;
                        if (xTarget > _Columns - 1 || xTarget < 0) { Debug.WriteLine("Off grid horiz"); return false; }
                        if (yTarget > _Rows - 1 || yTarget < 0) { Debug.WriteLine("Off grid vert"); return false; }
                        if (_Cells[xTarget, yTarget].Item != null && !_Cells[xTarget, yTarget].Selected) { Debug.WriteLine("Non empty cell:" + _Cells[colIndex, rowIndex].Address.ToString() + " to " + _Cells[xTarget, yTarget].Address.ToString()); return false; }
                    }
            return true;
        }

        void MoveSelectedCellsTo(Point currentPositionLocal)
        {

            Point? destinationPoint = this.GetGridCoordinatesFromLocation(currentPositionLocal, false, false);
            if (destinationPoint == null) throw new ApplicationException("Unexpected request to drop onto invalid target position");
            Point destination = destinationPoint.Value;
            Point source = _cellMouseClicked.Coordinates;
            int xOffset = destination.X - source.X;
            int yOffset = destination.Y - source.Y;
            CartesianGridCell sourceCell = null;
            CartesianGridCell targetCell = null;
            Dictionary<object, string> moveMap = new Dictionary<object, string>();

            for (int colIndex = _DragSelectedCellBounds.X; colIndex <= _DragSelectedCellBounds.X + _DragSelectedCellBounds.Width; colIndex++)
                for (int rowIndex = _DragSelectedCellBounds.Y; rowIndex <= _DragSelectedCellBounds.Y + _DragSelectedCellBounds.Height; rowIndex++)
                {
                    sourceCell = _Cells[colIndex, rowIndex];
                    if (sourceCell.Selected && sourceCell.Item != null)
                    {
                        int xTarget = colIndex + xOffset;
                        int yTarget = rowIndex + yOffset;
                        if (xTarget > _Columns - 1 || xTarget < 0) { throw new ApplicationException("Invalid drop request"); }
                        if (yTarget > _Rows - 1 || yTarget < 0) { throw new ApplicationException("Invalid drop request"); ; }
                        targetCell = _Cells[xTarget, yTarget];
                        if (targetCell.Item != null && !targetCell.Selected) { throw new ApplicationException("Invalid drop request"); }
                        _CellMapByObject[sourceCell.Item] = targetCell;
                        IGridCellInformation info = sourceCell.Item as IGridCellInformation;
                        if (info != null)
                            info.CurrentCartesianAddress = targetCell.Address;
                    }
                }
            LayoutItems();
            this.Invalidate();

        }

        void ClearDropTargetHighlights()
        {
            for (int i = 0; i < Capacity; i++)
                _CellMapByOrdinal[i].DropTargetHighlighted = false;

        }

        bool IsDropTargetValidHighlight(Point currentPositionLocal)
        {

            Point? destinationPoint = this.GetGridCoordinatesFromLocation(currentPositionLocal, false, false);
            if (destinationPoint == null) { Debug.WriteLine("Invalid destination point"); return false; }
            Point destination = destinationPoint.Value;
            Point source = _cellMouseClicked.Coordinates;
            int xOffset = destination.X - source.X;
            int yOffset = destination.Y - source.Y;
            CartesianGridCell target = null;
            ClearDropTargetHighlights();
            List<CartesianGridCell> targetCells = new List<CartesianGridCell>();

            for (int colIndex = _DragSelectedCellBounds.X; colIndex <= _DragSelectedCellBounds.X + _DragSelectedCellBounds.Width; colIndex++)
                for (int rowIndex = _DragSelectedCellBounds.Y; rowIndex <= _DragSelectedCellBounds.Y + _DragSelectedCellBounds.Height; rowIndex++)

                {
                    if (_Cells[colIndex, rowIndex].Selected && _Cells[colIndex, rowIndex].Item != null)
                    {
                        int xTarget = colIndex + xOffset;
                        int yTarget = rowIndex + yOffset;
                        if (xTarget > _Columns - 1 || xTarget < 0) { Debug.WriteLine("Off grid horiz"); return false; }
                        if (yTarget > _Rows - 1 || yTarget < 0) { Debug.WriteLine("Off grid vert"); return false; }
                        target = _Cells[xTarget, yTarget];
                        if (target.Item != null && !target.Selected) { Debug.WriteLine("Non empty cell:" + _Cells[colIndex, rowIndex].Address.ToString() + " to " + target.Address.ToString()); return false; }
                        //  Debug.WriteLine("Valid mapping:" + _Cells[colIndex, rowIndex].Address.ToString() + " to " + target.Address.ToString());
                        //  _Targets[target] = target.OverlayColor;
                        targetCells.Add(target);
                    }

                }
            //  foreach (var cell in _Targets.Keys)
            //   cell.OverlayColor = this.CutCellsColor;
            //     _SavedDroppedTargets = _Targets;
            foreach (var cell in targetCells)
                cell.DropTargetHighlighted = true;
            return true;
        }

        void HandleError(string message)
        {

            if (ThrowExceptionsOnErrors)
                throw new Exception(message);
            else
            {
                this.ErrorText = message;
                Redraw();
            }

        }

        CartesianGridCell GetFirstEmptyCell()

        {
            if (this.FillMode == Mapping.AutoFillMode.Repack)
            {
                for (int index = 0; index < Capacity; index++)
                    if (_CellMapByOrdinal[index].Item == null)
                        return _CellMapByOrdinal[index];
                return null;
            }
            else

            {
                CartesianGridCell lastFilledCell = GetLastFilledCell();
                if (lastFilledCell == null) return _CellMapByOrdinal[0]; //grid empty
                if (lastFilledCell.Ordinal == Capacity - 1) return null; //grid full
                return _CellMapByOrdinal[lastFilledCell.Ordinal + 1];
            }
        }

        CartesianGridCell GetLastFilledCell()
        {
            CartesianGridCell cell = null;
            for (int index = 0; index < Capacity; index++)
                if (_CellMapByOrdinal[index].Item != null)
                    cell = _CellMapByOrdinal[index];
            return cell;
        }

        int? GetIndexOfBoundObject(object findMe)
        {


            int index = 0;
            bool found = false;
            foreach (var obj in CurrentEnumerator)

            {
                if (obj.Equals(findMe))
                {
                    found = true;
                    break;
                }
                index++;
            }

            return (found) ? (int?)index : null;
        }

        void Clear(bool PreserveCaption = false)
        {
            ClearItems(true);
            ClearAllOverlays();
            ClearSelection();
            //    _Items = null;
            if (!PreserveCaption)
                this.Text = null;
            Redraw();
        }

        #endregion

        #region Public Properties

        public Mapping.AutoFillMode FillMode { get; set; } = Mapping.AutoFillMode.Preserve;

        public bool HideErrors { get; set; } = false;

        public bool AllowReordering { get; set; } = true;
        public Color CutCellsColor { get; set; } = Color.BurlyWood;

        public int CaptionHeight
        {
            get { return _CaptionHeight; }
            set
            {
                if (_CaptionHeight != value)
                {
                    _CaptionHeight = value;
                    Redraw();
                }
            }
        }

        public int ErrorCaptionHeight
        {
            get { return _ErrorCaptionHeight; }
            set { _ErrorCaptionHeight = value; }
        }

        public ICartesianMapper Mapper
        {
            get { return _Mapper; }
            set { _Mapper = value; }
        }

        public bool ThrowExceptionsOnErrors
        {
            get { return _ThrowExceptionsOnErrors; }
            set { _ThrowExceptionsOnErrors = value; }
        }

        public int SelectedCellBorderWidth
        {
            get { return _SelectedCellBorderWidth; }
            set
            {
                _SelectedCellBorderWidth = value;
                _SelectedCellBorderHalfWidth = value / 2;
                _SelectedCellBorderMiterHalfWidth = Convert.ToInt32(Math.Truncate(0.5 * value / Math.Sqrt(2)));
                _SelectedCellBorderMiterWidth = _SelectedCellBorderMiterHalfWidth * 2;
            }
        }

        public Color SelectedCellBorderColor
        {
            get { return _SelectedCellBorderColor; }
            set { _SelectedCellBorderColor = value; }
        }

        public SelectionMode CellSelectionMode
        {
            get { return _CellSelectionMode; }
            set
            {
                if (_CellSelectionMode != value)
                {
                    _CellSelectionMode = value;
                    ClearSelectionInternal(true);
                }

            }
        }

        public int GridLineWidth
        {
            get { return _GridLineWidth; }
            set { _GridLineWidth = value; Redraw(); }
        }


        public int CellHoverDelay { get; set; } = 500;

        public string ErrorText { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        public override string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                this.Redraw();
            }
        }

        public PropertyViewer PropertyViewer { get { return this._PPV.PropertyViewer; } }

        public int Capacity { get { return _Rows * _Columns; } }

        public new int Padding { get { return _GridPadding; } set { if (_GridPadding != value) { _GridPadding = value; Redraw(); } } }

        [ToolboxItem(false)]
        public class GridDimensions : Component
        {
            public int Rows { get; set; } = 8;
            public int Columns { get; set; } = 12;
        }


        public Size Dimensions
        {

            get { return new Size(_Columns, _Rows); }
            set { _Rows = value.Height; _Columns = value.Width; LayoutGrid(false); }

        }

        public Type DataBoundItemTypeRestriction

        {
            get { return _DataBoundItemTypeRestriction; }

            set
            {
                if (_DataBoundItemTypeRestriction == value) return;
                if (_DataBoundItemTypeRestriction != null && _DataSource != null)
                {
                    HandleError("Cannot set a DataBoundItemTypeRestriction with a non-null DataSource.  Unbind Grid and retry.");
                    return;
                }
                _DataBoundItemTypeRestriction = value;
            }
        }

        public string GetAddressFromOrdinal(int ordinal)
        {
            return this._Mapper.GetAddressFromOrdinal(ordinal);
        }

        public bool HasFillOrderViolation
        {
            get
            {
                //If we are in automatic fill mode, it should not be possible to have a sequence violation
                //   if (this.ItemFillMode == Mapping.FillMode.UseCartesianMapper) return false;

                //iterate through all bound items in order - any gap should give a violation
                bool FirstEmptyFound = false;
                for (int i = 0; i < Capacity; i++)
                {
                    if (_CellMapByOrdinal[i].Item != null && FirstEmptyFound)
                        return true;
                    if (_CellMapByOrdinal[i].Item == null)
                        FirstEmptyFound = true;
                }
                return false;
            }
        }

        public Mapping.FillOrder ItemFillOrder
        {

            get { return _Order; }  
            set
            {
                if (_Mapper != null)
                    _Mapper.Order = value;

                if (value != _Order)
                {
                    _Order = value;
                    LayoutGrid(true);
                }


            }
        }

        #endregion

        #region Public Methods

        public virtual object DataSource
        {
            get { return _DataSource; }

            set
            {
                lock (_BindingLock)
                {
                    if (value == _DataSource)
                        return;

                    if (_DataSource != null && _DataSource as IBindingList != null)
                        (_DataSource as IBindingList).ListChanged -= ListChanged;

                    if (value == null)
                    {
                        _DataSource = null;
                        this.Clear(false);
                        return;
                    }

                    _DataSource = value;

                    ClearItems(true);
                    ClearAllOverlays();
                    ClearSelection();

                    if (CurrentEnumerator == null || _DataSource is System.String)
                    {
                        ErrorText = "DataSource must implement IEnumerable interface on a class collection";
                        Redraw();
                        return;
                    }

                    string warning = "";

                    if (CurrentIBindingList == null)
                        warning += " DataSource does not support IBindingList. ";

                    var ObjectCache = new List<object>();
                    bool AllObjectsSupportCellBinding = true;
                    bool AllObjectsSupportINotifyPropertyChanged = true;
                    bool AllObjectsAreAssignableToBoundType = true;
                    foreach (object o in CurrentEnumerator)
                    {
                        if (ObjectCache.Contains(o))
                            throw new Exception("Duplicate object binding not allowed.  Use shallow copies if you need to bind replicates.");
                        ObjectCache.Add(o);
                        if (!(o is IGridCellInformation))
                            AllObjectsSupportCellBinding = false;
                        else
                        {
                            if (!String.IsNullOrEmpty((o as IGridCellInformation).CurrentCartesianAddress))
                                this.SetObjectAddress(o, (o as IGridCellInformation).CurrentCartesianAddress, true);

                        }
                        if (!(o is INotifyPropertyChanged))
                            AllObjectsSupportINotifyPropertyChanged = false;
                        if (_DataBoundItemTypeRestriction != null && !_DataBoundItemTypeRestriction.IsAssignableFrom(o.GetType()))
                            AllObjectsAreAssignableToBoundType = false;
                    }

                    if (!AllObjectsAreAssignableToBoundType)
                        warning += " Bound type violation, object(s) not assignable to " + _DataBoundItemTypeRestriction.ToString() + ". ";

                    if (!AllObjectsSupportCellBinding)
                        warning += " Some bound objects do not implement ICartesianGridCellBoundItem. ";

                    if (!AllObjectsSupportINotifyPropertyChanged)
                        warning += " Some bound objects do not implement INotifyPropertyChanged. ";

                    if (warning != "")
                        ErrorText = " Warning: Functionality will be limited. " + warning;
                    else
                        ErrorText = null;

                    var iBindingList = value as IBindingList;
                    if (iBindingList != null)
                        iBindingList.ListChanged += ListChanged;

                    LayoutItems();
                }
            }
        }

        public void SetOverlay(string Address, Color overlayColor, string overlayCaption)
        {
            Point address = _Mapper.GetCoordinatesFromAddress(Address);
            _Cells[address.X, address.Y].OverlayCaption = overlayCaption;
            _Cells[address.X, address.Y].OverlayColor = overlayColor;
            Redraw();
        }

        public void ClearSelection()
        {
            ClearSelectionInternal(true);
        }

        public void HighlightFillOrderViolations()
        {
            ClearAllOverlays();
            bool FirstEmptyFound = false;
            bool ViolationFound = false;
            for (int i = 0; i < Capacity; i++)
            {

                if (_CellMapByOrdinal[i].Item != null && FirstEmptyFound)
                {
                    _CellMapByOrdinal[i].OverlayColor = Color.Red;
                    _CellMapByOrdinal[i].OverlayCaption = "FILL VIOLATION";
                    ViolationFound = true;
                }

                if (_CellMapByOrdinal[i].Item == null)
                    FirstEmptyFound = true;
            }
            if (ViolationFound)
                Redraw();
        }

        public void ClearOverlay(string Address)
        {
            SetOverlay(Address, Color.Transparent, "");
        }

        public void ClearAllOverlays()
        {
            for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                for (int colIndex = 0; colIndex < _Columns; colIndex++)
                {
                    CartesianGridCell re = _Cells[colIndex, rowIndex];
                    re.OverlayColor = Color.Transparent;
                    re.OverlayCaption = "";
                }
            Redraw();
        }

        public List<object> GetOrderedItemList()
        {
            List<object> items = new List<object>();
            for (int index = 0; index < Capacity; index++)
                if (_CellMapByOrdinal[index].Item != null)
                    items.Add(_CellMapByOrdinal[index].Item);
            return items;
        }

        public int? GetIndexOfLastSample(Mapping.FillOrder fillOrder)

        {
            List<object> items = GetOrderedItemList();
            if (items.Count == 0) return null;
            object findMe = items[items.Count - 1];
            return GetIndexOfBoundObject(findMe);
        }

        public int? GetIndexOfFirstSample(Mapping.FillOrder fillOrder)

        {
            List<object> items = GetOrderedItemList();
            if (items.Count == 0) return null;
            object findMe = items[0];
            return GetIndexOfBoundObject(findMe);
        }

        public object GetLastItem()
        {
            List<object> items = GetOrderedItemList();
            if (items.Count == 0) return null;
            return items[items.Count - 1];
        }

        public Dictionary<string, object> GetItemDictionary()
        {

            Dictionary<string, object> items = new Dictionary<string, object>();
            for (int colIndex = 0; colIndex < _Columns; colIndex++)
                for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                {
                    CartesianGridCell re = _Cells[colIndex, rowIndex];
                    if (re.Item != null) items[re.Address] = re.Item;
                }

            return items;
        }

        public string GetObjectAddress(object boundItem)
        {
            return (_CellMapByObject == null || !_CellMapByObject.ContainsKey(boundItem)) ? null : _CellMapByObject[boundItem].Address;
        }


        public void SetObjectAddress(object boundItem, string address, bool DeferValidationAndRedraw = false)

        {
            if (!_CellMapByObject.ContainsKey(boundItem) && !DeferValidationAndRedraw)
                throw new Exception("Object " + boundItem.ToString() + " is not bound to the grid.  Cannot set address.");

            //this normalizes address format (e.g. A1 versus A02), if needed
            address = _Mapper.GetAddressFromOrdinal(_Mapper.GetOrdinalFromAddress(address));

            if (_CellMapByObject.ContainsKey(boundItem) &&
                _CellMapByObject[boundItem] != null &&
                _CellMapByObject[boundItem].Address == address) //nothing to do
                return;

            //TODO better collision options
            if (!DeferValidationAndRedraw)
                if (_CellMapByAddress.ContainsKey(address) && _CellMapByAddress[address].Item != null && _CellMapByAddress[address].Item != boundItem)
                    HandleError("Object collision - more than one object set to address " + address);

            if (_Mapper.IsValidAddess(address)) throw new Exception("Invalid address for grid specified" + address);
            CartesianGridCell newCell = _CellMapByAddress[address];
            newCell.Item = boundItem;
            _CellMapByObject[boundItem] = newCell;

            IGridCellInformation info = boundItem as IGridCellInformation;
            if (info != null)
                info.CurrentCartesianAddress = address;


            ClearSelectionInternal(false);

            if (!DeferValidationAndRedraw)
            {
                ClearAllOverlays();
                LayoutItems();
                Redraw();
            }
        }

        public override void Refresh()
        {

            LayoutItems();
            Redraw();

        }

        public void Repack()
        {
            if (this.CurrentEnumerator == null)
                return;
            ClearItems(true);
            int index = 0;
            foreach (var item in this.CurrentEnumerator)
            {
                CartesianGridCell cell = _CellMapByOrdinal[index];
                cell.Item = item;
                _CellMapByObject[item] = cell;
                index++;
            }
            LayoutItems();
            Redraw();

        }

        #endregion

        #region Events and Delegates


        public delegate void CellEnteredEventHandler(CartesianGridCell cell);
        public event CellEnteredEventHandler CellEntered;

        public delegate void CellLeftEventHandler(CartesianGridCell cell);
        public event CellLeftEventHandler CellLeft;

        public delegate void CellHoverEventHandler(CartesianGridCell cell);
        public event CellHoverEventHandler CellHover;

        void ListChanged(object sender, ListChangedEventArgs e)
        {
            //   System.Diagnostics.Debug.WriteLine(e.ListChangedType.ToString());

            ///    if (e.PropertyDescriptor != null)
            //         Debug.WriteLine("Property: " + e.PropertyDescriptor.ToString());

            switch (e.ListChangedType)
            {
                case ListChangedType.Reset:
                    Debug.WriteLine("ListChanged:Reset");
                    ClearAllOverlays();
                    ClearSelection();
                    LayoutItems();
                    break;
                case ListChangedType.ItemAdded:
                    Debug.WriteLine("ListChanged:ItemAdded");
                    ClearAllOverlays();
                    ClearSelection();
                    LayoutItems();
                    break;
                case ListChangedType.ItemDeleted:
                    Debug.WriteLine("ListChanged:ItemDeleted");
                    ClearAllOverlays();
                    ClearSelection();
                    LayoutItems();
                    break;
                case ListChangedType.ItemMoved:
                    Debug.WriteLine("ListChanged:ItemMoved");
                    ClearAllOverlays();
                    ClearSelection();
                    LayoutItems();
                    break;
                case ListChangedType.ItemChanged:
                    //TODO optimize
                    //Formally - CellAddress changes require both reordering and redrawing, but other property changes only require a redraw
                    //So this could be made more efficient, but for now we will just Reorder regardless
                    // Redraw();
                    //  Debug.WriteLine("ListChanged:ItemChanged");
                    LayoutItems();
                    break;
                case ListChangedType.PropertyDescriptorAdded:
                    break;
                case ListChangedType.PropertyDescriptorDeleted:
                    break;
                case ListChangedType.PropertyDescriptorChanged:
                    break;
                default:
                    break;
            }

        }

        Color GetGridBackColor(CartesianGridCell cell)
        {
            var boundItem = cell.Item as IGridCellInformation;
            if (boundItem != null && boundItem.GridCellBackColor.HasValue)
                return boundItem.GridCellBackColor.Value;
            else
            {
                var boundItem2 = cell.Item;
                if (boundItem2 != null)
                {
                    Type objType = boundItem2.GetType();
                    PropertyInfo myPropInfo = objType.GetProperty("BackColor");
                    if (myPropInfo != null)
                        return (System.Drawing.Color)myPropInfo.GetValue(boundItem2, null);
                }
            }

            return Color.Transparent;
        }

        double GetDistanceBetweenPoints(Point A, Point B)

        {

            double dX = A.X - B.X;
            double dY = A.Y - B.Y;
            double multi = dX * dX + dY * dY;
            return Math.Sqrt(multi);

        }


        protected override void OnPaint(PaintEventArgs pe)
        {

            lock (_DrawLock)
            {
                if (_Rows == 0 || _Columns == 0) return;
                float highlightBorderPenWidth = 2;

                using (Pen normalBorderPen = new Pen(this.ForeColor, (float)this.GridLineWidth))
                using (Pen highlightedBorderPen = new Pen(this.ForeColor, highlightBorderPenWidth))
                using (Brush fontBrush = new SolidBrush(this.ForeColor))
                using (Brush confirmedBrush = new SolidBrush(Color.LightGreen))
                using (Brush invalidBrush = new SolidBrush(Color.Red))
                using (Brush eraseBrush = new SolidBrush(this.BackColor))
                using (Pen erasePen = new Pen(this.BackColor, _SelectedCellBorderWidth))
                using (Brush selectedBrush = new SolidBrush(this.SelectedCellBorderColor))
                using (Brush selectedBrush2 = new HatchBrush(HatchStyle.Percent50, Color.White, this.SelectedCellBorderColor))
                {
                    erasePen.StartCap = LineCap.Flat;
                    erasePen.EndCap = LineCap.Flat;
                    Font labelFont = null;
                    pe.Graphics.Clear(this.BackColor);
                    //   erasePen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                    if (_Rows == 0 || _Columns == 0) return;
                    CartesianGridCell cell;
                    List<string> overlays = new List<string>();
                    overlays.Add("min");
                    for (int colIndex = 0; colIndex < _Columns; colIndex++)
                        for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                        {
                            cell = _Cells[colIndex, rowIndex];
                            if (cell.OverlayCaption != null & cell.OverlayCaption != "")
                                overlays.Add(cell.OverlayCaption);
                        }

                    StringFormat centeredFormat = new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap
                    };

                    cell = _Cells[0, 0];

                    if (ErrorText != null && ErrorText != "" && !HideErrors)
                    {
                        Rectangle captionRectangle = new Rectangle(0, 0, this.Width, this.ErrorCaptionHeight);
                        Font captionFontNew = FontHelper.GetFittingFont(this, captionRectangle, ErrorText, null, "Consolas");
                        pe.Graphics.DrawString(ErrorText, captionFontNew, invalidBrush, captionRectangle, centeredFormat);
                        captionFontNew.Dispose();
                    }

                    if (_Text != null && _Text != "")
                    {
                        Rectangle captionRectangle = new Rectangle(0, (this.ErrorText != null && this.ErrorText != "" && !HideErrors) ? this.ErrorCaptionHeight : 0, this.Width, this.CaptionHeight);
                        Font captionFontNew = FontHelper.GetFittingFont(this, captionRectangle, _Text, null, "Consolas");
                        pe.Graphics.DrawString(_Text, captionFontNew, fontBrush, captionRectangle, centeredFormat);
                        captionFontNew.Dispose();
                    }

                    Font addressFont = FontHelper.GetFittingFont(this, GetAddressRect(_Cells[0, 0].Rectangle), _Cells[0, 0].Address, centeredFormat, this.Font.Name);

                    for (int colIndex = 0; colIndex < _Columns; colIndex++)
                        for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                        {
                            cell = _Cells[colIndex, rowIndex];
                            Color c = GetGridBackColor(cell);
                            if (c != null)
                                using (Brush backcolorBrush = new SolidBrush(c))
                                    pe.Graphics.FillRectangle(backcolorBrush, cell.Rectangle);
                        }

                    //    Debug.WriteLine("Back color rectangles painted " + backColorCount.ToString());

                    Dictionary<object, string> CellLabels = new Dictionary<object, string>();

                    List<string> AllLabels = new List<string>();

                    if (CurrentEnumerator != null)
                        foreach (object boundItem in CurrentEnumerator)
                        {
                            var gridItem = boundItem as IGridCellInformation;
                            if (gridItem != null)
                                CellLabels[boundItem] = gridItem.GridCellLabel;
                            else
                                CellLabels[boundItem] = boundItem.ToString();
                        }

                    cell = _Cells[0, 0];
                    labelFont = FontHelper.GetFittingFont(this, GetLabelRect(cell.Rectangle), _CellPadding, new List<string>(CellLabels.Values), centeredFormat, "Consolas");
                    for (int colIndex = 0; colIndex < _Columns; colIndex++)
                        for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                        {
                            cell = _Cells[colIndex, rowIndex];
                            pe.Graphics.DrawRectangle(normalBorderPen, cell.Rectangle);
                            pe.Graphics.DrawString(cell.Address, addressFont, fontBrush, cell.AddressPosition);
                            if (cell.Item != null && CellLabels.ContainsKey(cell.Item))
                                pe.Graphics.DrawString(CellLabels[cell.Item], labelFont, fontBrush, cell.LabelRectangle, centeredFormat);
                        }

                    for (int colIndex = 0; colIndex < _Columns; colIndex++)
                        for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                        {
                            cell = _Cells[colIndex, rowIndex];
                            pe.Graphics.DrawRectangle((cell.BorderHighlighted) ? highlightedBorderPen : normalBorderPen, cell.Rectangle);
                        }


                    //draw selection, accounting for drag phase
                    if (_inDragPhase)
                    {
                        for (int colIndex2 = 0; colIndex2 < _Columns; colIndex2++)
                            for (int rowIndex2 = 0; rowIndex2 < _Rows; rowIndex2++)
                                if (_Cells[colIndex2, rowIndex2].Selected)
                                {
                                    CartesianGridCell cellToErase = _Cells[colIndex2, rowIndex2];
                                    pe.Graphics.FillRectangle(eraseBrush, cellToErase.Rectangle);
                                    pe.Graphics.DrawRectangle(normalBorderPen, cellToErase.Rectangle);
                                    pe.Graphics.DrawString(cellToErase.Address, addressFont, fontBrush, cellToErase.AddressPosition);
                                }
                    }
                    else
                        for (int colIndex = 0; colIndex < _Columns; colIndex++)
                            for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                            {
                                cell = _Cells[colIndex, rowIndex];
                                if (cell.Selected)
                                {
                                    bool needsTopBorder = (rowIndex == 0 || !_Cells[colIndex, rowIndex - 1].Selected);
                                    bool needsRightBorder = (colIndex == _Columns - 1 || !_Cells[colIndex + 1, rowIndex].Selected);
                                    bool needsBottomBorder = (rowIndex == _Rows - 1 || !_Cells[colIndex, rowIndex + 1].Selected);
                                    bool needsLeftBorder = (colIndex == 0 || !_Cells[colIndex - 1, rowIndex].Selected);
                                    bool needsTopLeftCutout = (colIndex > 0 && rowIndex > 0 && needsLeftBorder && needsTopBorder && _Cells[colIndex - 1, rowIndex - 1].Selected);
                                    bool needsBottomLeftCutout = (colIndex > 0 && rowIndex < _Rows - 1 && needsLeftBorder && needsBottomBorder && _Cells[colIndex - 1, rowIndex + 1].Selected);
                                    Point topBorderStart = new Point(cell.Rectangle.Left, cell.Rectangle.Top);
                                    Point topBorderEnd = new Point(cell.Rectangle.Right, cell.Rectangle.Top);
                                    Point rightBorderStart = new Point(cell.Rectangle.Right, cell.Rectangle.Top);
                                    Point rightBorderEnd = new Point(cell.Rectangle.Right, cell.Rectangle.Bottom);
                                    Point bottomBorderStart = new Point(cell.Rectangle.Right, cell.Rectangle.Bottom);
                                    Point bottomBorderEnd = new Point(cell.Rectangle.Left, cell.Rectangle.Bottom);
                                    Point leftBorderStart = new Point(cell.Rectangle.Left, cell.Rectangle.Bottom);
                                    Point leftBorderEnd = new Point(cell.Rectangle.Left, cell.Rectangle.Top);
                                    Rectangle topBorderRect = new Rectangle(topBorderStart.X - _SelectedCellBorderHalfWidth, topBorderStart.Y - _SelectedCellBorderHalfWidth, topBorderEnd.X - topBorderStart.X + _SelectedCellBorderHalfWidth * 2, topBorderEnd.Y - topBorderStart.Y + _SelectedCellBorderHalfWidth * 2);
                                    Rectangle rightBorderRect = new Rectangle(rightBorderStart.X - _SelectedCellBorderHalfWidth, rightBorderStart.Y - _SelectedCellBorderHalfWidth, rightBorderEnd.X - rightBorderStart.X + _SelectedCellBorderHalfWidth * 2, rightBorderEnd.Y - rightBorderStart.Y + _SelectedCellBorderHalfWidth * 2);
                                    Rectangle bottomBorderRect = new Rectangle(bottomBorderEnd.X - _SelectedCellBorderHalfWidth, bottomBorderEnd.Y - _SelectedCellBorderHalfWidth, bottomBorderStart.X - bottomBorderEnd.X + _SelectedCellBorderHalfWidth * 2, bottomBorderStart.Y - bottomBorderEnd.Y + _SelectedCellBorderHalfWidth * 2);
                                    Rectangle leftBorderRect = new Rectangle(leftBorderEnd.X - _SelectedCellBorderHalfWidth, leftBorderEnd.Y - _SelectedCellBorderHalfWidth, leftBorderStart.X - leftBorderEnd.X + _SelectedCellBorderHalfWidth * 2, leftBorderStart.Y - leftBorderEnd.Y + _SelectedCellBorderHalfWidth * 2);

                                    if (needsTopBorder)
                                        pe.Graphics.FillRectangle(selectedBrush2, topBorderRect);
                                    if (needsRightBorder)
                                        pe.Graphics.FillRectangle(selectedBrush2, rightBorderRect);
                                    if (needsBottomBorder)
                                        pe.Graphics.FillRectangle(selectedBrush2, bottomBorderRect);
                                    if (needsLeftBorder)
                                        pe.Graphics.FillRectangle(selectedBrush2, leftBorderRect);

                                    if (needsTopLeftCutout)
                                    {
                                        Point ptTopLeft = new Point(cell.Rectangle.X - _SelectedCellBorderHalfWidth, cell.Rectangle.Y - _SelectedCellBorderHalfWidth);
                                        Point ptBottomRight = new Point(cell.Rectangle.X + _SelectedCellBorderHalfWidth, cell.Rectangle.Y + _SelectedCellBorderHalfWidth);
                                        Point ptLeft = new Point(ptTopLeft.X - _SelectedCellBorderMiterWidth, ptTopLeft.Y);
                                        Point ptTop = new Point(ptTopLeft.X, ptTopLeft.Y - _SelectedCellBorderMiterWidth);
                                        Point ptRight = new Point(ptBottomRight.X + _SelectedCellBorderMiterWidth, ptBottomRight.Y);
                                        Point ptBottom = new Point(ptBottomRight.X, ptBottomRight.Y + _SelectedCellBorderMiterWidth);
                                        CartesianGridCell partnerCell = _Cells[cell.ColIndex - 1, cell.RowIndex - 1];
                                        int _SelectedCellBorderMiterHalfWidth = _SelectedCellBorderMiterWidth / 2;
                                        Point ptLeftSub = new Point(ptLeft.X + _SelectedCellBorderMiterHalfWidth, ptLeft.Y + _SelectedCellBorderMiterHalfWidth);
                                        Point ptTopSub = new Point(ptTop.X + _SelectedCellBorderMiterHalfWidth, ptTop.Y + _SelectedCellBorderMiterHalfWidth);
                                        Point ptRightSub = new Point(ptRight.X - _SelectedCellBorderMiterHalfWidth, ptRight.Y - _SelectedCellBorderMiterHalfWidth);
                                        Point ptBottomSub = new Point(ptBottom.X - _SelectedCellBorderMiterHalfWidth, ptBottom.Y - _SelectedCellBorderMiterHalfWidth);
                                        Point ptGradientStart = new Point(ptLeftSub.X - 1, ptLeftSub.Y - 1);
                                        Point ptGradientFinish = new Point(ptBottomSub.X + 1, ptBottomSub.Y + 1);
                                        using (LinearGradientBrush linGrBrush = new LinearGradientBrush(ptGradientStart, ptGradientFinish, GetGridBackColor(partnerCell), GetGridBackColor(cell)))
                                            pe.Graphics.FillPolygon(linGrBrush, new Point[] { ptLeft, ptTopLeft, ptTop, ptRight, ptBottomRight, ptBottom });
                                        using (var FillBrush = new SolidBrush(GetGridBackColor(partnerCell)))
                                        {
                                            pe.Graphics.FillPolygon(FillBrush, new Point[] { ptTop, ptTopSub, ptTopLeft });
                                            pe.Graphics.FillPolygon(FillBrush, new Point[] { ptLeft, ptLeftSub, ptTopLeft });
                                        }
                                        using (var FillBrush = new SolidBrush(GetGridBackColor(cell)))
                                        {
                                            pe.Graphics.FillPolygon(FillBrush, new Point[] { ptRight, ptRightSub, ptBottomRight });
                                            pe.Graphics.FillPolygon(FillBrush, new Point[] { ptBottom, ptBottomSub, ptBottomRight });
                                        }
                                    }

                                    if (needsBottomLeftCutout)
                                    {
                                        int _SelectedCellBorderMiterHalfWidth = _SelectedCellBorderMiterWidth / 2;
                                        Point ptTopRight = new Point(cell.Rectangle.X + _SelectedCellBorderHalfWidth, cell.Rectangle.Y + cell.Rectangle.Height - _SelectedCellBorderHalfWidth);
                                        Point ptBottomLeft = new Point(cell.Rectangle.X - _SelectedCellBorderHalfWidth, cell.Rectangle.Y + cell.Rectangle.Height + _SelectedCellBorderHalfWidth);
                                        Point ptLeft = new Point(ptBottomLeft.X - _SelectedCellBorderMiterWidth, ptBottomLeft.Y);
                                        Point ptTop = new Point(ptTopRight.X, ptTopRight.Y - _SelectedCellBorderMiterWidth);
                                        Point ptRight = new Point(ptTopRight.X + _SelectedCellBorderMiterWidth, ptTopRight.Y);
                                        Point ptBottom = new Point(ptBottomLeft.X, ptBottomLeft.Y + _SelectedCellBorderMiterWidth);
                                        Point ptLeftSub = new Point(ptLeft.X + _SelectedCellBorderMiterHalfWidth, ptLeft.Y - _SelectedCellBorderMiterHalfWidth);
                                        Point ptTopSub = new Point(ptTop.X - _SelectedCellBorderMiterHalfWidth, ptTop.Y + _SelectedCellBorderMiterHalfWidth);
                                        Point ptRightSub = new Point(ptRight.X - _SelectedCellBorderMiterHalfWidth, ptRight.Y + _SelectedCellBorderMiterHalfWidth);
                                        Point ptBottomSub = new Point(ptBottom.X + _SelectedCellBorderMiterHalfWidth, ptBottom.Y - _SelectedCellBorderMiterHalfWidth);
                                        CartesianGridCell partnerCell = _Cells[cell.ColIndex - 1, cell.RowIndex + 1];
                                        Point ptGradientStart = new Point(ptLeftSub.X - 1, ptLeftSub.Y + 1);
                                        Point ptGradientFinish = new Point(ptTopSub.X + 1, ptTopSub.Y - 1);
                                        using (LinearGradientBrush linGrBrush = new LinearGradientBrush(ptGradientStart, ptGradientFinish, GetGridBackColor(partnerCell), GetGridBackColor(cell)))
                                            pe.Graphics.FillPolygon(linGrBrush, new Point[] { ptLeft, ptTop, ptTopRight, ptRight, ptBottom, ptBottomLeft });
                                        using (var FillBrush = new SolidBrush(GetGridBackColor(cell)))
                                        {
                                            pe.Graphics.FillPolygon(FillBrush, new Point[] { ptTop, ptTopSub, ptTopRight });
                                            pe.Graphics.FillPolygon(FillBrush, new Point[] { ptRight, ptRightSub, ptTopRight });
                                        }
                                        using (var FillBrush = new SolidBrush(GetGridBackColor(partnerCell)))
                                        {
                                            pe.Graphics.FillPolygon(FillBrush, new Point[] { ptLeft, ptLeftSub, ptBottomLeft });
                                            pe.Graphics.FillPolygon(FillBrush, new Point[] { ptBottom, ptBottomSub, ptBottomLeft });
                                        }
                                    }


                                }
                            }


                    Font overlayFont = null;
                    if (CaptionOverlayOverrides != null)
                        overlayFont = FontHelper.GetFittingFont(this, GetLabelRect(cell.Rectangle), _CellPadding, CaptionOverlayOverrides, centeredFormat, "Consolas");
                    else
                        overlayFont = FontHelper.GetFittingFont(this, GetLabelRect(cell.Rectangle), _CellPadding, overlays, centeredFormat, "Consolas");


                    for (int colIndex = 0; colIndex < _Columns; colIndex++)
                        for (int rowIndex = 0; rowIndex < _Rows; rowIndex++)
                        {
                            cell = _Cells[colIndex, rowIndex];
                            if (cell.OverlayColor != null && cell.OverlayColor.A != 0)
                            {
                                Brush overlayBrush = new SolidBrush(cell.OverlayColor);
                                pe.Graphics.FillRectangle(overlayBrush, cell.Rectangle);
                                overlayBrush.Dispose();
                                if (!cell.Selected)
                                    pe.Graphics.DrawRectangle((cell.BorderHighlighted) ? highlightedBorderPen : normalBorderPen, cell.Rectangle);
                                if (cell.OverlayCaption != null && cell.OverlayCaption != "")
                                    pe.Graphics.DrawString(cell.OverlayCaption, overlayFont, fontBrush, cell.LabelRectangle, centeredFormat);
                                pe.Graphics.DrawString(cell.Address, addressFont, fontBrush, cell.AddressPosition);
                            }

                        }

                    overlayFont.Dispose();
                    addressFont.Dispose();
                    if (labelFont != null)
                        labelFont.Dispose();

                    Brush cutCellsBrush = new SolidBrush(this.CutCellsColor);
                    for (int i = 0; i < Capacity; i++)
                        if (_CellMapByOrdinal[i].DropTargetHighlighted)
                        {
                            pe.Graphics.FillRectangle(cutCellsBrush, _CellMapByOrdinal[i].Rectangle);
                            pe.Graphics.DrawRectangle((cell.BorderHighlighted) ? highlightedBorderPen : normalBorderPen, _CellMapByOrdinal[i].Rectangle);
                        }
                    cutCellsBrush.Dispose();

                    if (_inSelectRectanglePhase)
                        pe.Graphics.DrawRectangle(Pens.Red, getMouseMovedRectangle());
                }
            }


        }


        private void Control_Resize(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Cells == null) return;
            CartesianGridCell leftRectangle = null;
            CartesianGridCell enteredRectangle = null;
            Point? current = GetGridCoordinatesFromLocation(e.Location, false);
            bool changeDetected = false;
            bool offGrid = (current == null);

            if (offGrid)
                if (_lastMouseOverCell == null)
                    return;
                else
                {
                    changeDetected = true;
                    leftRectangle = _lastMouseOverCell;
                    enteredRectangle = null;
                }
            else //onGrid
             if (_lastMouseOverCell == null)
            {
                changeDetected = true;
                enteredRectangle = _Cells[current.Value.X, current.Value.Y];
                leftRectangle = null;
            }
            else //need to see if there is an actual change
                if (_lastMouseOverCell.ColIndex == current.Value.X && _lastMouseOverCell.RowIndex == current.Value.Y)
                return;
            else
            {
                changeDetected = true;
                leftRectangle = _lastMouseOverCell;
                enteredRectangle = _Cells[current.Value.X, current.Value.Y];
            }

            if (changeDetected)
            {
                StartHoverTimer();
                _lastMouseOverCell = enteredRectangle;
                if (leftRectangle != null)
                {
                    leftRectangle.BorderHighlighted = false;
                    CellLeft?.Invoke(leftRectangle);
                }
                if (enteredRectangle != null)
                {
                    enteredRectangle.BorderHighlighted = true;
                    CellEntered?.Invoke(enteredRectangle);
                }
                Redraw();
            }
        }


        void StartHoverTimer()
        {
            _HoverTimer.Stop();
            _HoverTimer.Interval = this.CellHoverDelay;
            _HoverTimer.Start();
        }

        void StopHoverTimer()
        {
            _HoverTimer.Stop();
        }


        public bool ShowObjectPropertyViewerOnHover { get; set; } = true;

        void _HoverTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //double check to see current cell is in front and current.  This shouldn't be needed, but ...
            this.Invoke((MethodInvoker)
                delegate
                {
                    _HoverTimer.Stop();
                    if (this.ParentForm.Handle.Equals(WinAPI.GetForegroundWindow()) && _lastMouseOverCell != null)
                    {
                        System.Diagnostics.Debug.WriteLine("hover");
                        var mousePosition = Cursor.Position;
                        var currentCell = this.GetGridCoordinatesFromLocation(Cursor.Position, true);
                        if (currentCell.HasValue && currentCell.Value.X == _lastMouseOverCell.ColIndex && currentCell.Value.Y == _lastMouseOverCell.RowIndex)
                        {
                            //fire event
                            CellHover?.Invoke(_lastMouseOverCell);
                            if (ShowObjectPropertyViewerOnHover && _lastMouseOverCell.Item != null)
                                ShowPropertyViewer(_lastMouseOverCell.Item);
                        }
                    }
                });
        }

        void ShowPropertyViewer(object o)
        {
            _PPV.SelectedObject = _lastMouseOverCell.Item;
            // _PPV.Opacity = 0.9;
            _PPV.Location = new Point(Cursor.Position.X - 30, Cursor.Position.Y - 30);
            //  Point currentCellPosition = this.PointToScreen(_LastKnownCell.Rectangle.Location);
            //  _PPV.Location = new Point(currentCellPosition.X+20, currentCellPosition.Y+20);
            _PPV.ShowDialog();

        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            StopHoverTimer();
            if (_lastMouseOverCell != null)
            {
                CellLeft?.Invoke(_lastMouseOverCell);
                _lastMouseOverCell.BorderHighlighted = false;
                _lastMouseOverCell = null;
                Redraw();
            }
        }


        #endregion

    }

}
