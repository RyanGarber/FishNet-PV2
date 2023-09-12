using System.Collections;

using UnityEngine;

using FishNet;
using FishNet.Object;
using FishNet.Broadcast;

using ParrelSync;

namespace FishNet_PV2
{
    public class Overview : MonoBehaviour
    {
        public static Bug Bug = Bug.None;
        
        [SerializeField] private GameObject _playerPrefab;
        
        private NetworkObject _spawnedPlayer;

        /// <summary>
        /// Start connection.
        /// </summary>
        private IEnumerator Start()
        {
            // Try to connect server
            if (ClonesManager.IsClone())
            {
                while (!InstanceFinder.IsClient)
                {
                    InstanceFinder.ClientManager.StartConnection();
                    yield return new WaitForSeconds(2.5f);
                }
            }
            // Start server
            else
            {
                InstanceFinder.ServerManager.StartConnection();
                InstanceFinder.ClientManager.StartConnection();
            }
        }

        /// <summary>
        /// Provide status and buttons to switch bugs.
        /// </summary>
        private void OnGUI()
        {
            // Client
            if (ClonesManager.IsClone())
            {
                GUILayout.Label("Client");
                
                // Status
                if (!InstanceFinder.IsClient)
                {
                    GUILayout.Label("Trying to connect to server.");
                    return;
                }
                
                GUILayout.Label("Client connected.");
            }
            // Server
            else
            {
                GUILayout.Label("Server");

                //  Status
                if (InstanceFinder.ServerManager.Clients.Count < 2)
                {
                    GUILayout.Label("Waiting for a client to connect.");
                    return;
                }

                // Choose bug
                GUI.enabled = Bug != Bug.ServerInputBug;
                if (GUILayout.Button("Server Input Bug")) Initialize(Bug.ServerInputBug);
                
                GUI.enabled = Bug != Bug.ObserverReplicateBug;
                if (GUILayout.Button("Observer Replicate Bug")) Initialize(Bug.ObserverReplicateBug);
                
                GUI.enabled = true;
            }
        }

        /// <summary>
        /// Set bug and spawn player.
        /// </summary>
        private void Initialize(Bug bug)
        {
            // Set bug
            InstanceFinder.ServerManager.Broadcast(new SetBugBroadcast { Value = Bug = bug });

            // Despawn previous player
            if (_spawnedPlayer != null)
            {
                InstanceFinder.ServerManager.Despawn(_spawnedPlayer);
            }
            
            // Spawn new player
            _spawnedPlayer = Instantiate(_playerPrefab).GetComponent<NetworkObject>();
            InstanceFinder.ServerManager.Spawn(
                nob: _spawnedPlayer,
                ownerConnection: Bug switch
                {
                    Bug.ServerInputBug => null,
                    Bug.ObserverReplicateBug => InstanceFinder.ClientManager.Connection
                }
            );
        }

        private void Awake() => InstanceFinder.ClientManager.RegisterBroadcast<SetBugBroadcast>(OnSetBugBroadcast);
        private static void OnSetBugBroadcast(SetBugBroadcast args) { Bug = args.Value; }
        private struct SetBugBroadcast : IBroadcast { public Bug Value; }
    }

    public enum Bug
    {
        None,
        ServerInputBug,
        ObserverReplicateBug
    }
}