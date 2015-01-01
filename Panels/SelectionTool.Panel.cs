﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Consoles;
using SadConsole.Controls;
using SadConsole.Input;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsoleEditor.Windows;

namespace SadConsoleEditor.Panels
{
    class SelectionToolAltPanel : CustomPanel
    {
        private CheckBox _skipEmptyColor;
        private CheckBox _altEmptyColorCheck;
        private Controls.ColorPresenter _altEmptyColor;


        public bool SkipEmptyCells { get { return _skipEmptyColor.IsSelected; } }
        public bool UseAltEmptyColor { get { return _altEmptyColorCheck.IsSelected; } }
        public Color AltEmptyColor { get { return _altEmptyColor.SelectedColor; } }

        public SelectionToolAltPanel()
        {
            Title = "Sel.Rect Options";

            _skipEmptyColor = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidth, 1);
            _skipEmptyColor.Text = "Skip Empty";

            _altEmptyColorCheck = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidth, 1);
            _altEmptyColorCheck.Text = "Use Alt. Empty";

            _altEmptyColor = new Controls.ColorPresenter("Alt. Empty Clr", Settings.Green, 18);
            _altEmptyColor.SelectedColor = Color.Black;

            Controls = new ControlBase[] { _skipEmptyColor, _altEmptyColorCheck, _altEmptyColor };
        }

        public override void ProcessMouse(MouseInfo info)
        {
        }

        public override int Redraw(ControlBase control)
        {
            if (control == _skipEmptyColor)
                return 1;

            return 0;
        }

        public override void Loaded()
        {
        }
    }

    class SelectionToolPanel : CustomPanel
    {
        private CloneState _state;
        private Button _reset;
        private Button _loadBrush;
        private Button _saveBrush;
        private Button _clone;
        private Button _clear;
        private Button _move;
        

        private Func<CellSurface> _saveBrushHandler;
        private Action<CellSurface> _loadBrushHandler;

        private int _currentStepChar = 175;


        public CloneState State
        {
            get { return _state; }
            set
            {
                _state = value;

                _saveBrush.IsEnabled = value == CloneState.Selected;
                _clone.IsEnabled = value == CloneState.Selected;
                _clear.IsEnabled = value == CloneState.Selected;
                _move.IsEnabled = value == CloneState.Selected;

                if (StateChangedHandler != null)
                    StateChangedHandler(value);
            } 
        }

        public Action<CloneState> StateChangedHandler;

        public enum CloneState
        {
            SelectingPoint1,
            SelectingPoint2,
            Selected,
            Clone,
            Clear,
            Move
        }

        

        public SelectionToolPanel(Action<CellSurface> loadBrushHandler, Func<CellSurface> saveBrushHandler)
        {
            _reset = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidth, 1);
            _reset.Text = "Reset Steps";
            _reset.ButtonClicked += (o, e) => State = CloneState.SelectingPoint1;

            _loadBrush = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidth, 1);
            _loadBrush.Text = "Import Brush";
            _loadBrush.ButtonClicked += _loadBrush_ButtonClicked;

            _saveBrush = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidth, 1);
            _saveBrush.Text = "Export Brush";
            _saveBrush.ButtonClicked += _saveBrush_ButtonClicked;

            _clone = new Button(Consoles.ToolPane.PanelWidth, 1);
            _clone.Text = "Clone";
            _clone.ButtonClicked += clone_ButtonClicked;

            _clear = new Button(Consoles.ToolPane.PanelWidth, 1);
            _clear.Text = "Clear";
            _clear.ButtonClicked += clear_ButtonClicked;

            _move = new Button(Consoles.ToolPane.PanelWidth, 1);
            _move.Text = "Move";
            _move.ButtonClicked += move_ButtonClicked;

            

            Controls = new ControlBase[] { _reset, _loadBrush, _saveBrush, _clone, _clear, _move };

            _loadBrushHandler = loadBrushHandler;
            _saveBrushHandler = saveBrushHandler;

            Title = "Clone";
            State = CloneState.SelectingPoint1;
        }

        private void move_ButtonClicked(object sender, EventArgs e)
        {
            State = CloneState.Move;
        }

        private void clear_ButtonClicked(object sender, EventArgs e)
        {
            State = CloneState.Clear;
        }

        private void clone_ButtonClicked(object sender, EventArgs e)
        {
            State = CloneState.Clone;
        }

        private void _saveBrush_ButtonClicked(object sender, EventArgs e)
        {
            SelectFilePopup popup = new SelectFilePopup();
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    CellSurface.Save(_saveBrushHandler(), popup.SelectedFile);
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.FileFilter = "*.con;*.console;*.brush";
            popup.SelectButtonText = "Save";
            popup.SkipFileExistCheck = true;
            popup.Show(true);
            popup.Center();
        }

        private void _loadBrush_ButtonClicked(object sender, EventArgs e)
        {
            SelectFilePopup popup = new SelectFilePopup();
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    if (System.IO.File.Exists(popup.SelectedFile))
                        _loadBrushHandler(CellSurface.Load(popup.SelectedFile));
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.FileFilter = "*.con;*.console;*.brush";
            popup.Show(true);
            popup.Center();
        }

        public override void ProcessMouse(MouseInfo info)
        {

        }

        public override int Redraw(ControlBase control)
        {
            if (control == _saveBrush || control == _move)
            {
                return 1;
            }

            return 0;
        }

        public override void Loaded()
        {
        }
    }
}
