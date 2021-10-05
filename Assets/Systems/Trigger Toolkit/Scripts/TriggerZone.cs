using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerCondition
{
    public enum EType
    {
        Enter,
        Exit,
        Stay
    }

    [SerializeField] EType Type;
    [SerializeField] List<string> Tags;
    [SerializeField] float FloatValue;
    [SerializeField] UnityEvent OnTrigger = new UnityEvent();

    [System.NonSerialized] Dictionary<GameObject, float> EntryTimes = new Dictionary<GameObject, float>();
    [System.NonSerialized] Dictionary<GameObject, bool> NotificationSent = new Dictionary<GameObject, bool>();

    bool CheckTags(string tag)
    {
        return Tags.Contains(tag);
    }

    public void OnTriggerEnter(Collider other)
    {
        // type doesn't match
        if (Type != EType.Enter)
            return;

        // do nothing if the tag doesn't match
        if (!CheckTags(other.gameObject.tag))
            return;

        Debug.Log("Enter");
        OnTrigger.Invoke();
    }

    public void OnTriggerExit(Collider other)
    {
        EntryTimes.Remove(other.gameObject);
        NotificationSent.Remove(other.gameObject);

        // type doesn't match
        if (Type != EType.Exit)
            return;

        // do nothing if the tag doesn't match
        if (!CheckTags(other.gameObject.tag))
            return;

        Debug.Log("Exit");
        OnTrigger.Invoke();
    }

    public void OnTriggerStay(Collider other)
    {
        // type doesn't match
        if (Type != EType.Stay)
            return;

        // do nothing if the tag doesn't match
        if (!CheckTags(other.gameObject.tag))
            return;

        if (EntryTimes.ContainsKey(other.gameObject) && !NotificationSent.ContainsKey(other.gameObject))
        {
            if ((Time.time - EntryTimes[other.gameObject]) > FloatValue)
            {
                NotificationSent[other.gameObject] = true;
                Debug.Log("Stay");
                OnTrigger.Invoke();
            }
        }
        else
            EntryTimes[other.gameObject] = Time.time;
    }    
}

[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour
{
    [SerializeField] List<TriggerCondition> Conditions;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        for (int index = 0; index < Conditions.Count; ++index)
            Conditions[index].OnTriggerEnter(other);
    }

    void OnTriggerExit(Collider other)
    {
        for (int index = 0; index < Conditions.Count; ++index)
            Conditions[index].OnTriggerExit(other);
    }

    void OnTriggerStay(Collider other)
    {
        for (int index = 0; index < Conditions.Count; ++index)
            Conditions[index].OnTriggerStay(other);
    }
}
