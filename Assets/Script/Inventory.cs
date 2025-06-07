using System;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Item Settings")] [SerializeField]
    private GameObject itemPrefab;

    private CapsuleCollider2D _capsuleCollider2D;

    private void Awake()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    public void AddItemToHead(GameObject item)
    {
        if (!_capsuleCollider2D)
        {
            Debug.LogWarning("collider is not assigned.");
            return;
        }


        Vector2 headLocation = new Vector2(
            _capsuleCollider2D.bounds.center.x,
            _capsuleCollider2D.bounds.max.y
        );

        item.transform.SetParent(this.gameObject.transform);
        item.transform.position = headLocation;
        item.transform.rotation = Quaternion.identity;

        itemPrefab = item;
    }

    public bool HasItem(string itemName)
    {
        return !itemPrefab.IsUnityNull() && itemPrefab.name.Equals(itemName);
    }
}