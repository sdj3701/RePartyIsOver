using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TargetingHandler : MonoBehaviour
{
    private float _detectionRadius = 1f;
    public LayerMask layerMask;
    private float maxAngle = 110f; // ���鿡�� �¿�� �ش� ������ŭ ��ġ

    Collider _nearestCollider;
    private float _nearestDistance;

    Actor _actor;
    InteractableObject[] _interactableObjects = new InteractableObject[30];
    InteractableObject _nearestObject;

    // Start is called before the first frame update
    void Start()
    {
        _actor = GetComponent<Actor>();
        layerMask |= 1 << (int)Define.Layer.Item;
        layerMask |= 1 << (int)Define.Layer.ClimbObject;
        layerMask |= 1 << (int)Define.Layer.InteractableObject;

        for (int i = 0; i < 6; i++)
        {
            if (gameObject.layer != (int)Define.Layer.Player1 + i)
                layerMask |= 1 << (int)Define.Layer.Player1 + i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 

    public InteractableObject SearchTarget(Grab.Side side)
    {
        //���߿� ��������� ���� ȿ����
        Collider[] colliders = new Collider[40];
        _nearestCollider = null;
        _nearestObject = null;
        _nearestDistance = Mathf.Infinity;
        Transform chestTransform = _actor.BodyHandler.Chest.transform;
        
        
        //���麤��
        Vector3 chestForward = -chestTransform.up;

        //üũ�� ���� ����
        Vector3 detectionDirection;
        if (side == Grab.Side.Left)
            detectionDirection = -chestTransform.right;
        else
            detectionDirection = chestTransform.right;

        float detectionRadius = _detectionRadius;

        int colliderCount;

        if (_actor.actorState == Actor.ActorState.Jump || _actor.actorState == Actor.ActorState.Fall)
        {
            detectionRadius += 1f;
            //colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + chestForward, detectionRadius, colliders, layerMask);
            colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + Vector3.up * 0.1f, detectionRadius, colliders, layerMask);

        }
        else
        {
            // �� �ȿ� �ݶ��̴� ����
            colliderCount = Physics.OverlapSphereNonAlloc(chestTransform.position + Vector3.up * 0.1f, detectionRadius, colliders, layerMask);
        }

        if (colliderCount <= 0 )
        {
            return null;
        }

        // �ٶ󺸴� ���� 180�� �̳��� �ݶ��̴� �� interatableObject ���������� Ȯ��
        for (int i = 0; i < colliderCount; i++)
        {
            Vector3 toCollider = colliders[i].transform.position - chestTransform.position;
            float angle = Vector3.Angle(chestForward, toCollider);
            float angle2 = Vector3.Angle(detectionDirection, toCollider);

            if (angle <= maxAngle && angle2 <= 150f && colliders[i].GetComponent<InteractableObject>())
            {

                float distanceWithPriority = Vector3.Distance(FindClosestCollisionPoint(colliders[i]),chestTransform.position);
                bool lowPriorityPart = true;

                //��ġŸ���� ���׵��ϰ�� �߿䵵�� ���� �� ������ ���� ���ؼ� ����Ÿ���� �� ���ɼ��� ����
                if (colliders[i].GetComponent<BodyPart>() !=null)
                {
                    for (int j = (int)Define.BodyPart.Hip; j < (int)Define.BodyPart.Hip + 1; j++)
                    {
                        if (colliders[i].gameObject == colliders[i].transform.root.GetComponent<BodyHandler>().BodyParts[j].gameObject)
                        {
                            lowPriorityPart = false;
                        }
                    }

                    if (lowPriorityPart)
                    {
                        distanceWithPriority *= 10f;
                    }
                }

                //���尡��� Ÿ�� ����
                if (_nearestObject == null || distanceWithPriority < _nearestDistance)
                {
                    _nearestCollider = colliders[i];
                    _nearestObject = colliders[i].GetComponent<InteractableObject>();
                    _nearestDistance = Vector3.Distance(FindClosestCollisionPoint(_nearestCollider), chestTransform.position);
                }
            }
        }

        
        if(_nearestCollider == null)
        {
            return null;
        }

        //Debug.Log(_nearestObject.gameObject + "�ֿ켱����");
        return _nearestObject;
    }

    public Vector3 FindClosestCollisionPoint(Collider collider)
    {
        if (collider == null)
        {
            Debug.Log("Ÿ�ٿ� �ݶ��̴��� ����");
            return Vector3.zero;
        }

        Vector3 start = _actor.BodyHandler.Chest.transform.position; 
        Vector3 direction = (collider.transform.position - start).normalized;
        float distance = Vector3.Distance(start, collider.transform.position);

        RaycastHit hit;

        if (Physics.Raycast(start, direction, out hit, distance, layerMask))
        {
            return hit.point;
        }
        else
        {
            Debug.Log("Ÿ�ٿ� ������ ����");
            return Vector3.zero;
        }
    }
}
