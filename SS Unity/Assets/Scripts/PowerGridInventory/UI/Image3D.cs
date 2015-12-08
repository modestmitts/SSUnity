/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AncientCraftGames.UI
{
    /// <summary>
    /// Utility component for rendering 3D meshes on a uGUI canvas.
    /// This can be though of a a 3D mesh-based UI.Image component.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Ancient Craft Games/UI/Image 3D")]
    public class Image3D : Graphic
    {
        #region Members
        /// <summary>
        /// The 3D mesh to display within the confines of the RectTransform.
        /// </summary>
        public Mesh Mesh
        {
            get { return _Mesh; }
            set
            {
                _Mesh = value;
                SetVerticesDirty();
                this.canvasRenderer.SetMaterial(material, null);
            }
        }
        [SerializeField]
        [Tooltip("The 3D mesh to display within the confines of the RectTransform.")]
        private Mesh _Mesh;

        /*
        /// <summary>
        /// 
        /// </summary>
        public override Material material
        {
            get
            {
                return base.material;
            }
            set
            {
                base.material = value;
            }
        }

        /// <summary>
        /// The 3D mesh to display within the confines of the RectTransform.
        /// </summary>
        public Material Material
        {
            get { return _Material; }
            set
            {
                material = value;
                _Material = value;
                SetMaterialDirty();
            }
        }
        [SerializeField]
        [Tooltip("The material to apply to the 3D mesh.")]
        private Material _Material;
        */

        /// <summary>
        /// The angle to display the model within the RectTransform.
        /// </summary>
        public Vector3 Rotation
        {
            get { return _Rotation; }
            set
            {
                _Rotation = value;
                SetVerticesDirty();
            }
        }
        [SerializeField]
        [Tooltip("The angle to display the model within the RectTransform. NOTE: Changing this value frequently can significantly impact performance when recalculating the mesh to fit within the RectTransform.")]
        private Vector3 _Rotation;

        /// <summary>
        /// If set, the mesh will maintain its original height-to-width aspect ratio when scaling.
        /// </summary>
        [HideInInspector]
        [Tooltip("If set, the mesh will maintain its original height-to-width aspect ratio when scaling.")]
        public bool PreserveAspect = true;
        #endregion


        #region Methods
        protected override void UpdateMaterial()
        {
            if (IsActive())
                canvasRenderer.SetMaterial(materialForRendering, null);
        }

        /// <summary>
        /// Overriden OnEnable event.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            SetAllDirty();
        }
        
        /// <summary>
        /// Provide mesh verts to the CanvasRenderer.
        /// </summary>
        /// <param name="vbo"></param>
        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            if (Mesh == null)
            {
                vbo.Clear();
                return;
            }
            Vector3[] vertices = Mesh.vertices;
            int[] triangles = Mesh.triangles;
            Vector3[] normals = Mesh.normals;
            Vector2[] UVs = Mesh.uv;


            //Ok, so the only way we're going to get the aabb *after* rotation is
            //to rotate all vert *before* we do anything else.
            //TODO: Consider skipping this step if all rotations are zero.
            Bounds rotMeshBounds;
            if (Rotation.Equals(Vector3.zero)) rotMeshBounds = Mesh.bounds;
            else RotateVerts(ref vertices, ref normals, out rotMeshBounds);
            Vector3 scale = Vector3.one;
            float scaleX = rectTransform.rect.size.x / rotMeshBounds.size.x;
            float scaleY = rectTransform.rect.size.y / rotMeshBounds.size.y;

            //Decide how we are going to scale. In the event that we scale without ratio considerations
            //we'll take the smaller scale factor and use that for the z-axis.
            if (PreserveAspect) scale *= Mathf.Min(scaleX, scaleY);
            else scale = new Vector3(scaleX, scaleY, Mathf.Min(scaleX, scaleY));
            Vector3 offset = new Vector3(-rotMeshBounds.center.x, -rotMeshBounds.center.y, 0.0f);

            //start transforming and adding the model's verts to the UI canvas
            vbo.Clear();
            UIVertex temp = new UIVertex();
            for (int i = 0; i < triangles.Length; ++i)
            {
                //first thing we do it get our vert and offset it
                //so that it appears within the rect
                temp.position = (vertices[triangles[i]]) + offset;
                temp.position.Scale(scale);

                //set the normals, UVs and such
                temp.uv0 = UVs[triangles[i]];
                temp.normal = normals[triangles[i]];
                temp.color = color;
                vbo.Add(temp);
                //UI only accepts quads so we need to add an extra copy of every third vertex
                if (i % 3 == 0) vbo.Add(temp);
            }

        }
        
        /// <summary>
        /// Helper method to rotate the verts of a raw mesh. The rotated verts,
        /// normal, and the new resulting AABB size will be returned.
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="size"></param>
        void RotateVerts(ref Vector3[] verts, ref Vector3[] normals, out Bounds aabb)
        {
            Quaternion rotation = Quaternion.Euler(Rotation);
            float minX = 0.0f, maxX = 0.0f, minY = 0.0f, maxY = 0.0f, minZ = 0.0f, maxZ = 0.0f;

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = rotation * verts[i];
                normals[i] = rotation * normals[i];
                if (verts[i].x < minX) minX = verts[i].x;
                if (verts[i].y < minY) minY = verts[i].y;
                if (verts[i].x < minZ) minZ = verts[i].z;

                if (verts[i].x > maxX) maxX = verts[i].x;
                if (verts[i].y > maxY) maxY = verts[i].y;
                if (verts[i].y > maxZ) maxZ = verts[i].z;
            }

            aabb = new Bounds();
            aabb.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }
        #endregion

    }
}
