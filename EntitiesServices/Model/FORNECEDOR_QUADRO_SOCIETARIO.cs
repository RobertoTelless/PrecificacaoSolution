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
    
    public partial class FORNECEDOR_QUADRO_SOCIETARIO
    {
        public int FOQS_CD_ID { get; set; }
        public int FORN_CD_ID { get; set; }
        public string FOQS_NM_QUALIFICACAO { get; set; }
        public string FOQS_NM_PAIS_ORIGEM { get; set; }
        public string FOQS_NM_REP_LEGAL { get; set; }
        public string FOQS_NM_QUALIFICACAO_REP_LEGAL { get; set; }
        public string FOQS_NM_NOME { get; set; }
        public Nullable<int> FOQS_IN_ATIVO { get; set; }
    
        public virtual FORNECEDOR FORNECEDOR { get; set; }
    }
}
