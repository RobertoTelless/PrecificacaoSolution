//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CLIENTE_ANOTACAO
    {
        public int CLAT_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public System.DateTime CLAT_DT_ANOTACAO { get; set; }
        public string CLAT_TX_ANOTACAO { get; set; }
        public int CLAT_IN_ATIVO { get; set; }
    
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
