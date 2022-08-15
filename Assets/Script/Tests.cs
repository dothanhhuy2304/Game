using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Tests : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update
    private Camera cam;
    [SerializeField] private float movingSpeed = 10f;
    [SerializeField] private Rigidbody2D rb;
    [Header("SetUp Patrol")] [SerializeField]
    private Vector3 target;

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float timeDurationMoving;
    private bool isFlip;

    [SerializeField] private GameObject targetA;
    [SerializeField] private GameObject targetB;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Touch");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var position = targetA.transform.position;
        var position1 = targetB.transform.position;
        float a = Vector3.Distance(position, position1);
        float b = (position - position1).magnitude;
        float c = (position - position1).sqrMagnitude;
        Debug.Log(a);
        Debug.Log(b);
        Debug.Log(c);
        TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        DateTime timeNow = TimeZoneInfo.ConvertTime(DateTime.Now, tzi);
        string dateNow = timeNow.Hour.ToString("00:00");
        Debug.Log(dateNow);

        Move();
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit2D)
            {
                Debug.Log(hit2D.collider.gameObject.name);
            }
        }
    }

    private void Move()
    {
        if (transform.position != target)
        {
            Vector3 moveDir = Vector3.MoveTowards(transform.position, target, movingSpeed * Time.fixedDeltaTime);
            rb.MovePosition(moveDir);
        }
        else
        {
            if (target == waypoints[0].position)
            {
                if (isFlip)
                {
                    isFlip = !isFlip;
                    StartCoroutine(SetTarget(waypoints[1].position, timeDurationMoving));
                }
            }
            else
            {
                if (!isFlip)
                {
                    isFlip = !isFlip;
                    StartCoroutine(SetTarget(waypoints[0].position, timeDurationMoving));
                }
            }
        }
    }

    private IEnumerator SetTarget(Vector3 pos, float timeSleep)
    {
        yield return new WaitForSeconds(timeSleep);
        target = pos;
        FaceToWards(pos - transform.position);
    }

    void FaceToWards(Vector3 direction)
    {
        if (direction.x > 0f)
        {
            transform.localEulerAngles = new Vector3(0, 180f, 0);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(eventData.position);
    }
}
