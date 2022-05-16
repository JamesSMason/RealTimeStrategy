using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody myRigidBody = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float destroyAfterSeconds = 5f;


    #region Server
    private void Start()
    {
        myRigidBody.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient)
            {
                Debug.Log("Hit object is ours"); return; }
        }

        Debug.Log("Hit object is enemy");

        if (networkIdentity.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
            Debug.Log("Hit object has health");
        }

        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion
}