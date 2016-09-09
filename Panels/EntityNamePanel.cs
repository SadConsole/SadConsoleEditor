﻿using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;
using SadConsole.Game;

namespace SadConsoleEditor.Panels
{
    class EntityNamePanel: CustomPanel
    {
        private Button setName;
        private DrawingSurface nameTitle;
        private GameObject entity;


        public EntityNamePanel()
        {
            Title = "Entity";

            nameTitle = new DrawingSurface(Consoles.ToolPane.PanelWidth - 3, 2);

            setName = new Button(3, 1);
            setName.ShowEnds = false;
            setName.Text = "Set";

            setName.ButtonClicked += (s, e) =>
            {
                Windows.RenamePopup rename = new Windows.RenamePopup(entity.Name);
                rename.Closed += (s2, e2) => { if (rename.DialogResult) entity.Name = rename.NewName; PrintName(); };
                rename.Center();
                rename.Show(true);
            };

            Controls = new ControlBase[] { setName, nameTitle };
        }

        private void PrintName()
        {
            nameTitle.Clear();
            nameTitle.Print(0, 0, "Name");
            nameTitle.Print(0, 1, entity.Name);
        }

        public override void Loaded()
        {
        }

        public override void ProcessMouse(MouseInfo info)
        {
        }

        public override int Redraw(ControlBase control)
        {
            if (control == setName)
            {
                control.Position = new Microsoft.Xna.Framework.Point(Consoles.ToolPane.PanelWidth - 3, control.Position.Y);
                return -1;
            }
            return 0;
        }

        public void SetEntity(GameObject entity)
        {
            this.entity = entity;
            PrintName();
            setName.IsEnabled = entity != null;
        }
    }
}
