using UnityEngine;

namespace LevelGeneration
{
    [RequireComponent(typeof(Map))]
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private Map _map;
    
        private void Start()
        {
            _map = GetComponent<Map>();
            _map.OnLoad += LevelLoadHandler;
        }

        private void LevelLoadHandler(LevelLoadData levelLoadData)
        {
            Instantiate(playerPrefab, levelLoadData.StartPosition, Quaternion.identity);
        }
    }
}