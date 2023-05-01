/**
 * ApplyTransforms allows a mesh to have its transform components be manipulated
 * via interactable rings that statically orbit the object.
 * @author Johannes Fung
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTransforms : MonoBehaviour
{
    /* Affects how quickly/slowly the transform will scale/translate/rotate */
    [SerializeField]
    private float scaleFactor = 0.001f;
    private float scaleDamping = 0.1f;
    [SerializeField]
    private float translateFactor = 0.001f;
    [SerializeField]
    private float angularVelocity = 0.1f;

    /* These parameters are used for scaling the scale/translate support mesh */
    private float scaleObjOffset = 1.5f;
    private float translateObjOffset = 1.8f;

    /* Used in the update function to perform calculations/trigger events */
    private Vector2 mousePos;
    private Camera mainCamera;
    private bool down;
    private GameObject target;

    /* Used in calculating the extents of this transform */
    private MeshRenderer renderer;
    private Vector3 extents;

    /* These GameObjects are used to mutate a transform */
    private GameObject xzRing;
    private GameObject xyRing;
    private GameObject yzRing;
    private GameObject xzScale;
    private GameObject xyScale;
    private GameObject yzScale;
    private GameObject xzTranslate;
    private GameObject xyTranslate;
    private GameObject yzTranslate;

    /* Vector3s used in translating/scaling */
    Vector3 scaleXZChange;
    Vector3 translateXZChange;

    Vector3 scaleXYChange;
    Vector3 translateXYChange;

    Vector3 scaleYZChange;
    Vector3 translateYZChange;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePos.x = mousePosition.x;
        mousePos.y = mousePosition.y;
        // First, we create the three rings.
        renderer = GetComponent<MeshRenderer>();
        extents = renderer.bounds.size;
        extents.x *= transform.localScale.x;
        extents.y *= transform.localScale.y;
        extents.z *= transform.localScale.z;
        CreateRing("XZ");
        CreateRing("XY");
        CreateRing("YZ");

        // We assign mesh colliders to each ring here.
        foreach (Transform childObject in transform)
        {
            // First we get the Mesh attached to the child object
            Mesh mesh = childObject.gameObject.GetComponent<MeshFilter>().mesh;
            // If we've found a mesh we can use it to add a collider
            if (mesh != null)
            {              
                CapsuleCollider collider = childObject.GetComponent<CapsuleCollider>();
                Destroy(collider);  
                // Add a new MeshCollider to the child object
                MeshCollider meshCollider = childObject.gameObject.AddComponent<MeshCollider>();
 
                // Finaly we set the Mesh in the MeshCollider
                meshCollider.sharedMesh = mesh;
            }
        }

        mainCamera = Camera.main;
        down = false;
        scaleXZChange = new Vector3(0, 0, scaleFactor);
        scaleXYChange = new Vector3(scaleFactor, 0, 0);
        scaleYZChange = new Vector3(0, scaleFactor, 0);
        translateXZChange = new Vector3(0, 0, translateFactor);
        translateXYChange = new Vector3(translateFactor, 0, 0);
        translateYZChange = new Vector3(0, translateFactor, 0);
    }

    void UpdateMousePos()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePos.x = mousePosition.x;
        mousePos.y = mousePosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            down = true;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                
                if (hit.collider != null)
                {
                    target = hit.collider.gameObject;
                    // Debug.Log("Left-click hit: " + hit.collider.gameObject.name);
                }
            }
        }
        if (down)
        {
            if (Input.GetMouseButtonUp(0))
            {
                down = false;
            }
            // If we're grabbing a rotation object.
            if (target == xzRing || target == yzRing || target == xyRing)
            {
                RotateObj();
            }
            else if (target == xzScale || target == yzScale || target == xyScale)
            {
                ScaleObj();
            }
            else if (target == xzTranslate || target == yzTranslate || target == xyTranslate)
            {
                TranslateObj();
            }
        }
        UpdateMousePos();
    }

    /**
     * This function is used to instantiate the rings that will allow the transformation
     * of an object.
     */
    void CreateRing(string plane)
    {
        /** 
         * Logically, each if case statement functions the same:
         * We create a cylinder for each plane and then we stretch/squish it to match the
         * extents of the object. In order to ensure that the radius of the ring is even,
         * the ring will be stretched out by the maximum of the (i, j) length where i,j
         * is dependent on the plane i.e. if we're looking at the xz-plane, then i,j correspond to x,z.
         * After the cylinder is created and scaled accordingly, rotate it to match the plane.
         * Then, we create the scale and translate objects and set them in place.
         */
        if (plane == "XZ") 
        {
            // Create the ring object
            xzRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            xzRing.name = plane + " ring";
            xzRing.transform.localPosition = transform.position;
            xzRing.transform.parent = transform;
            float extent = Mathf.Max(extents.x, extents.z);
            xzRing.transform.localScale = new Vector3(extent + 0.8f, 0.005f, extent + 0.8f);
            xzRing.transform.localRotation = Quaternion.identity;

            // Create the scale object
            xzScale = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            xzScale.name = plane + " scale";
            xzScale.transform.parent = transform;
            xzScale.transform.localPosition = new Vector3(0, 0, scaleObjOffset);
            xzScale.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            xzScale.transform.localRotation = Quaternion.identity;

            // Create the translate object
            xzTranslate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            xzTranslate.transform.parent = transform;
            xzTranslate.transform.localPosition = new Vector3(0, 0, translateObjOffset);
            xzTranslate.name = plane + " translate";
            xzTranslate.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            xzTranslate.transform.localRotation = Quaternion.identity;
        } 
        else if (plane == "XY") 
        {
            xyRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            xyRing.name = plane;
            xyRing.transform.position = transform.position;
            xyRing.transform.parent = transform;
            float extent = Mathf.Max(extents.x, extents.y);
            xyRing.transform.localScale = new Vector3(extent + 0.8f, 0.005f, extent + 0.8f);
            xyRing.transform.localRotation = Quaternion.identity;
            xyRing.transform.Rotate(0, 90.0f, 90.0f);

            xyScale = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            xyScale.name = plane + " scale";
            xyScale.transform.parent = transform;
            xyScale.transform.localPosition = new Vector3(scaleObjOffset, 0, 0);
            xyScale.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            xyScale.transform.localRotation = Quaternion.identity;

            xyTranslate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            xyTranslate.transform.parent = transform;
            xyTranslate.transform.localPosition = new Vector3(translateObjOffset, 0, 0);
            xyTranslate.name = plane + " translate";
            xyTranslate.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            xyTranslate.transform.localRotation = Quaternion.identity;

        } 
        else if (plane == "YZ")
        {
            yzRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            yzRing.name = plane;
            yzRing.transform.position = transform.position;
            yzRing.transform.parent = transform;
            float extent = Mathf.Max(extents.z, extents.y);
            yzRing.transform.localScale = new Vector3(extent + 0.8f, 0.005f, extent + 0.8f);
            yzRing.transform.localRotation = Quaternion.identity;
            yzRing.transform.Rotate(0, 0, 90.0f);

            yzScale = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            yzScale.name = plane + " scale";
            yzScale.transform.parent = transform;
            yzScale.transform.localPosition = new Vector3(0, scaleObjOffset, 0);
            yzScale.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            yzScale.transform.localRotation = Quaternion.identity;

            yzTranslate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            yzTranslate.transform.parent = transform;
            yzTranslate.transform.localPosition = new Vector3(0, translateObjOffset, 0);
            yzTranslate.name = plane + " translate";
            yzTranslate.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            yzTranslate.transform.localRotation = Quaternion.identity;
        }
    }
    
    void RotateObj()
    {
        Vector3 mousePosition = Input.mousePosition;
        float offset = mousePosition.x - mousePos.x;
        // Short circuit to prevent shenanigans from happening when mouse doesn't move
        if (offset == 0) return;
        float sign = offset > 0 ? 1.0f : -1.0f;
        if (target == xzRing)
        {
            transform.Rotate(0, angularVelocity * sign, 0);
        }
        else if (target == yzRing)
        {
            transform.Rotate(angularVelocity * sign, 0, 0);
        }
        else if (target == xyRing)
        {
            transform.Rotate(0, 0, angularVelocity * sign);
        }
    }

    void ScaleObj()
    {
        Vector3 mousePosition = Input.mousePosition;
        float offset = mousePosition.x - mousePos.x;
        // Short circuit to prevent shenanigans from happening when mouse doesn't move
        if (offset == 0) return;
        float sign = offset > 0 ? 1.0f : -1.0f;
        Debug.Log(transform.localScale);
        if (target == xzScale)
        {
            transform.localScale += scaleXZChange * sign;
        }
        else if (target == yzScale)
        {
            transform.localScale += scaleYZChange * sign;
        }
        else if (target == xyScale)
        {
            transform.localScale += scaleXYChange * sign;
        }
    }

    void TranslateObj()
    {
        Vector3 mousePosition = Input.mousePosition;
        float offset = mousePosition.x - mousePos.x;
        // Short circuit to prevent shenanigans from happening when mouse doesn't move
        if (offset == 0) return;
        float sign = offset > 0 ? 1.0f : -1.0f;
        Debug.Log(transform.localPosition);
        if (target == xzScale)
        {
            transform.localPosition += translateXZChange * sign;
        }
        else if (target == yzScale)
        {
            transform.localPosition += translateYZChange * sign;
        }
        else if (target == xyScale)
        {
            transform.localPosition += translateXYChange * sign;
        }
    }

}
