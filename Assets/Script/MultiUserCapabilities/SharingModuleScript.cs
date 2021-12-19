using UnityEngine;

namespace shoukailiang.MultiUserCapabilities
{
    public class SharingModuleScript : MonoBehaviour
    {
        private AnchorModuleScript anchorModuleScript;

        private void Start()
        {
            anchorModuleScript = GetComponent<AnchorModuleScript>();
        }

        public void ShareAzureAnchor()
        {
            Debug.Log("\nSharingModuleScript.ShareAzureAnchor()");

            GenericNetworkManager.Instance.azureAnchorId = anchorModuleScript.currentAzureAnchorID;
            Debug.Log("GenericNetworkManager.Instance.azureAnchorId: " + GenericNetworkManager.Instance.azureAnchorId);

            var pvLocalUser = GenericNetworkManager.Instance.localUser.gameObject;
            Debug.Log("pvLocalUser="+pvLocalUser);
            var pu = pvLocalUser.gameObject.GetComponent<PhotonUser>();
            Debug.Log("pu="+pu);
            pu.ShareAzureAnchorId();
        }

        public void GetAzureAnchor()
        {
            Debug.Log("\nSharingModuleScript.GetAzureAnchor()");
            Debug.Log("GenericNetworkManager.Instance.azureAnchorId: " + GenericNetworkManager.Instance.azureAnchorId);

            anchorModuleScript.FindAzureAnchor(GenericNetworkManager.Instance.azureAnchorId);
        }
    }
}
