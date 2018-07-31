using BDX.MagicShapes.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BDX.MagicShapes.Core.Data
{
    public class PolygonData
    {
        public Boolean Store(LinkedList<Polygon> polygons, String path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, polygons);
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public LinkedList<Polygon> Retrieve(String path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                LinkedList<Polygon> polygons = (LinkedList<Polygon>)formatter.Deserialize(stream);
                stream.Close();
                return polygons;
            }
            catch
            {
                return null;
            }
        }
    }
}
