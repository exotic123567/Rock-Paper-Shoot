using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : MonoBehaviour
{
    private const int MaxConnections = 10;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("ConnectionApprovalHandler::ApprovalCheck");
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;

        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxConnections)
        {
            response.Approved = false;
            response.Reason = "Server is FULL!!!";
            
        }
        response.Pending = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
