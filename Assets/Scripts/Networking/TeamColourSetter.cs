using Mirror;
using UnityEngine;

public class TeamColourSetter : NetworkBehaviour
{
    [SerializeField] private Renderer minimapColorRenderer = new Renderer();
    [SerializeField] private Renderer[] teamMaterialRenderers = new Renderer[0];
    [SerializeField] private Material[] materialPool = new Material[0];

    [SyncVar(hook = nameof(HandleTeamColourUpdated))] private Color teamColour = new Color();
    [SyncVar(hook = nameof(HandleTeamMaterialUpdated))] private int teamMaterialIndex;

    public int GetMaterialPoolSize()
    {
        return materialPool.Length;
    }

    #region Server

    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        teamColour = player.GetTeamColour();
        teamMaterialIndex = player.GetTeamMaterialIndex();
    }

    #endregion

    #region Client

    private void HandleTeamColourUpdated(Color oldColour, Color newColour)
    {
        minimapColorRenderer.material.SetColor("_BaseColor", newColour);
    }

    private void HandleTeamMaterialUpdated(int oldTeamMaterialIndex, int newTeamMaterialIndex)
    {
        foreach (var renderer in teamMaterialRenderers)
        {
            renderer.material = materialPool[newTeamMaterialIndex];
        }
    }

    #endregion
}