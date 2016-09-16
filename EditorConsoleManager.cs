﻿using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using System.Linq;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SadConsoleEditor
{
    static class EditorConsoleManager
    {
        private static ConsoleList Consoles;
        private static Consoles.QuickSelectPane quickSelectPane;
        private static Console topBarPane;
        private static Consoles.BorderConsole borderConsole;

        public static Dictionary<Type, FileLoaders.IFileLoader[]> EditorFileTypes;
        public static Dictionary<string, Editors.Editors> Editors;

        public static Editors.IEditor ActiveEditor;

        public static void Initialize()
        {
            Consoles = new ConsoleList();

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Engine.ConsoleRenderStack = Consoles;
            SadConsole.Engine.ActiveConsole = Consoles;
            SadConsole.Engine.EngineUpdated += Engine_EngineUpdated;
            SadConsole.Engine.EngineDrawFrame += Engine_EngineDrawFrame;

            // Create the basic consoles
            quickSelectPane = new SadConsoleEditor.Consoles.QuickSelectPane();
            quickSelectPane.Position = new Point(0, Settings.Config.WindowHeight - quickSelectPane.TextSurface.Height);
            quickSelectPane.Redraw();
            quickSelectPane.IsVisible = true;

            topBarPane = new SadConsole.Consoles.Console(Settings.Config.WindowWidth, 1);
            topBarPane.TextSurface.DefaultBackground = Settings.Color_MenuBack;
            topBarPane.Clear();
            topBarPane.MouseCanFocus = false;

            borderConsole = new SadConsoleEditor.Consoles.BorderConsole(10, 10);

            // Add the consoles to the main console list
            Consoles.Add(topBarPane);
            Consoles.Add(quickSelectPane);
            Consoles.Add(borderConsole);

            // Setup the file types for base editors.
            EditorFileTypes = new Dictionary<Type, FileLoaders.IFileLoader[]>(3);
            //EditorFileTypes.Add(typeof(Editors.DrawingEditor), new FileLoaders.IFileLoader[] { new FileLoaders.TextSurface() });

            // Add valid editors
            Editors = new Dictionary<string, SadConsoleEditor.Editors.Editors>();
            Editors.Add("Console Draw", SadConsoleEditor.Editors.Editors.Console);
            Editors.Add("Animated Game Object", SadConsoleEditor.Editors.Editors.GameObject);
            Editors.Add("Game Scene", SadConsoleEditor.Editors.Editors.Scene);
            Editors.Add("User Interface Console", SadConsoleEditor.Editors.Editors.GUI);

            // Show new window
            ShowStartup();
        }

        private static void ShowStartup()
        {
            Window.Prompt("Create new or open existing?", "New", "Open",
            (b) =>
            {
                if (b)
                {
                    Windows.NewConsolePopup popup = new Windows.NewConsolePopup();
                    popup.Center();
                    popup.AllowCancel = false;
                    popup.Closed += (s, e) => { if (!popup.DialogResult) ShowStartup(); };
                    popup.Show(true);
                }
                else
                {
                    Windows.SelectFilePopup popup = new Windows.SelectFilePopup();
                    popup.Center();
                    popup.Closed += (s, e) => { if (!popup.DialogResult) ShowStartup(); };
                    popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.LayeredTextSurface(), new FileLoaders.Scene(), new FileLoaders.Entity() };
                    popup.Show(true);
                }

            });
        }

        public static void UpdateBorder(Point position)
        {
            if (borderConsole.Width != ActiveEditor.Width + 2 || borderConsole.Height != ActiveEditor.Height + 2)
                borderConsole = new Consoles.BorderConsole(ActiveEditor.Width + 2, ActiveEditor.Height + 2);

            borderConsole.Position = position - new Point(1, 1);
        }

        public static void CenterEditor()
        {
            Point position = new Point();

            var screenSize = SadConsole.Engine.GetScreenSizeInCells(Settings.Config.ScreenFont);

            if (ActiveEditor.Width < screenSize.X)
                position.X = ((screenSize.X - 20) / 2) - (ActiveEditor.Width / 2);
            else
                position.X = ((screenSize.X - 20) - ActiveEditor.Width) / 2;

            if (ActiveEditor.Height < screenSize.Y)
                position.Y = (screenSize.Y / 2) - (ActiveEditor.Height / 2);
            else
                position.Y = (screenSize.Y - ActiveEditor.Height) / 2;

            ActiveEditor.Move(position.X, position.Y);
        }

        private static void Engine_EngineDrawFrame(object sender, EventArgs e)
        {

        }

        private static void Engine_EngineUpdated(object sender, EventArgs e)
        {

        }
    }
}