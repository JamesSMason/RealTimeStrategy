using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target = null;

    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        Debug.Log("Targeting");
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target)) {  return; }
        this.target = target;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    #endregion

    #region Client

    #endregion
}