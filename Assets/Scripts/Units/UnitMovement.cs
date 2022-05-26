using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Animator myAnimator = null;
    [SerializeField] private float chaseDistance = 10f;

    [SyncVar(hook = nameof(HandlePlayerWalking))] private bool isWalking = false;

    #region Server
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseDistance * chaseDistance)
            {
                bool hasCastToNavMesh = NavMesh.SamplePosition(target.transform.position, out NavMeshHit navMeshHit, 3f, NavMesh.AllAreas);
                if (hasCastToNavMesh)
                {
                    if (!isWalking)
                    {
                        isWalking = true;
                    }
                    agent.SetDestination(navMeshHit.position);
                }
            }
            else if (agent.hasPath)
            {
                isWalking = false;
                agent.ResetPath();
            }
        }
        if (!agent.hasPath) {  return; }
        if (agent.remainingDistance > agent.stoppingDistance) {  return; }
        isWalking = false;
        agent.ResetPath();
    }

    [Server]

    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 3f, NavMesh.AllAreas)) { return; }

        if (!isWalking)
        {
            isWalking = true;
        }
        agent.SetDestination(hit.position);
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    [Server]
    public void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    #endregion

    #region Client

    private void HandlePlayerWalking(bool oldValue, bool newValue)
    {
        myAnimator.SetBool("isWalking", newValue);
    }

    #endregion
}