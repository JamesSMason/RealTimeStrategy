using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private Animator myAnimator = null;
    [SerializeField] private float fireRange = 11f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    private float lastFireTime;
    Targetable target;

    #region Server

    [ServerCallback]
    private void Update()
    {
        target = targeter.GetTarget();

        if (target == null) {  return; }

        if (!CanFireAtTarget(target)) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > fireRate + lastFireTime)
        {
            RPCHandleShootingAnimation();
            
            lastFireTime = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget(Targetable target)
    {
        return (target.transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }

    [Command]
    public void CmdAttack()
    {
        Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);
        GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
        NetworkServer.Spawn(projectileInstance, connectionToClient);
    }


    #endregion

    #region Client

    [ClientRpc]
    private void RPCHandleShootingAnimation()
    {
        if (!hasAuthority) {  return; }

        if (myAnimator.GetBool("isWalking"))
        {
            myAnimator.SetBool("isWalking", false);
        }
        myAnimator.SetTrigger("isFiring");
    }

    public void Attack()
    {
        CmdAttack();
    }
    #endregion
}