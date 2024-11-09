using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
//using Unity.Services.Multiplay;
using System.Threading.Tasks;
using TMPro;

public class MultiplayManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private TMP_InputField portInputField;

    /*private IServerQueryHandler serverQueryHandler;

    private async void Start()
    {
        if (Application.platform == RuntimePlatform.LinuxServer)
        {
            // Set the target frame rate for optimal server performance
            Application.targetFrameRate = 60;

            // Initialize Unity services asynchronously
            await UnityServices.InitializeAsync();

            // Get server configuration from Multiplay service
            ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;

            // Start the server query handler asynchronously
            serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(10, "MyServer", "MyGameType","0", "TestMap");

            // Check if the server allocation ID is valid
            if (serverConfig.AllocationId != string.Empty)
            {
                // Set the connection data for Unity Transport (IP and Port)
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                    "0.0.0.0",               // Bind IP address for server
                    serverConfig.Port,       // Server port from the configuration
                    "0.0.0.0"                // Connection address for clients (can be the same or different)
                );

                // Start the server
                NetworkManager.Singleton.StartServer();

                // Indicate the server is ready for players
                await MultiplayService.Instance.ReadyServerForPlayersAsync();
            }
        }
    }
    
    private async void Update()
    {
        if (Application.platform == RuntimePlatform.LinuxServer)
        {
            if (serverQueryHandler != null)
            {
                // Update the current player count
                serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
                serverQueryHandler.UpdateServerCheck();

                // Delay before the next update to avoid excessive processing
                await Task.Delay(100);
            }
        }
    }*/

    public void JoinToServer()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ipAddressInputField.text, ushort.Parse(portInputField.text));
        NetworkManager.Singleton.StartClient();
    }
}