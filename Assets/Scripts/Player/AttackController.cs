using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour {
    [SerializeField] private BladeAttack meleeWeapon;
    [SerializeField] private PlayerGrab grab;
    [SerializeField] private Vector3 grabOffset;
    [SerializeField] private Vector3 grabSize;
    [SerializeField] private LayerMask grabbableMask;
    [SerializeField] private ParticleSystem[] feetParticles;

    private Animator anim;
    private int hashBGrabbing;
    private Renderer[] allRenderers;
    private Coroutine flickerCoroutine;

    private void Awake() {
        anim = GetComponent<Animator>();
        hashBGrabbing = Animator.StringToHash("grabbing");
        allRenderers = GetComponentsInChildren<Renderer>();
    }

    public void Flicker(float interval, int times) {
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        flickerCoroutine = StartCoroutine(DoFlickering(interval, times));
    }
    public IEnumerator DoFlickering(float interval, int times) {
        float halfInterval = interval / 2;
        while (times > 0) {
            SetVisible(false);
            yield return new WaitForSeconds(halfInterval);
            SetVisible(true);
            yield return new WaitForSeconds(halfInterval);
            times--;
        }
    }

    private void SetVisible(bool v) {
        foreach(Renderer r in allRenderers) {
            r.enabled = v;
        }
    }

    public void SetEnable(int value) {
        meleeWeapon.Active = value==1;
    }

    public void DoGrab() {
        Vector3 pos = transform.position + (transform.rotation * grabOffset);
        Collider[] foundObjects = Physics.OverlapBox(pos, grabSize / 2, transform.rotation, grabbableMask);
        Grabbable toGrab = null;
        float dist = float.MaxValue;
        foreach (Collider coll in foundObjects) {
            Grabbable g = coll.GetComponent<Grabbable>();
            if (g != null) {
                if (toGrab == null) toGrab = g;
                else {
                    float d = Vector3.Distance(transform.position, g.transform.position);
                    if (dist > d) {
                        dist = d;
                        toGrab = g;
                    }
                }
            }
        }
        if (toGrab != null) {
            grab.Grab(toGrab);
            anim.SetBool(hashBGrabbing, true);
        }
    }
    public void DoToss() {
        grab.Toss();
        anim.SetBool(hashBGrabbing, false);
    }

    public void EmitSoilParticles(int idx) {
        feetParticles[idx].Emit(30);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(grabOffset, grabSize);
    }
}
