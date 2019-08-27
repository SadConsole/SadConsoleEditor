﻿namespace SadConsoleEditor.Controls
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Controls;
    using SadConsole.Themes;
    using System;
    using System.Linq;
    using Console = SadConsole.Console;

    public class ColorPicker : SadConsole.Controls.ControlBase
    {
        public class ThemeType : ThemeBase
        {
            /// <inheritdoc />
            public override void Attached(ControlBase control)
            {
                if (!(control is ColorPicker)) throw new Exception($"Theme can only be added to a {nameof(ColorPicker)}");

                control.Surface = new CellSurface(control.Width, control.Height);
                control.Surface.Clear();
                base.Attached(control);
            }

            /// <inheritdoc />
            public override void UpdateAndDraw(ControlBase control, TimeSpan time)
            {
                if (!(control is ColorPicker picker)) return;

                if (!control.IsDirty) return;

                Cell appearance;

                if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                    appearance = Disabled;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(presenter.State, ControlStates.MouseRightButtonDown))
                //    appearance = MouseDown;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseOver))
                //    appearance = MouseOver;

                else if (Helpers.HasFlag(control.State, ControlStates.Focused))
                    appearance = Focused;

                else
                    appearance = Normal;

                Color[] colors = Color.White.LerpSteps(Color.Black, control.Height);
                Color[] colorsEnd = picker._masterColor.LerpSteps(Color.Black, control.Height);

                for (int y = 0; y < control.Height; y++)
                {
                    control.Surface[0, y].Background = colors[y];
                    control.Surface[control.Width - 1, y].Background = colorsEnd[y];

                    control.Surface[0, y].Foreground = new Color(255 - colors[y].R, 255 - colors[y].G, 255 - colors[y].B);
                    control.Surface[control.Width - 1, y].Foreground = new Color(255 - colorsEnd[y].R, 255 - colorsEnd[y].G, 255 - colorsEnd[y].B);



                    Color[] rowColors = colors[y].LerpSteps(colorsEnd[y], control.Width);

                    for (int x = 1; x < control.Width - 1; x++)
                    {
                        control.Surface[x, y].Background = rowColors[x];
                        control.Surface[x, y].Foreground = new Color(255 - rowColors[x].R, 255 - rowColors[x].G, 255 - rowColors[x].B);
                    }
                }

                control.IsDirty = false;
            }

            /// <inheritdoc />
            public override ThemeBase Clone()
            {
                return new ThemeType()
                {
                    Colors = Colors?.Clone(),
                    Normal = Normal.Clone(),
                    Disabled = Disabled.Clone(),
                    MouseOver = MouseOver.Clone(),
                    MouseDown = MouseDown.Clone(),
                    Selected = Selected.Clone(),
                    Focused = Focused.Clone(),
                };
            }
        }


        private Color _selectedColor;
        private Color _masterColor;
        private Point _selectedColorPosition;

        public event EventHandler SelectedColorChanged;

        public Color SelectedColor
        {
            get { return _selectedColor; }
            private set
            {
                if (_selectedColor != value)
                {
                    ResetSelectedIndex(value);
                    _selectedColor = value;
                    SelectedColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private Color SelectedColorSafe
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    SelectedColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Color MasterColor
        {
            get { return _masterColor; }
            set
            {
                if (_masterColor != value)
                {
                    _masterColor = value;
                    SelectedColor = value;
                    IsDirty = true;
                    //ResetSelectedColor();
                }
            }
        }


        public ColorPicker(int width, int height, Color color): base(width, height)
        {
            Theme = new ThemeType();
            Theme.UpdateAndDraw(this, TimeSpan.Zero);

            //SelectedHue = hue;

            SelectedColor = color;
            Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
        }

        private void ResetSelectedIndex(Color color)
        {
            Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
            _selectedColorPosition = new Point(Width - 1, 0);
            Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;

            this.IsDirty = true;
        }
        
        protected override void OnMouseIn(SadConsole.Input.MouseConsoleState info)
        {
            base.OnMouseIn(info);

            if (Parent.CapturedControl == null)
            {
                if (info.Mouse.LeftButtonDown)
                {
                    var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
                    Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
                    _selectedColorPosition = location;
                    SelectedColorSafe = Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Background;
                    Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
                    IsDirty = true;

                    Parent.CaptureControl(this);
                }
            }
        }

        public override bool ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
            if (Parent.CapturedControl == this)
            {
                if (info.Mouse.LeftButtonDown == false)
                    Parent.ReleaseControl();
                else
                {
                    var location = this.TransformConsolePositionByControlPosition(info.CellPosition);

                    //if (info.ConsolePosition.X >= Position.X && info.ConsolePosition.X < Position.X + Width)
                    if (location.X >= -6 && location.X <= Width + 5 && location.Y > -4 && location.Y < Height + 3)
                    {
                        Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
                        _selectedColorPosition = new Point(Microsoft.Xna.Framework.MathHelper.Clamp(location.X, 0, Width - 1), Microsoft.Xna.Framework.MathHelper.Clamp(location.Y, 0, Height - 1));
                        SelectedColorSafe = Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Background;
                        Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
                    }

                    IsDirty = true;
                }
            }

            return base.ProcessMouse(info);
        }
        
    }
}
