﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEntity : IEntity
{
    //表现层相关
    protected GameObject m_en_obj;
    public override int EnId
    {
        get; set;
    }

    public override void Init()
    {

    }

    public override void Update(float delta_time)
    {
        
    }

    public override void Destroy()
    {
        GameObject.Destroy(m_en_obj);
    }

    public override void SetPreOpType(EntityPredefined.EntityOpType op_type, EntityPredefined.EntityExtOpType ext_op_type)
    {
    }
    public override void ImplementCurFrameOpType()
    {

    }
    public override GameObject GetObj()
    {
        return null;
    }
    public override EntityPredefined.EntityOpType GetOpType()
    {
        return EntityPredefined.EntityOpType.EOT_INVALID;
    }
    public override EntityPredefined.EntityExtOpType GetExtOpType()
    {
        return EntityPredefined.EntityExtOpType.EEOT_NONE;
    }
    public override void Op(EntityPredefined.EntityOpType op_type, EntityPredefined.EntityExtOpType ext_op_type)
    {
    }
}
