using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 11f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 10f;

    private float lastFireTime;
   
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();

        if (targeter.GetTarget() == null) {  return; }

        if (!CanFireAtTarget(target)) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);
            
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget(Targetable target)
    {
        return (target.transform.position - transform.position).sqrMagnitude < fireRange * fireRange;
    }
}