using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class EnemyMovementScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created\
    [SerializeField] public int acceleration;
    [SerializeField] public int maxSpeed;
    [SerializeField] public GameObject mainTarget;
    [SerializeField] public int targetDistance;
    [SerializeField] public Transform modelTransform;
    private Rigidbody personalRigidbody;
    private UnityEngine.Vector3 tempTarget;
    private int frameCounter = 0;
    private  const int frameInterval = 10;
    private int successfullRouteFindCounter = 0;
    private  const int successfullRouteFindInterval = 10;
    private int angleStart = 0;
    private int angleIter = 10;
    private int angleMax = 120;
    private int framesDirection = 0;
    [SerializeField]
    private int rotationSpeed = 30;

    float timer = 0f;
const float interval = 1f;

    private int startingCheckDistanceMulti = 1;
    private int startingCheckDistanceMultiIter = 1;
    void Start()
    {
       personalRigidbody = this.GetComponent<Rigidbody>();
       tempTarget = mainTarget.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
         timer += Time.deltaTime;

    if (timer >= interval && framesDirection > 0)
    {
        timer -= interval; 
        framesDirection -=1;
        }
            
        if (UnityEngine.Vector3.Distance(this.gameObject.transform.position, tempTarget) > targetDistance)
        {
            
            UnityEngine.Vector3 velocity = personalRigidbody.linearVelocity;
            velocity = UnityEngine.Vector3.ClampMagnitude(velocity, maxSpeed);
            personalRigidbody.linearVelocity = velocity;
            UnityEngine.Vector3 direction = UnityEngine.Vector3.Normalize(tempTarget-this.gameObject.transform.position);
            UnityEngine.Quaternion facingRotation = UnityEngine.Quaternion.LookRotation(direction);
            
            personalRigidbody.AddForce(direction*acceleration);
            //Debug.DrawLine(personalRigidbody.transform.position,this.gameObject.transform.position+personalRigidbody.linearVelocity, Color.black, 1000f);
            
            if (frameCounter >= frameInterval)
            { 
                UnityEngine.Vector3 directionV2 = (tempTarget - transform.position).normalized;
                detectColision(directionV2, personalRigidbody.position, maxSpeed );
                 frameCounter = 0;
            }
           frameCounter += 1;
            velocity = personalRigidbody.linearVelocity;

            if (velocity.sqrMagnitude > 0.01f)
            {
            UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.LookRotation(velocity.normalized);

                modelTransform.rotation = UnityEngine.Quaternion.Slerp(
                modelTransform.rotation,
             targetRotation,
                rotationSpeed * Time.deltaTime
            );
}
        }
    }
    void detectColision(UnityEngine.Vector3 normalizedDirection,UnityEngine.Vector3 position, int checkDistance )
    {
        RaycastHit coll;
        Physics.Raycast(position, normalizedDirection, out coll, checkDistance);
        //Debug.DrawLine(position, position + normalizedDirection * checkDistance, Color.white, 10f);
        if (coll.collider != null && !coll.collider.gameObject.CompareTag("Player"))
        {   
            //Debug.DrawLine(position, position + normalizedDirection * checkDistance, Color.red, 10f);
           int angle = angleStart;
            angleSearch(position,normalizedDirection,checkDistance,angle);
        }else if (successfullRouteFindCounter >= successfullRouteFindInterval)
        {
            tempTarget = mainTarget.transform.position;
            //Debug.log("Changing route back");
            successfullRouteFindCounter=0;
        }
        successfullRouteFindCounter += 1;
    }
    private void angleSearch(UnityEngine.Vector3 position,UnityEngine.Vector3 normalizedDirection, int checkDistanceStart, int angle)
    {   
        var checkDistance = checkDistanceStart + startingCheckDistanceMultiIter;
        angle = angle + angleIter;
        //Debug.log("Angle " +angle);
        UnityEngine.Vector3[] scannedDirection = new UnityEngine.Vector3[] {UnityEngine.Vector3.up,UnityEngine.Vector3.down,UnityEngine.Vector3.left,UnityEngine.Vector3.right};
            List<CollisionCheckBeam> hits = new List<CollisionCheckBeam>();
            foreach (UnityEngine.Vector3 v in scannedDirection)
            {
                 RaycastHit temp = new RaycastHit();
                UnityEngine.Vector3 dir = (UnityEngine.Quaternion.AngleAxis(angle, v) * normalizedDirection).normalized;

                Physics.Raycast(position, dir, out temp, checkDistance);
                //Debug.DrawLine(position, position + dir * checkDistance, Color.blue, 10f);
                hits.Add(new CollisionCheckBeam(position + dir * checkDistance,temp));
                ////Debug.log("Collision with object imminent");
            }
            List<CollisionCheckBeam> freeCourse = hits.Where(n=>n.Hit.collider==null).ToList();
            if (freeCourse.Count()>0)
            {
                tempTarget = freeCourse.OrderByDescending(f=>UnityEngine.Vector3.Distance(f.End, tempTarget)).ToList().First().End;
                successfullRouteFindCounter=0;
                //Debug.DrawLine(position,tempTarget, Color.green, 2f);
                //Debug.log("Found alternative path");
            }else if (angle<angleMax)
                {
                        angleSearch(position,normalizedDirection,checkDistance,angle);
                        }
            else
            {
                tempTarget = hits.OrderByDescending(h=>h.Hit.distance).First().Hit.point;
                //Debug.DrawLine(position,tempTarget, Color.purple, 10f);
                //Debug.log("Looking for path");
            }
    }

    public struct CollisionCheckBeam{
        public CollisionCheckBeam(UnityEngine.Vector3 end, RaycastHit hit)
        {
           End=end;
           Hit=hit;

        }
        public UnityEngine.Vector3 End {get;}
        public RaycastHit Hit {get;}
        
    }
}
