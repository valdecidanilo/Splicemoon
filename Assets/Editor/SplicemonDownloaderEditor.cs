#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inventory;
using Models;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor
{
    public class SplicemonDownloaderEditor : EditorWindow
    {
        private int currentId = 1;
        private const int maxId = 1025;
        private bool isDownloading = false;

        [MenuItem("Tools/Splicemon Downloader")]
        public static void ShowWindow()
        {
            GetWindow<SplicemonDownloaderEditor>("Splicemon Downloader");
        }

        private void OnGUI()
        {
            if (!isDownloading)
            {
                if (GUILayout.Button("Iniciar Download"))
                {
                    isDownloading = true;
                    EditorCoroutineUtility.StartCoroutineOwnerless(DownloadAllSplicemons());
                }
            }
            else
            {
                GUILayout.Label("Downloading... " + currentId + "/" + maxId);
            }
        }

        private IEnumerator DownloadAllSplicemons()
        {
            while (currentId <= maxId)
            {
                Debug.Log($"Download Pokemon {currentId}...");
                yield return GetPokeData(currentId, pokeData =>
                {
                    Debug.Log($"Download Pokemon {currentId} Complete.");
                    var splicemon = ScriptableObject.CreateInstance<SplicemonObject>();
                    splicemon.Initialize(pokeData);

                    var fourfirst = splicemon.stats.possiblesMoveAttack.Take(4).ToList();

                    EditorCoroutineUtility.StartCoroutineOwnerless(GetMoveDetails(fourfirst, moveAttackList =>
                    {
                        Debug.Log($"Download Pokemon {currentId} MoveList Complete.");
                        splicemon.stats.movesAttack = new List<MoveDetails>(moveAttackList);

                        SaveSplicemonAsset(splicemon, pokeData.NameSpliceMoon);
                        Debug.Log($"Scriptable Criado: {pokeData.NameSpliceMoon}");
                    }));
                });

                currentId++;
                yield return new WaitForSeconds(1.5f); // delay para evitar excesso de requisições
            }

            isDownloading = false;
            Debug.Log("Todos os Pokémons foram baixados!");
        }

        private IEnumerator GetPokeData(int pokemonId, System.Action<PokeData> callback)
        {
            var url = $"https://pokeapi.co/api/v2/pokemon/{pokemonId}";
            var uwr = UnityWebRequest.Get(url);
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var pokeData = JsonConvert.DeserializeObject<PokeData>(uwr.downloadHandler.text);
                callback.Invoke(pokeData);
            }
            else
            {
                Debug.LogError($"Erro ao baixar Pokémon {pokemonId}");
                callback.Invoke(null);
            }
        }

        private IEnumerator GetMoveDetails(List<string> urls, System.Action<List<MoveDetails>> callback)
        {
            var moveDetailsList = new List<MoveDetails>();
            foreach (var url in urls)
            {
                if (string.IsNullOrEmpty(url)) continue;

                var uwr = UnityWebRequest.Get(url);
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    var moveDetails = JsonConvert.DeserializeObject<MoveDetails>(uwr.downloadHandler.text);
                    moveDetailsList.Add(moveDetails);
                }
                else
                {
                    Debug.LogError($"Erro ao baixar move: {url}");
                }

                yield return new WaitForSeconds(0.75f); // delay entre moves
            }

            callback.Invoke(moveDetailsList);
        }

        private void SaveSplicemonAsset(SplicemonObject splicemon, string name)
        {
            var path = "Assets/Resources/Pokemons/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            AssetDatabase.CreateAsset(splicemon, $"{path}{name}.asset");
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
