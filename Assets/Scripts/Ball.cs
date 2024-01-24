using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ball : MonoBehaviour
{
    [SerializeField] float numDotLine;

    [SerializeField] float Power = 10f;

    [Range(0, 1)]
    [SerializeField] float dotSpace;

    [SerializeField] Transform dotParent;

    [SerializeField] GameObject dotPrefabs;

    private List<GameObject> dots;

    Rigidbody2D rb;

    private bool _isDrag;

    public bool isDrag { get => _isDrag; }

    Vector2 startDragPos;

    Vector2 posSpawn;

    private float countResetPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dots = new List<GameObject>();
        posSpawn = transform.position;
        for (int i = 0; i < numDotLine; i++)
        {
            var g = Instantiate(dotPrefabs, dotParent);
            g.transform.localPosition = new Vector3(0, 0, 0);
            g.transform.localScale *= (1 - i / numDotLine);
            dots.Add(g);
        }

    }

    private void OnGUI()
    {

    }

    private void Update()
    {
        if (Time.timeScale > 0)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                _isDrag = true;
                OnStartDrag(Input.GetTouch(0).position);
            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
                && _isDrag)
            {
                _isDrag = false;
                OnEndDrag(Input.GetTouch(0).position);
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && _isDrag)
                OnDrag(Input.GetTouch(0).position);
#else

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _isDrag = true;
                OnStartDrag(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _isDrag = false;
                OnEndDrag(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
                OnDrag(Input.mousePosition);
#endif
        }
    }

    private void FixedUpdate()
    {
        if (isOutCamera() && countResetPos <= 0f)
        {
            countResetPos = 1f;
            return;
        }
        else if (countResetPos > 0f)
        {
            countResetPos -= Time.fixedDeltaTime;
        }
        if (isOutCamera() && countResetPos <= 0)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
            transform.position = posSpawn;
        }
    }

    public void OnDrag(Vector3 position)
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(position);
        float distance = Vector2.Distance(currentPos, startDragPos);
        var force = (startDragPos - currentPos).normalized * distance * Power;
        float space = dotSpace;
        float drag = 1 - dotSpace * rb.drag;
        for (int i = 0; i < dots.Count; i++)
        {
            float x = this.transform.position.x + force.x * space;
            float y = this.transform.position.y + force.y * space - (Physics2D.gravity.magnitude * space * space) / 2f;
            this.dots[i].transform.position = new Vector3(x, y);
            space += dotSpace;
        }

    }

    public void OnEndDrag(Vector3 position)
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(position);
        float distance = Vector2.Distance(currentPos, startDragPos);
        if (distance < .2f)
            return;
        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;
        //rb.angularVelocity = 0f;
        rb.AddForce(CaculatoVelocity(position), ForceMode2D.Impulse);
        //rb.velocity = CaculatoVelocity();
        dotParent.gameObject.SetActive(false);
    }

    public void OnStartDrag(Vector3 position)
    {
        rb.isKinematic = true;
        dotParent.gameObject.SetActive(true);
        startDragPos = Camera.main.ScreenToWorldPoint(position);
    }

    private Vector2 CaculatoVelocity(Vector3 position)
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(position);
        float distance = Vector2.Distance(currentPos, startDragPos);
        return (startDragPos - currentPos).normalized * distance * Power;
    }

    private bool isOutCamera()
    {
        Vector3 screenView = new Vector3(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        Vector2 min = Camera.main.transform.position - screenView;

        Vector2 max = Camera.main.transform.position + screenView;

        Vector2 pos = transform.position;
        return (pos.x < min.x || pos.y < min.y || pos.x > max.x);

    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Finish") && rb.velocity == Vector2.zero)
        {
            GameManager.Instance.GameSuccess();
        }
    }
}
