using Mirror;
using System;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDeath += ServerHandleDeath;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);

        health.ServerOnDeath -= ServerHandleDeath;
    }

    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client



    #endregion
}