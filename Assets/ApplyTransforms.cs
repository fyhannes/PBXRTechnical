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
    public Material ringMat;
    
    private MeshRenderer renderer;
    private Vector3 extents;
    
    Mesh ringMesh;
    GameObject xzRing;
    GameObject xyRing;
    GameObject yzRing;
    // MeshRenderer xzRingMR;
    // MeshRenderer xyRingMR;
    // MeshRenderer yzRingMR;
    // MeshFilter xzRingMF;
    // MeshFilter xyRingMF;
    // MeshFilter yzRingMF;
    
    // Start is called before the first frame update
    void Start()
    {
        // First, we create the three rings.
        renderer = GetComponent<MeshRenderer>();
        extents = renderer.bounds.size;
        extents.x *= transform.localScale.x;
        extents.y *= transform.localScale.y;
        extents.z *= transform.localScale.z;
        CreateRing("XZ");
        CreateRing("XY");
        CreateRing("YZ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * This function is used to instantiate the rings that will allow the transformation
     * of an object.
     */
    void CreateRing(string plane)
    {
        // TODO: See if I can refactor the duplicate code down here once I have functionality in place
        // First, we get the extents of the bounding box of this GameObject
        // (only works if the object has a MeshRenderer component )
        if (plane == "XZ") 
        {
            xzRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            xzRing.name = plane;
            xzRing.transform.localPosition = transform.position;
            xzRing.transform.parent = transform;
            float extent = Mathf.Max(extents.x, extents.z);
            xzRing.transform.localScale = new Vector3(extent + 0.5f, 0.001f, extent + 0.5f);
            xzRing.transform.localRotation = Quaternion.identity;
        } 
        else if (plane == "XY") 
        {
            xyRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            xyRing.name = plane;
            xyRing.transform.position = transform.position;
            xyRing.transform.parent = transform;
            float extent = Mathf.Max(extents.x, extents.y);
            xyRing.transform.localScale = new Vector3(extent + 0.5f, 0.001f, extent + 0.5f);
            xyRing.transform.localRotation = Quaternion.identity;
            xyRing.transform.Rotate(0, 90.0f, 90.0f);
        } 
        else if (plane == "YZ")
        {
            yzRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            yzRing.name = plane;
            yzRing.transform.position = transform.position;
            yzRing.transform.parent = transform;
            float extent = Mathf.Max(extents.z, extents.y);
            yzRing.transform.localScale = new Vector3(extent + 0.5f, 0.001f, extent + 0.5f);
            yzRing.transform.localRotation = Quaternion.identity;
            yzRing.transform.Rotate(0, 0, 90.0f);
        }
        else
        {
            // We should never be here. 
        }

    }
    
    void RotateObj()
    {
    }

    void ScaleObj()
    {

    }

    void TranslateObj()
    {

    }

}
