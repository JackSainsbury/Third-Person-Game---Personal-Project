using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootDrop{
	[SerializeField]
	private float m_chancePercentage = 50;
	[SerializeField]
	private GameObject m_prefab;

	public float LootChance{
		get{
			return m_chancePercentage;
		}
		set{
			m_chancePercentage = value;
		}
	}

	public GameObject LootPrefab{
		get{
			return m_prefab;
		}
		set{
			m_prefab = value;
		}
	}
}

[CreateAssetMenu(fileName = "Loot_Table", menuName = "Loot/Loot_Table", order = 2)]
public class SO_LootTable : ScriptableObject {
	[SerializeField]
	private LootDrop[] m_drops;

	/// <summary>
	/// Gets a loot object, from the loot table, by index.
	/// </summary>
	/// <returns>The loot object.</returns>
	/// <param name="index">Index.</param>
	public LootDrop GetLootObject(int index){
		return m_drops [index];
	}

	/// <summary>
	/// Gets the length of the loot table.
	/// </summary>
	/// <returns>The loot table length.</returns>
	public int GetLootTableLength(){
		return m_drops.Length;
	}
}