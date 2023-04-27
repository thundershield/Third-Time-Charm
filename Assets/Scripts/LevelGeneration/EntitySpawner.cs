using UnityEngine;

namespace LevelGeneration
{
    [RequireComponent(typeof(Map))]
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private GameObject bossPrefab;
        private Map _map;
    
        private void Start()
        {
            _map = GetComponent<Map>();
            _map.OnLoad += LevelLoadHandler;
        }

        private void LevelLoadHandler(LevelLoadData levelLoadData)
        {
            _map.SpawnObject(playerPrefab, levelLoadData.StartPosition);

            foreach (var enemyPosition in levelLoadData.EnemyPositions)
            {
                // Pick an enemy based on the position's associated spawn data.
                // This is necessary to make sure that the level is reproducible based on a seed.
                var enemy = enemyPrefabs[enemyPosition.SpawnData % enemyPrefabs.Length];
                _map.SpawnObject(enemy, enemyPosition.Position);
            }
            if(levelLoadData.BossPosition!=null){
                _map.SpawnObject(bossPrefab, levelLoadData.BossPosition.GetValueOrDefault());
            }
        }
    }
}