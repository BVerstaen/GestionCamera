using UnityEngine;

public class TriggerViewVolume : AViewVolume
{
    [SerializeField] private string _tagName;

    private void OnValidate()
    {
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
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
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        if (collider)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(collider.center, collider.size);
        }
    }
}
