using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            g.transform.localScale *= (1 -  i / numDotLine);
            dots.Add(g);
        }

    }

    private void Update()
    {
        if(Time.timeScale > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDrag = true;
                OnStartDrag();
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isDrag = false;
                OnEndDrag();
            }

            if (Input.GetMouseButton(0))
                OnDrag();

        }
    }

    private void FixedUpdate()
    {
        if (isOutCamera() && countResetPos <= 0f)
        {
            countResetPos = 1f;
            return;
        }
        else if(countResetPos > 0f)
        {
            countResetPos -= Time.fixedDeltaTime;
        }
        if(isOutCamera() && countResetPos <= 0)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
            transform.position = posSpawn;
        }
    }

    public void OnDrag()
    {
        Debug.Log("Dragggg");

        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

    public void OnEndDrag()
    {
        Debug.Log("End Drag");
        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;
        //rb.angularVelocity = 0f;
        rb.AddForce(CaculatoVelocity(), ForceMode2D.Impulse);
        //rb.velocity = CaculatoVelocity();
        dotParent.gameObject.SetActive(false);
    }

    public void OnStartDrag()
    {
        Debug.Log("Start Drag");

        rb.isKinematic = true;
        dotParent.gameObject.SetActive(true);
        startDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector2 CaculatoVelocity()
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
