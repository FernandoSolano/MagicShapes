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
        public Boolean Store(LinkedList<Rectangle> rectangles)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("MagicShapesSave.bin",
                                         FileMode.Create,
                                         FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, rectangles);
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public LinkedList<Rectangle> Retrieve()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("MagicShapesSave.bin",
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.Read);
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
