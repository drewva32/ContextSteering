using UnityEngine;
public class SmartSeek : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform player;
    [SerializeField] private float collisionDetectionRange;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float testAngle = 135f;

    private float[] _avoidanceArray = new float[7];
    private Rigidbody2D _rb;
    private Vector2 _steeringDirection;

    private void Awake() => _rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        //default to 3 because that is the one pointing directly forward when there are 7 possible directions.
        int chosenIndex = 3;
        float highestDot = -100;
        Vector2 directionToTarget = (player.position - transform.position).normalized;
        for (int i = 0; i < _avoidanceArray.Length; i++)
        {
            Vector2 currentDirection = GetIndexDirection(i);
            RaycastHit2D hit =
                Physics2D.Raycast(transform.position, currentDirection, collisionDetectionRange, wallLayer);
            _avoidanceArray[i] = hit.fraction;
            
            //if raycast collided with a wall, skip over this direction as a possible option to move in.
            if (_avoidanceArray[i] != 0)
                continue;
            //dot product returns values from -1 to 1.
            //-1 is returned if the two vectors are facing opposite directions.
            //1 is returned if the two vectors are facing the exact same direction.
            //So, the higher the dot product, the more that direction matches the direction to the player, and should be selected to move in.
            if (Vector2.Dot(directionToTarget,GetIndexDirection(i)) > highestDot)
            {
                highestDot = Vector2.Dot(directionToTarget,GetIndexDirection(i));
                chosenIndex = i;
            }
        }
        _steeringDirection = GetIndexDirection(chosenIndex);
    }

    private void FixedUpdate()
    {
        _rb.AddForce((_steeringDirection * speed) - _rb.velocity);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _rb.velocity);
    }
    
    //There are 7 possible directions to move in, each has an index in the _avoidanceArray
    //0 - 135 degrees
    //1 - 90 degrees(left)
    //2 - 45 degrees
    //3 - 0 degrees (straight forward)
    //4 - -45 degrees
    //5 - -90 degrees (right)
    //6 - -135 degrees
    
    private Vector2 GetIndexDirection(int index)
    {
        Vector2 startDirection = Quaternion.AngleAxis(testAngle, Vector3.forward) * transform.up;
        return Quaternion.AngleAxis(45 * -index, Vector3.forward) * startDirection;
    }
    
    //useful for visualizing the raycasts in game editor, be sure to activate gizmos so that this is visible.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < _avoidanceArray.Length; i++)
        {
            Gizmos.DrawRay(transform.position, GetIndexDirection(i) * collisionDetectionRange);
        }
    }
}

