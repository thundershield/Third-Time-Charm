using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour 
{
    public static LobbyManager Instance { get; private set; }
    public const string KEY_PLAYER_NAME = "PlayerName";
    public event EventHandler OnLeftLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public class LobbyEventArgs : EventArgs 
    {
        public Lobby lobby;
    }
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs 
    {
        public List<Lobby> lobbyList;
    }
    private float heartbeatTimer;
    private float lobbyPollTimer;
    private float refreshLobbyListTimer = 5f;
    private Lobby joinedLobby;
    private string playerName;
    private void Awake() 
    {
        Instance = this;
    }

    private void Update() 
    {
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    public async void Authenticate(string playerName) 
    {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);
        await UnityServices.InitializeAsync(initializationOptions);
        AuthenticationService.Instance.SignedIn += () => 
        {
            RefreshLobbyList();
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void HandleRefreshLobbyList() 
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) 
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f) 
            {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;
                RefreshLobbyList();
            }
        }
    }

    private async void HandleLobbyHeartbeat() 
    {
        if (IsLobbyHost()) 
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;
                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling() 
    {
        if (joinedLobby != null) 
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f) 
            {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;
                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                if (!IsPlayerInLobby()) 
                {
                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                    joinedLobby = null;
                }
            }
        }
    }

    public Lobby GetJoinedLobby() 
    {
        return joinedLobby;
    }

    public bool IsLobbyHost() 
    {
    return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby() 
    {
        if (joinedLobby != null && joinedLobby.Players != null) 
        {
            foreach (Player player in joinedLobby.Players) 
            {
                if (player.Id == AuthenticationService.Instance.PlayerId) 
                {
                return true;
                }
            }
        }
        return false;
    }

    private Player GetPlayer() 
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> 
        {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
        });
    }


    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate) 
    {
        Player player = GetPlayer();
        CreateLobbyOptions options = new CreateLobbyOptions 
        {
            Player = player,
            IsPrivate = isPrivate,
        };
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        joinedLobby = lobby;
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        Debug.Log("Created Lobby " + lobby.Name);
    }

    public async void RefreshLobbyList()
    {
        try 
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;
            options.Filters = new List<QueryFilter> 
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            options.Order = new List<QueryOrder> 
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };
            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        } 
        catch (LobbyServiceException e) 
        {
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) 
    {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions 
        {
            Player = player
        });
        joinedLobby = lobby;
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }
    public async void JoinLobby(Lobby lobby) 
    {
        Player player =GetPlayer();
        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions 
        {
            Player = player
        });
        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }
    public async void UpdatePlayerName(string playerName) 
    {
        this.playerName = playerName;
        if (joinedLobby != null) 
        {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();
                options.Data = new Dictionary<string, PlayerDataObject>() 
                {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public,value: playerName)
                    }
                };
                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;
                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            } 
            catch (LobbyServiceException e) 
            {
            }
        }
    }
    public async void QuickJoinLobby() 
    {
        try 
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        } 
        catch (LobbyServiceException e) 
        {
        }
    }

    public async void LeaveLobby() 
    {
        if 
        (joinedLobby != null) 
        {
            try 
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            } 
            catch (LobbyServiceException e) 
            {
            }
        }
    }
    public async void KickPlayer(string playerId) 
    {
        if (IsLobbyHost()) 
        {
            try 
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } 
            catch (LobbyServiceException e) 
            {
            }
        }
    }
}