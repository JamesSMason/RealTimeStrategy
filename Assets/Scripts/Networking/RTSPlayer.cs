using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> myUnits = new List<Unit>();


    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    #region Server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleDespawned;
    }

    private void ServerHandleSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) {  return; }

        myUnits.Add(unit);
    }

    private void ServerHandleDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleDepawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }
    }

    private void AuthorityHandleSpawned(Unit unit)
    {
        if (!hasAuthority) { return; }

        myUnits.Add(unit);
    }

    private void AuthorityHandleDepawned(Unit unit)
    {
        if (!hasAuthority) { return; }

        myUnits.Remove(unit);
    }

    #endregion
}