using UnityEngine;
using Unity.Collections;
using Unity.Netcode;


public class UnnamedStringMessageHandler : CustomUnnamedMessageHandler<string>
{

    protected override byte MessageType()
    {
        return 1;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        }
        else
        {
            SendUnnamedMessage("I am a client connecting!");
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.OnClientDisconnectCallback -= OnClientConnectedCallback;
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        SendUnnamedMessage($"Everyone welcome the newly joined client ({clientId})!");
    }
    
    protected override void OnReceivedUnnamedMessage(ulong clientId, FastBufferReader reader)
    {
        var stringMessage = string.Empty;
        reader.ReadValueSafe(out stringMessage);
        if (IsServer)
        {
            Debug.Log($"Server received unnamed message of type ({MessageType()}) from client " +
                $"({clientId}) that contained the string: \"{stringMessage}\"");
            SendUnnamedMessage($"Newly connected client sent this greeting: \"{stringMessage}\"");
        }
        else
        {
            Debug.Log(stringMessage);
        }
    }

    public override void SendUnnamedMessage(string dataToSend)
    {
        var writer = new FastBufferWriter(1100, Allocator.Temp);
        var customMessagingManager = NetworkManager.CustomMessagingManager;
        using (writer)
        {
            writer.WriteValueSafe(MessageType());
            writer.WriteValueSafe(dataToSend);
            if (IsServer)
            {
                customMessagingManager.SendUnnamedMessageToAll(writer);
            }
            else
            {
                customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
            }
        }
    }
}


public class CustomUnnamedMessageHandler<T> : NetworkBehaviour
{
    protected virtual byte MessageType()
    {
        return 0;
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.CustomMessagingManager.OnUnnamedMessage += ReceiveMessage;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.CustomMessagingManager.OnUnnamedMessage -= ReceiveMessage;
    }

    protected virtual void OnReceivedUnnamedMessage(ulong clientId, FastBufferReader reader)
    {
    }

    private void ReceiveMessage(ulong clientId, FastBufferReader reader)
    {
        var messageType = (byte)0;
        reader.ReadValueSafe(out messageType);
        if (messageType == MessageType())
        {
            OnReceivedUnnamedMessage(clientId, reader);
        }
    }

    public virtual void SendUnnamedMessage(T dataToSend)
    {

    }
}

