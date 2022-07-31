using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImpossibleOdds;
using ImpossibleOdds.Json;
using UnityEngine;

public class TestAsyncJsonProcessing : MonoBehaviour
{
	[SerializeField]
	private List<TextAsset> jsonAssets;

	private async void Start()
	{
		Task<object>[] deserializationTasks = new Task<object>[jsonAssets.Count];
		for (int i = 0; i < jsonAssets.Count; ++i)
		{
			deserializationTasks[i] = JsonProcessor.DeserializeAsync(jsonAssets[i].text);
		}

		await Task.WhenAll(deserializationTasks);

		Task<string>[] serializationTasks = new Task<string>[jsonAssets.Count];
		for (int i = 0; i < jsonAssets.Count; ++i)
		{
			serializationTasks[i] = JsonProcessor.SerializeAsync(deserializationTasks[i].Result);
		}

		await Task.WhenAll(serializationTasks);

		for (int i = 0; i < jsonAssets.Count; ++i)
		{
			Log.Info("Reserialized asset asynchronously:\n{0}", serializationTasks[i].Result);
		}
	}
}
