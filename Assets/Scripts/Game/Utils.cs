﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static Vector3 rotate90Y = new Vector3(0, 90, 0) ; 

  public static string Vector2INtToString(Vector2 vector2) {
        return "(" + (int)vector2.x + "," + (int)vector2.y + ")"; 
    }
    
    public static string Vector2IntToString(Vector2Int vector2Int) {
        return "(" + (int)vector2Int.x + "," + (int)vector2Int.y + ")"; 
    }

    public static Vector2Int Vector2IntAbs(Vector2Int input) {
        return new Vector2Int(Mathf.Abs(input.x), Mathf.Abs(input.y));
    }

    public static Vector2Int NormalizeVector2(Vector2Int input) {
        int x = 0, y = 0;   
        if(input.x != 0) {
            x = input.x / Mathf.Abs(input.x);
        }
        if (input.y != 0) {
            y = input.y / Mathf.Abs(input.y);
        }
        return new Vector2Int(x, y);
    }

    public static Vector2Int SwapVector2(Vector2Int input) {
        int x = input.x;
        input.x = input.y;
        input.y = x;
        return input;
    }

    public static Vector2Int Vector2FromDirection(Direction direc) {
        switch (direc) {
            case Direction.up:
                return new Vector2Int(0, 1);//0
            case Direction.down:
                return new Vector2Int(0, -1);//180
            case Direction.right:
                return new Vector2Int(1, 0);//90
            case Direction.left:
                return new Vector2Int(-1, 0);//270
            default:
                return new Vector2Int(0, 0);
        }
    }

    public static Vector2 SwapVector2(Vector2 input) {
        float x = input.x;
        input.x = input.y;
        input.y = x;
        return input;
    }

    public static Direction AngleToDirection(float angle) {
        angle = AngleTo90s(ReduceAngle(angle));
     //   Debug.Log("angle to direction angle " + angle);
        switch (angle) {
            case 0:
                return Direction.up;
            case 90:
                return Direction.right;
            case 180:
                return Direction.down;
            case 270:
                return Direction.left;
            default:
                return Direction.none;
        }
    }

    public static Direction AddDirection(Direction direc1, Direction direc2) {
        int angle = (int)direc1 + (int)direc2;
        return AngleToDirection(angle);
    }

    public static float AngleTo90s(float angle) {
        float angleDifference = angle % 90;
        if(angleDifference > 45) {
            angle += angleDifference;
        } else {
            angle -= angleDifference;
        }
        return Mathf.Round(angle);
    }

    public static float ReduceAngle(float angle) {
        while (angle < 0) {
            angle += 360;
        }
        return angle % 360;
    }

    public static float Angle(Vector2 from,Vector2 to) {
        return Mathf.Acos(Vector2.Dot(from, to) / (from.magnitude * to.magnitude));
    }

    public static Vector3 Vector3SetY(Vector3 input, float y) {
        return ChangeElemntVector3(input, y, 'y');
    }

    public static Vector3 ChangeElemntVector3(Vector3 input, float newNum, char setType) {
        switch(setType) {
            case 'x':
                return new Vector3(newNum, input.y, input.z);
            case 'y':
                return new Vector3(input.x, newNum, input.z);
            case 'z':
                return new Vector3(input.x, input.y, newNum);
            default:
                Debug.LogWarning(setType + " is not supported by this method");
                break;
        }
        return Vector3.zero;
    }

    public static Vector3 Vector3SetY(Transform transform) {
        return Vector3SetY(transform.position, GlobalPointers.itemHeight);
    }

    public static void CheckSingletonValid<T>(T instanceCheck) where T : MonoBehaviour {
        T[] temp = Transform.FindObjectsOfType<T>();
        if (temp.Length > 1) {
            foreach(T t in  temp) {
                Debug.Log(t);
            }
            throw new System.InvalidOperationException("you already have a singlton of type " + typeof(T));
        }
    }

    public static T GetFromDictionary<U,T>(U key, Dictionary<U, T> input) {
        T temp;
        input.TryGetValue(key, out temp);
        return temp;
    }
   
}
