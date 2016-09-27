﻿using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Panels
{
    class RecolorToolPanel: CustomPanel
    {
        private CheckBox ignoreForeCheck;
        private CheckBox ignoreBackCheck;

        public bool IgnoreForeground { get { return ignoreForeCheck.IsSelected; } }
        public bool IgnoreBackground { get { return ignoreBackCheck.IsSelected; } }


        public RecolorToolPanel()
        {
            ignoreBackCheck = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidth - 2, 1);
            ignoreBackCheck.Text = "Ignore Back";

            ignoreForeCheck = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidth - 2, 1);
            ignoreForeCheck.Text = "Ignore Fore";

            Title = "Recolor Options";

            Controls = new ControlBase[] { ignoreForeCheck, ignoreBackCheck };
        }

        public override void ProcessMouse(SadConsole.Input.MouseInfo info)
        {
        }

        public override int Redraw(SadConsole.Controls.ControlBase control)
        {
            return 0;
        }

        public override void Loaded()
        {
        }
    }
}