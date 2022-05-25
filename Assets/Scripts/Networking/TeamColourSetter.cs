using Mirror;
using UnityEngine;

public class TeamColourSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];
    [SerializeField] private Material[] materialPool = new Material[0];

    [SyncVar(hook = nameof(HandleTeamColourUpdated))] private int teamMaterialIndex;

    public int GetMaterialPoolSize()
    {
        return materialPool.Length;
    }

    #region Server

    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        teamMaterialIndex = player.GetTeamMaterialIndex();
    }

    #endregion

    #region Client

    private void HandleTeamColourUpdated(int oldTeamMaterialIndex, int newTeamMaterialIndex)
    {
        foreach (var renderer in colorRenderers)
        {
            renderer.material = materialPool[newTeamMaterialIndex];
        }
    }

    #endregion
}