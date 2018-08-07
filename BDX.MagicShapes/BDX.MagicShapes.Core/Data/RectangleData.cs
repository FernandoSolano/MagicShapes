using BDX.MagicShapes.Core.Model;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace BDX.MagicShapes.Core.Data
{
    public class RectangleData
    {

        public Boolean Store(AppState appState, String path)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(appState.GetType());
                TextWriter textWriter = new StreamWriter(path);
                xmlSerializer.Serialize(textWriter, appState);
                textWriter.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return false;
            }
        }

        public AppState Retrieve(String path)
        {
            try
            {
                AppState appState = new AppState();
                XmlSerializer xmlSerializer = new XmlSerializer(appState.GetType());
                TextReader textReader = new StreamReader(path);
                appState = (AppState)xmlSerializer.Deserialize(textReader);
                textReader.Close();
                return appState;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return null;
            }
        }
    }
}
