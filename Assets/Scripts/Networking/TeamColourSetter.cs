using Mirror;
using UnityEngine;

public class TeamColourSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColourUpdated))] private Color teamColour;

    #region Server

    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        teamColour = player.GetTeamColor();
    }

    #endregion

    #region Client

    private void HandleTeamColourUpdated(Color oldColour, Color newColour)
    {
        foreach (var renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColour);
        }
    }

    #endregion
}