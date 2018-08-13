using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Drop table for this object
public class Env_Dest_LootTable : Env_Destructable {

	// Loot table to drop from (chance)
	[SerializeField]
	private SO_LootTable m_lootTable;

    [SerializeField]
    private Vector3 m_spawnOffset;

	// How many items from the loot table can I drop (inclusively)
	[SerializeField]
	private int m_minDrops = 0;
	[SerializeField]
	private int m_maxDrops = 2;

	private bool m_hasInclusifiedTopRange = false;

	// Trigger the destroy
	public override void Attacked(Vector3 attackVector){
		// Protect against multiple hit loot table objects? But also increment once because Random.Range(int, int) is (inc, exc)!
		if (!m_hasInclusifiedTopRange) {
			m_maxDrops++;
			m_hasInclusifiedTopRange = true;
		}

		// Drops items
		int drops = Random.Range(m_minDrops, m_maxDrops);

		// Get the loot table length
		int tableLength = m_lootTable.GetLootTableLength();


		for (int i = 0; i < drops; ++i) {
			// Percentage (RNG)
			float perc = Random.Range (0.0f, 100.0f);

			// Grab all the drops possible
			List<GameObject> candidateDrops = new List<GameObject> ();


			// Get all the possible drops for this percentage
			for (int c = 0; c < tableLength; ++c) {
				LootDrop candidate = m_lootTable.GetLootObject (c);
				if (candidate != null) {
					if (perc <= candidate.LootChance) {
						// EG: Lootchance 90, our percentage was 67, it would be added, if our perc was 95 it would not. (psuedo 90%)
						candidateDrops.Add(candidate.LootPrefab);
					}

				}
			}

			// Spawn the loot
			if(candidateDrops.Count > 0){
				Instantiate (candidateDrops [Random.Range (0, candidateDrops.Count)], transform.position + m_spawnOffset, Quaternion.identity);
			}

		}

		// Call cleanup and base destructable code
		base.Attacked (attackVector);
	}
}
