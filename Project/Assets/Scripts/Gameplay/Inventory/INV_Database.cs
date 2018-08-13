using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("ItemCollection")]
public class INV_Database {

    [XmlArray("Items")]
    [XmlArrayItem("Item")]
    public List<INV_Item> m_items = new List<INV_Item>();
    /*
    public static INV_Database Load(string path)
    {
        TextAsset _xml 
    }
    */
}
