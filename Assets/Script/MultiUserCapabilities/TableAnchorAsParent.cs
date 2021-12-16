using UnityEngine;

namespace shoukailiang.MultiUserCapabilities
{
    public class TableAnchorAsParent : MonoBehaviour
    {
        private void Start()
        {   
            // 将当前物体挂载tableAnchor 下面
            if (TableAnchor.Instance != null) transform.parent = TableAnchor.Instance.transform;
        }
    }
}
