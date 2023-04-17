using UnityEngine;

namespace LevelGeneration
{
    [RequireComponent(typeof(Map))]
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject[] enemyPrefabs;
        private Map _map;
    
        private void Start()
        {
            _map = GetComponent<Map>();
            _map.OnLoad += LevelLoadHandler;
        }

        private void LevelLoadHandler(LevelLoadData levelLoadData)
        {
            Instantiate(playerPrefab, levelLoadData.StartPosition, Quaternion.identity);

            foreach (var enemyPosition in levelLoadData.EnemyPositions)
            {
                // Pick an enemy based on the position's associated spawn data.
                // This is necessary to make sure that the level is reproducible based on a seed.
                var enemy = enemyPrefabs[enemyPosition.SpawnData % enemyPrefabs.Length];
                Instantiate(enemy, enemyPosition.Position, Quaternion.identity);
            }
        }
    }
}