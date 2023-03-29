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
            GameObject player = Instantiate(playerPrefab, levelLoadData.StartPosition, Quaternion.identity);
            GameObject playerInventory = GameObject.FindGameObjectsWithTag("Inventory")[0];
            player.GetComponent<PlayerControler>().playerInventory = playerInventory.GetComponent<inventory>();
        }
    }
}