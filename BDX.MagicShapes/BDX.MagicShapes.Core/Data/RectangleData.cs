using BDX.MagicShapes.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BDX.MagicShapes.Core.Data
{
    public class RectangleData
    {
        public Boolean Store(AppState appState, String path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, appState);
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public AppState Retrieve(String path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                AppState appState = (AppState)formatter.Deserialize(stream);
                stream.Close();
                return appState;
            }
            catch
            {
                return null;
            }
        }
    }
}
