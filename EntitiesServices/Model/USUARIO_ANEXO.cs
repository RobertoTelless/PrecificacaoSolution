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
    
    public partial class USUARIO_ANEXO
    {
        public int USAN_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public string USAN_NM_TITULO { get; set; }
        public System.DateTime USAN_DT_ANEXO { get; set; }
        public int USAN_IN_TIPO { get; set; }
        public string USAN_AQ_ARQUIVO { get; set; }
        public int USAN_IN_ATIVO { get; set; }
    
        public virtual USUARIO USUARIO { get; set; }
    }
}
