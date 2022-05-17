using Mirror;
using System;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))] private int currentHealth;

    public event Action ServerOnDeath;

    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        UnitBase.ServerOnPlayerDeath += ServerHandlePlayerDeath;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDeath -= ServerHandlePlayerDeath;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth == 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth != 0) { return; }

        ServerOnDeath?.Invoke();
    }

    [Server]
    private void ServerHandlePlayerDeath(int connectionID)
    {
        if (connectionToClient.connectionId != connectionID) { return; }

        DealDamage(currentHealth);
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}