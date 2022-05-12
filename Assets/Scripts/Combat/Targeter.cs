using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target = null;

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
}