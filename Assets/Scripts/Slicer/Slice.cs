using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Slicer
{
    public class Slice
    {
        
        public static GameObject[] SliceGameObject(Plane plane, GameObject objectToCut)
        {
                
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;                
            SliceMetaData sliceMetaData = new SliceMetaData(plane, mesh);


            var positiveObject = CreateMeshGameObject(objectToCut);
            positiveObject.name = string.Format("{0}Positive", objectToCut.name);


            var negativeObject = CreateMeshGameObject(objectToCut);
            negativeObject.name = string.Format("{0}Negative", objectToCut.name);
                
            var positiveSideMesh = sliceMetaData.PositiveMesh;                
            var negativeSideMesh = sliceMetaData.NegativeMesh;
                
            positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMesh;
            negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMesh;
                
            SetupColliderAndRididbody(ref positiveObject, positiveSideMesh);                
            SetupColliderAndRididbody(ref negativeObject, negativeSideMesh);
                
            return new GameObject[] { positiveObject, negativeObject };
        }

        private static GameObject CreateMeshGameObject(GameObject originalObject)
        {
            var originalMaterials = originalObject.GetComponent<MeshRenderer>().materials;
            originalMaterials[0].mainTextureOffset = new Vector2(0.94f, -0.14f);

            GameObject newMeshObject = new GameObject();
            newMeshObject.AddComponent<MeshFilter>();
            newMeshObject.AddComponent<MeshRenderer>();
            newMeshObject.AddComponent<Sliceable>();

            newMeshObject.GetComponent<MeshRenderer>().materials = originalMaterials;

            newMeshObject.transform.localPosition = originalObject.transform.localPosition;
            newMeshObject.transform.localRotation = originalObject.transform.localRotation;
            newMeshObject.transform.localScale = originalObject.transform.localScale;

            newMeshObject.tag = originalObject.tag;

            return newMeshObject;
        }

        private static void SetupColliderAndRididbody(ref GameObject gameObject, Mesh mesh)
        {
            var collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
            collider.convex = true;

            var rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = true;
        }
    }
}