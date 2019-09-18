﻿using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.Controls;
using SadConsoleEditor.Windows;
using SadConsole;
using SadConsoleEditor.Panels;
using SadConsoleEditor.Consoles;
using SadConsoleEditor;

namespace EntityPlugin.Panels
{
    class AnimationFramesPanel : CustomPanel
    {
        private Button removeSelected;
        private Button moveSelectedUp;
        private Button moveSelectedDown;
        private Button addNewFrame;
        private Button addNewFrameFromFile;
        private Button saveFrameToFile;
        private Button clonePreviousFrame;

        private DrawingSurface framesCounterBox;
        private Button nextFrame;
        private Button previousFrame;

        private Action<CellSurface> frameChangeCallback;
        private AnimatedConsoleEditor currentAnimation;
        private CellSurface selectedFrame;

        public AnimationFramesPanel(Action<CellSurface> frameChangeCallback)
        {
            Title = "Frames";

            removeSelected = new Button(ToolPane.PanelWidthControls);
            removeSelected.Text = "Remove";
            removeSelected.Click += removeSelected_Click;

            moveSelectedUp = new Button(ToolPane.PanelWidthControls);
            moveSelectedUp.Text = "Move Up";
            moveSelectedUp.Click += moveSelectedUp_Click;

            moveSelectedDown = new Button(ToolPane.PanelWidthControls);
            moveSelectedDown.Text = "Move Down";
            moveSelectedDown.Click += moveSelectedDown_Click;

            addNewFrame = new Button(ToolPane.PanelWidthControls);
            addNewFrame.Text = "Add New";
            addNewFrame.Click += addNewFrame_Click;

            addNewFrameFromFile = new Button(ToolPane.PanelWidthControls);
            addNewFrameFromFile.Text = "Load From File";
            addNewFrameFromFile.Click += addNewFrameFromFile_Click;

            saveFrameToFile = new Button(ToolPane.PanelWidthControls);
            saveFrameToFile.Text = "Save Frame to File";
            saveFrameToFile.Click += saveFrameToFile_Click;

            clonePreviousFrame = new Button(ToolPane.PanelWidthControls);
            clonePreviousFrame.Text = "Copy prev. frame";
            clonePreviousFrame.Click += clonePreviousFrame_Click;

            // Frames area
            framesCounterBox = new DrawingSurface(ToolPane.PanelWidthControls, 1);
            framesCounterBox.OnDraw = (drawSurface) =>
            {
                ColoredString frameNumber = new ColoredString((currentAnimation.Frames.IndexOf(selectedFrame) + 1).ToString(), SadConsole.Themes.Library.Default.Colors.Blue, drawSurface.Theme.Colors.ControlBack);
                ColoredString frameSep = new ColoredString(" \\ ", SadConsole.Themes.Library.Default.Colors.Gray, drawSurface.Theme.Colors.ControlBack);
                ColoredString frameMax = new ColoredString(currentAnimation.Frames.Count.ToString(), SadConsole.Themes.Library.Default.Colors.Blue, drawSurface.Theme.Colors.ControlBack);
                drawSurface.Surface.Fill(SadConsole.Themes.Library.Default.Colors.Blue, drawSurface.Theme.Colors.ControlBack, 0, null);
                drawSurface.Surface.Print(0, 0, frameNumber + frameSep + frameMax);
            };

            nextFrame = new Button(4);
            nextFrame.Text = ">>";
            nextFrame.Click += nextFrame_Click;

            previousFrame = new Button(4);
            previousFrame.Text = "<<";
            previousFrame.Click += previousFrame_Click;

            this.frameChangeCallback = frameChangeCallback;
            
            Controls = new ControlBase[] { framesCounterBox, previousFrame, nextFrame, removeSelected, addNewFrame, clonePreviousFrame, moveSelectedUp, moveSelectedDown, addNewFrameFromFile, saveFrameToFile};
        }

        

        public void TryNextFrame()
        {
            if (nextFrame.IsEnabled)
                nextFrame_Click(null, EventArgs.Empty);
        }

        public void TryPreviousFrame()
        {
            if (previousFrame.IsEnabled)
                previousFrame_Click(null, EventArgs.Empty);
        }

        public void SetAnimation(AnimatedConsole animation)
        {
            currentAnimation = new AnimatedConsoleEditor(animation);

            selectedFrame = currentAnimation.Frames[0];

            EnableDisableControls(0);
            DrawFrameCount();

            frameChangeCallback(selectedFrame);
        }

