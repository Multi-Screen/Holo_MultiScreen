using UnityEngine;

namespace shoukailiang.MultiUserCapabilities
{
    public class TableAnchor : MonoBehaviour
    {
        public static TableAnchor Instance;
        
        // 单例
        private void Start()
        {   
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance == this) return;
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
    }
}
