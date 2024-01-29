using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InGameManager : NetworkBehaviour
{
    NetworkList<InGameStats> playerStats;

    Dictionary<ulong,Text> ScoreTexts = new Dictionary<ulong,Text>();
    public Text textObject;
    public RectTransform textGroup;

    private void Awake()
    {
        playerStats = new NetworkList<InGameStats>();
    }


    public override void OnNetworkSpawn()
    {
        playerStats.OnListChanged += PlayerStatsChanged;
        if (!IsServer)
        {
            return;
        }
        NetworkManager.Singleton.OnClientConnectedCallback += AddPlayerStats;
    }

    private void AddPlayerStats(ulong clientId)
    {
        foreach (var player in playerStats)
        {
            if(player.Equals(clientId))
            {
                return;
            }
        }
        playerStats.Add(new InGameStats(clientId, 0, 0));
    }

    [ServerRpc]
    public void AddKillServerRpc(ulong clientId)
    {
        for (int i = 0; i < playerStats.Count; i++)
        {
            if (playerStats[i].Equals(clientId))
            {
                var stats = playerStats[i];
                stats.AddKill(1);
                playerStats[i] = stats;
            }
        }
    }

    [ServerRpc]
    public void AddDeathServerRpc(ulong clientId)
    {
        for (int i = 0; i < playerStats.Count; i++)
        {
            if (playerStats[i].Equals(clientId))
            {
                var stats = playerStats[i];
                stats.AddDeath(1);
                playerStats[i] = stats;
            }
        }
    }

    private void TextRefresh()
    {
        foreach(var player in playerStats)
        {
            if(ScoreTexts.Count(item => item.Key == player.Id) == 0)
            {
                var text = Instantiate(textObject, textGroup);
                text.text = $"{player.Id}: {player.Kill}K/{player.Death}D";
                ScoreTexts.Add(player.Id, text);
            }
            else
            {
                ScoreTexts[player.Id].text = $"{player.Id}: {player.Kill}K/{player.Death}D";
            }
        }
    }

    public void PlayerStatsChanged(NetworkListEvent<InGameStats> stats)
    {
        Debug.Log("Pre:" + stats.PreviousValue.ToString() + "\nNew:" + stats.Value.ToString());
        TextRefresh();
    }
}

public struct InGameStats : INetworkSerializable, IEquatable<InGameStats>
{
    ulong _id;
    int _kill;
    int _death;

    public ulong Id { get => _id;}
    public int Kill { get => _kill;}
    public int Death { get => _death;}

    public InGameStats(ulong id, int kill, int death)
    {
        _id = id;
        _kill = kill;
        _death = death;
    }

    public override string ToString()
    {
        return $"id:{_id}, kill:{_kill}, death:{_death}";
    }

    public void AddKill(int value) { _kill += value; }
    public void AddDeath(int value) { _death += value; }

    public bool Equals(InGameStats other)
    {
        return _id.Equals(other._id);
    }
    public bool Equals(ulong id)
    {
        return _id.Equals(id);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _id);
        serializer.SerializeValue(ref _kill);
        serializer.SerializeValue(ref _death);
    }
}