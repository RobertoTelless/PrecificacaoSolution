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
    
    public partial class PRODUTO_ANEXO
    {
        public int PRAN_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public string PRAN_NM_TITULO { get; set; }
        public System.DateTime PRAN_DT_ANEXO { get; set; }
        public int PRAN_IN_TIPO { get; set; }
        public string PRAN_AQ_ARQUIVO { get; set; }
        public int PRAN_IN_ATIVO { get; set; }
    
        public virtual PRODUTO PRODUTO { get; set; }
    }
}
