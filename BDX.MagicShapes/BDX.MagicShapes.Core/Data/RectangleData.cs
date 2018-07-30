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
        public Boolean Store(LinkedList<Rectangle> rectangles, String path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, rectangles);
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public LinkedList<Rectangle> Retrieve(String path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                LinkedList<Rectangle> rectangles = (LinkedList<Rectangle>)formatter.Deserialize(stream);
                stream.Close();
                return rectangles;
            }
            catch
            {
                return null;
            }
        }
    }
}
