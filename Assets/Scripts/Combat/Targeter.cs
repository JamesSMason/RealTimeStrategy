using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target = null;

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    public Targetable GetTarget()
    {
        return target;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target)) {  return; }
        this.target = target;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    [Server]
    public void ServerHandleGameOver()
    {
        ClearTarget();
    }
}