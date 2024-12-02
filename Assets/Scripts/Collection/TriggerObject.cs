using UnityEngine;

public class TriggerObject : DraggableObject
{
    [SerializeField] protected string triggerTag;
    protected GameObject currentTarget;

    protected override void UpdateMaterials()
    {
        if (spriteRenderer != null)
        {
            if (isDragging)
            {
                if (isInvalidPosition)
                {
                    spriteRenderer.material = invalidMaterial;
                }
                else if (currentTarget != null)
                {
                    spriteRenderer.material = canMergeMaterial;
                    UpdateTargetMaterial();
                }
                else
                {
                    spriteRenderer.material = draggingMaterial;
                }
            }
            else
            {
                base.UpdateMaterials();
            }
        }
    }

    private void UpdateTargetMaterial()
    {
        if (currentTarget != null)
        {
            SpriteRenderer targetRenderer = currentTarget.GetComponent<SpriteRenderer>();
            if (targetRenderer != null)
            {
                targetRenderer.material = canMergeMaterial;
            }
        }
    }

    protected override bool IsValidPlacement()
    {
        if (!isDragging) return true;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            transform.position,
            col.bounds.size,
            transform.rotation.eulerAngles.z,
            collisionLayer
        );

        foreach (Collider2D otherCol in colliders)
        {
            if (otherCol != col && otherCol.gameObject != gameObject)
            {
                CollectibleObject collectible = otherCol.GetComponent<CollectibleObject>();
                if (collectible != null && !collectible.IsUnlocked())
                {
                    // 检查是否匹配的触发组合
                    if (CompareTag(collectible.GetUnlockMethod().ToString()))
                    {
                        currentTarget = otherCol.gameObject;
                        return true;
                    }
                }
            }
        }
        
        currentTarget = null;
        return base.IsValidPlacement();  // 如果不是有效组合或物品已解锁，使用基类的碰撞检测
    }

    protected override void CompletePlace()
    {
        if (currentTarget != null && !isInvalidPosition)
        {
            CollectibleObject collectible = currentTarget.GetComponent<CollectibleObject>();
            if (collectible != null)
            {
                collectible.HandleTriggerEffect();
            }
            gameObject.SetActive(false);
        }
        base.CompletePlace();
    }
}