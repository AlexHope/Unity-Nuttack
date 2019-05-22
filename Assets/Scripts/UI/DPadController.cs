using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Text;

public class DPadController : MonoBehaviour
{
    public enum Direction { None, Up, Down, Left, Right };

    public PlayerController player;

    private Direction m_currentDirection;
    private RectTransform m_dpadRectTransform;

    // Use this for initialization
    void Start()
    {
        m_dpadRectTransform = GetComponent<RectTransform>();
    }

    public Direction CurrentDirection { get { return m_currentDirection; } }

    void CalculateCurrentDirection()
    {
        m_currentDirection = Direction.None;

        if (Input.touches.Length == 0)
        {
            return;
        }

        Vector2 center = new Vector2(m_dpadRectTransform.position.x, m_dpadRectTransform.position.y);
        Vector2 currentTouchedPoint = new Vector2();
        Vector2 toTouched = new Vector2();

        bool check = false;

        foreach (Touch touch in Input.touches)
        {
            currentTouchedPoint = touch.position;/*new Vector2(Input.mousePosition.x, Input.mousePosition.y);*/
            toTouched = currentTouchedPoint - center;

            if (toTouched.magnitude <= GetComponent<RectTransform>().rect.width)
            {
                check = true;
                break;
            }
        }

        if (!check)
        {
            return;
        }

        Vector2 upVector = new Vector2(center.x, center.y + 100) - center;

        float angle = Vector2.Angle(upVector, toTouched);

        if (angle <= 45)
        {
            m_currentDirection = Direction.Up;
        }
        else if (angle >= 145)
        {
            m_currentDirection = Direction.Down;
        }
        else if (angle > 45 && angle < 145)
        {
            if (center.x < currentTouchedPoint.x)
            {
                m_currentDirection = Direction.Right;
            }
            else
            {
                m_currentDirection = Direction.Left;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        player.Blocking = false;
        player.AimingUp = false;

        CalculateCurrentDirection();

        switch (m_currentDirection)
        {
            case Direction.Left: player.MoveDirection = -1;
                break;
            case Direction.Right: player.MoveDirection = 1;
                break;
            case Direction.Up: player.AimingUp = true;
                break;
            case Direction.Down: if (player.CanBlock) player.Blocking = true;
                break;
        }
    }
}
