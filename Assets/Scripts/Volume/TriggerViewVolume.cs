using UnityEngine;

public class TriggerViewVolume : AViewVolume
{
    [SerializeField] private string _tagName;

    private void OnValidate()
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogWarning($"GameObject on TriggeredViewVolume need to have a collider in mode trigger (name {gameObject.name})");
            collider = gameObject.AddComponent<BoxCollider>();
            return;
        }
        if (!collider.isTrigger)
        {
            Debug.LogWarning($"Box collider need to have mode trigger (name {gameObject.name})");
            collider.isTrigger = true;
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_tagName))
            SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_tagName))
            SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Collider collider = gameObject.GetComponent<BoxCollider>();
        if (collider == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;

        //Box collider
        if (collider is BoxCollider box)
        {
            Gizmos.DrawWireCube(box.center, box.size);
            return;
        }

        //Sphere collider
        if (collider is SphereCollider sphere)
        {
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            return;
        }

        //Capsule collider
        if (collider is CapsuleCollider capsule)
        {
            Vector3 size = Vector3.zero;
            switch (capsule.direction)
            {
                case 0: // X axis
                    size = new Vector3(capsule.height, capsule.radius * 2, capsule.radius * 2);
                    break;
                case 1: // Y axis
                    size = new Vector3(capsule.radius * 2, capsule.height, capsule.radius * 2);
                    break;
                case 2: // Z axis
                    size = new Vector3(capsule.radius * 2, capsule.radius * 2, capsule.height);
                    break;
            }
            Gizmos.DrawWireCube(capsule.center, size);
        }
    }
}
