using UnityEngine;
[CreateAssetMenu(fileName = "ItemSpawnData", menuName = "Game/Item Spawn Data")]
public class ItemSpawnData : ScriptableObject
{
    public GameObject prefab;
    [Range(0, 1f)]
    public float probability; // weight value
}
