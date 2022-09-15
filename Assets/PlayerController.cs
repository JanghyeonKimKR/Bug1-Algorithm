using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject target;
    int mode;                                   // 1 : motion-to-goal  ,  2 : boundary following
    Vector3 dir, curr, pre;
    float angle, diff, speed, d_angle;
    bool contact;
    Vector3 hitting_point, leave_point;
    int count;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Mode : Motion-to-goal...");
        mode = 1;
        speed = 0.3f;
        curr = transform.position;
        pre = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // update curr, pre
        pre = curr;
        curr = transform.position;

        // operator
        if (mode == 1)
            Motion_to_Goal();
        else
            Boundary_Following();
    }

    // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌을 시작했을 때
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Obstacle")) 
        { 
            if (mode == 1) { 
                Debug.Log("Mode : Boundary-Fallowing...");
                mode = 2;
                contact = true;
                hitting_point = transform.position;
                leave_point = transform.position;
                count = 0;
                d_angle = 0.5f;
            }
        }
    }

    // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌중일 때
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Obstacle"))
        {
            contact = true;
        }
    }

    // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌이 끝났을 때
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Obstacle"))
        {
            contact = false;
        }
    }

    // Functions for Bug1 Algorithm
    private void Motion_to_Goal()
    {
        // compute dir, angle
        dir = target.transform.position - transform.position;
        angle = Quaternion.FromToRotation(Vector3.forward, dir).eulerAngles.y;

        // translation + rotaton
        transform.position += dir * speed * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }

    private void Boundary_Following()
    {
        // Hitting Point Check
        if (d_angle > 0)
        {
            if (Vector3.Distance(curr, hitting_point) < 0.05 && count > 1000)
            {
                d_angle = -d_angle;
                angle += 180;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                return;
            }
            else
                count++;
        }
        // Leave Point Check
        else
        {
            if(Vector3.Distance(curr, leave_point) < 0.05)
            {
                mode = 1;
                return;
            }

        }

        // Update Leave Point
        if (Vector3.Distance(curr, target.transform.position) < Vector3.Distance(leave_point, target.transform.position))
            leave_point = curr;

        // Compute dir, angle
        diff = Vector3.Distance(curr, pre);
        angle = transform.rotation.eulerAngles.y;
        if (diff < 0.01 | contact)
        {
            if (!contact)
                angle -= d_angle;
            else
                angle += d_angle;
        }
        dir = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));


        // translation + rotaton
        transform.position += 3 * dir * speed * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}


