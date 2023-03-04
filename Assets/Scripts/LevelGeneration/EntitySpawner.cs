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

            // foreach (var enemyPosition in levelLoadData.EnemyPositions)
            // {
            //     Instantiate(playerPrefab, enemyPosition + Vector2.one * 0.5f, Quaternion.identity);
            // }
        }
    }
}