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
    
    public partial class SERVICO_ANEXO
    {
        public int SEAN_CD_ID { get; set; }
        public int SERV_CD_ID { get; set; }
        public string SEAN_NM_TITULO { get; set; }
        public System.DateTime SEAN_DT_ANEXO { get; set; }
        public int SEAN_IN_TIPO { get; set; }
        public string SEAN_AQ_ARQUIVO { get; set; }
        public int SEAN_IN_ATIVO { get; set; }
    
        public virtual SERVICO SERVICO { get; set; }
    }
}
