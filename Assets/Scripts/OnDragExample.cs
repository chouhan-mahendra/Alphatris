//Attach this script to the GameObject you would like to detect dragging on
//Attach an Event Trigger component to the GameObject (Click the Add Component button and go to Event>Event Trigger)
//Make sure the Camera you are using has a Physics Raycaster (Click the Add Component button and go to Event>Physics Raycaster) so it can detect clicks on GameObjects.

using UnityEngine;
using UnityEngine.EventSystems;

public class OnDragExample : MonoBehaviour
{
    void Start()
    {
        //Fetch the Event Trigger component from your GameObject
        EventTrigger trigger = GetComponent<EventTrigger>();
        //Create a new entry for the Event Trigger
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //Add a Drag type event to the Event Trigger
        entry.eventID = EventTriggerType.Drag;
        //call the OnDragDelegate function when the Event System detects dragging
        entry.callback.AddListener((data) => { OnDragSelectDelgate((PointerEventData)data); });
        //Add the trigger entry
        trigger.triggers.Add(entry);

    }

    public void OnDragDelegate(PointerEventData data)
    {
        //Create a ray going from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(data.position);
        //Calculate the distance between the Camera and the GameObject, and go this distance along the ray
        Vector3 rayPoint = ray.GetPoint(Vector3.Distance(transform.position, Camera.main.transform.position));
        //Move the GameObject when you drag it
        transform.position = new Vector3(rayPoint.x, rayPoint.y, transform.position.z);
    }

    public void OnDragSelectDelgate(PointerEventData data)
    {
        var ray = Camera.main.ScreenPointToRay(data.position);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.tag.Equals("Cube"))
            {
                Alphabet alphabet = hitObject.GetComponent<Alphabet>();
                Debug.Log("onDragged select " + alphabet.character);
                alphabet.SetIsSelected(true);
                MenuController.Instance.setDrag(true);
            }
        }
    }
}