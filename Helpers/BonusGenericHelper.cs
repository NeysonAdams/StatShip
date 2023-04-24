using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGenericHelper 
{
    private BonusModel model;

    float probabilityTotalWeight;

    public BonusGenericHelper(Model _model)
    {
        model = _model as BonusModel;
		CalculateProbabilities();

	}

	private void CalculateProbabilities()
	{
		List<BonusProbabilty> bonus_probabilities = model.probabilties;
		// Prevent editor from crashing when the item list is empty :)
		if (bonus_probabilities != null && bonus_probabilities.Count > 0)
		{

			float currentProbabilityWeightMaximum = 0f;

			// Sets the weight ranges of the selected items.
			foreach (BonusProbabilty lootDropItem in bonus_probabilities)
			{

				if (lootDropItem.probability_weght < 0f)
				{
					// Prevent usage of negative weight.
					Debug.Log("You can't have negative weight on an item. Reseting item's weight to 0.");
					lootDropItem.probability_weght = 0f;
				}
				else
				{
					lootDropItem.probabilityRangeFrom = currentProbabilityWeightMaximum;
					currentProbabilityWeightMaximum += lootDropItem.probability_weght;
					lootDropItem.probabilityRangeTo = currentProbabilityWeightMaximum;
				}

			}

			probabilityTotalWeight = currentProbabilityWeightMaximum;

			// Calculate percentage of item drop select rate.
			foreach (BonusProbabilty lootDropItem in bonus_probabilities)
			{
				lootDropItem.probability_percent = ((lootDropItem.probability_weght) / probabilityTotalWeight) * 100;
			}

		}

	}

	public Transform DropBonus()
	{
		List<Transform> bonuses = model.prefabs;
		List<BonusProbabilty> bonus_probabilities = model.probabilties;
		float pickedNumber = Random.Range(0, probabilityTotalWeight);
		int i = 0;
		// Find an item whose range contains pickedNumber
		foreach (BonusProbabilty probability in bonus_probabilities)
		{
			// If the picked number matches the item's range, return item
			if (pickedNumber > probability.probabilityRangeFrom && pickedNumber < probability.probabilityRangeTo)
			{
				if (probability.type == BonusType.NONE)
					return null;
				return bonuses[i-1];
			}
			i++;
		}

		// If item wasn't picked... Notify programmer via console and return the first item from the list
		Debug.LogError("Item couldn't be picked... Be sure that all of your active loot drop tables have assigned at least one item!");
		return null;
	}
}
