using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;


/// <summary>
/// 
/// XML FILE IS IN THE StreamingAssets DIRECTORY
/// 
/// </summary>

[XmlRoot]
public sealed class INV_Item
{
    [XmlAttribute("ID")]
    public ID m_Id { get; private set; }

    [XmlElement("Name")]
    public string m_Name { get; private set; }

    [XmlElement("PrefabPath")]
    public string m_PrefabPath { get; private set; }
}