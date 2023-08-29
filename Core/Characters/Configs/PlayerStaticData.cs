using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TOAFL.Core.Characters
{
    [CreateAssetMenu(menuName = "Scriptable/StaticData/Player", fileName = "PlayerStaticData")]
    public class PlayerStaticData : ScriptableObject
    {
        [SerializeField] private AssetReferenceGameObject playerReference;

        public AssetReferenceGameObject PlayerReference => playerReference;
    }
}