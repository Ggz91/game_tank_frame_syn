﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEntity : CEntity
{
    public TankEntity(int en_id)
    {
        Init();
    }
    string private_path = "Prefabs/Tank";
    public override void Init()
    {
        if (null == m_en_obj)
        {
            Object obj = Resources.Load(private_path);
            m_en_obj = GameObject.Instantiate(obj) as GameObject;
            if (null == m_en_obj)
            {
                Debug.Log("Fail to create tank");
            }
        }
        m_op_type = EntityPredefined.EntityOpType.EOT_IDLE;
    }
    //逻辑层相关的东西
    Vector3 target_pos;
    Vector3 cur_pos;

    protected EntityPredefined.EntityOpType m_op_type;
    protected EntityPredefined.EntityCampType m_camp_type;
    float acc_time;
    const float fixed_time = 0.02f;
    const float speed = 1.0f;
    void UpdatePosLerp()
    {
        //测试单机运动控制
//         acc_time += Time.deltaTime;
//         while(acc_time > fixed_time)
//         {
//             m_en_obj.transform.position = target_pos;
//             RecordCurPos();
//             UpdateTargetPos(m_op_type);
//             acc_time -= fixed_time;
//         }
//         float ratio = (acc_time % fixed_time) / fixed_time;
//          m_en_obj.transform.position = Vector3.Lerp(cur_pos, target_pos, ratio);
    }

    void Fire()
    {

    }
    public override GameObject GetObj()
    {
        return m_en_obj;
    }
    public override void Update()
    {
        if (null == GetObj())
        {
            return;
        }

        base.Update();
        
        switch(m_op_type)
        {
            case EntityPredefined.EntityOpType.EOT_IDLE:
            case EntityPredefined.EntityOpType.EOT_FORWARD:
            case EntityPredefined.EntityOpType.EOT_BACKWARD:
            case EntityPredefined.EntityOpType.EOT_LEFT:
            case EntityPredefined.EntityOpType.EOT_RIGHT:
            UpdatePosLerp();
            break;
            case EntityPredefined.EntityOpType.EOT_FIRE:
            Fire();
            break;
        }

    }

    public void UpdateTargetPos(EntityPredefined.EntityOpType op_type)
    {
        if (op_type == EntityPredefined.EntityOpType.EOT_IDLE)
        {
            return;
        }
        target_pos += m_en_obj.transform.forward * speed * Time.deltaTime;
    }
    
    Vector3 VectorInPlane(Vector3 vec, Vector3 plane_normal)
    {
        Vector3 bitangent = Vector3.Cross(vec, plane_normal);
        Vector3 xz_vec = Vector3.Normalize(Vector3.Cross(plane_normal, bitangent));
        return xz_vec;
    }

    void UpdateRotation(EntityPredefined.EntityOpType op_type)
    {
        //按照相机的左右来转向，而不是根据实体自己的
        if (op_type != m_op_type && (op_type > EntityPredefined.EntityOpType.EOT_IDLE && op_type < EntityPredefined.EntityOpType.EOT_FIRE))
        {
            GameObject cam = GameObject.Find(EntityPredefined.SceneCamera);
            if (null == cam)
            {
                Debug.Log("Fail to find scene camera");
                return;
            }
            Vector3 cam_forward = cam.transform.forward;
            //计算在xz平面的投影
            Vector3 axis = new Vector3(0, 1, 0);
            Vector3 cam_xz_forward = VectorInPlane(cam_forward, axis);
            float delta_angle = 0.0f;
            switch(op_type)
            {
                case EntityPredefined.EntityOpType.EOT_BACKWARD:
                delta_angle = 180;
                break;
                case EntityPredefined.EntityOpType.EOT_LEFT:
                delta_angle = -90;
                break;
                case EntityPredefined.EntityOpType.EOT_RIGHT:
                delta_angle = 90;
                break;
            }
            //旋转后的方向
            Vector3 self_forward = VectorInPlane(m_en_obj.transform.forward, axis);
            Vector3 cross_cam_self_forward = Vector3.Cross(cam_xz_forward, self_forward);
            float inverse_jude = Vector3.Dot(cross_cam_self_forward, axis) < 0 ? -1 : 1;
            float delat_cam_to_self = Vector3.Angle(cam_xz_forward, self_forward) * inverse_jude;
            delta_angle -= delat_cam_to_self;
            m_en_obj.transform.RotateAroundLocal(axis, Mathf.Deg2Rad * delta_angle);
        }
    }

    public override void Op(EntityPredefined.EntityOpType op_type)
    {
        base.Op(op_type);
        
        int offset = (int) (op_type - EntityPredefined.EntityOpType.EOT_FORWARD);
        if (offset <= (int)EntityPredefined.EntityOpType.EOT_RIGHT)
        {
            RecordCurPos();
            UpdateRotation(op_type);
            UpdateTargetPos(op_type);
        }
                
        //跟前面的api调用顺序相关
        m_op_type = op_type;
    }
    public override void RecordCurPos()
    {
        cur_pos = target_pos;
    }
}