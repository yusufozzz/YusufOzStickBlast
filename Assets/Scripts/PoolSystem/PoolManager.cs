using GameManagement;
using UnityEngine;

namespace PoolSystem
{
    public class PoolManager: ManagerBase
    {
        [field: SerializeField]
        public HighlightPool HighlightPool { get; private set; }

        
    }
}