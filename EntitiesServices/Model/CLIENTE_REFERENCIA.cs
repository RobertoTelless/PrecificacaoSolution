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
    
    public partial class CLIENTE_REFERENCIA
    {
        public int CLRE_CD_ID { get; set; }
        public int CLIE_CD_ID { get; set; }
        public string CLRE_NM_NOME { get; set; }
        public string CLRE_NR_TELEFONE { get; set; }
        public string CLRE_NM_EMAIL { get; set; }
        public int CLRE_IN_ATIVO { get; set; }
    
        public virtual CLIENTE CLIENTE { get; set; }
    }
}
