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
    
    public partial class CRM_PEDIDO_VENDA_ANEXO
    {
        public int CRPA_CD_ID { get; set; }
        public int CRPV_CD_ID { get; set; }
        public string CRPA_NM_TITULO { get; set; }
        public System.DateTime CRPA_DT_ANEXO { get; set; }
        public int CRPA_IN_TIPO { get; set; }
        public string CRPA_AQ_ARQUIVO { get; set; }
        public int CRPA_IN_ATIVO { get; set; }
    
        public virtual CRM_PEDIDO_VENDA CRM_PEDIDO_VENDA { get; set; }
    }
}