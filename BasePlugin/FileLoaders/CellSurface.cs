﻿using System;
using SadConsole;
using System.Linq;

namespace SadConsoleEditor.FileLoaders
{
    public class CellSurface : IFileLoader
    {

        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "surface", "surfacez" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Single Surface";
            }
        }

        public string Id => "SURFACE";

        public object Load(string file)
        {
            return SadConsole.Serializer.Load<SadConsole.CellSurface>(file, file.EndsWith('z'));
        }

        public bool Save(object surface, string file)
        {
            try
            {
                var cellSurface = new SadConsole.CellSurface(((SadConsole.CellSurface)surface).Width, ((SadConsole.CellSurface)surface).Height);

                ((SadConsole.CellSurface)surface).Copy(cellSurface);

                SadConsole.Serializer.Save<SadConsole.CellSurface>(cellSurface, file, file.EndsWith('z'));

                return true;
            }
            catch (Exception e)
            {
                SadConsoleEditor.MainConsole.Instance.ShowSaveErrorPopup();
                return false;
            }
        }
    }
}
