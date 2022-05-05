using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TodoNotes.Data;

public class SettingXml
{
    public SettingXml()
    {
        XmlDocument xd = new XmlDocument();
        try
        {
            xd.Load("connections.xml");
        }
        catch (Exception e)
        {
            CreateNewDocument();
        }
    }

    public void CreateNewDocument()
    {
        XmlDocument xd = new XmlDocument();
        XmlTextWriter xtw = new XmlTextWriter("connections.xml", Encoding.UTF8);
        xtw.WriteStartDocument();
        xtw.WriteStartElement("Connections");
        xtw.WriteEndDocument();
        xtw.Close();

        xd.Load("connections.xml");

        XmlNode connection = xd.CreateElement("connection");
        connection.InnerText = "http://localhost:63847" + "/chathub";
        xd.DocumentElement?.AppendChild(connection);

        XmlNode connection2 = xd.CreateElement("connection");
        connection2.InnerText = "https://localhost:44389" + "/chathub";
        xd.DocumentElement?.AppendChild(connection2);
        xd.Save("connections.xml");
    }

    public List<string> ReadDocument()
    {
        var result = new List<string>();
        XmlDocument xd = new XmlDocument();
        xd.Load("connections.xml");

        XmlElement connections = xd.DocumentElement;

        if (connections != null)
            foreach (XmlNode connection in connections)
            {
                result.Add(connection.InnerText);
            }

        return result;
    }
}