        private void nextFrame_Click(object sender, EventArgs e)
        {
            var currentIndex = currentAnimation.Frames.IndexOf(selectedFrame) + 1;

            selectedFrame = currentAnimation.Frames[currentIndex];

            EnableDisableControls(currentIndex);

            frameChangeCallback(selectedFrame);

            SadConsoleEditor.MainConsole.Instance.ToolsPane.RedrawPanels();
        }
        private void clonePreviousFrame_Click(object sender, EventArgs e)
        {
            var prevIndex = currentAnimation.Frames.IndexOf(selectedFrame) - 1;

            var prevFrame = currentAnimation.Frames[prevIndex];

            prevFrame.Copy(selectedFrame);
        }

        private void previousFrame_Click(object sender, EventArgs e)
        {
            var currentIndex = currentAnimation.Frames.IndexOf(selectedFrame) - 1;

            selectedFrame = currentAnimation.Frames[currentIndex];

            EnableDisableControls(currentIndex);

            frameChangeCallback(selectedFrame);

            SadConsoleEditor.MainConsole.Instance.ToolsPane.RedrawPanels();
        }
        private void EnableDisableControls(int currentIndex)
        {
            previousFrame.IsEnabled = currentIndex != 0;
            nextFrame.IsEnabled = currentIndex != currentAnimation.Frames.Count - 1;

            removeSelected.IsEnabled = currentAnimation.Frames.Count != 1;
            moveSelectedUp.IsEnabled = currentAnimation.Frames.Count != 1 && currentIndex != currentAnimation.Frames.Count - 1;
            moveSelectedDown.IsEnabled = currentAnimation.Frames.Count != 1 && currentIndex != 0;
            removeSelected.IsEnabled = currentAnimation.Frames.Count != 1;
            clonePreviousFrame.IsEnabled = currentIndex != 0;
        }

        void saveFrameToFile_Click(object sender, EventArgs e)
        {
            SelectFilePopup popup = new SelectFilePopup(new[] { "SURFACE" });
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    popup.SelectedLoader.Save(selectedFrame, popup.SelectedFile);
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.SelectButtonText = "Save";
            popup.SkipFileExistCheck = true;
            popup.Show(true);
            popup.Center();
        }

        void addNewFrameFromFile_Click(object sender, EventArgs e)
        {
            SelectFilePopup popup = new SelectFilePopup(new[] { "SURFACE", "CONSOLE" });
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    var surface = (CellSurface)popup.SelectedLoader.Load(popup.SelectedFile);
                    var newFrame = currentAnimation.CreateFrame();

                    surface.Copy(newFrame);

                    EnableDisableControls(0);
                    DrawFrameCount();
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.Show(true);
            popup.Center();
        }

        void moveSelectedDown_Click(object sender, EventArgs e)
        {
            var index = currentAnimation.Frames.IndexOf(selectedFrame);
            currentAnimation.RemoveFrame(selectedFrame);
            currentAnimation.InsertFrame(index - 1, selectedFrame);

            EnableDisableControls(currentAnimation.Frames.IndexOf(selectedFrame));
            DrawFrameCount();
        }

        void moveSelectedUp_Click(object sender, EventArgs e)
        {
            var index = currentAnimation.Frames.IndexOf(selectedFrame);
            currentAnimation.RemoveFrame(selectedFrame);
            currentAnimation.InsertFrame(index + 1, selectedFrame);

            EnableDisableControls(currentAnimation.Frames.IndexOf(selectedFrame));
            DrawFrameCount();
        }

        void removeSelected_Click(object sender, EventArgs e)
        {
            currentAnimation.RemoveFrame(selectedFrame);
            selectedFrame = currentAnimation.Frames[0];

            EnableDisableControls(0);
            DrawFrameCount();
        }

        void addNewFrame_Click(object sender, EventArgs e)
        {
            var frame = currentAnimation.CreateFrame();
            frame.Fill(currentAnimation.DefaultForeground, currentAnimation.DefaultBackground, 0, null);
            EnableDisableControls(currentAnimation.Frames.IndexOf(selectedFrame));
            DrawFrameCount();
        }

        public override void ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
        }

        private void DrawFrameCount() =>
            framesCounterBox.IsDirty = true;

        public override int Redraw(ControlBase control)
        {
            if (control == framesCounterBox)
            {
                DrawFrameCount();
            }
            else if (control == nextFrame)
            {
                nextFrame.Position = new Microsoft.Xna.Framework.Point(previousFrame.Position.X + previousFrame.Width + 1, previousFrame.Position.Y);
                return 0;
            }

            else if (control == moveSelectedDown || control == addNewFrame)
                return 1;

            return 0;
        }

        public override void Loaded()
        {
            
        }
    }
}
