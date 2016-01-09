using UnityEngine;
using System.Collections;

public class FloatNum : MonoBehaviour
{
    Vector2 StartPos;

    void Awake()
    {
        StartPos = transform.position;
    }

    void OnEnable()
    {
        transform.position = StartPos;
    }

    void OnDisable()
    {
        transform.position = StartPos;
    }

	void Update()
    {
        this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y + 1.0f * Time.deltaTime);
    }
}
