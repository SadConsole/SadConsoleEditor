﻿namespace SadConsoleEditor.Tools
{
    using SadConsole;
    using SadConsole.Surfaces;
    using SadConsole.GameHelpers;
    using SadConsole.Input;
    using SadConsoleEditor.Panels;

    class TextTool : ITool
    {
        public const string ID = "TEXT";

        private bool writing;
        private Console tempConsole;
        private SadConsole.Effects.EffectsManager blinkManager;

        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Text"; }
        }
        public char Hotkey { get { return 't'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        private RecolorToolPanel settingsPanel;

        public GameObject Brush;

        public override string ToString()
        {
            return Title;
        }

        public TextTool()
        {
            settingsPanel = new RecolorToolPanel();

            ControlPanels = new CustomPanel[] { settingsPanel, CharacterPickPanel.SharedInstance };
            tempConsole = new Console(1, 1);
        }

        public void OnSelected()
        {
            Brush = new SadConsole.GameHelpers.GameObject(1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            Brush.Animation.CreateFrame();
            Brush.IsVisible = false;

            blinkManager = new SadConsole.Effects.EffectsManager(Brush.Animation.Frames[0]);

            RefreshTool();
            MainScreen.Instance.Brush = Brush;

            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            CharacterPickPanel.SharedInstance.HideCharacter = true;
        }

        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            CharacterPickPanel.SharedInstance.HideCharacter = false;
            writing = false;
        }

        private void CharPanelChanged(object sender, System.EventArgs e)
        {
            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(sender, e);
            RefreshTool();
        }

        public void RefreshTool()
        {
            SadConsoleEditor.Settings.QuickEditor.TextSurface = Brush.Animation.Frames[0];
            SadConsoleEditor.Settings.QuickEditor.Fill(CharacterPickPanel.SharedInstance.SettingForeground,
                                      CharacterPickPanel.SharedInstance.SettingBackground, 95);

            SadConsole.Effects.Blink blinkEffect = new SadConsole.Effects.Blink();
            blinkEffect.BlinkSpeed = 0.35f;
            blinkManager.SetEffect(Brush.Animation.Frames[0][0], blinkEffect);

            Brush.Animation.IsDirty = true;

        }

        public void Update()
        {
            blinkManager.UpdateEffects(SadConsole.Global.GameTimeElapsedUpdate);
        }

        public bool ProcessKeyboard(Keyboard info, ISurface surface)
        {
            if (writing)
            {
                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    writing = false;
                    Brush.IsVisible = false;
                    MainScreen.Instance.AllowKeyboardToMoveConsole = true;
                }
                else
                {
                    //tempConsole.BasicSurface = (ISurfaceRendered)surface;
                    tempConsole.VirtualCursor.PrintAppearance = new Cell(CharacterPickPanel.SharedInstance.SettingForeground, CharacterPickPanel.SharedInstance.SettingBackground);
                    tempConsole.ProcessKeyboard(info);
                    Brush.Position = tempConsole.VirtualCursor.Position;
                }

                return true;
            }

            return false;
        }

        public void ProcessMouse(MouseConsoleState info, ISurface surface, bool isInBounds)
        {

            if (info.IsOnConsole && info.Mouse.LeftClicked)
            {
                MainScreen.Instance.AllowKeyboardToMoveConsole = false;
                writing = true;

                tempConsole.TextSurface = (ISurface)surface;
                tempConsole.VirtualCursor.Position = Brush.Position = info.ConsolePosition;

                Brush.IsVisible = true;
            }
        }
       
    }
}

