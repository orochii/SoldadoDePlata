using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour {
    private static bool waiting;
    public static bool Waiting { get { return waiting; } set { waiting = value; } }

    private static List<GameEvent> allEvents = new List<GameEvent>();

    public static void CheckAllPages() {
        foreach(GameEvent e in allEvents) {
            e.CheckPages();
        }
    }

    [System.Serializable]
    public class EventCondition {
        public int switchId;
        public GameEventPage page;
    }

    [SerializeField] private EventCondition[] pages;
    private int currentPage = 0;

    private void CheckPages() {
        currentPage = 0;
        // Find current page (last ones get priority)
        for (int i = 0; i < pages.Length; i++) {
            bool globalSwitch = pages[i].switchId < 0 || GameManager.GetSwitch(pages[i].switchId);
            if (globalSwitch) {
                currentPage = i;
            }
        }
        // Enable/disable pages so there is only one active.
        for (int i = 0; i < pages.Length; i++) {
            pages[i].page.gameObject.SetActive(currentPage == i);
        }
    }

    private void Awake() {
        allEvents.Add(this);
        CheckPages();
    }

    private void OnDestroy() {
        allEvents.Remove(this);
    }
}
