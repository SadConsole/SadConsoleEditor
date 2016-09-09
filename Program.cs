﻿#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace SadConsoleEditor
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Load our program settings
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ProgramSettings));
            using (var fileObject = System.IO.File.OpenRead("Settings.json"))
                Settings.Config = serializer.ReadObject(fileObject) as ProgramSettings;


            // Setup the engine and creat the main window.
            SadConsole.Engine.Initialize(Settings.Config.ProgramFontFile, Settings.Config.WindowWidth, Settings.Config.WindowHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Engine.EngineStart += Engine_EngineStart;

            // Hook the update event that happens each frame so we can trap keys and respond.
            //SadConsole.Engine.EngineUpdated += Engine_EngineUpdated;

            //SadConsole.Engine.EngineDrawFrame += Engine_EngineDrawFrame;

            // Start the game.
            SadConsole.Engine.Run();
        }

        private static void Engine_EngineStart(object sender, EventArgs e)
        {
            SadConsole.Engine.MonoGameInstance.Window.Title = "SadConsole Editor - v" + System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString();

            
            // Load screen font
            var font = SadConsole.Engine.LoadFont(Settings.Config.ScreenFontFile);
            Settings.Config.ScreenFont = font.GetFont(SadConsole.Font.FontSizes.One);

            // Setup GUI themes
            Settings.SetupThemes();

            // Helper editor for any text surface
            Settings.QuickEditor = new SadConsole.Consoles.SurfaceEditor(new SadConsole.Consoles.TextSurface(10, 10, SadConsole.Engine.DefaultFont));

            // Setup system to run
            SadConsole.Engine.ConsoleRenderStack = EditorConsoleManager.Instance;
            SadConsole.Engine.ActiveConsole = EditorConsoleManager.Instance;

            // Start
            SadConsole.Libraries.GameHelpers.Initialize();
            EditorConsoleManager.Instance.ShowNewConsolePopup(false);
        }
    }
#endif
}
