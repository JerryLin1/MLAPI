﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace MLAPI
{
    public abstract class NetworkedBehaviour : MonoBehaviour
    {
        protected bool isLocalPlayer
        {
            get
            {
                return networkedObject.isLocalPlayer;
            }
        }
        protected bool isServer
        {
            get
            {
                return NetworkingManager.singleton.isServer;
            }
        }
        protected bool isClient
        {
            get
            {
                return NetworkingManager.singleton.isClient;
            }
        }
        protected NetworkedObject networkedObject
        {
            get
            {
                if(_networkedObject == null)
                {
                    _networkedObject = GetComponentInParent<NetworkedObject>();
                }
                return _networkedObject;
            }
        }
        private NetworkedObject _networkedObject = null;
        public uint objectNetworkId
        {
            get
            {
                return networkedObject.NetworkId;
            }
        }

        public int ownerClientId
        {
            get
            {
                return networkedObject.OwnerClientId;
            }
        }

        //Change data type
        private Dictionary<string, int> registeredMessageHandlers = new Dictionary<string, int>();

        public int RegisterMessageHandler(string name, Action<int, byte[]> action)
        {
            int counter = NetworkingManager.singleton.AddIncomingMessageHandler(name, action);
            registeredMessageHandlers.Add(name, counter);
            return counter;
        }

        public void DeregisterMessageHandler(string name, int counter)
        {
            NetworkingManager.singleton.RemoveIncomingMessageHandler(name, counter);
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<string, int> pair in registeredMessageHandlers)
            {
                DeregisterMessageHandler(pair.Key, pair.Value);
            }
        }

        public void SendToServer(string messageType, string channelName, byte[] data)
        {
            if (isServer)
            {
                NetworkingManager.singleton.InvokeMessageHandlers(messageType, data, -1);
            }
            else
            {
                NetworkingManager.singleton.Send(NetworkingManager.singleton.serverClientId, messageType, channelName, data);
            }
        }

        public void SendToLocalClient(string messageType, string channelName, byte[] data)
        {
            if (!isServer)
            {
                Debug.LogWarning("MLAPI: Sending messages from client to other clients is not yet supported");
                return;
            }
            NetworkingManager.singleton.Send(ownerClientId, messageType, channelName, data);
        }

        public void SendToNonLocalClients(string messageType, string channelName, byte[] data)
        {
            if (!isServer)
            {
                Debug.LogWarning("MLAPI: Sending messages from client to other clients is not yet supported");
                return;
            }
            NetworkingManager.singleton.Send(messageType, channelName, data, ownerClientId);
        }

        public void SendToClient(int clientId, string messageType, string channelName, byte[] data)
        {
            if (!isServer)
            {
                Debug.LogWarning("MLAPI: Sending messages from client to other clients is not yet supported");
                return;
            }
            NetworkingManager.singleton.Send(clientId, messageType, channelName, data);
        }

        public void SendToClients(int[] clientIds, string messageType, string channelName, byte[] data)
        {
            if (!isServer)
            {
                Debug.LogWarning("MLAPI: Sending messages from client to other clients is not yet supported");
                return;
            }
            NetworkingManager.singleton.Send(clientIds, messageType, channelName, data);
        }

        public void SendToClients(List<int> clientIds, string messageType, string channelName, byte[] data)
        {
            if (!isServer)
            {
                Debug.LogWarning("MLAPI: Sending messages from client to other clients is not yet supported");
                return;
            }
            NetworkingManager.singleton.Send(clientIds, messageType, channelName, data);
        }

        public void SendToClients(string messageType, string channelName, byte[] data)
        {
            if (!isServer)
            {
                Debug.LogWarning("MLAPI: Sending messages from client to other clients is not yet supported");
                return;
            }
            NetworkingManager.singleton.Send(messageType, channelName, data);
        }

        public NetworkedObject GetNetworkedObject(uint networkId)
        {
            return NetworkingManager.singleton.SpawnedObjects[networkId];
        }
    }
}